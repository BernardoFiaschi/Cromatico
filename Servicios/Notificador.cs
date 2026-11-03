namespace Cromatico.Servicios
{
    public enum TipoAviso
    {
        Info,
        Exito,
        Advertencia,
        Error
    }

    public class Notificador
    {
        public event Action<TipoAviso, string>? Aviso;

        public void Informar(string texto) => Aviso?.Invoke(TipoAviso.Info, texto);
        public void Exito(string texto) => Aviso?.Invoke(TipoAviso.Exito, texto);
        public void Advertir(string texto) => Aviso?.Invoke(TipoAviso.Advertencia, texto);
        public void Error(string texto) => Aviso?.Invoke(TipoAviso.Error, texto);
    }
}
