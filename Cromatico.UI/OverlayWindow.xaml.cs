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
using System.Windows.Threading;
using CorreccionCromatica;
using System.Drawing;
using Cromatico.Entidades;
using CorreccionCromatica;

namespace Cromatico.UI;
public partial class OverlayWindow : Window
{
    private DispatcherTimer timer;
    private ServicioVistaPrevia vista;
    private BitmapSource CrearImagen(byte[] bgra,int ancho,int alto)
        {
        return BitmapSource.Create(ancho,alto,96,96,PixelFormats.Bgra32,null,bgra,ancho * 4);
        }
    public OverlayWindow()
    {
        InitializeComponent();
        timer = new DispatcherTimer();
        timer.Interval =
        TimeSpan.FromMilliseconds(1000);

        timer.Tick += Timer_Tick;

        timer.Start();
        
    }
    private void Timer_Tick(object sender,EventArgs e)
    {
        ActualizarPantalla();
    }

    public void ActualizarPantalla()
    {
        Visibility = Visibility.Hidden;
        vista = new ServicioVistaPrevia();

        Dispatcher.Invoke(DispatcherPriority.Render,new Action(() => { }));

        Thread.Sleep(100);

        var captura =vista.CapturarPantalla();
        Visibility = Visibility.Visible;

        if (!captura.Exitoso)
        return;

        var resultado =vista.Generar(TipoDeficiencia.Protan,1.0,1.0);

        if (!resultado.Exitoso ||
            resultado.Valor == null ||
            vista.Imagen == null)
            return;

        imgOverlay.Source =CrearImagen(resultado.Valor,vista.Imagen.Ancho,vista.Imagen.Alto);
    }

}