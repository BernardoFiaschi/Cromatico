
using Cromatico.Entidades;
namespace Cromatico.UI;

public class ServicioAsociaciones
{
    private readonly ServicioConfiguracion _almacen;

    public ServicioAsociaciones(ServicioConfiguracion almacen)
    {
        _almacen = almacen;
    }
    public IReadOnlyList<AsociacionAplicacion>ObtenerTodas()
    {
        try
        {
            if (_almacen == null)
            {
                GuardarExcepcion.registrarExcepcion(new ArgumentNullException(nameof(_almacen)), "ServicioAsociaciones.ObtenerTodas");
                return new List<AsociacionAplicacion>();
            }
            return _almacen.Config.Asociaciones;
        }
        catch (Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex, "ServicioAsociaciones.ObtenerTodas");
            return new List<AsociacionAplicacion>();
        }
    
    }
    public ResultadoOperacion Asociar(string proceso,string ruta,string perfilId)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(proceso))
            {
                return ResultadoOperacion.Error("La aplicación seleccionada es inválida.");
            }
            if(string.IsNullOrWhiteSpace(ruta))
            {
                return ResultadoOperacion.Error("La ruta de la aplicación es inválida.");
            }

            if(string.IsNullOrWhiteSpace(perfilId))
            {
                return ResultadoOperacion.Error("Debe seleccionar un perfil.");
            }
            

            bool existe = _almacen.Config.Asociaciones.Any
            (a => string.Equals(a.Proceso,proceso,StringComparison.OrdinalIgnoreCase));

            if(existe)
            {
                return ResultadoOperacion.Error("La aplicación ya posee un perfil asociado.");
            }

            var perfil =_almacen.Config.BuscarPerfil(perfilId);

            if(perfil == null)
            {
                return ResultadoOperacion.Error(
                "El perfil seleccionado no existe.");
            }

            var resultado= _almacen.Modificar(c =>{
                c.Asociaciones.Add(new AsociacionAplicacion
                    {
                        Proceso = proceso,
                        Ruta = ruta,
                        PerfilId = perfilId
                    });},"guardar la asociación de aplicación");
            if (resultado.Exitoso)
            {
                resultado.Mensaje="Aplicacion asociada exitosamente!";
            }else resultado.Mensaje="Aplicacion no asociada.";
            
            return resultado;
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex, "ServicioAsociaciones.Asociar");
            return ResultadoOperacion.Error("Ocurrió un error al asociar la aplicación: " + ex.Message);
        }
    }
    public ResultadoOperacion Eliminar(string proceso)
    {
        try
        {
            var asociacion =_almacen.Config.Asociaciones.FirstOrDefault
            (a => string.Equals(a.Proceso,proceso,StringComparison.OrdinalIgnoreCase));

            if(asociacion == null)
            {
                return ResultadoOperacion.Error("La asociación no existe.");
            }

            var resultado = _almacen.Modificar(c =>{c.Asociaciones.RemoveAll(
                a => string.Equals(a.Proceso,proceso,StringComparison.OrdinalIgnoreCase));
                },"eliminar la asociación");
            if (resultado.Exitoso)
            {
                resultado.Mensaje="Asociacion eliminada exitosamente!";
            }else resultado.Mensaje="Asociacio no eliminada.";
            return resultado;
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex, "ServicioAsociaciones.Eliminar");
            return ResultadoOperacion.Error("Ocurrió un error al eliminar la asociación: " + ex.Message);
        }
        
    }

}