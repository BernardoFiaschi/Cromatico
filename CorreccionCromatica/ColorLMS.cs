namespace CorreccionCromatica;
public class ColorLMS
{
    private double L;
    private double M;
    private double S;
    public ColorLMS(double l,double m, double s)
    {
        L=l;
        M=m;
        S=s;
    }
    public ColorLMS()
    {
        L=0;
        M=0;
        S=0;
    }

    public double l{
        get{return L;}
        set{L=value;}
    }
    public double m
    {
        get{return M;}
        set{M=value;}
    }
    public double s
    {
        get{return S;}
        set{S=value;}
    }
}