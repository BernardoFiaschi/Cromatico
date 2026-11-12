using System.Globalization;
using Cromatico.Modelos;

namespace Cromatico.Datos
{
    // Microsoft.Data.Sqlite ejecuta los métodos async de forma sincrónica, por eso se usan los sincrónicos.
    public class RepositorioUsuariosSqlite : IRepositorioUsuarios
    {
        private readonly BaseDatosSqlite _db;

        public RepositorioUsuariosSqlite(BaseDatosSqlite db)
        {
            _db = db;
        }

        public Task<Usuario?> BuscarUsuarioAsync(string email)
        {
            using var conexion = _db.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                SELECT Email, PasswordHash, Salt, FechaRegistro
                FROM Usuarios
                WHERE Email = $email";
            comando.Parameters.AddWithValue("$email", email.Trim());

            using var lector = comando.ExecuteReader();
            if (!lector.Read()) return Task.FromResult<Usuario?>(null);

            var usuario = new Usuario
            {
                Email = lector.GetString(0),
                PasswordHash = lector.GetString(1),
                Salt = lector.GetString(2),
                FechaRegistro = DateTime.Parse(lector.GetString(3), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            };
            return Task.FromResult<Usuario?>(usuario);
        }

        public Task GuardarUsuarioAsync(Usuario usuario)
        {
            using var conexion = _db.Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = @"
                INSERT INTO Usuarios (Email, PasswordHash, Salt, FechaRegistro)
                VALUES ($email, $hash, $salt, $fecha)
                ON CONFLICT(Email) DO UPDATE SET
                    PasswordHash = excluded.PasswordHash,
                    Salt = excluded.Salt";
            comando.Parameters.AddWithValue("$email", usuario.Email.Trim());
            comando.Parameters.AddWithValue("$hash", usuario.PasswordHash);
            comando.Parameters.AddWithValue("$salt", usuario.Salt);
            comando.Parameters.AddWithValue("$fecha", usuario.FechaRegistro.ToString("O", CultureInfo.InvariantCulture));
            comando.ExecuteNonQuery();
            return Task.CompletedTask;
        }
    }
}
