using Cromatico.Algoritmo;
using Cromatico.Interop;
using Cromatico.Modelos;

namespace Cromatico.Servicios
{
    public class ServicioVistaPrevia
    {
        private readonly Func<int, CapturaPantalla> _capturador;
        private byte[]? _ultimaCorregida;

        public ServicioVistaPrevia(Func<int, CapturaPantalla>? capturador = null)
        {
            _capturador = capturador ?? (ancho => CapturadorPantalla.Capturar(ancho));
        }

        public CapturaPantalla? Imagen { get; private set; }

        public bool Disponible => Imagen != null;

        public byte[]? UltimaCorregida => _ultimaCorregida;

        public ResultadoOperacion CapturarPantalla(int anchoMaximo = 960)
        {
            try
            {
                var captura = _capturador(anchoMaximo);
                if (captura == null || captura.Bgra.Length < captura.Ancho * captura.Alto * 4 || captura.Ancho <= 0 || captura.Alto <= 0)
                    return ResultadoOperacion.Error("La captura de pantalla no devolvió una imagen válida. Se mantiene la vista previa anterior.");

                Imagen = captura;
                _ultimaCorregida = null;
                return ResultadoOperacion.Ok("Pantalla capturada.");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Error($"No se pudo capturar la pantalla: {ex.Message} Se mantiene la vista previa anterior.");
            }
        }

        public ResultadoOperacion UsarImagenReferencia()
        {
            try
            {
                Imagen = ImagenReferencia.Generar();
                _ultimaCorregida = null;
                return ResultadoOperacion.Ok("Imagen de referencia cargada.");
            }
            catch (Exception ex)
            {
                return ResultadoOperacion.Error("No se pudo generar la imagen de referencia: " + ex.Message);
            }
        }

        public ResultadoOperacion<byte[]> Generar(TipoDeficiencia tipo, double severidad, double intensidad)
        {
            if (Imagen == null)
                return ResultadoOperacion<byte[]>.Error(
                    "La vista previa no está disponible: no hay una captura ni una imagen de referencia para procesar.",
                    "SinImagen");

            if (!PerfilCorreccion.ValorValido(severidad) || !PerfilCorreccion.ValorValido(intensidad))
                return ResultadoOperacion<byte[]>.Error(
                    "Los parámetros de corrección están fuera de rango (0 % a 100 %). Se mantiene la última vista previa válida.",
                    "Parametros");

            try
            {
                var matriz = Daltonizador.CalcularMatriz(tipo, severidad, intensidad);
                var salida = new byte[Imagen.Bgra.Length];
                Daltonizador.Aplicar(Imagen.Bgra, salida, Imagen.Ancho * Imagen.Alto, matriz);
                _ultimaCorregida = salida;
                return ResultadoOperacion<byte[]>.Ok(salida);
            }
            catch (Exception ex)
            {
                return ResultadoOperacion<byte[]>.Error(
                    $"No se pudo actualizar la vista previa: {ex.Message} Se mantiene la última vista previa válida.", "Procesamiento");
            }
        }
    }
}
