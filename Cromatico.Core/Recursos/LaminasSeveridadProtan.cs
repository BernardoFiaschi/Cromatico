using Cromatico.Modelos;
namespace Cromatico.Recursos
{
    public static class LaminasSeveridadProtan
    {
        public static List<LaminaSeveridad> obtenerLaminasSeveridadProtan()
        {
            List<LaminaSeveridad> laminas = new List<LaminaSeveridad>();

            LaminaSeveridad lamina1 = new LaminaSeveridad();
            lamina1.numero = 1;
            lamina1.respuestaCorrecta = "";
            lamina1.dificultad = PuntosDificultadLaminaSeveridad.facil;
            lamina1.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina1.imagen = "Severidad1";
            laminas.Add(lamina1);

            LaminaSeveridad lamina2 = new LaminaSeveridad();
            lamina2.numero = 2;
            lamina2.respuestaCorrecta = "";
            lamina2.dificultad = PuntosDificultadLaminaSeveridad.facil;
            lamina2.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina2.imagen = "Severidad2";
            laminas.Add(lamina2);

            return laminas;
        }
    }
}