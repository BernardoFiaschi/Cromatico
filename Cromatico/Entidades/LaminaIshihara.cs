public class LaminaIshihara
{
    public LaminaIshihara(){}
    private int Numero;
    private tipoLaminaIshihara Tipo;
    private string Imagen;
    private string RespuestaCorrecta;
    private string RespuestaRojaProtan;
    private string RespuestaVerdeDeutan;
    private string RespuestaMonocromatica;
    private string RespuestaDaltonica;

    public int numero{
        get{return Numero;}
        set{Numero=value;}
    }
    public tipoLaminaIshihara tipo{
        get{return Tipo;}
        set{Tipo=value;}
    }
    public string imagen{
        get{return Imagen;}
        set{Imagen=value;}
    }
    public string respuestaCorrecta{
        get{return RespuestaCorrecta;}
        set{RespuestaCorrecta=value;}
    }
    public string respuestaRojaProtan{
        get{return RespuestaRojaProtan;}
        set{RespuestaRojaProtan=value;}
    }
    public string respuestaVerdeDeutan{
        get{return RespuestaVerdeDeutan;}
        set{RespuestaVerdeDeutan=value;}
    }
    public string respuestaMonocromatica{
        get{return RespuestaMonocromatica;}
        set{RespuestaMonocromatica=value;}
    }
    public string respuestaDaltonica
    {
        get{return RespuestaDaltonica;}
        set {RespuestaDaltonica=value;}
    }
}