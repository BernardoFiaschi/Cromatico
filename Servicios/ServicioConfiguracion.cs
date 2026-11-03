using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class ServicioConfiguracion
    {
        private readonly string _carpeta;
        private readonly string _ruta;

        public ConfiguracionApp Config { get; } = ConfiguracionApp.Predeterminada();

        public event Action? Guardada;

        public string Ruta => _ruta;

        public ServicioConfiguracion(string clave, string? carpeta = null)
        {
            _carpeta = carpeta ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cromatico", "Config");
            _ruta = Path.Combine(_carpeta, clave + ".json");
        }

        public static string ClaveParaEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return "local";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
            return "cuenta-" + Convert.ToHexString(hash)[..16];
        }

        public string? Cargar()
        {
            if (!File.Exists(_ruta))
            {
                Config.CopiarDesde(ConfiguracionApp.Predeterminada());
                return null;
            }

            try
            {
                var json = File.ReadAllText(_ruta);
                var leida = JsonSerializer.Deserialize<ConfiguracionApp>(json, ConfiguracionApp.OpcionesJson);
                string motivo = "no tiene datos";
                if (leida == null || !leida.EsValida(out motivo))
                    return Descartar($"la configuración guardada está incompleta ({motivo})");

                leida.AsegurarPredefinidos();
                Config.CopiarDesde(leida);
                return null;
            }
            catch (JsonException)
            {
                return Descartar("la configuración guardada está dañada");
            }
            catch (IOException ex)
            {
                return Descartar("no se pudo leer la configuración guardada: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Descartar("no se pudo leer la configuración guardada: " + ex.Message);
            }
        }

        private string Descartar(string motivo)
        {
            try { File.Copy(_ruta, _ruta + ".danado.bak", overwrite: true); } catch { }
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
                Escribir();
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
            try { Escribir(); }
            catch (Exception ex)
            {
                return ResultadoOperacion.Error("No se pudo guardar la configuración: " + ex.Message);
            }
            Guardada?.Invoke();
            return ResultadoOperacion.Ok();
        }

        private void Escribir()
        {
            Directory.CreateDirectory(_carpeta);
            var json = JsonSerializer.Serialize(Config, ConfiguracionApp.OpcionesJson);
            var tmp = _ruta + ".tmp";
            File.WriteAllText(tmp, json);
            File.Move(tmp, _ruta, overwrite: true);
        }
    }
}
