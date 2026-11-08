namespace CorreccionCromatica;
using Cromatico.Entidades;
public static class CorreccionLMS
{
    const double factor_base = 0.5;
    public static ColorLMS Corregir(ColorLMS color, TipoDeficiencia deficiencia, 
    double severidad,double intensidad,double errorR,double errorG,double errorB)
    {
        if(color == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ColorLMS es nulo."),
             "CorreccionLMS.Corregir");
            return new ColorLMS(0,0,0);
        }
        if(deficiencia == TipoDeficiencia.Ninguna || deficiencia == TipoDeficiencia.Indeterminado)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Deficiencia nula o indeterminada."),
             "CorreccionLMS.Corregir");
            return color;
        }
        if(severidad < 0.0 || severidad > 1.0)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Severidad fuera de rango (0.0 a 1.0)."),
             "CorreccionLMS.Corregir");
            return color;
        }
        if(intensidad < 0.0 || intensidad > 1.0)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Intensidad fuera de rango (0.0 a 1.0)."),
             "CorreccionLMS.Corregir");
            return color;
        }
        if(errorR < -255.0 || errorR > 255.0 || errorG < -255.0 || errorG > 255.0 || 
        errorB < -255.0 || errorB > 255.0)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Error de color fuera de rango (-255 a 255)."),
             "CorreccionLMS.Corregir");
            return color;
        }
        try
        {
            severidad = Math.Clamp(severidad, 0.0, 1.0);
            double factor= intensidad * severidad * factor_base;

            switch (deficiencia)
            {
                case TipoDeficiencia.Protan:
                    return new ColorLMS(color.l,
                    color.m + (errorR * factor),
                    color.s + (errorR * factor * 0.25));
                    

                case TipoDeficiencia.Deutan:
                    return new ColorLMS(color.l + (errorG * factor ),
                    color.m,
                    color.s + (errorG * factor * 0.25));

                case TipoDeficiencia.Tritan:
                    return new ColorLMS(color.l + (errorB * factor * 0.25),
                    color.m + (errorB * factor),
                    color.s);

                default:
                    return color;
            }
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"CorreccionLMS.Corregir");
            return new ColorLMS(0,0,0);
        }
        
    }
   
}