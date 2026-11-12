using System.Runtime.InteropServices;

namespace Cromatico.Interop
{
    public static class MagnificationNativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MAGCOLOREFFECT
        {
            [MarshalAs(UnmanagedType.ByValArray,SizeConst = 25)]
            public float[] transform;
        }
        [DllImport("Magnification.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagInitialize();
        [DllImport("Magnification.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagUninitialize();
        [DllImport("Magnification.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagSetFullscreenColorEffect(ref MAGCOLOREFFECT effect);
        [DllImport("Magnification.dll",SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagGetFullscreenColorEffect(out MAGCOLOREFFECT effect);
        

    }
}