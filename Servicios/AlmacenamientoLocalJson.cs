using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class AlmacenamientoLocalJson : IAlmacenamientoLocal
    {
        private readonly string _rutaUsuarios;

        public AlmacenamientoLocalJson(string? carpetaDatos = null)
        {
            var carpeta = carpetaDatos ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cromatico");

            Directory.CreateDirectory(carpeta);
            _rutaUsuarios = Path.Combine(carpeta, "usuarios.json");
        }

        public async Task<Usuario?> BuscarUsuarioAsync(string email)
        {
            var usuarios = await CargarUsuariosAsync();
            return usuarios.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task GuardarUsuarioAsync(Usuario usuario)
        {
            var usuarios = await CargarUsuariosAsync();
            usuarios.RemoveAll(u => string.Equals(u.Email, usuario.Email, StringComparison.OrdinalIgnoreCase));
            usuarios.Add(usuario);

            var json = JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_rutaUsuarios, json);
        }

        private async Task<List<Usuario>> CargarUsuariosAsync()
        {
            if (!File.Exists(_rutaUsuarios)) return new List<Usuario>();

            var json = await File.ReadAllTextAsync(_rutaUsuarios);
            return JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
        }
    }
}
