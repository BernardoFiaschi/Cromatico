using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Cromatico.Interop;
using Cromatico.Modelos;
using Cromatico.Servicios;

namespace Cromatico.Vistas
{
    public partial class PaginaVistaPrevia : UserControl
    {
        private readonly ServicioVistaPrevia _vista = new();
        private readonly DispatcherTimer _retardo = new() { Interval = TimeSpan.FromMilliseconds(40) };
        private ContextoApp? _ctx;
        private bool _cargando;
        private bool _inicializada;
        private string? _perfilId;
        private WriteableBitmap? _bmpOriginal;
        private WriteableBitmap? _bmpCorregida;

        public PaginaVistaPrevia()
        {
            InitializeComponent();
            _retardo.Tick += (_, _) => { _retardo.Stop(); Actualizar(); };
            Editor.Cambio += () => { _retardo.Stop(); _retardo.Start(); };
            Loaded += AlCargar;
            Unloaded += AlDescargar;
        }

        private void AlCargar(object sender, RoutedEventArgs e)
        {
            _ctx = ContextoApp.Actual;
            if (_ctx == null) return;
            _ctx.Perfiles.Cambio += AlCambiarPerfiles;

            if (!_inicializada)
            {
                _inicializada = true;
                _perfilId ??= _ctx.Almacen.Config.PerfilActivoId;
                var r = _vista.UsarImagenReferencia();
                if (!r.Exitoso) MostrarMensaje(TipoAviso.Error, r.Mensaje);
                MostrarOriginal();
            }
            LlenarPerfiles();
        }

        private void AlDescargar(object sender, RoutedEventArgs e)
        {
            _retardo.Stop();
            if (_ctx != null) _ctx.Perfiles.Cambio -= AlCambiarPerfiles;
            _ctx = null;
        }

        public void SeleccionarPerfil(string id)
        {
            _perfilId = id;
            if (_ctx != null) LlenarPerfiles();
        }

        private void AlCambiarPerfiles()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(AlCambiarPerfiles); return; }
            LlenarPerfiles();
        }

        private void LlenarPerfiles()
        {
            if (_ctx == null) return;
            var cfg = _ctx.Almacen.Config;
            if (cfg.BuscarPerfil(_perfilId) == null) _perfilId = cfg.PerfilActivoId;

            _cargando = true;
            try
            {
                ComboPerfil.Items.Clear();
                foreach (var p in _ctx.Perfiles.Perfiles)
                {
                    string texto = p.Nombre + (p.Id == cfg.PerfilActivoId ? "  ·  activo" : "");
                    var item = new ComboBoxItem { Content = texto, Tag = p.Id };
                    ComboPerfil.Items.Add(item);
                    if (p.Id == _perfilId) ComboPerfil.SelectedItem = item;
                }
                CargarPerfilEnEditor();
            }
            finally { _cargando = false; }
            Actualizar();
        }

        private void CargarPerfilEnEditor()
        {
            var p = _ctx?.Almacen.Config.BuscarPerfil(_perfilId);
            if (p == null) return;
            Editor.Establecer(p.Tipo, p.Severidad, p.Intensidad);
            TextoPerfilNota.Text = p.EsPredefinido
                ? "Perfil predefinido: se guarda como copia personalizada."
                : "Perfil personalizado.";
        }

        private void AlElegirPerfil(object sender, SelectionChangedEventArgs e)
        {
            if (_cargando || _ctx == null) return;
            if (ComboPerfil.SelectedItem is not ComboBoxItem { Tag: string id }) return;
            _perfilId = id;
            _cargando = true;
            try { CargarPerfilEnEditor(); } finally { _cargando = false; }
            Actualizar();
        }

        private void MostrarOriginal()
        {
            var img = _vista.Imagen;
            if (img == null) { ImagenOriginal.Source = null; return; }
            _bmpOriginal = Volcar(_bmpOriginal, img.Ancho, img.Alto, img.Bgra);
            ImagenOriginal.Source = _bmpOriginal;
        }

        private static WriteableBitmap Volcar(WriteableBitmap? destino, int ancho, int alto, byte[] bgra)
        {
            if (destino == null || destino.PixelWidth != ancho || destino.PixelHeight != alto)
                destino = new WriteableBitmap(ancho, alto, 96, 96, PixelFormats.Bgr32, null);
            destino.WritePixels(new Int32Rect(0, 0, ancho, alto), bgra, ancho * 4, 0);
            return destino;
        }

        private void Actualizar()
        {
            if (_ctx == null) return;

            if (!_vista.Disponible)
            {
                TextoNoDisponible.Visibility = _bmpCorregida == null ? Visibility.Visible : Visibility.Collapsed;
                MostrarMensaje(TipoAviso.Advertencia, "La vista previa no está disponible: no hay una captura ni una imagen de referencia. Podés seguir editando los parámetros.");
                return;
            }

            var r = _vista.Generar(Editor.Tipo, Editor.Severidad, Editor.Intensidad);
            if (r.Exitoso && r.Valor != null)
            {
                var img = _vista.Imagen!;
                _bmpCorregida = Volcar(_bmpCorregida, img.Ancho, img.Alto, r.Valor);
                ImagenCorregida.Source = _bmpCorregida;
                TextoNoDisponible.Visibility = Visibility.Collapsed;
                OcultarMensaje();
            }
            else
            {
                TextoNoDisponible.Visibility = _bmpCorregida == null ? Visibility.Visible : Visibility.Collapsed;
                MostrarMensaje(TipoAviso.Error, r.Mensaje);
            }
        }

        private async void AlPulsarCapturar(object sender, RoutedEventArgs e)
        {
            var ventana = Window.GetWindow(this);
            BotonCapturar.IsEnabled = false;
            ResultadoOperacion resultado;
            try
            {
                ventana?.Hide();
                await Task.Delay(350);
                resultado = _vista.CapturarPantalla();
            }
            catch (Exception ex)
            {
                resultado = ResultadoOperacion.Error("No se pudo capturar la pantalla: " + ex.Message);
            }
            finally
            {
                if (ventana != null) { ventana.Show(); ventana.Activate(); }
                BotonCapturar.IsEnabled = true;
            }

            if (resultado.Exitoso)
            {
                MostrarOriginal();
                Actualizar();
                MostrarMensaje(TipoAviso.Exito, resultado.Mensaje);
            }
            else MostrarMensaje(TipoAviso.Error, resultado.Mensaje);
        }

        private void AlPulsarReferencia(object sender, RoutedEventArgs e)
        {
            var r = _vista.UsarImagenReferencia();
            if (r.Exitoso) { MostrarOriginal(); Actualizar(); }
            else MostrarMensaje(TipoAviso.Error, r.Mensaje);
        }

        private bool Guardar(bool aplicar)
        {
            if (_ctx == null || _perfilId == null) return false;

            var r = _ctx.Perfiles.GuardarParametros(_perfilId, Editor.Tipo, Editor.Severidad, Editor.Intensidad);
            if (!r.Exitoso || r.Valor == null)
            {
                MostrarMensaje(TipoAviso.Error, r.Mensaje);
                return false;
            }

            _perfilId = r.Valor.Id;
            string mensaje = r.Mensaje;

            if (aplicar)
            {
                var s = _ctx.Perfiles.Seleccionar(_perfilId);
                if (!s.Exitoso) { MostrarMensaje(TipoAviso.Error, s.Mensaje); LlenarPerfiles(); return false; }
                _ctx.AutoPerfil.Recalcular();
                mensaje = string.IsNullOrEmpty(mensaje) ? "Perfil guardado y aplicado." : mensaje + " Aplicado.";
            }

            LlenarPerfiles();
            MostrarMensaje(TipoAviso.Exito, string.IsNullOrEmpty(mensaje) ? "Perfil guardado." : mensaje);
            return true;
        }

        private void AlPulsarGuardar(object sender, RoutedEventArgs e) => Guardar(false);
        private void AlPulsarGuardarAplicar(object sender, RoutedEventArgs e) => Guardar(true);

        private void MostrarMensaje(TipoAviso tipo, string texto)
        {
            var (prefijo, clave) = tipo switch
            {
                TipoAviso.Error => ("Error", "BrushError"),
                TipoAviso.Advertencia => ("Atención", "BrushAdvertencia"),
                TipoAviso.Exito => ("Listo", "BrushOk"),
                _ => ("Info", "BrushAcentoClaro")
            };
            TextoMensaje.Inlines.Clear();
            TextoMensaje.Inlines.Add(new System.Windows.Documents.Run(prefijo + ": ")
            {
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource(clave)
            });
            TextoMensaje.Inlines.Add(new System.Windows.Documents.Run(texto));
            PanelMensaje.BorderBrush = (Brush)FindResource(clave);
            PanelMensaje.Visibility = Visibility.Visible;
        }

        private void OcultarMensaje() => PanelMensaje.Visibility = Visibility.Collapsed;
    }
}
