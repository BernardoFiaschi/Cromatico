using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using Cromatico.Modelos;
using Cromatico.Recursos;
using Cromatico.Servicios;

namespace Cromatico.UI.Vistas
{

    public partial class VentanaEvaluacion : Window
    {
        private int indiceActual;

        private EtapaEvaluacion etapaActual;

        private List<LaminaIshihara> laminas;
        private List<LaminaSeveridad> laminasSeveridad;

        private List<RespuestaUsuario> respuestas;
        private ResultadoTest? resultadoDeteccion;
        private ResultadoTest? resultadoDiscriminacion;
        private string? ObtenerRespuestaSeveridad()
        {
            if (rbTriangulo.IsChecked == true)
                return "triangulo";

            if (rbCuadrado.IsChecked == true)
                return "cuadrado";

            if (rbCirculo.IsChecked == true)
                return "circulo";

            if (rbNada.IsChecked == true)
                return "nada";

            return null;
        }


        public VentanaEvaluacion()
        {
            InitializeComponent();

            etapaActual = EtapaEvaluacion.Deteccion;

            indiceActual = 0;

            respuestas = new List<RespuestaUsuario>();
            laminas = new List<LaminaIshihara>();
            laminasSeveridad = new List<LaminaSeveridad>();

            CargarDeteccion();
        }

        private void CargarDeteccion()
        {
            txtRespuesta.Visibility = Visibility.Visible;

            panelSeveridad.Visibility = Visibility.Collapsed;
            laminas = LaminasDeteccionIshihara.ObtenerLaminasDeteccion();

            MostrarLamina();
            respuestas.Clear();
            txtRespuesta.Focus();
        }

        private void MostrarLamina()
        {
            string rutaImagen;
            int numero;
            if (etapaActual == EtapaEvaluacion.Severidad)
            {
                rbTriangulo.IsChecked = false;
                rbCuadrado.IsChecked = false;
                rbCirculo.IsChecked = false;
                rbNada.IsChecked = false;
            }
            else
            {
                txtRespuesta.Clear();
                txtRespuesta.Focus();
            }
            if (etapaActual == EtapaEvaluacion.Severidad)
            {
                if (indiceActual < 0 || indiceActual >= laminasSeveridad.Count)
                    return;
                LaminaSeveridad laminaSeve = laminasSeveridad[indiceActual];
                numero = laminaSeve.numero;
                rutaImagen = laminaSeve.imagen;
            }
            else
            {
                if (indiceActual < 0 || indiceActual >= laminas.Count)
                    return;
                LaminaIshihara laminaIshi = laminas[indiceActual];
                numero = laminaIshi.numero;
                rutaImagen = laminaIshi.imagen;
            }
            txtNumeroLamina.Text = $"Lámina {numero}";

            string rutaCompleta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    rutaImagen);

            if (!File.Exists(rutaCompleta))
            {
                MessageBox.Show($"No se encontró la imagen:\n{rutaCompleta}");

                return;
            }

            imgLamina.Source = new BitmapImage(new Uri(
                        rutaCompleta,
                        UriKind.Absolute));
        }
        private void BtnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            RespuestaUsuario respuesta = new RespuestaUsuario();
            if (etapaActual == EtapaEvaluacion.Severidad)
            {
                respuesta.numeroLamina = laminasSeveridad[indiceActual].numero;
                string? respuestaUsuario = ObtenerRespuestaSeveridad();
                if (respuestaUsuario == null)
                {
                    MessageBox.Show("Seleccioná una respuesta antes de continuar.");

                    return;
                }
                respuesta.respuesta = respuestaUsuario;
            }
            else
            {
                respuesta.numeroLamina = laminas[indiceActual].numero;
                respuesta.respuesta = txtRespuesta.Text.Trim();
            }

            respuestas.Add(respuesta);

            indiceActual++;

            int cantidadLaminas = etapaActual == EtapaEvaluacion.Severidad
            ? laminasSeveridad.Count : laminas.Count;
            if (indiceActual >= cantidadLaminas)
            {
                if (etapaActual == EtapaEvaluacion.Deteccion)
                {
                    FinalizarDeteccion();
                }
                else if (etapaActual == EtapaEvaluacion.Discriminacion)
                {
                    FinalizarDiscriminacion();
                }
                else if (etapaActual == EtapaEvaluacion.Severidad)
                {
                    FinalizarSeveridad();
                }
                return;
            }

            MostrarLamina();

            if (etapaActual == EtapaEvaluacion.Severidad)
            {
                rbTriangulo.IsChecked = false;
                rbCuadrado.IsChecked = false;
                rbCirculo.IsChecked = false;
                rbNada.IsChecked = false;
            }
            else
            {
                txtRespuesta.Clear();
                txtRespuesta.Focus();
            }
        }

        private void FinalizarDeteccion()
        {
            EvaluadorIshihara evaluador = new EvaluadorIshihara();

            resultadoDeteccion = evaluador.Evaluar(respuestas);

            if (resultadoDeteccion.deficiencia == TipoDeficiencia.Ninguna)
            {
                MessageBox.Show("No se detectó deficiencia visual.");

                return;
            }
            MessageBox.Show("Se detectó deficiencia visual.");
            CargarDiscriminacion();
        }
        private void CargarDiscriminacion()
        {
            txtRespuesta.Visibility = Visibility.Visible;

            panelSeveridad.Visibility = Visibility.Collapsed;
            etapaActual = EtapaEvaluacion.Discriminacion;

            indiceActual = 0;

            respuestas.Clear();

            laminas = LaminasDiscriminacionIshihara.ObtenerLaminasDiscriminacion();

            MostrarLamina();
            txtRespuesta.Clear();
            txtRespuesta.Focus();
        }
        private void FinalizarDiscriminacion()
        {
            EvaluadorDiscriminacionIhihara evaluadorDiscriminacion = new EvaluadorDiscriminacionIhihara();

            resultadoDiscriminacion = evaluadorDiscriminacion.evaluarDiscrimincacion(respuestas);
            MessageBox.Show($"Deficiencia detectada: {resultadoDiscriminacion.deficiencia}");
            CargarSeveridad();
        }
        private void CargarSeveridad()
        {
            indiceActual = 0;
            etapaActual = EtapaEvaluacion.Severidad;

            respuestas.Clear();

            txtRespuesta.Visibility = Visibility.Collapsed;

            panelSeveridad.Visibility = Visibility.Visible;

            if (resultadoDiscriminacion?.deficiencia == TipoDeficiencia.Protan)
            {
                laminasSeveridad = LaminasSeveridadProtan.obtenerLaminasSeveridadProtan();
            }
            else
            {
                laminasSeveridad = LaminasSeveridadDeutan.obtenerLaminasSeveridadDeutan();
            }

            MostrarLamina();
        }
        private void FinalizarSeveridad()
        {
            string debug = "";

            foreach (var r in respuestas)
            {
                debug += r.respuesta + "\n";
            }

            txtNumeroLamina.Text = debug;

            EvaluadorSeveridad evaluador = new EvaluadorSeveridad();
            ResultadoTestSeveridad resultado = evaluador.evaluarSeveridad(
                    resultadoDiscriminacion!.deficiencia, respuestas);

            MessageBox.Show($"Deficiencia: {resultadoDiscriminacion.deficiencia}\n" +
                $"Severidad: {resultado.severidad}\n" +
                $"Porcentaje: {resultado.porcentaje:P}");
        }
    }
}