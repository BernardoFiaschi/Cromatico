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

namespace Cromatico.UI;
public partial class CorreccionWindow : Window
{
    private Bitmap imagenOriginal;
    public CorreccionWindow()
{
    InitializeComponent();
    imagenOriginal =new Bitmap("ImagenesPrueba\\prueba.png");

    imgOriginal.Source =HelperBitMapImage.CargarImagen("ImagenesPrueba\\prueba.png");

    cmbDeficiencia.SelectedIndex = 0;

    ActualizarVistaPrevia();
}

    private void cmbDeficiencia_SelectionChanged(object sender,SelectionChangedEventArgs e)
{
    ActualizarVistaPrevia();
}

    private void Slider_ValueChanged(object sender,RoutedPropertyChangedEventArgs<double> e)
{
    if(!IsLoaded)
        return;

    ActualizarVistaPrevia();
}

private void ActualizarVistaPrevia()
{
    if(cmbDeficiencia == null ||
    sliderSeveridad == null ||
    sliderIntensidad == null ||
    imgCorregida == null)
{
    return;
}


txtSeveridad.Text = $"Severidad: {sliderSeveridad.Value:F2}";
txtIntensidad.Text =$"Intensidad: {sliderIntensidad.Value:F2}";
TipoDeficiencia tipo =TipoDeficiencia.Protan;

if(cmbDeficiencia.SelectedIndex == 1)
{
    tipo = TipoDeficiencia.Deutan;
}

if(cmbDeficiencia.SelectedIndex == 2)
{
    tipo = TipoDeficiencia.Tritan;
}

string archivoOriginalSimulada = $"OriginalSimulada_{tipo}.png";
Title = $"{tipo} - {DateTime.Now:HH:mm:ss}";

ConfiguracionCorreccion config =new ConfiguracionCorreccion(tipo,sliderSeveridad.Value,
        sliderIntensidad.Value);

Bitmap corregida =ProcesadorImagen.CorregirImagen(imagenOriginal,config);

Bitmap corregidaSimulada =SimuladorImagenMachado.SimularImagen(corregida,tipo);

Bitmap originalSimulada =SimuladorImagenMachado.SimularImagen(imagenOriginal,tipo);

System.Drawing.Color pixelPrueba =
    originalSimulada.GetPixel(0,0);

MessageBox.Show(
    $"Tipo: {tipo}\n" +
    $"R={pixelPrueba.R}\n" +
    $"G={pixelPrueba.G}\n" +
    $"B={pixelPrueba.B}");

    corregida.Save("Temporal.png");

corregidaSimulada.Save("TemporalSimulada.png");

originalSimulada.Save(archivoOriginalSimulada);

imgCorregida.Source =HelperBitMapImage.CargarImagen("Temporal.png");

imgCorregidaSimulada.Source =HelperBitMapImage.CargarImagen("TemporalSimulada.png");

imgOriginalSimulada.Source =HelperBitMapImage.CargarImagen(archivoOriginalSimulada);
}
}