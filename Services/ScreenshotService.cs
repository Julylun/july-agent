using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using JulyAgent.Interfaces;

namespace JulyAgent.Services
{
    public class ScreenshotService : IScreenshotService
    {
        public Bitmap CapturePrimaryScreen()
        {
            var screen = Screen.PrimaryScreen;
            var bounds = screen?.Bounds ?? Screen.AllScreens[0].Bounds;
            var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            return bitmap;
        }
    }
}
