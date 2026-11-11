using System.Linq;
using System.Text.RegularExpressions;

namespace Cromatico.Validaciones
{
    public static class ValidadorCredenciales
    {
        private static readonly Regex PatronEmail = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static bool EmailValido(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && PatronEmail.IsMatch(email);
        }

        public static bool PasswordValida(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool tieneLetra = password.Any(char.IsLetter);
            bool tieneNumero = password.Any(char.IsDigit);
            return tieneLetra && tieneNumero;
        }
    }
}
