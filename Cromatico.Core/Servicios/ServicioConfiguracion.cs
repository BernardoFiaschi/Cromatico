using System.Security.Cryptography;
using System.Text;
using Cromatico.Datos;
using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class ServicioConfiguracion
    {
        private readonly IRepositorioConfiguracion _repositorio;
        private readonly string _clave;

        public ConfiguracionApp Config { get; } = ConfiguracionApp.Predeterminada();

        public event Action? Guardada;

        public string Ruta => _repositorio.Ubicacion;

        public ServicioConfiguracion(string clave, IRepositorioConfiguracion repositorio)
        {
            _clave = clave;
            _repositorio = repositorio;
        }

        public static string ClaveParaEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return "local";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
            return "cuenta-" + Convert.ToHexString(hash)[..16];
        }

        public string? Cargar()
        {
            try
            {
                var leida = _repositorio.Leer(_clave);
                if (leida == null)
                {
                    Config.CopiarDesde(ConfiguracionApp.Predeterminada());
                    try { _repositorio.Escribir(_clave, Config); } catch { }
                    return null;
                }

                if (!leida.EsValida(out var motivo))
                    return Descartar($"la configuración guardada está incompleta ({motivo})");

                leida.AsegurarPredefinidos();
                Config.CopiarDesde(leida);
                return null;
            }
            catch (Exception ex)
            {
                return Descartar("no se pudo leer la configuración guardada: " + ex.Message);
            }
        }

        private string Descartar(string motivo)
        {
            Config.CopiarDesde(ConfiguracionApp.Predeterminada());
            return $"Se restauró la configuración predeterminada porque {motivo}.";
        }

        public ResultadoOperacion Reemplazar(ConfiguracionApp nueva)
        {
            return Modificar(c => c.CopiarDesde(nueva), "guardar la configuración recuperada", conservarFecha: true);
        }

        // Aplica el cambio y guarda; si el guardado falla, restaura el estado anterior.
        public ResultadoOperacion Modificar(Action<ConfiguracionApp> cambio, string accion, bool conservarFecha = false)
        {
            var respaldo = Config.Clonar();
            try
            {
                cambio(Config);
                if (!conservarFecha) Config.UltimaModificacionUtc = DateTime.UtcNow;
                _repositorio.Escribir(_clave, Config);
            }
            catch (Exception ex)
            {
                Config.CopiarDesde(respaldo);
                return ResultadoOperacion.Error(
                    $"No se pudo {accion}: {ex.Message} Se restauró la última configuración válida.");
            }

            Guardada?.Invoke();
            return ResultadoOperacion.Ok();
        }

        public ResultadoOperacion Guardar()
        {
            try { _repositorio.Escribir(_clave, Config); }
            catch (Exception ex)
            {
                return ResultadoOperacion.Error("No se pudo guardar la configuración: " + ex.Message);
            }
            Guardada?.Invoke();
            return ResultadoOperacion.Ok();
        }
    }
}
