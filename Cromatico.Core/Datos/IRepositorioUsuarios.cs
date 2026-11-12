using Cromatico.Modelos;

namespace Cromatico.Datos
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario?> BuscarUsuarioAsync(string email);
        Task GuardarUsuarioAsync(Usuario usuario);
    }
}
