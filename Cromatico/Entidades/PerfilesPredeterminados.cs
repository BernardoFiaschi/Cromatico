namespace Cromatico.Entidades;
public static class PerfilesPredeterminados
    {
        public const string IdNatural = "natural";
        public const string IdPreciso = "preciso";
        public const string IdGaming = "gaming";
        public const string IdMultimedia = "multimedia";
        public const string IdOficina = "oficina";

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
            },
            new PerfilCorreccion
            {
                Id = IdGaming, Nombre = "Perfil Gaming", EsPredefinido = true,
                Tipo = TipoDeficiencia.Deutan, Severidad = 0.7, Intensidad = 1.0 
            },
            new PerfilCorreccion
            {
                Id = IdMultimedia, Nombre = "Perfil Multimedia", EsPredefinido = true,
                Tipo = TipoDeficiencia.Deutan, Severidad = 0.7, Intensidad = 0.8 
            },
            new PerfilCorreccion
            {
                Id = IdOficina, Nombre = "Perfil Oficina", EsPredefinido = true,
                Tipo = TipoDeficiencia.Deutan, Severidad = 0.5, Intensidad = 0.4 
            }

        };
    }