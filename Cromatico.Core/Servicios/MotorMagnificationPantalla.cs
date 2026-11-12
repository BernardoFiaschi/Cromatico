using Cromatico.Algoritmo;
using Cromatico.Interop;
using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class MotorMagnificationPantalla : IMotorCorreccion, IDisposable
    {
        private MagnificationNativeMethods.MAGCOLOREFFECT _efectoOriginal;
        private PerfilCorreccion? _perfilActual;
        private bool _inicializado;

        public MotorMagnificationPantalla()
        {
            try
            {
                bool ok = MagnificationNativeMethods.MagInitialize();

                if (!ok)
                {
                    GuardarExcepcion.registrarExcepcion(new InvalidOperationException(
                        "No se pudo inicializarMagnification API."), "MotroMagnificationPantalla");
                    throw new InvalidOperationException("No se pudo inicializar Magnification API.");
                }

                if (!MagnificationNativeMethods.MagGetFullscreenColorEffect(out _efectoOriginal)
                    || _efectoOriginal.transform == null || _efectoOriginal.transform.Length != 25)
                    _efectoOriginal = MagnificationColorAdapter.Convertir(Matriz3.Identidad);
                _inicializado = true;
            }
            catch (Exception ex)
            {
                GuardarExcepcion.registrarExcepcion(ex, "MotorMagnificationPantalla");
            }

        }
        public ResultadoOperacion AplicarPerfil(PerfilCorreccion perfil)
        {
            try
            {
                _perfilActual=perfil.Clonar();
                var matriz = Daltonizador.CalcularMatriz(perfil);
                var efecto = MagnificationColorAdapter.Convertir(matriz);
                bool ok = MagnificationNativeMethods.MagSetFullscreenColorEffect(ref efecto);
                if (!ok)
                {
                    return ResultadoOperacion.Error("No se pudo aplicar el efecto de color");
                }
                return ResultadoOperacion.Ok();
            }catch(Exception ex)
            {
                GuardarExcepcion.registrarExcepcion(ex,"MotorMagnificationPantalla.AplicarPerfil");
                return ResultadoOperacion.Error(ex.Message);
            }
        }
        public void Restaurar()
        {
            if (!_inicializado)
                return;
            _inicializado = false;
            MagnificationNativeMethods.MagSetFullscreenColorEffect(ref _efectoOriginal);

            MagnificationNativeMethods.MagUninitialize();
        }

        public void Dispose()
        {
            Restaurar();
        }
    }
}