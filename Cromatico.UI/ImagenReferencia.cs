namespace Cromatico.UI;
    public static class ImagenReferencia{
        public static CapturaPantalla Generar(int ancho = 640, int alto = 360)
        {
            var px = new byte[ancho * alto * 4];

            int franjaEspectro = alto * 38 / 100;
            int franjaMuestras = alto * 32 / 100;
            int franjaDegradados = alto - franjaEspectro - franjaMuestras;

            var muestras = new (double r, double g, double b)[]
            {
                (0.90, 0.10, 0.10), (0.95, 0.55, 0.10), (0.95, 0.85, 0.15), (0.20, 0.70, 0.25),
                (0.10, 0.65, 0.60), (0.15, 0.35, 0.90), (0.55, 0.25, 0.80), (0.85, 0.30, 0.65)
            };

            for (int y = 0; y < alto; y++)
                for (int x = 0; x < ancho; x++)
                {
                    double r, g, b;
                    double tx = x / (double)(ancho - 1);

                    if (y < franjaEspectro)
                    {
                        double v = 1.0 - 0.45 * (y / (double)franjaEspectro);
                        (r, g, b) = Hsv(tx * 300.0, 0.85, v);
                    }
                    else if (y < franjaEspectro + franjaMuestras)
                    {
                        int i = Math.Min(muestras.Length - 1, (int)(tx * muestras.Length));
                        (r, g, b) = muestras[i];

                        double dentro = tx * muestras.Length - i;
                        if (dentro < 0.04 || dentro > 0.96) (r, g, b) = (0.08, 0.08, 0.09);
                    }
                    else
                    {
                        bool arriba = y < franjaEspectro + franjaMuestras + franjaDegradados / 2;
                        (r, g, b) = arriba
                            ? (0.9 * (1 - tx) + 0.1 * tx, 0.1 * (1 - tx) + 0.8 * tx, 0.15)
                            : (0.1 * (1 - tx) + 0.95 * tx, 0.2 * (1 - tx) + 0.9 * tx, 0.9 * (1 - tx) + 0.1 * tx);
                    }

                    int o = (y * ancho + x) * 4;
                    px[o] = ABajo(b); px[o + 1] = ABajo(g); px[o + 2] = ABajo(r); px[o + 3] = 255;
                }

            return new CapturaPantalla { Ancho = ancho, Alto = alto, Bgra = px };
        }

        private static byte ABajo(double v) => (byte)Math.Clamp((int)Math.Round(v * 255), 0, 255);

        private static (double r, double g, double b) Hsv(double h, double s, double v)
        {
            h = (h % 360 + 360) % 360;
            double c = v * s, hp = h / 60.0, x = c * (1 - Math.Abs(hp % 2 - 1)), m = v - c;
            (double r, double g, double b) = hp switch
            {
                < 1 => (c, x, 0.0),
                < 2 => (x, c, 0.0),
                < 3 => (0.0, c, x),
                < 4 => (0.0, x, c),
                < 5 => (x, 0.0, c),
                _ => (c, 0.0, x)
            };
            return (r + m, g + m, b + m);
        }
    }

