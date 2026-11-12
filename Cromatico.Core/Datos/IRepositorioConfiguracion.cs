using Cromatico.Modelos;

namespace Cromatico.Datos
{
    public interface IRepositorioConfiguracion
    {
        string Ubicacion { get; }

        // Devuelve null si todavía no hay nada guardado para esa clave.
        ConfiguracionApp? Leer(string clave);

        void Escribir(string clave, ConfiguracionApp configuracion);
    }
}
