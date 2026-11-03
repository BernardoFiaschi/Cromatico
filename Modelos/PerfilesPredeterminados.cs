namespace Cromatico.Modelos
{
    public static class PerfilesPredeterminados
    {
        public const string IdNatural = "natural";
        public const string IdPreciso = "preciso";

        public static List<PerfilCorreccion> Crear() => new()
        {
            new PerfilCorreccion
            {
                Id = IdNatural, Nombre = "Modo Natural", EsPredefinido = true,
                Tipo = TipoDeficiencia.Deutan, Severidad = 0.6, Intensidad = 0.5
            },
            new PerfilCorreccion
            {
                Id = IdPreciso, Nombre = "Modo Preciso", EsPredefinido = true,
                Tipo = TipoDeficiencia.Deutan, Severidad = 0.6, Intensidad = 1.0
            }
        };
    }
}
