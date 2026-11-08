namespace CorreccionCromatica;
using Cromatico.Entidades;
public class ConfiguracionCorreccion
{
     private TipoDeficiencia Deficiencia;

    private double Severidad;

    private double Intensidad;
    public ConfiguracionCorreccion(TipoDeficiencia deficiencia, double severidad, double intensidad)
    {
        Deficiencia=deficiencia;
        Severidad=severidad;
        Intensidad=intensidad;
    }

    public TipoDeficiencia deficiencia
    {
        get{return Deficiencia;}
        set{Deficiencia=value;}
    }
    public double severidad
    {
        get{return Severidad;}
        set{Severidad=value;}
    }
    public double intensidad
    {
        get{return Intensidad;}
        set{Intensidad=value;}
    }
}