using System.Windows;

namespace Cromatico.Vistas
{
    public partial class DialogoNombre : Window
    {
        public string Resultado { get; private set; } = string.Empty;

        public DialogoNombre(Window? propietario, string titulo, string detalle, string valorInicial, string? error = null)
        {
            InitializeComponent();
            TemaVentana.AplicarBarraOscura(this);
            if (propietario != null) Owner = propietario;
            Title = titulo;
            Titulo.Text = titulo;
            Detalle.Text = detalle;
            Caja.Text = valorInicial;
            if (!string.IsNullOrEmpty(error))
            {
                TextoError.Text = "Error: " + error;
                TextoError.Visibility = Visibility.Visible;
            }
            Loaded += (_, _) => { Caja.Focus(); Caja.SelectAll(); };
        }

        private void AlAceptar(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Caja.Text))
            {
                TextoError.Text = "Error: escribí un nombre.";
                TextoError.Visibility = Visibility.Visible;
                return;
            }
            Resultado = Caja.Text.Trim();
            DialogResult = true;
        }
    }
}
