using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ryokohbato_life
{
  public static class NativeMethods
  {
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(IntPtr hObject);

    /// <summary>
    /// Bitmap画像をImageSourceに変換
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static ImageSource ToImageSource(this Bitmap bitmap)
    {
      var handle = bitmap.GetHbitmap();
      try
      {
        return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
      }
      finally
      {
        DeleteObject(handle);
      }
    }
  }
}
