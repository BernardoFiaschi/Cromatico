namespace Cromatico.Entidades;
    public class Usuario
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }

