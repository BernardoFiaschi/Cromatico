using System.Windows;
using System.Windows.Controls;
using Cromatico.Modelos;
using Cromatico.Servicios;

namespace Cromatico.Vistas
{
    public partial class PaginaPerfiles : UserControl
    {
        public sealed class FilaPerfil
        {
            public string Id { get; init; } = "";
            public string Nombre { get; init; } = "";
            public string Resumen { get; init; } = "";
            public string Etiqueta { get; init; } = "";
            public bool EsPredefinido { get; init; }
            public bool EsActivo { get; init; }
            public Visibility VisibleActivo => EsActivo ? Visibility.Visible : Visibility.Collapsed;
        }

        private ContextoApp? _ctx;
        private bool _cargando;
        private string? _seleccionId;

        public PaginaPerfiles()
        {
            InitializeComponent();
            Loaded += AlCargar;
            Unloaded += AlDescargar;
        }

        private void AlCargar(object sender, RoutedEventArgs e)
        {
            _ctx = ContextoApp.Actual;
            if (_ctx == null) return;
            _ctx.Perfiles.Cambio += AlCambiarPerfiles;
            _seleccionId ??= _ctx.Almacen.Config.PerfilActivoId;
            Refrescar();
        }

        private void AlDescargar(object sender, RoutedEventArgs e)
        {
            if (_ctx != null) _ctx.Perfiles.Cambio -= AlCambiarPerfiles;
            _ctx = null;
        }

        private void AlCambiarPerfiles()
        {
            if (!Dispatcher.CheckAccess()) { Dispatcher.BeginInvoke(AlCambiarPerfiles); return; }
            Refrescar();
        }

        private void Refrescar()
        {
            if (_ctx == null) return;
            var cfg = _ctx.Almacen.Config;
            if (cfg.BuscarPerfil(_seleccionId) == null) _seleccionId = cfg.PerfilActivoId;

            _cargando = true;
            try
            {
                var filas = _ctx.Perfiles.Perfiles.Select(p => new FilaPerfil
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Resumen = p.Resumen,
                    EsPredefinido = p.EsPredefinido,
                    Etiqueta = p.EsPredefinido ? "Predefinido" : "Personalizado",
                    EsActivo = p.Id == cfg.PerfilActivoId
                }).ToList();

                Lista.ItemsSource = filas;
                Lista.SelectedItem = filas.FirstOrDefault(f => f.Id == _seleccionId);
            }
            finally { _cargando = false; }
            ActualizarBotones();
        }

        private FilaPerfil? Seleccionada => Lista.SelectedItem as FilaPerfil;

        private void AlCambiarSeleccion(object sender, SelectionChangedEventArgs e)
        {
            if (_cargando) return;
            if (Seleccionada != null) _seleccionId = Seleccionada.Id;
            ActualizarBotones();
        }

        private void ActualizarBotones()
        {
            var s = Seleccionada;
            bool hay = s != null;
            BotonUsar.IsEnabled = hay && !s!.EsActivo;
            BotonAjustar.IsEnabled = hay;
            BotonDuplicar.IsEnabled = hay;
            BotonRenombrar.IsEnabled = hay && !s!.EsPredefinido;
            BotonEliminar.IsEnabled = hay && !s!.EsPredefinido && !s.EsActivo;
            BotonEliminar.ToolTip = hay && s!.EsActivo ? "No se puede eliminar el perfil activo." : null;
        }

        private void Avisar(ResultadoOperacion r, string? exito = null)
        {
            if (_ctx == null) return;
            if (r.Exitoso) { if (!string.IsNullOrEmpty(exito ?? r.Mensaje)) _ctx.Avisos.Exito(exito ?? r.Mensaje); }
            else _ctx.Avisos.Error(r.Mensaje);
        }

        private void AlPulsarUsar(object sender, RoutedEventArgs e)
        {
            if (_ctx == null || Seleccionada == null) return;
            var r = _ctx.Perfiles.Seleccionar(Seleccionada.Id);
            if (r.Exitoso) _ctx.AutoPerfil.Recalcular();
            Avisar(r, $"Perfil «{Seleccionada.Nombre}» activado.");
            Refrescar();
        }

        private void AlPulsarAjustar(object sender, RoutedEventArgs e)
        {
            if (Seleccionada != null) VentanaPrincipal.Actual?.IrA("vista", Seleccionada.Id);
        }

        private void AlPulsarDuplicar(object sender, RoutedEventArgs e)
        {
            if (_ctx == null || Seleccionada == null) return;
            string? error = null;
            string sugerido = Seleccionada.Nombre + " (copia)";
            while (true)
            {
                var dlg = new DialogoNombre(Window.GetWindow(this), "Nuevo perfil",
                    $"Se crea a partir de «{Seleccionada.Nombre}».", sugerido, error);
                if (dlg.ShowDialog() != true) return;

                var r = _ctx.Perfiles.Duplicar(Seleccionada.Id, dlg.Resultado);
                if (r.Exitoso && r.Valor != null)
                {
                    _seleccionId = r.Valor.Id;
                    Avisar(r);
                    Refrescar();
                    return;
                }
                error = r.Mensaje;
                sugerido = dlg.Resultado;
            }
        }

        private void AlPulsarRenombrar(object sender, RoutedEventArgs e)
        {
            if (_ctx == null || Seleccionada == null || Seleccionada.EsPredefinido) return;
            string? error = null;
            string sugerido = Seleccionada.Nombre;
            while (true)
            {
                var dlg = new DialogoNombre(Window.GetWindow(this), "Renombrar perfil",
                    "Elegí un nombre que no esté en uso.", sugerido, error);
                if (dlg.ShowDialog() != true) return;

                var r = _ctx.Perfiles.Renombrar(Seleccionada.Id, dlg.Resultado);
                if (r.Exitoso) { Avisar(r, "Perfil renombrado."); Refrescar(); return; }
                error = r.Mensaje;
                sugerido = dlg.Resultado;
            }
        }

        private void AlPulsarEliminar(object sender, RoutedEventArgs e)
        {
            if (_ctx == null || Seleccionada == null) return;
            var resp = MessageBox.Show(Window.GetWindow(this),
                $"¿Eliminar el perfil «{Seleccionada.Nombre}»? Esta acción no se puede deshacer.",
                "Eliminar perfil", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (resp != MessageBoxResult.Yes) return;

            var r = _ctx.Perfiles.Eliminar(Seleccionada.Id);
            if (r.Exitoso) _seleccionId = _ctx.Almacen.Config.PerfilActivoId;
            Avisar(r, "Perfil eliminado.");
            Refrescar();
        }
    }
}
