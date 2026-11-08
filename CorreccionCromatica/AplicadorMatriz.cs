using Cromatico.Entidades;
namespace CorreccionCromatica;
public  class AplicadorMatriz
{
    public static ColorLMS Aplicar(ColorLMS color,Matriz3x3 matriz)
    {
        if(matriz==null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Matriz nula"),"AplicadorMatriz.Aplicar");
            return new ColorLMS(0,0,0);
        }
        if(color==null)
        {
            GuardarExcepcion.registrarExcepcion(new Exception("Color nulo"),"AplicadorMatriz.Aplicar");
            return new ColorLMS(0,0,0);
        }
        try
        {
            double nuevoL= matriz.m11 * color.l + matriz.m12 * color.m + matriz.m13 * color.s;
            double nuevoM= matriz.m21 * color.l + matriz.m22 * color.m + matriz.m23 * color.s;
            double nuevoS= matriz.m31 * color.l + matriz.m32 * color.m + matriz.m33 * color.s;
            return new ColorLMS(nuevoL,nuevoM,nuevoS);
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"AplicadorMatriz.Aplicar");
            return new ColorLMS(0,0,0);
        }
        
    }
}