public class EvaluadorDiscriminacionIhihara
{
    public EvaluadorDiscriminacionIhihara(){}

    public ResultadoTest evaluarDiscrimincacion(List<RespuestaUsuario> respuestas)
    {
        int Aciertos=0;
        int ErroresRojoProtan=0;
        int ErroresVerdeDeutan=0;
        int ErroresOtros=0;
        TipoDeficiencia deficiencia = TipoDeficiencia.RojoVerde;

        var laminasDiscriminacion=LaminasDiscriminacionIshihara.ObtenerLaminasDiscriminacion();

        try
        {
            if(respuestas==null || respuestas.Count == 0)
        {
            return new ResultadoTest
            {
                deficiencia=TipoDeficiencia.Indeterminado,
                confianza=0
            };
        }
        foreach(var respuesta in respuestas)
        {
            var laminaDiscriminacion = laminasDiscriminacion.FirstOrDefault(l=>l.numero == respuesta.numeroLamina);
            if(laminaDiscriminacion != null)
            {
                if(respuesta.respuesta == laminaDiscriminacion.respuestaCorrecta)
                {
                    Aciertos++;
                }
                else if (respuesta.respuesta == laminaDiscriminacion.respuestaRojaProtan)
                {
                    ErroresRojoProtan++;
                }
                else if (respuesta.respuesta == laminaDiscriminacion.respuestaVerdeDeutan)
                {
                    ErroresVerdeDeutan++;
                }
                else
                {
                    ErroresOtros++;
                }
            }
        }
        
        deficiencia=evaluadorDiscriminacion(Aciertos,ErroresRojoProtan,ErroresVerdeDeutan,ErroresOtros);
        ResultadoTest resultadoDiscriminatorioFinal= new ResultadoTest();
        resultadoDiscriminatorioFinal.deficiencia=deficiencia;
        resultadoDiscriminatorioFinal.aciertos=Aciertos;
        int mayor = Math.Max(Aciertos,Math.Max(ErroresRojoProtan,ErroresVerdeDeutan));
        resultadoDiscriminatorioFinal.confianza =(double)mayor / respuestas.Count;
        

        return resultadoDiscriminatorioFinal;

        }catch(Exception e)
        {
         GuardarExcepcion.registrarExcepcion(e,"EvaluadorDiscriminacionIshihara");
         return new ResultadoTest
            {
                deficiencia=TipoDeficiencia.Indeterminado,
                confianza=0
            };   
        }

        
    }

    public TipoDeficiencia evaluadorDiscriminacion(int aciertos,int erroresRojoProtan,
     int erroresVerdeDeutan,int erroresOtros)
    {
        TipoDeficiencia discriminacion=TipoDeficiencia.Indeterminado;
        const int umbral=2;
        
        if(aciertos >= umbral && aciertos > erroresRojoProtan)
        {
            discriminacion=TipoDeficiencia.Ninguna;
        }
        else if(erroresRojoProtan >= umbral && erroresRojoProtan > erroresVerdeDeutan)
        {
            discriminacion=TipoDeficiencia.RojoProtan;
        }
        else if(erroresVerdeDeutan >=umbral && erroresVerdeDeutan > erroresRojoProtan)
        {
            discriminacion=TipoDeficiencia.VerdeDeutan;
        }
        else
        {
            discriminacion=TipoDeficiencia.Indeterminado;
        }
        return discriminacion;
    }

}