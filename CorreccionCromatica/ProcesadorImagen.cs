using System.Drawing;
using Cromatico.Entidades;
namespace CorreccionCromatica;

public static class ProcesadorImagen
{
    public static Bitmap CorregirImagen(Bitmap imagen, ConfiguracionCorreccion configuracion)
    {
        if(imagen == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto Bitmap es nulo."),
             "ProcesadorImagen.CorregirImagen");
            return new Bitmap(1, 1);
        }
        if(configuracion == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto ConfiguracionCorreccion es nulo."),
             "ProcesadorImagen.CorregirImagen");
            return new Bitmap(imagen.Width, imagen.Height);
        }
        try
        {
            Bitmap imagenCorregida = new Bitmap(imagen.Width, imagen.Height);

            for(int x = 0; x < imagen.Width; x++)
            {
                for(int y = 0; y < imagen.Height; y++)
                {
                    Color pixel = imagen.GetPixel(x,y);

                    ColorRGB colorOriginal =new ColorRGB(pixel.R,pixel.G,pixel.B);

                    ColorRGB colorCorregido =MotorCorreccion.corregir(colorOriginal,configuracion);

                    Color nuevoColor =Color.FromArgb(colorCorregido.r,colorCorregido.g,
                    colorCorregido.b);

                    imagenCorregida.SetPixel(x,y,nuevoColor);
                }
            }

            return imagenCorregida;
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"ProcesadorImagen.CorregirImagen");
            return new Bitmap(imagen.Width, imagen.Height);
        }
        
    }

}