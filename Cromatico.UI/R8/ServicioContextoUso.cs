using Cromatico.Entidades;
namespace Cromatico.UI;

public static class ServicioContextoUso
{

    public static PerfilCorreccion? ObtenerPerfil(InfoProceso info,IEnumerable<PerfilCorreccion> perfiles)
    {
        try
        {
            
            if(info == null)
            {
                GuardarExcepcion.registrarExcepcion(
                new ArgumentNullException(nameof(info)),
                "ServicioContextoUso.ObtenerPerfil");

                return null;
            }
            if(perfiles == null)
            {
                GuardarExcepcion.registrarExcepcion(
                new ArgumentNullException(nameof(perfiles)),
                "ServicioContextoUso.ObtenerPerfil");

                return null;
            }
            if(string.IsNullOrWhiteSpace(info.Nombre))
            {
                GuardarExcepcion.registrarExcepcion(new Exception(
                "El nombre del proceso está vacío."),"ServicioContextoUso.ObtenerPerfil");
                return null;
            }
            string nombre =info.Nombre.Trim().ToLowerInvariant();
            var Contextos = ContextosPredeterminados.Obtener();
            
            foreach(var contexto in Contextos)
                {
                    bool coincide =contexto.Aplicaciones.Any(palabra =>nombre.Contains(
                        palabra.ToLowerInvariant()));

                if(coincide)
                    {
                       var perfil =perfiles.FirstOrDefault(p => p.Nombre.Contains(
                        contexto.Nombre,StringComparison.OrdinalIgnoreCase));
                        if(perfil == null)
                        {
                            GuardarExcepcion.registrarExcepcion(new Exception(
                            $"No se encontró un perfil para el contexto {contexto.Nombre}"),
                            "ServicioContextoUso.ObtenerPerfil");
                            return null;
                        }

                    return perfil;
                    }
                }

            return null;

        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex,"ServicioContextoUso.ObtenerPerfil");
            return null;
        }
        
    }
}