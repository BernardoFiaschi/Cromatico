using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using Cromatico.Modelos;
using Cromatico.Recursos;
using Cromatico.Servicios;
using Cromatico.Vistas;

namespace Cromatico.UI.Vistas
{

    public partial class PaginaEvaluacion : UserControl
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


        public PaginaEvaluacion()
        {
            InitializeComponent();

            respuestas = new List<RespuestaUsuario>();
            laminas = new List<LaminaIshihara>();
            laminasSeveridad = new List<LaminaSeveridad>();

            Loaded += (_, _) => MostrarResultadoGuardado();
            Unloaded += (_, _) =>
            {
                if (panelTest.Visibility == Visibility.Visible)
                    VolverAlInicioDelTest();
            };
        }

        private static void MostrarMensaje(TipoAviso tipo, string mensaje)
        {
            var ctx = ContextoApp.Actual;
            if (ctx == null)
                return;

            switch (tipo)
            {
                case TipoAviso.Error: ctx.Avisos.Error(mensaje); break;
                case TipoAviso.Advertencia: ctx.Avisos.Advertir(mensaje); break;
                case TipoAviso.Exito: ctx.Avisos.Exito(mensaje); break;
                default: ctx.Avisos.Informar(mensaje); break;
            }
        }

        private void BtnComenzar_Click(object sender, RoutedEventArgs e)
        {
            etapaActual = EtapaEvaluacion.Deteccion;

            indiceActual = 0;

            ContextoApp.Actual?.AutoPerfil.Suspender();
            panelInicio.Visibility = Visibility.Collapsed;
            panelTest.Visibility = Visibility.Visible;

            CargarDeteccion();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VolverAlInicioDelTest();
        }

        private void VolverAlInicioDelTest()
        {
            ContextoApp.Actual?.AutoPerfil.Reanudar();
            panelTest.Visibility = Visibility.Collapsed;
            panelInicio.Visibility = Visibility.Visible;
            MostrarResultadoGuardado();
        }

        private void MostrarResultadoGuardado()
        {
            var config = ContextoApp.Actual?.Almacen.Config;
            if (config == null)
                return;

            if (config.EvaluacionCompleta)
            {
                txtResultado.Text = config.TipoDetectada == TipoDeficiencia.Ninguna
                    ? "Último resultado: no se detectó deficiencia visual."
                    : $"Último resultado: {config.TipoDetectada}, severidad {config.NivelDetectado}.";
                btnComenzar.Content = "Repetir test";
            }
            else
            {
                txtResultado.Text = "Todavía no hiciste el test.";
                btnComenzar.Content = "Comenzar test";
            }
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
                MostrarMensaje(TipoAviso.Error, $"No se encontró la imagen: {rutaCompleta}");

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
                    MostrarMensaje(TipoAviso.Advertencia, "Seleccioná una respuesta antes de continuar.");

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
                MostrarMensaje(TipoAviso.Info, "No se detectó deficiencia visual.");
                ContextoApp.Actual?.Almacen.Modificar(c =>
                {
                    c.EvaluacionCompleta = true;
                    c.TipoDetectada = TipoDeficiencia.Ninguna;
                    c.NivelDetectado = NivelSeveridad.Indeterminada;
                },
        "guardar resultado del test");
                VolverAlInicioDelTest();
                VentanaPrincipal.Actual?.IrA("inicio");
                return;
            }
            MostrarMensaje(TipoAviso.Info, "Se detectó deficiencia visual. Seguimos con la siguiente etapa.");
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
            MostrarMensaje(TipoAviso.Info, $"Deficiencia detectada: {resultadoDiscriminacion.deficiencia.Nombre()}");
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
            EvaluadorSeveridad evaluador = new EvaluadorSeveridad();
            ResultadoTestSeveridad resultado = evaluador.evaluarSeveridad(
                    resultadoDiscriminacion!.deficiencia, respuestas);
            var ctx = ContextoApp.Actual;
            ctx.Almacen.Modificar(c =>
            {
                c.EvaluacionCompleta = true;
                c.TipoDetectada = resultadoDiscriminacion.deficiencia;
                c.NivelDetectado = resultado.severidad;
                var perfil = c.BuscarPerfil(c.PerfilActivoId);
                if (perfil != null)
                {
                    perfil.Tipo = resultadoDiscriminacion.deficiencia;

                    perfil.Severidad = resultado.severidad
                     switch
                    {
                        NivelSeveridad.Leve => 0.3,
                        NivelSeveridad.Moderada => 0.6,
                        NivelSeveridad.Severa => 0.9,
                        _ => 0.6
                    };
                }
            },
    "guardar resultado del test");

            VolverAlInicioDelTest();
            VentanaPrincipal.Actual?.IrA("vista");
        }
    }
}