using Cromatico.Entidades;
namespace CorreccionCromatica;

public static class MatricesMachado{

    public static Matriz3x3 ObtenerProtan100()
    {
        return new Matriz3x3(
            0.152286, 1.052583, -0.204868,
            0.114503, 0.786281, 0.099216,
            -0.003882, -0.048116, 1.051998);
    }
    public static Matriz3x3 ObtenerDeutan100()
{
    return new Matriz3x3(
        0.367322, 0.860646, -0.227968,
        0.280085, 0.672501,  0.047413,
       -0.011820, 0.042940,  0.968881
    );
}
public static Matriz3x3 ObtenerTritan100()
{
    return new Matriz3x3(
         1.255528, -0.076749, -0.178779,
        -0.078411,  0.930809,  0.147602,
         0.004733,  0.691367,  0.303900
    );
}
    public static Matriz3x3 Obtener(TipoDeficiencia deficiencia, double sveridad)
    {
        var matriz = new Matriz3x3();
        switch (deficiencia)
        {
            case TipoDeficiencia.Protan:
            
        matriz.m11=0;
        matriz.m12=0;
        matriz.m13=0;
        matriz.m21=0;
        matriz.m22=0;
        matriz.m23=0;
        matriz.m31=0;
        matriz.m32=0;
        matriz.m33=0;
        return matriz;
        break;
        case TipoDeficiencia.Deutan:
        matriz.m11=0;
        matriz.m12=0;
        matriz.m13=0;
        matriz.m21=0;
        matriz.m22=0;
        matriz.m23=0;
        matriz.m31=0;
        matriz.m32=0;
        matriz.m33=0;
        return matriz;
        break;
        case TipoDeficiencia.Tritan:
        matriz.m11=0;
        matriz.m12=0;
        matriz.m13=0;
        matriz.m21=0;
        matriz.m22=0;
        matriz.m23=0;
        matriz.m31=0;
        matriz.m32=0;
        matriz.m33=0;
        return matriz;
        break;
        default:
        matriz.m11=0;
        matriz.m12=0;
        matriz.m13=0;
        matriz.m21=0;
        matriz.m22=0;
        matriz.m23=0;
        matriz.m31=0;
        matriz.m32=0;
        matriz.m33=0;
        return matriz;
        break;
        }        
    }
}