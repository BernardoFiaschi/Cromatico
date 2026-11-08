using System.Drawing;
using System.Windows.Forms;
using Cromatico.Entidades;

namespace CorreccionCromatica;

public static class CapturadorPantalla
{
    public static Bitmap Capturar()
    {
        try
        {
            Rectangle area =Screen.PrimaryScreen.Bounds;
            Bitmap captura =new Bitmap(area.Width,area.Height);

            using(Graphics g =Graphics.FromImage(captura)){
                g.CopyFromScreen(area.Left,area.Top,0,0,area.Size);
            }

            return captura;
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"CapturadorPantalla.Capturar");
            return new Bitmap(1,1);
        }
        
    }
}