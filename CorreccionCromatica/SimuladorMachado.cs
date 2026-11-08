using System.Drawing;
using Cromatico.Entidades;
namespace CorreccionCromatica;

public static class SimuladorMachado
{
    public static ColorRGB Simular(ColorRGB color, TipoDeficiencia deficiencia)
    {
        if(color == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ColorRGB es nulo."),
             "SimuladorMachado.Simular");
            return new ColorRGB(0, 0, 0);
        }
        if(deficiencia == TipoDeficiencia.Ninguna || deficiencia == TipoDeficiencia.Indeterminado)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Deficiencia nula o indeterminada."),
             "SimuladorMachado.Simular");
            return color;
        }
        ColorLMS lms = ConversorLMS.RGBaLMS(color);
        Matriz3x3 matriz;
        try
        {
            switch (deficiencia)
            {
                case TipoDeficiencia.Protan:
                matriz=MatricesMachado.ObtenerProtan100();
                break;
                case TipoDeficiencia.Deutan:
                matriz=MatricesMachado.ObtenerDeutan100();
                break;
                case TipoDeficiencia.Tritan:
                matriz=MatricesMachado.ObtenerTritan100();
                break;
                default:
                return color;
            }
            ColorLMS transformado = AplicadorMatriz.Aplicar(lms,matriz);
            ColorRGB rgbSimulado= new ColorRGB(0,0,0);
            return rgbSimulado=ConversorLMS.LMSaRGB(transformado);
            }
        catch(Exception e)
        {
         GuardarExcepcion.registrarExcepcion(e,"SimuladorMachado.Simular");
          return new ColorRGB(0,0,0);
        }
       
    }
}