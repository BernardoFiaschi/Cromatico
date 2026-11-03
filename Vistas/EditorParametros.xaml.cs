using System.Windows.Controls;
using Cromatico.Modelos;

namespace Cromatico.Vistas
{
    public partial class EditorParametros : UserControl
    {
        private bool _silencio;

        public event Action? Cambio;

        public EditorParametros()
        {
            InitializeComponent();
            _silencio = true;
            ComboEje.Items.Add(new ComboBoxItem { Content = EjeCorreccion.RojoVerde.Nombre(), Tag = EjeCorreccion.RojoVerde });
            ComboEje.Items.Add(new ComboBoxItem { Content = EjeCorreccion.AzulAmarillo.Nombre(), Tag = EjeCorreccion.AzulAmarillo });
            _silencio = false;
            Establecer(TipoDeficiencia.Deutan, 0.6, 0.5);
        }

        public TipoDeficiencia Tipo =>
            (ComboTipo.SelectedItem as ComboBoxItem)?.Tag is TipoDeficiencia t ? t : TipoDeficiencia.Deutan;

        public double Severidad => DeslizadorSeveridad.Value / 100.0;
        public double Intensidad => DeslizadorIntensidad.Value / 100.0;

        public void Establecer(TipoDeficiencia tipo, double severidad, double intensidad)
        {
            bool previo = _silencio;
            _silencio = true;
            try
            {
                SeleccionarEje(tipo.Eje());
                LlenarTipos(tipo.Eje(), tipo);
                DeslizadorSeveridad.Value = Math.Round(Math.Clamp(severidad, 0, 1) * 100);
                DeslizadorIntensidad.Value = Math.Round(Math.Clamp(intensidad, 0, 1) * 100);
                ActualizarEtiquetas();
            }
            finally { _silencio = previo; }
        }

        private void SeleccionarEje(EjeCorreccion eje)
        {
            foreach (ComboBoxItem it in ComboEje.Items)
                if (it.Tag is EjeCorreccion e && e == eje) { ComboEje.SelectedItem = it; return; }
        }

        private void LlenarTipos(EjeCorreccion eje, TipoDeficiencia seleccionado)
        {
            ComboTipo.Items.Clear();
            var tipos = eje == EjeCorreccion.RojoVerde
                ? new[] { TipoDeficiencia.Protan, TipoDeficiencia.Deutan }
                : new[] { TipoDeficiencia.Tritan };
            foreach (var t in tipos)
            {
                var item = new ComboBoxItem { Content = t.Nombre(), Tag = t };
                ComboTipo.Items.Add(item);
                if (t == seleccionado) ComboTipo.SelectedItem = item;
            }
            if (ComboTipo.SelectedItem == null && ComboTipo.Items.Count > 0) ComboTipo.SelectedIndex = 0;
        }

        private void AlCambiarEje(object sender, SelectionChangedEventArgs e)
        {
            if (_silencio || ComboEje.SelectedItem is not ComboBoxItem { Tag: EjeCorreccion eje }) return;
            _silencio = true;
            try
            {
                LlenarTipos(eje, eje == EjeCorreccion.RojoVerde ? TipoDeficiencia.Deutan : TipoDeficiencia.Tritan);
            }
            finally { _silencio = false; }
            Cambio?.Invoke();
        }

        private void AlCambiarTipo(object sender, SelectionChangedEventArgs e)
        {
            if (_silencio) return;
            Cambio?.Invoke();
        }

        private void AlCambiarValor(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValorIntensidad == null || ValorSeveridad == null) return;
            ActualizarEtiquetas();
            if (_silencio) return;
            Cambio?.Invoke();
        }

        private void ActualizarEtiquetas()
        {
            ValorSeveridad.Text = $"{DeslizadorSeveridad.Value:0} %";
            ValorIntensidad.Text = $"{DeslizadorIntensidad.Value:0} %";
        }
    }
}
