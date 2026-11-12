using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Cromatico.Servicios;
using Cromatico.UI.Vistas;

namespace Cromatico.Vistas
{
    public partial class VentanaPrincipal : Window
    {
        public sealed record Pagina(string Clave, string Titulo, Func<UserControl> Crear);

        // Cada entrada es una pestaña; otros módulos pueden agregar las suyas.
        public static List<Pagina> Paginas { get; } = new()
        {
            new("inicio",   "Inicio",        () => new PaginaInicio()),
            new("vista",    "Vista previa",  () => new PaginaVistaPrevia()),
            new("perfiles", "Perfiles",      () => new PaginaPerfiles()),
            new("aplicaciones", "Aplicaciones", () => new PaginaAsociaciones()),
            new("test",     "Test",          () => new PaginaEvaluacion()),
            new("ajustes",  "Ajustes",       () => new PaginaAjustes()),
        };

        public static VentanaPrincipal? Actual { get; private set; }

        private readonly ContextoApp _ctx;
        private readonly Dictionary<string, UserControl> _cache = new();
        private readonly Dictionary<string, RadioButton> _pestanas = new();
        private readonly DispatcherTimer _temporizadorAviso = new() { Interval = TimeSpan.FromSeconds(8) };

        public VentanaPrincipal(ContextoApp ctx)
        {
            InitializeComponent();
            TemaVentana.AplicarBarraOscura(this);
            _ctx = ctx;
            Actual = this;

            ConstruirPestanas();
            ActualizarCuenta();

            _ctx.Avisos.Aviso += AlLlegarAviso;
            _ctx.EstadoNubeCambiado += ActualizarCuenta;
            _temporizadorAviso.Tick += (_, _) => { _temporizadorAviso.Stop(); BarraAviso.Visibility = Visibility.Collapsed; };

            foreach (var (tipo, texto) in _ctx.AvisosIniciales) MostrarAviso(tipo, texto);

            Closed += (_, _) =>
            {
                _ctx.Avisos.Aviso -= AlLlegarAviso;
                _ctx.EstadoNubeCambiado -= ActualizarCuenta;
                if (ReferenceEquals(Actual, this)) Actual = null;
                _ctx.Dispose();
            };

            IrA(Paginas[0].Clave);
            
        }

        private void ConstruirPestanas()
        {
            foreach (var p in Paginas)
            {
                var rb = new RadioButton
                {
                    Content = p.Titulo,
                    Tag = p.Clave,
                    GroupName = "Pestanas",
                    Style = (Style)FindResource("PestanaNav")
                };
                rb.Checked += (s, _) => { if (s is RadioButton r && r.Tag is string k) MostrarPagina(k); };
                _pestanas[p.Clave] = rb;
                BarraPestanas.Children.Add(rb);
            }
        }

        public void IrA(string clave, string? perfilId = null)
        {
            if (!_pestanas.TryGetValue(clave, out var rb)) return;
            if (rb.IsChecked != true) rb.IsChecked = true; else MostrarPagina(clave);

            if (perfilId != null && _cache.TryGetValue(clave, out var pagina) && pagina is PaginaVistaPrevia vp)
                vp.SeleccionarPerfil(perfilId);
        }

        private void MostrarPagina(string clave)
        {
            try
            {
                if (!_cache.TryGetValue(clave, out var pagina))
                {
                    var def = Paginas.First(p => p.Clave == clave);
                    pagina = def.Crear();
                    _cache[clave] = pagina;
                }
                Contenido.Content = pagina;
            }
            catch (Exception ex)
            {
                MostrarAviso(TipoAviso.Error, "No se pudo abrir la pantalla: " + ex.Message);
            }
        }

        private void ActualizarCuenta()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(ActualizarCuenta); return; }

            if (_ctx.ModoSinCuenta)
            {
                TextoCuenta.Text = "Sin cuenta";
                TextoNube.Text = "Solo en este equipo";
            }
            else
            {
                TextoCuenta.Text = _ctx.Email;
                TextoNube.Text = _ctx.NubeDisponible ? "Sincronizado" : "Sin conexión (guardado local)";
            }
        }

        private void AlLlegarAviso(TipoAviso tipo, string texto)
        {
            if (Dispatcher.CheckAccess()) MostrarAviso(tipo, texto);
            else Dispatcher.BeginInvoke(() => MostrarAviso(tipo, texto));
        }

        private void MostrarAviso(TipoAviso tipo, string texto)
        {
            var (prefijo, clave) = tipo switch
            {
                TipoAviso.Error => ("Error", "BrushError"),
                TipoAviso.Advertencia => ("Atención", "BrushAdvertencia"),
                TipoAviso.Exito => ("Listo", "BrushOk"),
                _ => ("Info", "BrushAcentoClaro")
            };
            TextoAviso.Inlines.Clear();
            TextoAviso.Inlines.Add(new System.Windows.Documents.Run(prefijo + ": ")
            {
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource(clave)
            });
            TextoAviso.Inlines.Add(new System.Windows.Documents.Run(texto));
            BarraAviso.BorderBrush = (Brush)FindResource(clave);
            BarraAviso.Visibility = Visibility.Visible;

            _temporizadorAviso.Stop();

            if (tipo != TipoAviso.Error) _temporizadorAviso.Start();
        }

        private void AlCerrarAviso(object sender, RoutedEventArgs e)
        {
            _temporizadorAviso.Stop();
            BarraAviso.Visibility = Visibility.Collapsed;
        }

        public void CerrarSesion()
        {
            var inicio = new VentanaInicio();
            Application.Current.MainWindow = inicio;
            inicio.Show();
            Close();
        }
    }
}
