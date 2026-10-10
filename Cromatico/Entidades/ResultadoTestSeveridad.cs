public class ResultadoTestSeveridad
{
    public ResultadoTestSeveridad(){}
    private int PuntajeObtenido;
    private int PuntajeMaximo;
    private double Porcentaje;
    private NivelSeveridad Severidad;
    private TipoDeficiencia Deficiencia;

    public int puntajeObtenido
    {
        get {return PuntajeObtenido;}
        set {PuntajeObtenido=value;}
    }
    public int puntajeMaximo
    {
        get {return PuntajeMaximo;}
        set {PuntajeMaximo=value;}
    }
    public double porcentaje
    {
        get {return Porcentaje;}
        set {Porcentaje=value;}
    }
    public NivelSeveridad severidad
    {
        get {return Severidad;}
        set {Severidad=value;}
    }
    public TipoDeficiencia deficiencia
    {
        get {return Deficiencia;}
        set {Deficiencia=value;}
    }

}