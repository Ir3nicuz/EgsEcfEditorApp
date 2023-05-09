using System.Drawing;
using System.Drawing.Drawing2D;

namespace GenericMethods
{
    public static class ImageAjustments
    {
        public static Bitmap AddGap(Bitmap image, int edgeLength, int xGap, int yGap)
        {
            Bitmap bmp = new Bitmap(edgeLength + 2 * xGap, edgeLength + 2 * yGap);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.CompositingMode = CompositingMode.SourceCopy;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gfx.DrawImage(image, new Rectangle(xGap, yGap, edgeLength, edgeLength));
            }
            return bmp;
        }
    }
}
