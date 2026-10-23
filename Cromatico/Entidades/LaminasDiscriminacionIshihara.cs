public static class LaminasDiscriminacionIshihara
{
    public static List<LaminaIshihara> ObtenerLaminasDiscriminacion()
    {
        List<LaminaIshihara> laminas = new List<LaminaIshihara>();
        LaminaIshihara lamina1 = new LaminaIshihara();
        lamina1.numero=22;
        lamina1.tipo=tipoLaminaIshihara.Discriminacion;
        lamina1.imagen="LaminasIshihara/Lamina22.png";
        lamina1.respuestaCorrecta="26";
        lamina1.respuestaRojaProtan="6";
        lamina1.respuestaVerdeDeutan="2";
        laminas.Add(lamina1);
        LaminaIshihara lamina2 = new LaminaIshihara();
        lamina2.numero=23;
        lamina2.tipo=tipoLaminaIshihara.Discriminacion;
        lamina2.imagen="LaminasIshihara/Lamina23.png";
        lamina2.respuestaCorrecta="42";
        lamina2.respuestaRojaProtan="2";
        lamina2.respuestaVerdeDeutan="4";
        laminas.Add(lamina2);
        LaminaIshihara lamina3 = new LaminaIshihara();
        lamina3.numero=24;
        lamina3.tipo=tipoLaminaIshihara.Discriminacion;
        lamina3.imagen="LaminasIshihara/Lamina24.png";
        lamina3.respuestaCorrecta="35";
        lamina3.respuestaRojaProtan="5";
        lamina3.respuestaVerdeDeutan="3";
        laminas.Add(lamina3);
        LaminaIshihara lamina4 = new LaminaIshihara();
        lamina4.numero=25;
        lamina4.tipo=tipoLaminaIshihara.Discriminacion;
        lamina4.imagen="LaminasIshihara/Lamina25.png";
        lamina4.respuestaCorrecta="96";
        lamina4.respuestaRojaProtan="6";
        lamina4.respuestaVerdeDeutan="9";
        laminas.Add(lamina4);

        return laminas;
    }
}