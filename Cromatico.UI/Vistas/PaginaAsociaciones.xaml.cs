using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Cromatico.Modelos;
using Cromatico.Servicios;
using Microsoft.Win32;

namespace Cromatico.UI.Vistas
{
    public class AplicacionDisponible
    {
        private string Proceso = string.Empty;
        private string Ruta = string.Empty;
        private string Titulo = string.Empty;
        public AplicacionDisponible() { }

        public string proceso
        {
            get { return Proceso; }
            set { Proceso = value; }
        }
        public string ruta
        {
            get { return Ruta; }
            set { Ruta = value; }
        }
        public string titulo
        {
            get { return Titulo; }
            set { Titulo = value; }
        }
        public override string ToString()
        {
            return $"{Titulo}  ({Proceso})";
        }
    }

    public class FilaAsociacion
    {
        private string Proceso = string.Empty;
        private string Ruta = string.Empty;
        private string Perfil = string.Empty;
        public FilaAsociacion() { }

        public string proceso
        {
            get { return Proceso; }
            set { Proceso = value; }
        }
        public string ruta
        {
            get { return Ruta; }
            set { Ruta = value; }
        }
        public string perfil
        {
            get { return Perfil; }
            set { Perfil = value; }
        }
    }

    public partial class PaginaAsociaciones : UserControl
    {
        private ServicioAsociaciones? servicioAsociaciones;

        public PaginaAsociaciones()
        {
            InitializeComponent();
            Loaded += (_, _) => Cargar();
        }

        private void Cargar()
        {
            var ctx = ContextoApp.Actual;
            if (ctx == null)
                return;

            servicioAsociaciones = new ServicioAsociaciones(ctx.Almacen);

            if (cbAplicaciones.Items.Count == 0)
                CargarAplicacionesAbiertas();

            CargarPerfiles();
            CargarAsociaciones();
        }

        private void CargarAplicacionesAbiertas()
        {
            List<AplicacionDisponible> aplicaciones = new List<AplicacionDisponible>();
            int procesoPropio = Environment.ProcessId;

            foreach (var proceso in Process.GetProcesses())
            {
                try
                {
                    if (proceso.Id == procesoPropio || proceso.MainWindowHandle == IntPtr.Zero
                        || string.IsNullOrWhiteSpace(proceso.MainWindowTitle))
                        continue;

                    string? ruta = proceso.MainModule?.FileName;
                    if (string.IsNullOrEmpty(ruta))
                        continue;

                    aplicaciones.Add(new AplicacionDisponible
                    {
                        proceso = proceso.ProcessName.ToLowerInvariant(),
                        ruta = ruta,
                        titulo = proceso.MainWindowTitle
                    });
                }
                catch
                {
                    // Windows no deja leer algunos procesos del sistema: se omiten.
                }
                finally
                {
                    proceso.Dispose();
                }
            }

            cbAplicaciones.Items.Clear();
            foreach (var aplicacion in aplicaciones.GroupBy(a => a.proceso).Select(g => g.First()).OrderBy(a => a.titulo))
            {
                cbAplicaciones.Items.Add(aplicacion);
            }
        }

        private void CargarPerfiles()
        {
            var ctx = ContextoApp.Actual;
            if (ctx == null)
                return;

            var seleccionado = cbPerfiles.SelectedItem as PerfilCorreccion;
            cbPerfiles.ItemsSource = ctx.Perfiles.Perfiles.ToList();
            cbPerfiles.SelectedItem = ctx.Perfiles.Perfiles.FirstOrDefault(p => p.Id == seleccionado?.Id)
                ?? ctx.Perfiles.PerfilActivo;
        }

        private void CargarAsociaciones()
        {
            var ctx = ContextoApp.Actual;
            if (ctx == null || servicioAsociaciones == null)
                return;

            List<FilaAsociacion> filas = servicioAsociaciones.ObtenerTodas().Select(a => new FilaAsociacion
            {
                proceso = a.Proceso,
                ruta = a.Ruta,
                perfil = ctx.Almacen.Config.BuscarPerfil(a.PerfilId)?.Nombre ?? "(perfil eliminado)"
            }).ToList();

            lstAsociaciones.ItemsSource = filas;
            txtSinAsociaciones.Visibility = filas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarAplicacionesAbiertas();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialogo = new OpenFileDialog
            {
                Filter = "Aplicaciones (*.exe)|*.exe",
                Title = "Elegí la aplicación"
            };

            if (dialogo.ShowDialog() != true)
                return;

            AplicacionDisponible aplicacion = new AplicacionDisponible
            {
                proceso = Path.GetFileNameWithoutExtension(dialogo.FileName).ToLowerInvariant(),
                ruta = dialogo.FileName,
                titulo = Path.GetFileName(dialogo.FileName)
            };
            cbAplicaciones.Items.Add(aplicacion);
            cbAplicaciones.SelectedItem = aplicacion;
        }

        private void BtnAsociar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ctx = ContextoApp.Actual;
                if (ctx == null || servicioAsociaciones == null)
                    return;

                if (cbAplicaciones.SelectedItem is not AplicacionDisponible aplicacion)
                {
                    ctx.Avisos.Advertir("Elegí una aplicación de la lista o buscá su archivo .exe.");
                    return;
                }
                if (cbPerfiles.SelectedItem is not PerfilCorreccion perfil)
                {
                    ctx.Avisos.Advertir("Elegí un perfil.");
                    return;
                }

                ResultadoOperacion resultado = servicioAsociaciones.Asociar(aplicacion.proceso, aplicacion.ruta, perfil.Id);
                if (resultado.Exitoso)
                {
                    ctx.AutoPerfil.Recalcular();
                    ctx.Avisos.Exito(resultado.Mensaje);
                }
                else ctx.Avisos.Error(resultado.Mensaje);
                CargarAsociaciones();
            }
            catch (Exception ex)
            {
                GuardarExcepcion.registrarExcepcion(ex, "PaginaAsociaciones.Asociar");
                ContextoApp.Actual?.Avisos.Error("Ocurrió un error al asociar la aplicación: " + ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ctx = ContextoApp.Actual;
                if (ctx == null || servicioAsociaciones == null)
                    return;

                if (lstAsociaciones.SelectedItem is not FilaAsociacion fila)
                {
                    ctx.Avisos.Advertir("Elegí la asociación que querés quitar.");
                    return;
                }

                ResultadoOperacion resultado = servicioAsociaciones.Eliminar(fila.proceso);
                if (resultado.Exitoso)
                {
                    ctx.AutoPerfil.Recalcular();
                    ctx.Avisos.Exito(resultado.Mensaje);
                }
                else ctx.Avisos.Error(resultado.Mensaje);
                CargarAsociaciones();
            }
            catch (Exception ex)
            {
                GuardarExcepcion.registrarExcepcion(ex, "PaginaAsociaciones.Eliminar");
                ContextoApp.Actual?.Avisos.Error("Ocurrió un error al eliminar la asociación: " + ex.Message);
            }
        }
    }
}
