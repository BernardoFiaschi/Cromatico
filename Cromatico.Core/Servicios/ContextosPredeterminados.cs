using Cromatico.Modelos;
namespace Cromatico.Servicios
{
    public static class ContextosPredeterminados
    {
        public static List<ContextoUso> Obtener()
        {
            return new List<ContextoUso>
        {
            new ContextoUso
            {
                Nombre = "Oficina",
                Aplicaciones =
                {
                    "word",
                    "excel",
                    "powerpnt",
                    "outlook",
                    "onenote",
                    "acrord32"
                }
            },

            new ContextoUso
            {
                Nombre = "Multimedia",
                Aplicaciones =
                {
                    "vlc",
                    "spotify",
                    "netflix",
                    "disney",
                    "player"
                }
            },

            new ContextoUso
            {
                Nombre = "Gaming",
                Aplicaciones =
                {
                    "steam",
                    "riot",
                    "epicgames",
                    "battle",
                    "ubisoft",
                    "game"
                }
            }
        };
        }
    }
}