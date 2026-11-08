using System.Drawing;
using Cromatico.Entidades;

namespace CorreccionCromatica;

public static class SimuladorImagenMachado
{
    public static Bitmap SimularImagen(Bitmap imagen,TipoDeficiencia deficiencia)
    {
        if(imagen == null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("El objeto Bitmap es nulo."),
             "SimuladorImagenMachado.SimularImagen");
            return new Bitmap(1, 1);
        }
        if(deficiencia == TipoDeficiencia.Ninguna || deficiencia == TipoDeficiencia.Indeterminado)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Deficiencia nula o indeterminada."),
             "SimuladorImagenMachado.SimularImagen");
            return new Bitmap(imagen.Width, imagen.Height);
        }
        try
        {
            Bitmap resultado =new Bitmap(imagen.Width,imagen.Height);

            for(int x = 0;x < imagen.Width;x++)
            {
                for(int y = 0;y < imagen.Height;y++)
                {
                    Color pixel =imagen.GetPixel(x,y);

                    ColorRGB original =new ColorRGB(pixel.R,pixel.G,pixel.B);

                    ColorRGB simulado =SimuladorMachado.Simular(original,deficiencia);

                    resultado.SetPixel(x,y,Color.FromArgb(simulado.r,simulado.g,simulado.b));
                }
            }

            return resultado;
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"SimuladorImagenMachado.SimularImagen");
            return new Bitmap(imagen.Width,imagen.Height);
        }
        
    }
}