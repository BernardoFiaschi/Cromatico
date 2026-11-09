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
            lamina1.respuestaCorrecta = "triangulo";
            lamina1.dificultad = PuntosDificultadLaminaSeveridad.facil;
            lamina1.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina1.imagen = "LaminasSeveridad/LaminaProtan1.png";
            laminas.Add(lamina1);

            LaminaSeveridad lamina2 = new LaminaSeveridad();
            lamina2.numero = 2;
            lamina2.respuestaCorrecta = "triangulo";
            lamina2.dificultad = PuntosDificultadLaminaSeveridad.facil;
            lamina2.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina2.imagen = "LaminasSeveridad/LaminaProtan2.png";
            laminas.Add(lamina2);

            LaminaSeveridad lamina3 = new LaminaSeveridad();
            lamina3.numero = 3;
            lamina3.respuestaCorrecta = "triangulo";
            lamina3.dificultad = PuntosDificultadLaminaSeveridad.medio;
            lamina3.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina3.imagen = "LaminasSeveridad/LaminaProtan3.png";
            laminas.Add(lamina3);

            LaminaSeveridad lamina4 = new LaminaSeveridad();
            lamina4.numero = 4;
            lamina4.respuestaCorrecta = "triangulo";
            lamina4.dificultad = PuntosDificultadLaminaSeveridad.dificil;
            lamina4.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina4.imagen = "LaminasSeveridad/LaminaProtan4.png";
            laminas.Add(lamina4);

            LaminaSeveridad lamina5 = new LaminaSeveridad();
            lamina5.numero = 5;
            lamina5.respuestaCorrecta = "triangulo";
            lamina5.dificultad = PuntosDificultadLaminaSeveridad.muyDificil;
            lamina5.tipoDeficiencia = TipoDeficiencia.Protan;
            lamina5.imagen = "LaminasSeveridad/LaminaProtan5.png";
            laminas.Add(lamina5);

            return laminas;
        }
    }
}