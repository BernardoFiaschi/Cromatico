public class ResultadoTest
{
    public ResultadoTest(){}
    private TipoDeficiencia Deficiencia;
    private double Severidad;
    private int Aciertos;
    private int Errores;
    private double Confianza;
    private NivelSeveridad SeveridadNivel;
    public TipoDeficiencia deficiencia{
        get{return Deficiencia;}
        set{Deficiencia=value;}
    }
    public double severidad{
        get{return Severidad;}
        set{Severidad=value;}
    }
    public int aciertos{
        get{return Aciertos;}
        set{Aciertos=value;}
    }
    public int errores{
        get{return Errores;}
        set{Errores=value;}
    }
    public double confianza{
        get{return Confianza;}
        set{Confianza=value;}
    }
    public NivelSeveridad severidadNivel
    {
        get {return SeveridadNivel;}
        set {SeveridadNivel=value;}
    }
}