using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
namespace Cromatico.UI;

public partial class TestWindow : Window
{
    private int indiceActual=0;
    private List<LaminaIshihara> laminas;
    private List<RespuestaUsuario> respuestas= new();
    private TipoTest Tipo;
    
    public TestWindow(List<LaminaIshihara> Laminas,TipoTest Tipo)
    {
        this.laminas=Laminas;
        this.Tipo=Tipo;
        
        InitializeComponent();

        MostrarLamina();
        
    }
    private void MostrarLamina()
    {
        try
        {
        if(indiceActual<0||indiceActual>=laminas.Count) return;

        LaminaIshihara lamina = laminas[indiceActual];
        txtNumeroLamina.Text=$"Lámina {lamina.numero}";
        string rutaCompleta =
        System.IO.Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        lamina.imagen);
            if (!File.Exists(rutaCompleta))
            {
                throw new FileNotFoundException("NO se encontro la imagen",rutaCompleta);
            }
            else
            {
                imgLamina.Source =new BitmapImage(new Uri(rutaCompleta, UriKind.Absolute));
            }
        }catch(Exception ex)
        {
            string nombreImagen=indiceActual < laminas.Count ? laminas[indiceActual].imagen 
            : "Indice invalido";
            GuardarExcepcion.registrarExcepcion(ex,$"Error cargando {nombreImagen}");
            MessageBox.Show(ex.Message);
        }
        
    }
    private void BtnSiguiente_Click(object sender,RoutedEventArgs e)
    {
        try
        {
            string texto = txtRespuesta.Text.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show("Debe ingresar una respuesta.");
                return;
            }
            bool esNumero=int.TryParse(texto,out _);
            if(!esNumero && !texto.Equals("nada", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("La respuesta debe ser un número o la palabra 'nada'.");
                return;
            }
            
            RespuestaUsuario respuesta=new RespuestaUsuario();
            if(texto.Equals("nada",StringComparison.OrdinalIgnoreCase)){
                texto="nada";
            }
            respuesta.respuesta=texto;
            respuesta.numeroLamina=laminas[indiceActual].numero;
            respuestas.Add(respuesta);
            indiceActual++;
            if (indiceActual >= laminas.Count)
                {
                    finalizarTest();
                    return;
                }
            txtRespuesta.Text="";
            MostrarLamina();
            
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex,$"Error al guardar respuesta numero: {indiceActual}");
        }
        
    }

    private void finalizarTest()
    {
        switch (Tipo)
        {
            case TipoTest.evaluacion:
            finalizarDeteccion();
            break;
            case TipoTest.discriminacion:
            finalizarDiscriminacion();
            break;
            case TipoTest.severidad:
            finalizarSeveridad();
            break;
        }
    }

    private void finalizarDeteccion()
    {
        EvaluadorIshihara evaluador = new EvaluadorIshihara();
        ResultadoTest resultado = new ResultadoTest();
        resultado = evaluador.Evaluar(respuestas);
        MessageBox.Show($"Deficiencia: {resultado.deficiencia},"+
        $" Aciertos: {resultado.aciertos},"+$" Errores: {resultado.errores},"+
        $" Confianza del test: {resultado.confianza}%");

        if(resultado.deficiencia == TipoDeficiencia.RojoVerde)
        {
            var laminasDiscriminacion = LaminasDiscriminacionIshihara.ObtenerLaminasDiscriminacion();
            TestWindow ventanaDiscriminacion = new TestWindow(laminasDiscriminacion,TipoTest.discriminacion);
            ventanaDiscriminacion.Show();
            this.Close();
            return;
        }
        else
        {
            this.Close();
            return;
        }
    }
    private void finalizarDiscriminacion()
    {
        EvaluadorDiscriminacionIhihara evalDiscriminacion = new EvaluadorDiscriminacionIhihara();
        ResultadoTest resultado= evalDiscriminacion.evaluarDiscrimincacion(respuestas);
        MessageBox.Show($"Deficiencia: {resultado.deficiencia},"+
        $" Aciertos: {resultado.aciertos},"+$" Errores: {resultado.errores},"+
        $" Confianza del test: {resultado.confianza}%");
        if(resultado.deficiencia == TipoDeficiencia.RojoProtan)
        {
            MessageBox.Show("se deberia ejecutar severidad con laminas protan");
            this.Close();
            return;

        }else if(resultado.deficiencia == TipoDeficiencia.VerdeDeutan)
        {
            MessageBox.Show("se deberia ejecutar severidad con laminas deutan");
            this.Close();
            return;
        }
    }
    private void finalizarSeveridad()
    {
        
    }
}