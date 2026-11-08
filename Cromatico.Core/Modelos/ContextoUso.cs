namespace Cromatico.Modelos
{
    public class ContextoUso
    {
        public string Nombre { get; set; }
        public List<string> Aplicaciones { get; set; } = new List<string>();

    }
}