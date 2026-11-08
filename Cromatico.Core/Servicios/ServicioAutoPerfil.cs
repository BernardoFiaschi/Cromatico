using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public enum OrigenPerfil
    {
        Predeterminado,
        Aplicacion,
        Contexto
    }

    public sealed class PerfilEfectivo
    {
        public PerfilCorreccion Perfil { get; init; } = new();
        public OrigenPerfil Origen { get; init; }
        public string? Aplicacion { get; init; }

        public string Descripcion => Origen switch
        {
            OrigenPerfil.Aplicacion => $"{Perfil.Nombre} (asociado a {Aplicacion})",
            OrigenPerfil.Contexto => $"{Perfil.Nombre} (por contexto de uso: {Aplicacion})",
            _ => $"{Perfil.Nombre} (perfil predeterminado)"
        };

        internal bool Equivale(PerfilEfectivo? otro) =>
            otro != null && otro.Origen == Origen && otro.Aplicacion == Aplicacion &&
            otro.Perfil.Id == Perfil.Id && otro.Perfil.Tipo == Perfil.Tipo &&
            otro.Perfil.Severidad == Perfil.Severidad && otro.Perfil.Intensidad == Perfil.Intensidad;
    }

    public class ServicioAutoPerfil
    {
        private readonly ServicioConfiguracion _almacen;
        private readonly ServicioPerfiles _perfiles;
        private readonly IMotorCorreccion _motor;
        private readonly Notificador _avisos;

        private InfoProceso? _ultimaVentana;
        private string? _ultimoErrorAvisado;

        public ServicioAutoPerfil(ServicioConfiguracion almacen, ServicioPerfiles perfiles,
            IMotorCorreccion motor, Notificador avisos)
        {
            _almacen = almacen;
            _perfiles = perfiles;
            _motor = motor;
            _avisos = avisos;

            Efectivo = Resolver(null);

            _perfiles.Cambio += () => Recalcular();
            _almacen.Guardada += () => Recalcular();
        }

        // Gancho opcional: si devuelve un perfil, tiene prioridad sobre el global.
        public Func<InfoProceso, PerfilCorreccion?>? ResolutorDeContexto { get; set; }

        public PerfilEfectivo Efectivo { get; private set; }

        public InfoProceso? UltimaVentana => _ultimaVentana;

        public event Action? EfectivoCambiado;

        public void AlCambiarVentana(InfoProceso? info, string? error)
        {
            if (info == null)
            {
                if (error != null && error != _ultimoErrorAvisado)
                {
                    _ultimoErrorAvisado = error;
                    _avisos.Advertir(error + " Se mantiene el perfil de corrección actual.");
                }
                return;
            }

            _ultimoErrorAvisado = null;
            _ultimaVentana = info;
            Recalcular();
        }

        public PerfilEfectivo Resolver(InfoProceso? info)
        {
            var config = _almacen.Config;
            var global = _perfiles.PerfilActivo;
            var predeterminado = new PerfilEfectivo { Perfil = global, Origen = OrigenPerfil.Predeterminado };

            if (!config.AutoPerfilesActivo || info == null || info.EsPropia)
                return predeterminado;

            var asociacion = config.Asociaciones.FirstOrDefault(a =>
                string.Equals(a.Proceso, info.Nombre, StringComparison.OrdinalIgnoreCase));
            if (asociacion != null)
            {
                var perfil = config.BuscarPerfil(asociacion.PerfilId);
                if (perfil != null)
                    return new PerfilEfectivo { Perfil = perfil, Origen = OrigenPerfil.Aplicacion, Aplicacion = asociacion.Proceso };
            }

            var porContexto = ResolutorDeContexto?.Invoke(info);
            if (porContexto != null)
                return new PerfilEfectivo { Perfil = porContexto, Origen = OrigenPerfil.Contexto, Aplicacion = info.Nombre };

            return predeterminado;
        }

        public void Recalcular()
        {
            var nuevo = Resolver(_ultimaVentana);
            if (nuevo.Equivale(Efectivo)) return;

            Efectivo = nuevo;

            var r = _motor.AplicarPerfil(nuevo.Perfil);
            if (!r.Exitoso) _avisos.Error(r.Mensaje);

            EfectivoCambiado?.Invoke();
        }

        public ResultadoOperacion CambiarAutoActivo(bool activo)
        {
            return _almacen.Modificar(c => c.AutoPerfilesActivo = activo,
                "guardar la opción de cambio automático de perfil");
        }
    }
}
