// See https://aka.ms/new-console-template for more information

    var laminas = LaminasDeteccionIshihara.ObtenerLaminasDeteccion();
   



var respuestas = new System.Collections.Generic.List<RespuestaUsuario>();
    RespuestaUsuario respuesta1 = new RespuestaUsuario();
    respuesta1.numeroLamina = 22;
    respuesta1.respuesta = "26";
    respuestas.Add(respuesta1);
    RespuestaUsuario respuesta2 = new RespuestaUsuario();
    respuesta2.numeroLamina = 23;
    respuesta2.respuesta = "4";
    respuestas.Add(respuesta2);

    EvaluadorIshihara evaluador = new EvaluadorIshihara();
    ResultadoTest resultado = evaluador.Evaluar(laminas, respuestas);

    Console.WriteLine("Deficiencia: " + resultado.deficiencia);
    Console.WriteLine("Severidad: " + resultado.severidad);
    Console.WriteLine("Aciertos: " + resultado.aciertos);
    Console.WriteLine("Errores: " + resultado.errores);
