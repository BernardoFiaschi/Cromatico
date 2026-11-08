using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class ServicioSincronizacionSimulado : IServicioSincronizacion
    {
        private readonly string _carpeta;
        private readonly SemaphoreSlim _acceso = new(1, 1);

        public ServicioSincronizacionSimulado(string? carpeta = null)
        {
            _carpeta = carpeta ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cromatico", "NubeSimulada");
        }

        // CROMATICO_SIMULAR_FALLA_RED=1 fuerza el error de red.
        public static bool FallaDeRedSimulada
            => Environment.GetEnvironmentVariable("CROMATICO_SIMULAR_FALLA_RED") == "1";

        public async Task VerificarConexionAsync()
        {
            await Task.Delay(150);
            if (FallaDeRedSimulada)
                throw new SincronizacionException("No se pudo conectar con el servidor.");
        }

        public async Task<ConfiguracionApp?> DescargarAsync(string email)
        {
            await VerificarConexionAsync();

            await _acceso.WaitAsync();
            try
            {
                var ruta = RutaDe(email);
                if (!File.Exists(ruta)) return null;

                try
                {
                    var json = await File.ReadAllTextAsync(ruta);
                    var config = JsonSerializer.Deserialize<ConfiguracionApp>(json, ConfiguracionApp.OpcionesJson);
                    if (config == null || !config.EsValida(out var motivo))
                        throw new SincronizacionException(
                            "Los datos guardados en el servidor están dañados o incompletos.");
                    config.AsegurarPredefinidos();
                    return config;
                }
                catch (JsonException)
                {
                    throw new SincronizacionException("Los datos guardados en el servidor están dañados.");
                }
                catch (IOException ex)
                {
                    throw new SincronizacionException("No se pudieron leer los datos del servidor: " + ex.Message);
                }
            }
            finally { _acceso.Release(); }
        }

        public async Task SubirAsync(string email, ConfiguracionApp configuracion)
        {
            await VerificarConexionAsync();

            await _acceso.WaitAsync();
            try
            {
                try
                {
                    Directory.CreateDirectory(_carpeta);
                    var json = JsonSerializer.Serialize(configuracion, ConfiguracionApp.OpcionesJson);
                    var ruta = RutaDe(email);
                    var tmp = ruta + ".tmp";
                    await File.WriteAllTextAsync(tmp, json);
                    File.Move(tmp, ruta, overwrite: true);
                }
                catch (IOException ex)
                {
                    throw new SincronizacionException("No se pudieron guardar los datos en el servidor: " + ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new SincronizacionException("No se pudieron guardar los datos en el servidor: " + ex.Message);
                }
            }
            finally { _acceso.Release(); }
        }

        private string RutaDe(string email)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
            return Path.Combine(_carpeta, Convert.ToHexString(hash)[..24] + ".json");
        }
    }
}
