using System.Windows;
using System.Windows.Threading;

namespace Cromatico.UI
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += AlFallarLaInterfaz;
        }

        // Al salir desde cualquier ventana, la pantalla vuelve a sus colores originales.
        protected override void OnExit(ExitEventArgs e)
        {
            Servicios.ContextoApp.Actual?.Dispose();
            base.OnExit(e);
        }

        private void AlFallarLaInterfaz(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var ctx = Servicios.ContextoApp.Actual;
            if (ctx != null) ctx.Avisos.Error("Ocurrió un error inesperado: " + e.Exception.Message);
            else MessageBox.Show("Ocurrió un error inesperado: " + e.Exception.Message, "Cromático",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }
}
