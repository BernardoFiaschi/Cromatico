using System.Collections;
using Cromatico.Entidades;
namespace CorreccionCromatica;

public static class MotorCorreccion
{
    public static ColorRGB corregir(ColorRGB original, ConfiguracionCorreccion configuracion)
    {
        if(original==null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ColorRGB es nulo."),
             "MotorCorreccion.corregir");
            return new ColorRGB(0,0,0);
        }
        if(configuracion==null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ConfiguracionCorreccion es nulo."),
             "MotorCorreccion.corregir");
            return new ColorRGB(0,0,0);
        }
        try
        {
            ColorRGB rgbRimulado= SimuladorMachado.Simular(original,configuracion.deficiencia);
            double errorR=original.r - rgbRimulado.r;
            double errorG=original.g - rgbRimulado.g;
            double errorB=original.b - rgbRimulado.b;
            ColorLMS lms= ConversorLMS.RGBaLMS(original);
            ColorLMS lmsCorregido= CorreccionLMS.Corregir(lms,configuracion.deficiencia,
            configuracion.severidad,configuracion.intensidad,errorR,errorG,errorB);


            return ConversorLMS.LMSaRGB(lmsCorregido);
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"MotorCorreccion.corregir");
            return new ColorRGB(0,0,0);
        }
        
        
    }
    
}