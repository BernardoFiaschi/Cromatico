using System;
using System.IO;
using System.Windows.Media.Imaging;
using Cromatico.Entidades;

namespace Cromatico.UI;

public static class HelperBitMapImage
{
    public static BitmapImage CargarImagen(string ruta)
    {
        try
        {
            BitmapImage imagen =new BitmapImage();

            imagen.BeginInit();

            imagen.UriSource =new Uri(Path.GetFullPath(ruta));

            imagen.CacheOption =BitmapCacheOption.OnLoad;

            imagen.EndInit();

            return imagen;
        }catch(Exception e)
        {
            GuardarExcepcion.registrarExcepcion(e,"HelperBitMapImage.CargarImagen");
            return new BitmapImage();
        }
        
    }
}