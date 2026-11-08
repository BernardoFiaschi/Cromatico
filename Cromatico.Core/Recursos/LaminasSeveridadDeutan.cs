using Cromatico.Modelos;
namespace Cromatico.Recursos
{
    public static class LaminasSeveridadDeutan
    {
        public static List<LaminaSeveridad> obtenerLaminasSeveridadDeutan()
        {
            List<LaminaSeveridad> laminas = new List<LaminaSeveridad>();

            LaminaSeveridad lamina1 = new LaminaSeveridad();
            lamina1.numero = 1;
            lamina1.respuestaCorrecta = "";
            lamina1.dificultad = PuntosDificultadLaminaSeveridad.facil;
            lamina1.tipoDeficiencia = TipoDeficiencia.Deutan;
            lamina1.imagen = "Severidad1";
            laminas.Add(lamina1);

            LaminaSeveridad lamina2 = new LaminaSeveridad();
            lamina1.numero = 2;
            lamina1.respuestaCorrecta = "";
            lamina1.dificultad = PuntosDificultadLaminaSeveridad.facil;
            lamina1.tipoDeficiencia = TipoDeficiencia.Deutan;
            lamina1.imagen = "Severidad2";
            laminas.Add(lamina2);

            return laminas;
        }

    }
}