namespace Cromatico.Modelos
{
    public class InfoProceso
    {
        public int Pid { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ruta { get; set; }
        public bool EsPropia { get; set; }
    }
}
