using System.Windows;
using System.Windows.Controls;
using Cromatico.Modelos;
using Cromatico.Servicios;

namespace Cromatico.Vistas
{
    public partial class PaginaInicio : UserControl
    {
        private ContextoApp? _ctx;
        private bool _cargando;

        public PaginaInicio()
        {
            InitializeComponent();
            Loaded += AlCargar;
            Unloaded += AlDescargar;
        }

        private void AlCargar(object sender, RoutedEventArgs e)
        {
            _ctx = ContextoApp.Actual;
            if (_ctx == null) return;
            _ctx.AutoPerfil.EfectivoCambiado += Refrescar;
            _ctx.Perfiles.Cambio += Refrescar;
            _ctx.EstadoNubeCambiado += Refrescar;
            Refrescar();
        }

        private void AlDescargar(object sender, RoutedEventArgs e)
        {
            if (_ctx == null) return;
            _ctx.AutoPerfil.EfectivoCambiado -= Refrescar;
            _ctx.Perfiles.Cambio -= Refrescar;
            _ctx.EstadoNubeCambiado -= Refrescar;
            _ctx = null;
        }

        private void Refrescar()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(Refrescar); return; }
            if (_ctx == null) return;

            _cargando = true;
            try
            {
                var ef = _ctx.AutoPerfil.Efectivo;
                TextoPerfil.Text = ef.Perfil.Nombre;
                TextoOrigen.Text = ef.Origen switch
                {
                    OrigenPerfil.Aplicacion => $"Aplicado automáticamente por la aplicación «{ef.Aplicacion}».",
                    OrigenPerfil.Contexto => $"Aplicado por el contexto de uso ({ef.Aplicacion}).",
                    _ => "Perfil global (predeterminado)."
                };
                TextoResumen.Text = ef.Perfil.Resumen;

                bool auto = _ctx.Almacen.Config.AutoPerfilesActivo;
                InterruptorAuto.IsChecked = auto;
                TextoEstadoAuto.Text = auto ? "Activado" : "Desactivado: se usa siempre el perfil global.";

                var v = _ctx.AutoPerfil.UltimaVentana;
                if (v == null)
                {
                    TextoVentana.Text = "Sin detectar";
                    TextoRuta.Text = "Si no se puede leer la ventana activa se mantiene el perfil actual.";
                }
                else
                {
                    TextoVentana.Text = v.EsPropia ? "Cromático (esta aplicación)" : v.Nombre;
                    TextoRuta.Text = v.Ruta ?? "Ruta no disponible";
                }

                ComboGlobal.Items.Clear();
                foreach (var p in _ctx.Perfiles.Perfiles)
                {
                    var item = new ComboBoxItem { Content = p.Nombre, Tag = p.Id };
                    ComboGlobal.Items.Add(item);
                    if (p.Id == _ctx.Almacen.Config.PerfilActivoId) ComboGlobal.SelectedItem = item;
                }

                if (_ctx.ModoSinCuenta)
                {
                    TextoCuenta.Text = "Estás usando Cromático sin cuenta. Tus perfiles se guardan solo en este equipo.";
                    BotonSincronizar.IsEnabled = false;
                }
                else
                {
                    string ultima = _ctx.UltimaSincronizacionUtc is DateTime d
                        ? "Última sincronización: " + d.ToLocalTime().ToString("g") + "."
                        : "Todavía sin sincronizar en esta sesión.";
                    TextoCuenta.Text = $"{_ctx.Email}. " + (_ctx.NubeDisponible ? ultima : "Sin conexión: los cambios quedan guardados en este equipo.");
                    BotonSincronizar.IsEnabled = true;
                }
            }
            finally { _cargando = false; }
        }

        private void AlPulsarAuto(object sender, RoutedEventArgs e)
        {
            if (_ctx == null || _cargando) return;
            var r = _ctx.AutoPerfil.CambiarAutoActivo(InterruptorAuto.IsChecked == true);
            if (!r.Exitoso) _ctx.Avisos.Error(r.Mensaje);
            else _ctx.AutoPerfil.Recalcular();
            Refrescar();
        }

        private void AlElegirGlobal(object sender, SelectionChangedEventArgs e)
        {
            if (_ctx == null || _cargando) return;
            if (ComboGlobal.SelectedItem is not ComboBoxItem { Tag: string id }) return;
            if (id == _ctx.Almacen.Config.PerfilActivoId) return;

            var r = _ctx.Perfiles.Seleccionar(id);
            if (!r.Exitoso) _ctx.Avisos.Error(r.Mensaje);
            else _ctx.AutoPerfil.Recalcular();
            Refrescar();
        }

        private void AlPulsarAjustar(object sender, RoutedEventArgs e)
            => VentanaPrincipal.Actual?.IrA("vista", _ctx?.Almacen.Config.PerfilActivoId);

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
    }
}
