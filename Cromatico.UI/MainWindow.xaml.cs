using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cromatico.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// Interaction logic for TestWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void BtnIniciar_Click(object sender, RoutedEventArgs e)
    {
        var laminasDeteccion=LaminasDeteccionIshihara.ObtenerLaminasDeteccion();
        TipoTest tipo=TipoTest.evaluacion;
        TestWindow ventana = new TestWindow(laminasDeteccion,tipo);
        ventana.Show();
        this.Close();
    }
}