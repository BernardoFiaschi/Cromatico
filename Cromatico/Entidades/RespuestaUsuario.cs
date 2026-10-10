public class RespuestaUsuario
{
    public RespuestaUsuario(){}
    private int NumeroLamina;
    private string Respuesta;

    public int numeroLamina{
        get{return NumeroLamina;}
        set{NumeroLamina=value;}
    }
    public string respuesta{
        get{return Respuesta;}
        set{Respuesta=value;}
    }
}