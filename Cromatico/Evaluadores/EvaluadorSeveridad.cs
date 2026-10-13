using System.Reflection;
using System.Reflection.Metadata.Ecma335;

public class EvaluadorSeveridad
{
    public EvaluadorSeveridad(){}
    public ResultadoTestSeveridad evaluarSeveridad(TipoDeficiencia deficiencia, 
    List<RespuestaUsuarioSeveridad> respuestas)
    {
        int puntajeObtenido=0;
        int puntajeMaximo=0;
        double porcentaje=0;
        List<LaminaSeveridad> laminas = new List<LaminaSeveridad>();
        ResultadoTestSeveridad resultado =new ResultadoTestSeveridad();

        if(respuestas == null || respuestas.Count == 0)
        {
            resultado.severidad=NivelSeveridad.Indeterminada;
            return resultado;
        }

        if(deficiencia == TipoDeficiencia.RojoProtan)
        {
            laminas=LaminasSeveridadRojoProtan.obtenerLaminasSeveridadProtan();
        }
        else if(deficiencia == TipoDeficiencia.VerdeDeutan)
        {
            laminas=LaminasSeveridadVerdeDeutan.obtenerLaminasVerdeDeutan();
        }
        if(respuestas.Count != laminas.Count)
        {
            resultado.severidad= NivelSeveridad.Indeterminada;
            return resultado;
        }
        else
        {
            for(int i = 0; i < laminas.Count; i ++)
            {
                puntajeMaximo += (int) laminas[i].dificultad;
                if(respuestas[i].respuesta == laminas[i].respuestaCorrecta)
                {
                    puntajeObtenido += (int)laminas[i].dificultad;
                }
            }
        }
        
        porcentaje =(double)puntajeObtenido/puntajeMaximo;
        resultado.porcentaje =porcentaje;
        resultado.puntajeObtenido =puntajeObtenido;
        resultado.puntajeMaximo =puntajeMaximo;

        if(porcentaje >= 0.80)
        {
            resultado.severidad =NivelSeveridad.Leve;
        }
        else if(porcentaje >= 0.50)
        {
            resultado.severidad =NivelSeveridad.Moderada;
        }
        else resultado.severidad =NivelSeveridad.Severa;

        return resultado;
    }
}
    
