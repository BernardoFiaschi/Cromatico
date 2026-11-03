namespace Cromatico.Modelos
{
    public class ResultadoOperacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;

        public static ResultadoOperacion Ok(string mensaje = "")
            => new ResultadoOperacion { Exitoso = true, Mensaje = mensaje };

        public static ResultadoOperacion Error(string mensaje, string codigo = "")
            => new ResultadoOperacion { Exitoso = false, Mensaje = mensaje, Codigo = codigo };
    }

    public class ResultadoOperacion<T> : ResultadoOperacion
    {
        public T? Valor { get; set; }

        public static ResultadoOperacion<T> Ok(T valor, string mensaje = "")
            => new ResultadoOperacion<T> { Exitoso = true, Valor = valor, Mensaje = mensaje };

        public new static ResultadoOperacion<T> Error(string mensaje, string codigo = "")
            => new ResultadoOperacion<T> { Exitoso = false, Mensaje = mensaje, Codigo = codigo };
    }
}
