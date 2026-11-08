namespace Cromatico.Entidades;
    public enum CodigoAuth
    {
        Ninguno,
        EmailInvalido,
        PasswordInvalida,
        CuentaExistente,
        UsuarioInexistente,
        PasswordIncorrecta,
        FalloRed
    }

    public class ResultadoAutenticacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public CodigoAuth Codigo { get; set; } = CodigoAuth.Ninguno;
        public bool ModoSinCuenta { get; set; }
        public Usuario? UsuarioActual { get; set; }

        public static ResultadoAutenticacion Ok(string mensaje, Usuario? usuario = null, bool sinCuenta = false)
            => new ResultadoAutenticacion { Exitoso = true, Mensaje = mensaje, UsuarioActual = usuario, ModoSinCuenta = sinCuenta };

        public static ResultadoAutenticacion Error(string mensaje, CodigoAuth codigo = CodigoAuth.Ninguno)
            => new ResultadoAutenticacion { Exitoso = false, Mensaje = mensaje, Codigo = codigo };
    }
