using Cromatico.Entidades;
namespace CorreccionCromatica;

public static class ConversorLMS
{
    public static ColorLMS RGBaLMS(ColorRGB color)
    {
        if(color == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ColorRGB no puede ser nulo."),
            "ConversorLMS.RGBaLMS");
            return new ColorLMS(0,0,0);
        }
        try
        {
            double r = color.r;
            double g = color.g;
            double b = color.b;

            double l = 0.31399022 * r + 0.63951294 * g + 0.04649755 * b;

            double m = 0.15537241 * r + 0.75789446 * g + 0.08670142 * b;

            double s = 0.01775239 * r + 0.10944209 * g + 0.87256922 * b;
            return new ColorLMS(l,m,s);
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"ConversorLMS.RGBaLMS");
            return new ColorLMS(0,0,0);
        }

    }
    public static ColorRGB LMSaRGB(ColorLMS color)
    {
        if (color == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ColorLMS no puede ser nulo."),
            "ConversorLMS.LMSaRGB");
            return new ColorRGB(0,0,0);
        }
        try
        {
            double r = 5.47221206 * color.l + -4.64196010 * color.m + 0.16963708 * color.s;
            double g = -1.12524190 * color.l + 2.29317094 * color.m + -0.16789520 * color.s;
            double b = 0.02980165 * color.l + -0.19318073 * color.m + 1.16364789 * color.s;
            int rojo = Math.Clamp((int)Math.Round(r), 0, 255);
            int verde = Math.Clamp((int)Math.Round(g), 0, 255);
            int azul = Math.Clamp((int)Math.Round(b), 0, 255);
            var colorRgb = new ColorRGB(rojo,verde,azul);
            return colorRgb;
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"ConversorLMS.LMSaRGB");
            return new ColorRGB(0,0,0);
        }
        
    }
}