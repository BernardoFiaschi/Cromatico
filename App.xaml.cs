using System.Windows;
using System.Windows.Threading;

namespace Cromatico
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += AlFallarLaInterfaz;
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
