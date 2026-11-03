using System.Text.Json.Serialization;

namespace Cromatico.Modelos
{
    public class PerfilCorreccion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Nombre { get; set; } = string.Empty;
        public bool EsPredefinido { get; set; }
        public TipoDeficiencia Tipo { get; set; } = TipoDeficiencia.Deutan;
        public double Severidad { get; set; } = 0.6;
        public double Intensidad { get; set; } = 0.7;

        [JsonIgnore]
        public EjeCorreccion Eje => Tipo.Eje();

        [JsonIgnore]
        public string Resumen =>
            $"{Eje.Nombre()} · {Tipo.Nombre()} · Severidad {Severidad:P0} · Intensidad {Intensidad:P0}";

        public PerfilCorreccion Clonar() => (PerfilCorreccion)MemberwiseClone();

        public static bool ValorValido(double v) => !double.IsNaN(v) && !double.IsInfinity(v) && v >= 0 && v <= 1;

        public bool ParametrosValidos() => ValorValido(Severidad) && ValorValido(Intensidad);
    }
}
