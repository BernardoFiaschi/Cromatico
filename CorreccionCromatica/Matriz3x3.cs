namespace CorreccionCromatica;
public class Matriz3x3
{
    private double M11;
    private double M12;
    private double M13;
    private double M21;
    private double M22;
    private double M23;
    private double M31;
    private double M32;
    private double M33;
    public Matriz3x3(){}

    public Matriz3x3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M31 = m31;
        M32 = m32;
        M33 = m33;
    }
    public double m11
    {
        get{return M11;}
        set{M11=value;}
    }
    public double m12
    {
        get{return M12;}
        set{M12=value;}
    }
    public double m13
    {
        get{return M13;}
        set{M13=value;}
    }
    public double m21
    {
        get{return M21;}
        set{M21=value;}
    }
    public double m22
    {
        get{return M22;}
        set{M22=value;}
    }
    public double m23
    {
        get{return M23;}
        set{M23=value;}
    }
    public double m31
    {
        get{return M31;}
        set{M31=value;}
    }
    public double m32
    {
        get{return M32;}
        set{M32=value;}
    }
    public double m33
    {
        get{return M33;}
        set{M33=value;}
    }

}