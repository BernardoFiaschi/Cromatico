using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class SincronizacionException : Exception
    {
        public SincronizacionException(string mensaje) : base(mensaje) { }
    }

    public interface IServicioSincronizacion
    {
        Task VerificarConexionAsync();

        Task<ConfiguracionApp?> DescargarAsync(string email);

        Task SubirAsync(string email, ConfiguracionApp configuracion);
    }
}
