using Cromatico.Modelos;
namespace Cromatico.Servicios
{
    public sealed class ContextoApp : IDisposable
    {
        public static ContextoApp? Actual { get; private set; }

        public Usuario? Usuario { get; }
        public bool ModoSinCuenta { get; }
        public Notificador Avisos { get; } = new();
        public ServicioConfiguracion Almacen { get; }
        public IMotorCorreccion Motor { get; }
        public ServicioPerfiles Perfiles { get; }
        public ServicioAutoPerfil AutoPerfil { get; }
        public MonitorVentanaActiva Monitor { get; } = new();
        public IServicioSincronizacion Sincronizacion { get; }

        public List<(TipoAviso tipo, string texto)> AvisosIniciales { get; } = new();

        public bool NubeDisponible { get; private set; } = true;
        public DateTime? UltimaSincronizacionUtc { get; private set; }
        public event Action? EstadoNubeCambiado;

        public string Email => Usuario?.Email ?? string.Empty;

        private ContextoApp(ResultadoAutenticacion acceso, IServicioSincronizacion sincronizacion,
            IMotorCorreccion motor, ServicioConfiguracion almacen)
        {
            Usuario = acceso.UsuarioActual;
            ModoSinCuenta = acceso.ModoSinCuenta || acceso.UsuarioActual == null;
            Sincronizacion = sincronizacion;
            Motor = motor;
            Almacen = almacen;

            Perfiles = new ServicioPerfiles(Almacen, Motor);
            AutoPerfil = new ServicioAutoPerfil(Almacen, Perfiles, Motor, Avisos);


            AutoPerfil.ResolutorDeContexto = info => ServicioContextoUso.ObtenerPerfil(info,
            Perfiles.Perfiles);


            Almacen.Guardada += () => { if (!ModoSinCuenta) _ = SubirSilenciosoAsync(); };
        }

        // Llamar desde el hilo de la interfaz.
        public static async Task<ContextoApp> CrearAsync(
            ResultadoAutenticacion acceso, IServicioSincronizacion sincronizacion,
            IMotorCorreccion? motor = null, string? carpetaConfig = null)
        {
            bool sinCuenta = acceso.ModoSinCuenta || acceso.UsuarioActual == null;
            var clave = ServicioConfiguracion.ClaveParaEmail(sinCuenta ? null : acceso.UsuarioActual!.Email);

            var almacen = new ServicioConfiguracion(clave, carpetaConfig);
            var avisoCarga = almacen.Cargar();

            var ctx = new ContextoApp(acceso, sincronizacion, motor ?? new MotorNulo(), almacen);
            if (avisoCarga != null) ctx.AvisosIniciales.Add((TipoAviso.Advertencia, avisoCarga));

            if (!ctx.ModoSinCuenta)
            {
                var r = await ctx.SincronizarAsync();
                if (!r.Exitoso) ctx.AvisosIniciales.Add((TipoAviso.Error, r.Mensaje));
                else if (!string.IsNullOrEmpty(r.Mensaje)) ctx.AvisosIniciales.Add((TipoAviso.Exito, r.Mensaje));
            }

            Actual = ctx;
            ctx.IniciarMonitoreo();
            return ctx;
        }

        public async Task<ResultadoOperacion> SincronizarAsync()
        {
            if (ModoSinCuenta)
                return ResultadoOperacion.Error("Estás usando Cromático sin cuenta: no hay nada que sincronizar.");

            try
            {
                var nube = await Sincronizacion.DescargarAsync(Email);
                var local = Almacen.Config.UltimaModificacionUtc;
                string mensaje;

                if (nube == null)
                {
                    await Sincronizacion.SubirAsync(Email, Almacen.Config.Clonar());
                    mensaje = "Tu cuenta todavía no tenía datos: se guardaron tus perfiles actuales.";
                }
                else if (local == DateTime.MinValue || nube.UltimaModificacionUtc > local)
                {
                    var r = Almacen.Reemplazar(nube);
                    if (!r.Exitoso)
                        return ResultadoOperacion.Error(
                            "Se descargaron los datos de tu cuenta pero no se pudieron aplicar en este equipo. " + r.Mensaje);
                    mensaje = "Se recuperaron tus perfiles y configuraciones desde tu cuenta.";
                }
                else if (nube.UltimaModificacionUtc < local)
                {
                    await Sincronizacion.SubirAsync(Email, Almacen.Config.Clonar());
                    mensaje = "Los cambios de este equipo eran más recientes: se actualizó tu cuenta.";
                }
                else
                {
                    mensaje = "Tus perfiles ya estaban al día con tu cuenta.";
                }

                MarcarNube(true);
                Perfiles.NotificarCambio();
                return ResultadoOperacion.Ok(mensaje);
            }
            catch (SincronizacionException ex)
            {
                MarcarNube(false);

                return ResultadoOperacion.Error(
                    $"No se pudieron recuperar los perfiles de tu cuenta: {ex.Message} Se usan los datos guardados en este equipo.");
            }
        }

        private async Task SubirSilenciosoAsync()
        {
            try
            {
                await Sincronizacion.SubirAsync(Email, Almacen.Config.Clonar());
                if (!NubeDisponible) Avisos.Exito("Se restableció la conexión: tus cambios se sincronizaron con tu cuenta.");
                MarcarNube(true);
            }
            catch (SincronizacionException ex)
            {
                if (NubeDisponible)
                    Avisos.Advertir($"No se pudo sincronizar con tu cuenta ({ex.Message}). Los cambios quedaron guardados en este equipo.");
                MarcarNube(false);
            }
        }

        private void MarcarNube(bool disponible)
        {
            NubeDisponible = disponible;
            if (disponible) UltimaSincronizacionUtc = DateTime.UtcNow;
            EstadoNubeCambiado?.Invoke();
        }

        private void IniciarMonitoreo()
        {
            Monitor.VentanaCambiada += AutoPerfil.AlCambiarVentana;

            if (!Monitor.Iniciar())
            {
                AvisosIniciales.Add((TipoAviso.Advertencia,
                    "No se pudo iniciar la detección automática de aplicaciones: se usará siempre el perfil predeterminado."));
                return;
            }

            var (proceso, error) = Monitor.LeerActual();
            AutoPerfil.AlCambiarVentana(proceso, error);
        }

        public void Dispose()
        {
            Monitor.VentanaCambiada -= AutoPerfil.AlCambiarVentana;
            Monitor.Dispose();
            if (ReferenceEquals(Actual, this)) Actual = null;
        }
    }
}
