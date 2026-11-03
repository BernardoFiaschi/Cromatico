namespace Cromatico.Algoritmo
{
    public sealed class Matriz3
    {
        private readonly double[] _v;

        public Matriz3(params double[] valores)
        {
            if (valores == null || valores.Length != 9)
                throw new ArgumentException("Una matriz 3x3 necesita exactamente 9 valores.");
            _v = (double[])valores.Clone();
        }

        public double this[int fila, int columna] => _v[fila * 3 + columna];

        public static Matriz3 Identidad { get; } = new Matriz3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public static Matriz3 operator *(Matriz3 a, Matriz3 b)
        {
            var r = new double[9];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    double s = 0;
                    for (int k = 0; k < 3; k++) s += a[i, k] * b[k, j];
                    r[i * 3 + j] = s;
                }
            return new Matriz3(r);
        }

        public static Matriz3 operator +(Matriz3 a, Matriz3 b)
        {
            var r = new double[9];
            for (int i = 0; i < 9; i++) r[i] = a._v[i] + b._v[i];
            return new Matriz3(r);
        }

        public static Matriz3 operator -(Matriz3 a, Matriz3 b)
        {
            var r = new double[9];
            for (int i = 0; i < 9; i++) r[i] = a._v[i] - b._v[i];
            return new Matriz3(r);
        }

        public static Matriz3 operator *(double k, Matriz3 a)
        {
            var r = new double[9];
            for (int i = 0; i < 9; i++) r[i] = k * a._v[i];
            return new Matriz3(r);
        }

        public Matriz3 Mezclar(Matriz3 otra, double t) => (1 - t) * this + t * otra;

        public double[] Multiplicar(double[] vector)
        {
            var r = new double[3];
            for (int i = 0; i < 3; i++)
                r[i] = this[i, 0] * vector[0] + this[i, 1] * vector[1] + this[i, 2] * vector[2];
            return r;
        }

        public Matriz3 Inversa()
        {
            double a = this[0, 0], b = this[0, 1], c = this[0, 2];
            double d = this[1, 0], e = this[1, 1], f = this[1, 2];
            double g = this[2, 0], h = this[2, 1], i = this[2, 2];

            double det = a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g);
            if (Math.Abs(det) < 1e-12)
                throw new InvalidOperationException("La matriz no es invertible.");

            double inv = 1.0 / det;
            return new Matriz3(
                (e * i - f * h) * inv, (c * h - b * i) * inv, (b * f - c * e) * inv,
                (f * g - d * i) * inv, (a * i - c * g) * inv, (c * d - a * f) * inv,
                (d * h - e * g) * inv, (b * g - a * h) * inv, (a * e - b * d) * inv);
        }
    }
}
