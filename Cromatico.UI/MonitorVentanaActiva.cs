using System.IO;
using System.Text;
using Cromatico.Entidades;

namespace Cromatico.UI;

    public sealed class MonitorVentanaActiva : IDisposable
    {
        private IntPtr _hook = IntPtr.Zero;
        private NativeMethods.WinEventDelegate? _callback;

        public event Action<InfoProceso?, string?>? VentanaCambiada;

        public bool EnEjecucion => _hook != IntPtr.Zero;

        public bool Iniciar()
        {
            if (EnEjecucion) return true;

            try
            {
                _callback = AlCambiarForeground;
                _hook = NativeMethods.SetWinEventHook(
                    NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero, _callback, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT);
            }
            catch (Exception)
            {
                _hook = IntPtr.Zero;
            }

            if (_hook == IntPtr.Zero)
            {
                _callback = null;
                return false;
            }
            return true;
        }

        public void Detener()
        {
            if (_hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }
            _callback = null;
        }

        public (InfoProceso? proceso, string? error) LeerActual()
        {
            try { return Leer(NativeMethods.GetForegroundWindow()); }
            catch (Exception ex) { return (null, "Error al leer la ventana activa: " + ex.Message); }
        }

        private void AlCambiarForeground(IntPtr hook, uint evento, IntPtr hwnd, int idObject, int idChild, uint hilo, uint tiempo)
        {
            if (hwnd == IntPtr.Zero) return;
            var (proceso, error) = Leer(hwnd);
            VentanaCambiada?.Invoke(proceso, error);
        }

        private static (InfoProceso?, string?) Leer(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return (null, "No hay una ventana activa que identificar.");

            try
            {
                NativeMethods.GetWindowThreadProcessId(hwnd, out uint pid);
                if (pid == 0)
                    return (null, "No se pudo identificar el proceso de la ventana activa.");

                IntPtr proceso = NativeMethods.OpenProcess(NativeMethods.PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
                if (proceso == IntPtr.Zero)
                    return (null, "No se pudo leer la aplicación activa (acceso denegado).");

                try
                {
                    var sb = new StringBuilder(1024);
                    uint tam = (uint)sb.Capacity;
                    if (!NativeMethods.QueryFullProcessImageName(proceso, 0, sb, ref tam))
                        return (null, "No se pudo obtener el nombre de la aplicación activa.");

                    var ruta = sb.ToString();
                    return (new InfoProceso
                    {
                        Pid = (int)pid,
                        Ruta = ruta,
                        Nombre = Path.GetFileNameWithoutExtension(ruta).ToLowerInvariant(),
                        EsPropia = pid == (uint)Environment.ProcessId
                    }, null);
                }
                finally { NativeMethods.CloseHandle(proceso); }
            }
            catch (Exception ex)
            {
                return (null, "Error al leer la ventana activa: " + ex.Message);
            }
        }

        public void Dispose() => Detener();
    }
