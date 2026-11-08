using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cromatico.Entidades;
public class ConfiguracionApp
    {
        public int Version { get; set; } = 1;

        public List<PerfilCorreccion> Perfiles { get; set; } = new();
        public string PerfilActivoId { get; set; } = PerfilesPredeterminados.IdNatural;

        public bool AutoPerfilesActivo { get; set; } = true;

        public List<AsociacionAplicacion> Asociaciones { get; set; } = new();

        public DateTime UltimaModificacionUtc { get; set; } = DateTime.MinValue;

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Extras { get; set; }

        public static readonly JsonSerializerOptions OpcionesJson = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static ConfiguracionApp Predeterminada() => new()
        {
            Perfiles = PerfilesPredeterminados.Crear(),
            PerfilActivoId = PerfilesPredeterminados.IdNatural
        };

        public PerfilCorreccion? BuscarPerfil(string? id)
            => id == null ? null : Perfiles.FirstOrDefault(p => p.Id == id);

        public bool EsValida(out string motivo)
        {
            if (Perfiles == null || Perfiles.Count == 0) { motivo = "no contiene perfiles"; return false; }
            if (Perfiles.Any(p => p == null || string.IsNullOrWhiteSpace(p.Id) || string.IsNullOrWhiteSpace(p.Nombre)))
            { motivo = "hay perfiles sin identificador o sin nombre"; return false; }
            if (Perfiles.Select(p => p.Id).Distinct().Count() != Perfiles.Count)
            { motivo = "hay perfiles duplicados"; return false; }
            if (Perfiles.Any(p => !p.ParametrosValidos()))
            { motivo = "hay perfiles con parámetros fuera de rango"; return false; }
            if (BuscarPerfil(PerfilActivoId) == null)
            { motivo = "el perfil activo no existe"; return false; }
            motivo = string.Empty;
            return true;
        }

        public void AsegurarPredefinidos()
        {
            Perfiles ??= new List<PerfilCorreccion>();
            foreach (var p in PerfilesPredeterminados.Crear())
                if (BuscarPerfil(p.Id) == null) Perfiles.Insert(0, p);
            if (BuscarPerfil(PerfilActivoId) == null) PerfilActivoId = PerfilesPredeterminados.IdNatural;
            Asociaciones ??= new List<AsociacionAplicacion>();
        }

        public ConfiguracionApp Clonar()
        {
            var json = JsonSerializer.Serialize(this, OpcionesJson);
            return JsonSerializer.Deserialize<ConfiguracionApp>(json, OpcionesJson)!;
        }

        public void CopiarDesde(ConfiguracionApp otra)
        {
            var origen = otra.Clonar();
            foreach (var prop in typeof(ConfiguracionApp).GetProperties())
                if (prop.CanWrite && prop.CanRead && prop.GetIndexParameters().Length == 0)
                    prop.SetValue(this, prop.GetValue(origen));
        }
    }
