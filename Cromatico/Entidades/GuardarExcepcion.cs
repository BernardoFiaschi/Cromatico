public static class GuardarExcepcion
{
    private static readonly string rutaException = "excepciones.log";
    public static void registrarExcepcion(System.Exception e, string contexto){
        string mensaje =
        $"[{DateTime.Now}]\n" +
        $"Contexto: {contexto}\n" +
        $"{e}\n\n";

    File.AppendAllText(rutaException, mensaje);
    }
}