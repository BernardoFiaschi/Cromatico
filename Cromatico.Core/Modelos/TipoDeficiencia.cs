namespace Cromatico.Modelos
{
    public enum TipoDeficiencia
    {
        Protan,
        Deutan,
        Tritan,
        Indeterminado,
        Ninguna
    }

    public enum EjeCorreccion
    {
        RojoVerde,
        AzulAmarillo
    }

    public static class TipoDeficienciaExtensiones
    {
        public static EjeCorreccion Eje(this TipoDeficiencia tipo)
            => tipo == TipoDeficiencia.Tritan ? EjeCorreccion.AzulAmarillo : EjeCorreccion.RojoVerde;

        public static string Nombre(this TipoDeficiencia tipo) => tipo switch
        {
            TipoDeficiencia.Protan => "Protan (rojo)",
            TipoDeficiencia.Deutan => "Deután (verde)",
            _ => "Tritan (azul)"
        };

        public static string Nombre(this EjeCorreccion eje)
            => eje == EjeCorreccion.RojoVerde ? "Rojo / Verde" : "Azul / Amarillo";
    }
}
