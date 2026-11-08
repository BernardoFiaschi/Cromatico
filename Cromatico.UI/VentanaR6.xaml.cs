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
using System.Windows.Threading;
using CorreccionCromatica;
using System.Drawing;
using Microsoft.Win32;
using Cromatico.Entidades;

namespace Cromatico.UI;

public partial class VentanaR6 : Window
{
    private readonly ServicioConfiguracion _config;
    private readonly ServicioAsociaciones _asociaciones;

    public VentanaR6()
    {
        InitializeComponent();

        _config =new ServicioConfiguracion("local");

        _config.Cargar();
        /* prueba R8
        _asociaciones =new ServicioAsociaciones(_config);
        _autoPerfil =
    new ServicioAutoPerfil(
        _config,
        new ServicioPerfiles(
            _config,
            new MotorNulo()),
        new MotorNulo(),
        new Notificador());
        _autoPerfil.ResolutorDeContexto =
    info => ServicioContextoUso.ObtenerPerfil(
        info,
        _config.Config.Perfiles);
        _monitor =
    new MonitorVentanaActiva();
    bool iniciado =
    _monitor.Iniciar();

MessageBox.Show(
    $"Monitor iniciado: {iniciado}");
    _monitor.VentanaCambiada +=
    (info,error) =>
{
    _autoPerfil.AlCambiarVentana(
        info,
        error);
};
_autoPerfil.EfectivoCambiado += () =>
{
    Dispatcher.Invoke(() =>
    {
        MessageBox.Show(
            $"Perfil: {_autoPerfil.Efectivo.Perfil.Nombre}\n" +
            $"Origen: {_autoPerfil.Efectivo.Origen}\n" +
            $"Aplicacion: {_autoPerfil.Efectivo.Aplicacion}");
    });
};
*/

        CargarPerfiles();

        CargarAsociaciones();
    }

    private void CargarPerfiles()
    {
        try
        {
            
            cmbPerfiles.Items.Clear();

            foreach(var perfil in _config.Config.Perfiles)
            {
                cmbPerfiles.Items.Add(perfil);
            }

            cmbPerfiles.DisplayMemberPath = "Nombre";
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex,"Ventana R6 CargarPerfiles");
        }
        
    }

    private void CargarAsociaciones()
    {
        try
        {
            lstAsociaciones.Items.Clear();

            foreach(var asociacion in _asociaciones.ObtenerTodas())
            {
                lstAsociaciones.Items.Add($"{asociacion.Proceso} -> {asociacion.PerfilId}");
            }
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex,"VentanR6 CargarAsociaciones");
        }
        
    }

    private void btnBuscarAplicacion_Click(object sender,RoutedEventArgs e)
    {
        try
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Ejecutables (*.exe)|*.exe";

            if(dlg.ShowDialog() == true)
            {
                txtRuta.Text = dlg.FileName;

                txtAplicacion.Text =Path.GetFileNameWithoutExtension(dlg.FileName);
            }
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex,"VentanaR6 btnBuscarAplicacion_Click");
        }
        
    }

    private void btnAsociar_Click(object sender,RoutedEventArgs e)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(txtAplicacion.Text))
            {
                MessageBox.Show("Primero debe seleccionar una aplicación.");
                return;
            }
            if(string.IsNullOrWhiteSpace(txtRuta.Text))
            {
                MessageBox.Show("Primero debe seleccionar una aplicación.");
                return;
            }
            if(cmbPerfiles.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un perfil.");
                return;
            }

            if(cmbPerfiles.SelectedItem is not PerfilCorreccion perfil)
            return;

            var resultado = _asociaciones.Asociar(txtAplicacion.Text,txtRuta.Text,perfil.Id);

            MessageBox.Show(resultado.Mensaje);

            if(resultado.Exitoso)
            {
                CargarAsociaciones();
            }
        }catch(Exception ex)
        {
            GuardarExcepcion.registrarExcepcion(ex,"VentanaR6 BtnAsociar_Click");
        }
        
    }

    private void btnEliminar_Click(object sender,RoutedEventArgs e)
    {
        /* prueba R6
        if(lstAsociaciones.SelectedItem == null)
        {
            MessageBox.Show("Debe seleccionar una asociación para eliminar.");
            return;
        }
        string texto = lstAsociaciones.SelectedItem.ToString() ?? "";

        string proceso = texto.Split("->")[0].Trim();

        var resultado =_asociaciones.Eliminar(proceso);

        MessageBox.Show(resultado.Mensaje);

        if(resultado.Exitoso)
        {
            CargarAsociaciones();
        }*/
    }
}