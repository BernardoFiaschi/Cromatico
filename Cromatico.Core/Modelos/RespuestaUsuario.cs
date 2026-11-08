namespace Cromatico.Modelos
{
    public class RespuestaUsuario
    {
        private int NumeroLamina;
        private string Respuesta;
        public RespuestaUsuario(){}

        public int numeroLamina
        {
            get{return NumeroLamina;}
            set{NumeroLamina=value;}
        }
        public string respuesta
        {
            get{return Respuesta;}
            set{Respuesta=value;}
        }
    }
}