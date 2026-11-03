namespace Cromatico.Modelos
{
    public class AsociacionAplicacion
    {
        // Nombre del proceso en minúsculas y sin extensión (ej. "winword").
        public string Proceso { get; set; } = string.Empty;
        public string Ruta { get; set; } = string.Empty;
        public string PerfilId { get; set; } = string.Empty;
    }
}
