using System.Runtime.InteropServices;
namespace Cromatico.UI;

public sealed class CapturaPantalla
    {
        public int Ancho { get; init; }
        public int Alto { get; init; }
        public byte[] Bgra { get; init; } = Array.Empty<byte>();
    }

    public sealed class CapturaException : Exception
    {
        public CapturaException(string mensaje) : base(mensaje) { }
    }

    public static class CapturadorPantalla
    {
        public static CapturaPantalla Capturar(int anchoMaximo = 960)
        {
            IntPtr hdcPantalla = IntPtr.Zero, hdcMemoria = IntPtr.Zero, bitmap = IntPtr.Zero, anterior = IntPtr.Zero;
            try
            {
                hdcPantalla = NativeMethods.GetDC(IntPtr.Zero);
                if (hdcPantalla == IntPtr.Zero) throw new CapturaException("No se pudo acceder a la pantalla.");

                // Resolución física, independiente de la escala de Windows.
                int ancho = NativeMethods.GetDeviceCaps(hdcPantalla, NativeMethods.DESKTOPHORZRES);
                int alto = NativeMethods.GetDeviceCaps(hdcPantalla, NativeMethods.DESKTOPVERTRES);
                if (ancho <= 0 || alto <= 0) throw new CapturaException("No se pudo obtener el tamaño de la pantalla.");

                hdcMemoria = NativeMethods.CreateCompatibleDC(hdcPantalla);
                if (hdcMemoria == IntPtr.Zero) throw new CapturaException("No se pudo preparar la captura.");

                var info = new NativeMethods.BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf<NativeMethods.BITMAPINFOHEADER>(),
                    biWidth = ancho,
                    biHeight = -alto,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0
                };

                bitmap = NativeMethods.CreateDIBSection(hdcMemoria, ref info, 0, out IntPtr bits, IntPtr.Zero, 0);
                if (bitmap == IntPtr.Zero || bits == IntPtr.Zero) throw new CapturaException("No se pudo reservar memoria para la captura.");

                anterior = NativeMethods.SelectObject(hdcMemoria, bitmap);

                if (!NativeMethods.BitBlt(hdcMemoria, 0, 0, ancho, alto, hdcPantalla, 0, 0,
                        NativeMethods.SRCCOPY | NativeMethods.CAPTUREBLT))
                    throw new CapturaException("Windows rechazó la captura de la pantalla.");

                var datos = new byte[ancho * alto * 4];
                Marshal.Copy(bits, datos, 0, datos.Length);
                return Reducir(datos, ancho, alto, anchoMaximo);
            }
            catch (CapturaException) { throw; }
            catch (Exception ex)
            {
                throw new CapturaException("No se pudo capturar la pantalla: " + ex.Message);
            }
            finally
            {
                if (anterior != IntPtr.Zero && hdcMemoria != IntPtr.Zero) NativeMethods.SelectObject(hdcMemoria, anterior);
                if (bitmap != IntPtr.Zero) NativeMethods.DeleteObject(bitmap);
                if (hdcMemoria != IntPtr.Zero) NativeMethods.DeleteDC(hdcMemoria);
                if (hdcPantalla != IntPtr.Zero) NativeMethods.ReleaseDC(IntPtr.Zero, hdcPantalla);
            }
        }

        public static CapturaPantalla Reducir(byte[] bgra, int ancho, int alto, int anchoMaximo)
        {
            int k = Math.Max(1, (int)Math.Ceiling(ancho / (double)Math.Max(1, anchoMaximo)));
            int nw = Math.Max(1, ancho / k), nh = Math.Max(1, alto / k);
            var salida = new byte[nw * nh * 4];

            for (int y = 0; y < nh; y++)
                for (int x = 0; x < nw; x++)
                {
                    int sb = 0, sg = 0, sr = 0;
                    for (int dy = 0; dy < k; dy++)
                    {
                        int fila = ((y * k + dy) * ancho + x * k) * 4;
                        for (int dx = 0; dx < k; dx++)
                        {
                            int o = fila + dx * 4;
                            sb += bgra[o]; sg += bgra[o + 1]; sr += bgra[o + 2];
                        }
                    }
                    int n = k * k, d = (y * nw + x) * 4;
                    salida[d] = (byte)(sb / n);
                    salida[d + 1] = (byte)(sg / n);
                    salida[d + 2] = (byte)(sr / n);
                    // BitBlt deja el alfa en 0.
                    salida[d + 3] = 255;
                }

            return new CapturaPantalla { Ancho = nw, Alto = nh, Bgra = salida };
        }
    }