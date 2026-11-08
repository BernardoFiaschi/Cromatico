using Cromatico.Modelos;
using Cromatico.Recursos;

namespace Cromatico.Servicios
{
    public class EvaluadorSeveridad
    {
        public EvaluadorSeveridad() { }
        public ResultadoTestSeveridad evaluarSeveridad(TipoDeficiencia deficiencia,
        List<RespuestaUsuario> respuestas)
        {
            int puntajeObtenido = 0;
            int puntajeMaximo = 0;
            double porcentaje = 0;
            List<LaminaSeveridad> laminas = new List<LaminaSeveridad>();
            ResultadoTestSeveridad resultado = new ResultadoTestSeveridad();
            try
            {


                if (respuestas == null || respuestas.Count == 0)
                {
                    resultado.severidad = NivelSeveridad.Indeterminada;
                    return resultado;
                }

                if (deficiencia == TipoDeficiencia.Protan)
                {
                    laminas = LaminasSeveridadProtan.obtenerLaminasSeveridadProtan();
                }
                else if (deficiencia == TipoDeficiencia.Deutan)
                {
                    laminas = LaminasSeveridadDeutan.obtenerLaminasSeveridadDeutan();
                }
                if (respuestas.Count != laminas.Count)
                {
                    resultado.severidad = NivelSeveridad.Indeterminada;
                    return resultado;
                }
                else
                {
                    for (int i = 0; i < laminas.Count; i++)
                    {
                        puntajeMaximo += (int)laminas[i].dificultad;
                        if (respuestas[i].respuesta == laminas[i].respuestaCorrecta)
                        {
                            puntajeObtenido += (int)laminas[i].dificultad;
                        }
                    }
                }
                if (puntajeMaximo == 0)
                {
                    resultado.severidad = NivelSeveridad.Indeterminada;
                    return resultado;

                }
                porcentaje = (double)puntajeObtenido / puntajeMaximo;
                resultado.porcentaje = porcentaje;
                resultado.puntajeObtenido = puntajeObtenido;
                resultado.puntajeMaximo = puntajeMaximo;

                if (porcentaje >= 0.80)
                {
                    resultado.severidad = NivelSeveridad.Leve;
                }
                else if (porcentaje >= 0.50)
                {
                    resultado.severidad = NivelSeveridad.Moderada;
                }
                else resultado.severidad = NivelSeveridad.Severa;

                return resultado;
            }
            catch (Exception e)
            {
                GuardarExcepcion.registrarExcepcion(e, "EvaluadorSeveridad");
                resultado.severidad = NivelSeveridad.Indeterminada;
                return resultado;
            }
        }
    }
}
