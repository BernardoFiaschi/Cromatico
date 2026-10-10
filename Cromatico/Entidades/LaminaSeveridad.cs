public class LaminaSeveridad
{
    public LaminaSeveridad(){}
    private int Numero;
    private string RespuestaCorrecta;
    private PuntosDificultadLaminaSeveridad  Dificultad;
    private TipoDeficiencia Deficiencia;
    private string Imagen;

    public int numero{
        get {return Numero;}
        set {Numero=value;}
    }
    public string respuestaCorrecta
    {
        get {return RespuestaCorrecta;}
        set {RespuestaCorrecta=value;}
    }
    public PuntosDificultadLaminaSeveridad dificultad
    {
        get {return Dificultad;}
        set {Dificultad=value;}
    }
    public TipoDeficiencia tipoDeficiencia{
        get {return Deficiencia;}
        set {Deficiencia=value;}
    }
    public string imagen
    {
        get {return Imagen;}
        set {Imagen=value;}
    }

}