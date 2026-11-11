using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public interface IServicioAutenticacion
    {
        Task<ResultadoAutenticacion> RegistrarseAsync(string email, string password);
        Task<ResultadoAutenticacion> IniciarSesionAsync(string email, string password);
        ResultadoAutenticacion UsarSinCuenta();
    }
}
