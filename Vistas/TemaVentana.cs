using System.Windows;
using System.Windows.Interop;
using Cromatico.Interop;

namespace Cromatico.Vistas
{
    public static class TemaVentana
    {
        public static void AplicarBarraOscura(Window ventana)
        {
            ventana.SourceInitialized += (_, _) =>
            {
                try
                {
                    var hwnd = new WindowInteropHelper(ventana).Handle;
                    int oscuro = 1;
                    NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref oscuro, sizeof(int));
                }
                catch { }
            };
        }
    }
}
