
namespace Cromatico.Entidades;

    public class ServicioPerfiles
    {
        private readonly ServicioConfiguracion _almacen;
        private readonly IMotorCorreccion _motor;

        public ServicioPerfiles(ServicioConfiguracion almacen, IMotorCorreccion motor)
        {
            _almacen = almacen;
            _motor = motor;
        }

        private ConfiguracionApp Config => _almacen.Config;

        public IReadOnlyList<PerfilCorreccion> Perfiles => Config.Perfiles;

        public PerfilCorreccion PerfilActivo
            => Config.BuscarPerfil(Config.PerfilActivoId) ?? Config.Perfiles[0];

        public event Action? Cambio;

        public void NotificarCambio() => Cambio?.Invoke();

        public ResultadoOperacion Seleccionar(string id)
        {
            var perfil = Config.BuscarPerfil(id);
            if (perfil == null)
                return ResultadoOperacion.Error("El perfil seleccionado no existe.");

            var r = _almacen.Modificar(c => c.PerfilActivoId = id, "guardar el perfil seleccionado");
            if (!r.Exitoso) return r;

            Cambio?.Invoke();
            return EnviarAlMotor(perfil);
        }

        public ResultadoOperacion<PerfilCorreccion> GuardarParametros(
            string perfilBaseId, TipoDeficiencia tipo, double severidad, double intensidad)
        {
            var baseP = Config.BuscarPerfil(perfilBaseId);
            if (baseP == null)
                return ResultadoOperacion<PerfilCorreccion>.Error("El perfil que se intenta modificar no existe.");

            if (!PerfilCorreccion.ValorValido(severidad) || !PerfilCorreccion.ValorValido(intensidad))
                return ResultadoOperacion<PerfilCorreccion>.Error("La severidad y la intensidad deben estar entre 0 % y 100 %.");

            string idGuardado = baseP.Id;
            string mensaje;

            var r = _almacen.Modificar(c =>
            {
                var original = c.BuscarPerfil(perfilBaseId)!;
                PerfilCorreccion destino;

                if (original.EsPredefinido)
                {
                    var nombre = $"{original.Nombre} (personalizado)";
                    var existente = c.Perfiles.FirstOrDefault(p => !p.EsPredefinido && p.Nombre == nombre);
                    if (existente != null)
                        destino = existente;
                    else
                    {
                        destino = new PerfilCorreccion { Nombre = nombre, EsPredefinido = false };
                        c.Perfiles.Add(destino);
                    }

                    if (c.PerfilActivoId == original.Id) c.PerfilActivoId = destino.Id;
                }
                else destino = original;

                destino.Tipo = tipo;
                destino.Severidad = severidad;
                destino.Intensidad = intensidad;
                idGuardado = destino.Id;
            }, "guardar los cambios del perfil");

            if (!r.Exitoso)
                return ResultadoOperacion<PerfilCorreccion>.Error(r.Mensaje);

            var guardado = Config.BuscarPerfil(idGuardado)!;
            mensaje = baseP.EsPredefinido
                ? $"Los cambios se guardaron como «{guardado.Nombre}»."
                : $"Perfil «{guardado.Nombre}» actualizado.";

            Cambio?.Invoke();
            if (guardado.Id == Config.PerfilActivoId) EnviarAlMotor(guardado);
            return ResultadoOperacion<PerfilCorreccion>.Ok(guardado, mensaje);
        }

        public ResultadoOperacion<PerfilCorreccion> Duplicar(string perfilBaseId, string nombre)
        {
            var baseP = Config.BuscarPerfil(perfilBaseId);
            if (baseP == null)
                return ResultadoOperacion<PerfilCorreccion>.Error("El perfil que se intenta duplicar no existe.");

            var errorNombre = ValidarNombre(nombre, null);
            if (errorNombre != null) return ResultadoOperacion<PerfilCorreccion>.Error(errorNombre);

            var nuevo = baseP.Clonar();
            nuevo.Id = Guid.NewGuid().ToString("N");
            nuevo.Nombre = nombre.Trim();
            nuevo.EsPredefinido = false;

            var r = _almacen.Modificar(c => c.Perfiles.Add(nuevo), "crear el perfil");
            if (!r.Exitoso) return ResultadoOperacion<PerfilCorreccion>.Error(r.Mensaje);

            Cambio?.Invoke();
            return ResultadoOperacion<PerfilCorreccion>.Ok(nuevo, $"Perfil «{nuevo.Nombre}» creado.");
        }

        public ResultadoOperacion Renombrar(string id, string nombre)
        {
            var perfil = Config.BuscarPerfil(id);
            if (perfil == null) return ResultadoOperacion.Error("El perfil no existe.");
            if (perfil.EsPredefinido) return ResultadoOperacion.Error("Los perfiles predefinidos no se pueden renombrar.");

            var errorNombre = ValidarNombre(nombre, id);
            if (errorNombre != null) return ResultadoOperacion.Error(errorNombre);

            var r = _almacen.Modificar(c => c.BuscarPerfil(id)!.Nombre = nombre.Trim(), "renombrar el perfil");
            if (r.Exitoso) Cambio?.Invoke();
            return r;
        }

        public ResultadoOperacion Eliminar(string id)
        {
            var perfil = Config.BuscarPerfil(id);
            if (perfil == null) return ResultadoOperacion.Error("El perfil no existe.");
            if (perfil.EsPredefinido) return ResultadoOperacion.Error("Los perfiles predefinidos no se pueden eliminar.");

            var usos = Config.Asociaciones.Count(a => a.PerfilId == id);
            if (usos > 0)
                return ResultadoOperacion.Error(
                    $"El perfil «{perfil.Nombre}» está asociado a {usos} aplicación(es). Cambiá esas asociaciones antes de eliminarlo.");

            bool eraActivo = Config.PerfilActivoId == id;
            var r = _almacen.Modificar(c =>
            {
                c.Perfiles.RemoveAll(p => p.Id == id);
                if (eraActivo) c.PerfilActivoId = PerfilesPredeterminados.IdNatural;
            }, "eliminar el perfil");
            if (!r.Exitoso) return r;

            Cambio?.Invoke();
            if (eraActivo) EnviarAlMotor(PerfilActivo);
            return r;
        }

        public ResultadoOperacion ActualizarPredefinidos(TipoDeficiencia tipo, double severidad, double intensidadRecomendada)
        {
            if (!PerfilCorreccion.ValorValido(severidad) || !PerfilCorreccion.ValorValido(intensidadRecomendada))
                return ResultadoOperacion.Error("Los valores sugeridos por el test están fuera de rango.");

            var r = _almacen.Modificar(c =>
            {
                foreach (var p in c.Perfiles.Where(p => p.EsPredefinido))
                {
                    p.Tipo = tipo;
                    p.Severidad = severidad;
                    p.Intensidad = p.Id == PerfilesPredeterminados.IdNatural
                        ? Math.Clamp(intensidadRecomendada * 0.65, 0.1, 1.0)
                        : intensidadRecomendada;
                }
            }, "guardar la configuración sugerida");
            if (!r.Exitoso) return r;

            Cambio?.Invoke();
            return EnviarAlMotor(PerfilActivo);
        }

        public ResultadoOperacion EnviarAlMotor(PerfilCorreccion perfil) => _motor.AplicarPerfil(perfil);

        private string? ValidarNombre(string nombre, string? idActual)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "El nombre del perfil no puede estar vacío.";
            if (nombre.Trim().Length > 40) return "El nombre del perfil es demasiado largo (máximo 40 caracteres).";
            bool repetido = Config.Perfiles.Any(p => p.Id != idActual &&
                string.Equals(p.Nombre, nombre.Trim(), StringComparison.OrdinalIgnoreCase));
            return repetido ? "Ya existe un perfil con ese nombre." : null;
        }
    }

