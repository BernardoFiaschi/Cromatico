using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public interface IAlmacenamientoLocal
    {
        Task<Usuario?> BuscarUsuarioAsync(string email);
        Task GuardarUsuarioAsync(Usuario usuario);
    }
}
