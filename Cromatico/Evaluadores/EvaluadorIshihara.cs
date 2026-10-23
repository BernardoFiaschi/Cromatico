public class EvaluadorIshihara{
    public EvaluadorIshihara(){}
    public ResultadoTest Evaluar(List<RespuestaUsuario> respuestas)
    {
        var laminas=LaminasDeteccionIshihara.ObtenerLaminasDeteccion();

        int Aciertos=0;
        int ErroresDaltonicos=0;
        int ErroresMonocromatica=0;
        int ErroresOtros=0;
        TipoDeficiencia deficiencia = TipoDeficiencia.Ninguna;

        try{

        if(respuestas==null || respuestas.Count == 0)
        {
            return new ResultadoTest 
            {
                deficiencia=TipoDeficiencia.Indeterminado,
                confianza=0
            };
        }            
        
        foreach (var respuesta in respuestas ){

            
            var lamina = laminas.FirstOrDefault(l => l.numero == respuesta.numeroLamina);
            
                if (lamina != null)
                {
                    if (respuesta.respuesta == lamina.respuestaCorrecta )
                    {
                        Aciertos++;
                    }
                    else if (respuesta.respuesta == lamina.respuestaDaltonica)
                    {
                        ErroresDaltonicos++;
                    }
                    else if(respuesta.respuesta == lamina.respuestaMonocromatica)
                    {
                        ErroresMonocromatica++;
                    }
                    else
                    {
                        ErroresOtros++;
                    }

                }
            
        }
        
        
        deficiencia = EvaluarDeficiencia(Aciertos,ErroresDaltonicos,ErroresMonocromatica,ErroresOtros);
        ResultadoTest resultadoFinal = new ResultadoTest();
        resultadoFinal.deficiencia = deficiencia;
        resultadoFinal.aciertos = Aciertos;
        resultadoFinal.errores = ErroresDaltonicos + ErroresMonocromatica + ErroresOtros;
        int mayor =Math.Max(Aciertos,Math.Max(ErroresDaltonicos,ErroresMonocromatica));
        resultadoFinal.confianza= (double)(mayor / respuestas.Count)*100;
        
        return resultadoFinal;
        }catch(Exception e)
        {
         GuardarExcepcion.registrarExcepcion(e,"EvaluadorIshihara");
          return new ResultadoTest 
            {
                deficiencia=TipoDeficiencia.Indeterminado,
                confianza=0
            };  
        }

    }

    public TipoDeficiencia EvaluarDeficiencia(int aciertos,int erroresDaltonicos,
    int erroresMonocromatica,int erroresOtros)
    {
        const int umbral=4;           
        TipoDeficiencia deficiencia = TipoDeficiencia.Ninguna;

        if (erroresMonocromatica >=8)
        {
        deficiencia = TipoDeficiencia.Total;
        }
        else if (erroresDaltonicos >=umbral && erroresDaltonicos > aciertos)
        {
        deficiencia = TipoDeficiencia.RojoVerde;
        }
        else if (aciertos >=umbral && aciertos > erroresDaltonicos)
        {
        deficiencia = TipoDeficiencia.Ninguna;;
        }
        else
        {
        deficiencia = TipoDeficiencia.Indeterminado;
        }
        return deficiencia;
    }    
}