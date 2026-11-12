using Cromatico.Algoritmo;

namespace Cromatico.Interop
{
    public static class MagnificationColorAdapter
    {
        public static MagnificationNativeMethods.MAGCOLOREFFECT Convertir(Matriz3 matriz)
        {
            return new MagnificationNativeMethods.MAGCOLOREFFECT
            {
                transform =
                [
                    (float)matriz[0,0], (float)matriz[0,1], (float)matriz[0,2], 0f, 0f,

                    (float)matriz[1,0], (float)matriz[1,1], (float)matriz[1,2], 0f, 0f,

                    (float)matriz[2,0], (float)matriz[2,1], (float)matriz[2,2], 0f, 0f,

                    0f, 0f, 0f, 1f, 0f,

                    0f, 0f, 0f, 0f, 1f
                ]
            };
        }
    }
}
