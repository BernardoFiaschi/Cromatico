using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cromatico.Modelos;
using Cromatico.Servicios;
using Cromatico.UI.Vistas;

namespace Cromatico.Vistas
{
    public partial class VentanaInicio : Window
    {
        private readonly IServicioSincronizacion _sincronizacion = new ServicioSincronizacionSimulado();
        private readonly ServicioAutenticacion _auth;
        private bool _ocupado;

        public VentanaInicio()
        {
            InitializeComponent();
            TemaVentana.AplicarBarraOscura(this);
            _auth = new ServicioAutenticacion(new AlmacenamientoLocalJson(), _sincronizacion);
            Loaded += (_, _) => CajaEmail.Focus();
        }

        private bool ModoRegistro => OpcionRegistrar.IsChecked == true;

        private void AlCambiarModo(object sender, RoutedEventArgs e)
        {
            if (BotonPrincipal == null) return;
            BotonPrincipal.Content = ModoRegistro ? "Crear cuenta" : "Iniciar sesión";
            AyudaPassword.Visibility = ModoRegistro ? Visibility.Visible : Visibility.Collapsed;
            OcultarMensaje();
        }

        private void AlTeclearPassword(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) AlPulsarPrincipal(sender, e);
        }

        private async void AlPulsarPrincipal(object sender, RoutedEventArgs e)
        {
            if (_ocupado) return;
            OcultarMensaje();
            EstablecerOcupado(true);
            try
            {
                var email = CajaEmail.Text.Trim();
                var pass = CajaPassword.Password;
                var resultado = ModoRegistro
                    ? await _auth.RegistrarseAsync(email, pass)
                    : await _auth.IniciarSesionAsync(email, pass);

                if (resultado.Exitoso) await EntrarAsync(resultado);
                else MostrarFalla(resultado);
            }
            catch (Exception ex)
            {
                MostrarMensaje(TipoAviso.Error, "Error: no se pudo completar la operación (" + ex.Message + ").");
            }
            finally
            {
                EstablecerOcupado(false);
            }
        }

        private async void AlPulsarSinCuenta(object sender, RoutedEventArgs e)
        {
            
            if (_ocupado) return;
            EstablecerOcupado(true);
            try { await EntrarAsync(_auth.UsarSinCuenta()); }
            catch (Exception ex) { MostrarMensaje(TipoAviso.Error, "Error: no se pudo iniciar la sesión local (" + ex.Message + ")."); }
            finally { EstablecerOcupado(false); }
        }

        private void AlPulsarContinuarLocal(object sender, RoutedEventArgs e) => AlPulsarSinCuenta(sender, e);

        private void AlPulsarCrearDesdeError(object sender, RoutedEventArgs e)
        {
            OpcionRegistrar.IsChecked = true;
            MostrarMensaje(TipoAviso.Info, "Información: elegí una contraseña para crear la cuenta " + CajaEmail.Text.Trim() + ".");
            CajaPassword.Focus();
        }

        private void MostrarFalla(ResultadoAutenticacion r)
        {
            MostrarMensaje(TipoAviso.Error, "Error: " + r.Mensaje);

            bool usuarioInexistente = r.Codigo == CodigoAuth.UsuarioInexistente;
            bool fallaRed = r.Codigo == CodigoAuth.FalloRed;
            if (usuarioInexistente || fallaRed)
            {
                PanelAcciones.Visibility = Visibility.Visible;
                BotonCrearDesdeError.Visibility = usuarioInexistente ? Visibility.Visible : Visibility.Collapsed;
                BotonContinuarLocal.Content = fallaRed ? "Continuar sin iniciar sesión" : "Usar sin cuenta (solo local)";
            }
        }

        private async Task EntrarAsync(ResultadoAutenticacion acceso)
        {
            MostrarMensaje(TipoAviso.Info, acceso.ModoSinCuenta
                ? "Cargando tus perfiles locales…"
                : "Recuperando tus perfiles y configuraciones…");

            var ctx = await ContextoApp.CrearAsync(acceso, _sincronizacion);

            if (!ctx.Almacen.Config.EvaluacionCompleta)
            {
                var evaluacion = new VentanaEvaluacion();
                Application.Current.MainWindow = evaluacion;
                evaluacion.Show();
                Close();
                return;
            }

            var principal = new VentanaPrincipal(ctx);
            Application.Current.MainWindow = principal;
            
            principal.Show();
            Close();
        }

        private void EstablecerOcupado(bool ocupado)
        {
            _ocupado = ocupado;
            BotonPrincipal.IsEnabled = !ocupado;
            BotonSinCuenta.IsEnabled = !ocupado;
            BotonCrearDesdeError.IsEnabled = !ocupado;
            BotonContinuarLocal.IsEnabled = !ocupado;
            CajaEmail.IsEnabled = !ocupado;
            CajaPassword.IsEnabled = !ocupado;
            Cursor = ocupado ? Cursors.Wait : null;
        }

        private void OcultarMensaje()
        {
            PanelMensaje.Visibility = Visibility.Collapsed;
            PanelAcciones.Visibility = Visibility.Collapsed;
            BotonCrearDesdeError.Visibility = Visibility.Collapsed;
        }

        private void MostrarMensaje(TipoAviso tipo, string texto)
        {
            PanelAcciones.Visibility = Visibility.Collapsed;
            BotonCrearDesdeError.Visibility = Visibility.Collapsed;
            TextoMensaje.Text = texto;
            var clave = tipo switch
            {
                TipoAviso.Error => "BrushError",
                TipoAviso.Advertencia => "BrushAdvertencia",
                TipoAviso.Exito => "BrushOk",
                _ => "BrushAcentoClaro"
            };
            var pincel = (Brush)FindResource(clave);
            PanelMensaje.BorderBrush = pincel;
            TextoMensaje.Foreground = (Brush)FindResource("BrushTexto");
            PanelMensaje.Visibility = Visibility.Visible;
        }
    }
}
