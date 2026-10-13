using System.Collections.Generic;
using System.Runtime.InteropServices;

var laminas = LaminasDeteccionIshihara.ObtenerLaminasDeteccion();
var resultado = new ResultadoTest();
   



var respuestas = new System.Collections.Generic.List<RespuestaUsuario>();

    RespuestaUsuario respuesta1 = new RespuestaUsuario();
    respuesta1.numeroLamina = 2;
    respuesta1.respuesta = "8";
    respuestas.Add(respuesta1);

    RespuestaUsuario respuesta2 = new RespuestaUsuario();
    respuesta2.numeroLamina = 3;
    respuesta2.respuesta = "6";
    respuestas.Add(respuesta2);

    RespuestaUsuario respuesta3 = new RespuestaUsuario();
    respuesta3.numeroLamina = 4;
    respuesta3.respuesta = "29";
    respuestas.Add(respuesta3);

    RespuestaUsuario respuesta4 = new RespuestaUsuario();
    respuesta4.numeroLamina = 5;
    respuesta4.respuesta = "35";
    respuestas.Add(respuesta4);

    RespuestaUsuario respuesta5 = new RespuestaUsuario();
    respuesta5.numeroLamina = 6;
    respuesta5.respuesta = "2";
    respuestas.Add(respuesta5);

    RespuestaUsuario respuesta6 = new RespuestaUsuario();
    respuesta6.numeroLamina = 7;
    respuesta6.respuesta = "5";
    respuestas.Add(respuesta6);

    RespuestaUsuario respuesta7 = new RespuestaUsuario();
    respuesta7.numeroLamina = 8;
    respuesta7.respuesta = "17";
    respuestas.Add(respuesta7);

    RespuestaUsuario respuesta8 = new RespuestaUsuario();
    respuesta8.numeroLamina = 9;
    respuesta8.respuesta = "21";
    respuestas.Add(respuesta8);

   EvaluadorIshihara evaluadorIshihara = new EvaluadorIshihara();

   resultado=evaluadorIshihara.Evaluar(respuestas);
   Console.WriteLine("DETECCION");
   Console.Write("deficiencia: ");
   Console.WriteLine(resultado.deficiencia.ToString());
   Console.Write("Aciertos: ");
   Console.WriteLine(resultado.aciertos.ToString());
   Console.Write("Errores: ");
   Console.WriteLine(resultado.errores.ToString());
   Console.Write("Confianza de la evaluacion: ");
   Console.WriteLine(resultado.confianza);
   Console.WriteLine("");

   if(resultado.deficiencia == TipoDeficiencia.RojoVerde)
{
    var discriminacion = new EvaluadorDiscriminacionIhihara();

    var respuestasDiscriminacion = new System.Collections.Generic.List<RespuestaUsuario>();

    RespuestaUsuario respuestaD1 = new RespuestaUsuario();
    respuestaD1.numeroLamina = 22;
    respuestaD1.respuesta = "6";
    respuestasDiscriminacion.Add(respuestaD1);

    RespuestaUsuario respuestaD2 = new RespuestaUsuario();
    respuestaD2.numeroLamina = 23;
    respuestaD2.respuesta = "2";
    respuestasDiscriminacion.Add(respuestaD2);

    RespuestaUsuario respuestaD3 = new RespuestaUsuario();
    respuestaD3.numeroLamina = 24;
    respuestaD3.respuesta = "35";
    respuestasDiscriminacion.Add(respuestaD3);

    RespuestaUsuario respuestaD4 = new RespuestaUsuario();
    respuestaD4.numeroLamina = 25;
    respuestaD4.respuesta = "96";
    respuestasDiscriminacion.Add(respuestaD4);

    var resultadoDiscriminacion = new ResultadoTest();
    resultadoDiscriminacion=discriminacion.evaluarDiscrimincacion(respuestasDiscriminacion);
    if (resultadoDiscriminacion.deficiencia == TipoDeficiencia.RojoProtan)
    {
         Console.WriteLine("DISCRIMINACION");
   Console.Write("deficiencia: ");
   Console.WriteLine(resultadoDiscriminacion.deficiencia.ToString());
   Console.Write("Aciertos: ");
   Console.WriteLine(resultadoDiscriminacion.aciertos.ToString());
   Console.Write("Errores: ");
   Console.WriteLine(resultadoDiscriminacion.errores.ToString());
   Console.Write("Confianza de la evaluacion: ");
   Console.WriteLine(resultadoDiscriminacion.confianza);
   Console.WriteLine("");

   var evaluadroSeveridad = new EvaluadorSeveridad();
    }else if (resultadoDiscriminacion.deficiencia == TipoDeficiencia.VerdeDeutan)
    {
         Console.WriteLine("DISCRIMINACION");
   Console.Write("deficiencia: ");
   Console.WriteLine(resultadoDiscriminacion.deficiencia.ToString());
   Console.Write("Aciertos: ");
   Console.WriteLine(resultadoDiscriminacion.aciertos.ToString());
   Console.Write("Errores: ");
   Console.WriteLine(resultadoDiscriminacion.errores.ToString());
   Console.Write("Confianza de la evaluacion: ");
   Console.WriteLine(resultadoDiscriminacion.confianza);
   Console.WriteLine("");

   var evaluadroSeveridad = new EvaluadorSeveridad();
        
    }else if(resultadoDiscriminacion.deficiencia == TipoDeficiencia.Ninguna)
    {
        Console.WriteLine("NO POSEE DEFICIENCIA (no deberia pasar ya que en primer evaluacion se detecto)");
    }
    else
    {
        Console.WriteLine("ALGO RARO AL EVALUAR DISCRIMINACION (REVISAR)");
        Console.WriteLine("DISCRIMINACION");
   Console.Write("deficiencia: ");
   Console.WriteLine(resultadoDiscriminacion.deficiencia.ToString());
   Console.Write("Aciertos: ");
   Console.WriteLine(resultadoDiscriminacion.aciertos.ToString());
   Console.Write("Errores: ");
   Console.WriteLine(resultadoDiscriminacion.errores.ToString());
   Console.Write("Confianza de la evaluacion: ");
   Console.WriteLine(resultadoDiscriminacion.confianza);
   Console.WriteLine("");
    }

    


} else if( resultado.deficiencia == TipoDeficiencia.Indeterminado)
{
    Console.WriteLine("SE DEBE VOLVER A EVALUAR");
}else if(resultado.deficiencia == TipoDeficiencia.Ninguna)
{
    Console.WriteLine("NO POSEE DEFICIENCIA");
}
else
{
    Console.WriteLine("ALGO RARO PASA AL EVALUAR (REVISAR)");
}



   





   