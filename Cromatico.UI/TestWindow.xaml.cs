using System.Windows;
namespace Cromatico.UI;

public partial class TestWindow : Window
{
    private int indiceActual=0;
    private List<LaminaIshihara> laminas;
    private List<RespuestaUsuario> respuestas= new();
    public TestWindow()
    {
        laminas=LaminasDeteccionIshihara.ObtenerLaminasDeteccion();
        
        InitializeComponent();

        MostrarLamina();
        
    }
    private void MostrarLamina()
    {
        LaminaIshihara lamina = laminas[indiceActual];
        txtNumeroLamina.Text=$"Lámina {lamina.numero}";
    }
    private void BtnSiguiente_Click(object sender,RoutedEventArgs e)
    {
        
        RespuestaUsuario respuesta=new RespuestaUsuario();
        respuesta.numeroLamina=laminas[indiceActual].numero;
        respuesta.respuesta=txtRespuesta.Text;
        respuestas.Add(respuesta);
        indiceActual++;
        if (indiceActual >= laminas.Count)
        {
            MessageBox.Show($"Respuestas guardadas: {respuestas.Count}");
            this.Close();
            return;
        }
        MostrarLamina();
        txtRespuesta.Text="";
    }
}