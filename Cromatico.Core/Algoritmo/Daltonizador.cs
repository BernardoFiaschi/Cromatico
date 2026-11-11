using Cromatico.Modelos;

namespace Cromatico.Algoritmo
{
    public static class Daltonizador
    {
        public static readonly Matriz3 RgbALms = new Matriz3(
            17.8824, 43.5161, 4.11935,
            3.45565, 27.1554, 3.86714,
            0.0299566, 0.184309, 1.46709);

        public static readonly Matriz3 LmsARgb = RgbALms.Inversa();

        private static Matriz3 SimulacionLms(TipoDeficiencia tipo) => tipo switch
        {
            TipoDeficiencia.Protan => new Matriz3(0, 2.02344, -2.52581, 0, 1, 0, 0, 0, 1),

            TipoDeficiencia.Deutan => new Matriz3(1, 0, 0, 0.494207, 0, 1.24827, 0, 0, 1),

            _ => new Matriz3(1, 0, 0, 0, 1, 0, -0.395913, 0.801109, 0)
        };

        private static Matriz3 Desplazamiento(TipoDeficiencia tipo) => tipo switch
        {
            TipoDeficiencia.Protan or TipoDeficiencia.Deutan => new Matriz3(0, 0, 0, 0.7, 1, 0, 0.7, 0, 1),

            _ => new Matriz3(1, 0, 0.7, 0, 1, 0.7, 0, 0, 0)
        };

        public static Matriz3 MatrizSimulacion(TipoDeficiencia tipo, double severidad)
        {
            severidad = Acotar(severidad);
            var parcial = Matriz3.Identidad.Mezclar(SimulacionLms(tipo), severidad);
            return LmsARgb * parcial * RgbALms;
        }

        public static Matriz3 CalcularMatriz(PerfilCorreccion perfil)
            => CalcularMatriz(perfil.Tipo, perfil.Severidad, perfil.Intensidad);

        // M = (1-t)·I + t·(I + Shift·(I - Sim)); t = intensidad, Sim = simulación a severidad dada.
        public static Matriz3 CalcularMatriz(TipoDeficiencia tipo, double severidad, double intensidad)
        {
            intensidad = Acotar(intensidad);
            var simulada = MatrizSimulacion(tipo, severidad);
            var error = Matriz3.Identidad - simulada;
            var corregida = Matriz3.Identidad + Desplazamiento(tipo) * error;
            return Matriz3.Identidad.Mezclar(corregida, intensidad);
        }

        public static void Aplicar(byte[] origen, byte[] destino, int pixeles, Matriz3 m)
        {
            if (origen == null || destino == null) throw new ArgumentNullException();
            if (pixeles < 0 || origen.Length < pixeles * 4 || destino.Length < pixeles * 4)
                throw new ArgumentException("Los buffers no alcanzan para la cantidad de píxeles indicada.");

            float m00 = (float)m[0, 0], m01 = (float)m[0, 1], m02 = (float)m[0, 2];
            float m10 = (float)m[1, 0], m11 = (float)m[1, 1], m12 = (float)m[1, 2];
            float m20 = (float)m[2, 0], m21 = (float)m[2, 1], m22 = (float)m[2, 2];

            if (pixeles >= 200_000)
            {
                const int bloque = 4096;
                int bloques = (pixeles + bloque - 1) / bloque;
                Parallel.For(0, bloques, b =>
                {
                    int desde = b * bloque;
                    int hasta = Math.Min(pixeles, desde + bloque);
                    Procesar(origen, destino, desde, hasta, m00, m01, m02, m10, m11, m12, m20, m21, m22);
                });
            }
            else
            {
                Procesar(origen, destino, 0, pixeles, m00, m01, m02, m10, m11, m12, m20, m21, m22);
            }
        }

        private static void Procesar(byte[] src, byte[] dst, int desde, int hasta,
            float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            for (int p = desde; p < hasta; p++)
            {
                int o = p * 4;
                float b = src[o], g = src[o + 1], r = src[o + 2];

                dst[o + 2] = ABajo(m00 * r + m01 * g + m02 * b);
                dst[o + 1] = ABajo(m10 * r + m11 * g + m12 * b);
                dst[o] = ABajo(m20 * r + m21 * g + m22 * b);
                dst[o + 3] = 255;
            }
        }

        private static byte ABajo(float v)
        {
            if (v <= 0f) return 0;
            if (v >= 255f) return 255;
            return (byte)(v + 0.5f);
        }

        private static double Acotar(double v)
        {
            if (double.IsNaN(v)) return 0;
            return v < 0 ? 0 : (v > 1 ? 1 : v);
        }
    }
}
