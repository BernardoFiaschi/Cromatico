using System.Windows;
using System.Windows.Controls;
using Cromatico.Servicios;

namespace Cromatico.Vistas
{
    public partial class PaginaAjustes : UserControl
    {
        private ContextoApp? _ctx;

        public PaginaAjustes()
        {
            InitializeComponent();
            Loaded += AlCargar;
            Unloaded += AlDescargar;
        }

        private void AlCargar(object sender, RoutedEventArgs e)
        {
            _ctx = ContextoApp.Actual;
            if (_ctx == null) return;
            _ctx.EstadoNubeCambiado += Refrescar;
            CajaRuta.Text = _ctx.Almacen.Ruta;
            Refrescar();
        }

        private void AlDescargar(object sender, RoutedEventArgs e)
        {
            if (_ctx != null) _ctx.EstadoNubeCambiado -= Refrescar;
            _ctx = null;
        }

        private void Refrescar()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(Refrescar); return; }
            if (_ctx == null) return;

            if (_ctx.ModoSinCuenta)
            {
                TextoCuenta.Text = "Sin cuenta";
                TextoEstado.Text = "Los perfiles solo existen en este equipo. Cerrá la sesión para iniciar sesión o crear una cuenta.";
                BotonSincronizar.IsEnabled = false;
                BotonCerrarSesion.Content = "Iniciar sesión o crear cuenta";
            }
            else
            {
                TextoCuenta.Text = _ctx.Email;
                string ultima = _ctx.UltimaSincronizacionUtc is DateTime d
                    ? "Última sincronización: " + d.ToLocalTime().ToString("g")
                    : "Todavía sin sincronizar en esta sesión";
                TextoEstado.Text = _ctx.NubeDisponible
                    ? ultima + "."
                    : "Sin conexión con la cuenta. Los cambios se guardan en este equipo y se subirán cuando vuelva la conexión.";
                BotonSincronizar.IsEnabled = true;
                BotonCerrarSesion.Content = "Cerrar sesión";
            }
        }

        private async void AlPulsarSincronizar(object sender, RoutedEventArgs e)
        {
            if (_ctx == null) return;
            var ctx = _ctx;
            BotonSincronizar.IsEnabled = false;
            try
            {
                var r = await ctx.SincronizarAsync();
                if (r.Exitoso) ctx.Avisos.Exito(string.IsNullOrEmpty(r.Mensaje) ? "Sincronizado." : r.Mensaje);
                else ctx.Avisos.Error(r.Mensaje);
                ctx.AutoPerfil.Recalcular();
                ctx.Perfiles.NotificarCambio();
            }
            finally
            {
                if (_ctx != null) Refrescar();
            }
        }

        private void AlPulsarCerrarSesion(object sender, RoutedEventArgs e)
            => VentanaPrincipal.Actual?.CerrarSesion();
    }
}
