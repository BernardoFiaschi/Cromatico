namespace CorreccionCromatica;

public class ColorRGB
{
    private int R;
    private int G;
    private int B;
    public ColorRGB(int r,int g, int b)
    {
        R=r;
        G=g;
        B=b;
    }
    public ColorRGB()
    {
        R=0;
        G=0;
        B=0;
    }
    public int r{
        get{return R;}
        set{R=value;}
    }
    public int g{
        get{return G;}
        set{G=value;}
    }
    public int b{
        get{return B;}
        set{B=value;}
    }

}