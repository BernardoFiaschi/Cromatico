using System.Security.Cryptography;
using System.Text;
using Cromatico.Modelos;
using Cromatico.Validaciones;

namespace Cromatico.Servicios
{
    public class ServicioAutenticacion : IServicioAutenticacion
    {
        private const string PrefijoHash = "pbkdf2-sha256";
        private const int Iteraciones = 600_000;

        private readonly IAlmacenamientoLocal _almacenamiento;
        private readonly IServicioSincronizacion _sincronizacion;

        public ServicioAutenticacion(IAlmacenamientoLocal almacenamiento, IServicioSincronizacion sincronizacion)
        {
            _almacenamiento = almacenamiento;
            _sincronizacion = sincronizacion;
        }

        public ResultadoAutenticacion UsarSinCuenta()
        {
            return ResultadoAutenticacion.Ok("Sesión local iniciada sin cuenta.", sinCuenta: true);
        }

        public async Task<ResultadoAutenticacion> RegistrarseAsync(string email, string password)
        {
            email = (email ?? string.Empty).Trim();

            if (!ValidadorCredenciales.EmailValido(email))
                return ResultadoAutenticacion.Error("El correo electrónico tiene un formato inválido.", CodigoAuth.EmailInvalido);

            if (!ValidadorCredenciales.PasswordValida(password))
                return ResultadoAutenticacion.Error("La contraseña debe tener al menos 8 caracteres, incluyendo letras y números.", CodigoAuth.PasswordInvalida);

            var fallaRed = await ComprobarConexionAsync();
            if (fallaRed != null) return fallaRed;

            var existente = await _almacenamiento.BuscarUsuarioAsync(email);
            if (existente != null)
                return ResultadoAutenticacion.Error("Ya existe una cuenta registrada con ese correo.", CodigoAuth.CuentaExistente);

            var salt = GenerarSalt();
            var usuario = new Usuario
            {
                Email = email,
                Salt = salt,
                PasswordHash = HashPassword(password, salt),
                FechaRegistro = DateTime.UtcNow
            };

            await _almacenamiento.GuardarUsuarioAsync(usuario);

            return ResultadoAutenticacion.Ok("Cuenta creada correctamente.", usuario);
        }

        public async Task<ResultadoAutenticacion> IniciarSesionAsync(string email, string password)
        {
            email = (email ?? string.Empty).Trim();

            if (!ValidadorCredenciales.EmailValido(email))
                return ResultadoAutenticacion.Error("El correo electrónico tiene un formato inválido.", CodigoAuth.EmailInvalido);

            var fallaRed = await ComprobarConexionAsync();
            if (fallaRed != null) return fallaRed;

            var usuario = await _almacenamiento.BuscarUsuarioAsync(email);
            if (usuario == null)
                return ResultadoAutenticacion.Error("No existe una cuenta registrada con ese correo.", CodigoAuth.UsuarioInexistente);

            if (!VerificarPassword(password ?? string.Empty, usuario))
                return ResultadoAutenticacion.Error("La contraseña es incorrecta.", CodigoAuth.PasswordIncorrecta);

            return ResultadoAutenticacion.Ok("Inicio de sesión exitoso.", usuario);
        }

        private async Task<ResultadoAutenticacion?> ComprobarConexionAsync()
        {
            try
            {
                await _sincronizacion.VerificarConexionAsync();
                return null;
            }
            catch (SincronizacionException)
            {
                return ResultadoAutenticacion.Error(
                    "No se pudo conectar con el servidor para autenticar. Revisá tu conexión o continuá sin iniciar sesión.",
                    CodigoAuth.FalloRed);
            }
        }

        private static string GenerarSalt()
        {
            var bytes = new byte[16];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        internal static string HashPassword(string password, string salt)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password, Convert.FromBase64String(salt), Iteraciones, HashAlgorithmName.SHA256, 32);
            return $"{PrefijoHash}${Iteraciones}${Convert.ToBase64String(hash)}";
        }

        internal static bool VerificarPassword(string password, Usuario usuario)
        {
            try
            {
                if (usuario.PasswordHash.StartsWith(PrefijoHash + "$", StringComparison.Ordinal))
                {
                    var partes = usuario.PasswordHash.Split('$');
                    if (partes.Length != 3 || !int.TryParse(partes[1], out var iteraciones)) return false;

                    var esperado = Convert.FromBase64String(partes[2]);
                    var calculado = Rfc2898DeriveBytes.Pbkdf2(
                        password, Convert.FromBase64String(usuario.Salt), iteraciones, HashAlgorithmName.SHA256, esperado.Length);
                    return CryptographicOperations.FixedTimeEquals(esperado, calculado);
                }

                var legado = SHA256.HashData(Encoding.UTF8.GetBytes(password + usuario.Salt));
                var guardado = Convert.FromBase64String(usuario.PasswordHash);
                return CryptographicOperations.FixedTimeEquals(legado, guardado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
