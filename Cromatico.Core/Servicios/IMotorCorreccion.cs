using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public interface IMotorCorreccion
    {
        ResultadoOperacion AplicarPerfil(PerfilCorreccion perfil);
    }

    public sealed class MotorNulo : IMotorCorreccion
    {
        public PerfilCorreccion? UltimoPerfil { get; private set; }

        public ResultadoOperacion AplicarPerfil(PerfilCorreccion perfil)
        {
            UltimoPerfil = perfil.Clonar();
            return ResultadoOperacion.Ok();
        }
    }
}
