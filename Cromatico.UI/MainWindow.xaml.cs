using System.Collections.Generic;
using System.IO;
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
using CorreccionCromatica;
using System.Drawing;
using Cromatico.Entidades;
using CorreccionCromatica;

namespace Cromatico.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// Interaction logic for TestWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
     private BitmapSource CrearImagen(byte[] bgra,int ancho,int alto)
        {
        return BitmapSource.Create(ancho,alto,96,96,PixelFormats.Bgra32,null,bgra,ancho * 4);
        }
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
    private void btnProbar_Click(object sender,RoutedEventArgs e)
    {
        VentanaR6 ventana = new VentanaR6();

        ventana.Show();
    }
    
}