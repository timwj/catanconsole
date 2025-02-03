using System.Drawing;
using System.Text;
using Pastel;
namespace CatanConsole
{

    /// <summary>
    /// Rectangular grid of 'pixels' (chars), has an optional title.
    /// </summary>
    public class Box
    {
        public Pixel[,] pixels;
        public int h; public int w; // height and width
        public string title;
        public Color bgColor;

        /// <param name="lines">Make a box with lines as text inside, make sure the lines fit, no wrapping</param>
        /// <param name="fgColor">textcolor in the box</param>
        /// <param name="bgColor">backgroundcolor in the box</param>
        /// <param name="h">height in chars</param>
        /// <param name="w">width in chars</param>
        /// <param name="title">title in top left corner</param>
        public Box(List<string> lines, Color fgColor, Color bgColor, int h, int w, string title = "")
        {
            this.h = h;
            this.w = w;
            pixels = new Pixel[h, w];
            this.bgColor = bgColor;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (i < lines.Count && j < lines[i].Length)
                        pixels[i, j] = new Pixel(lines[i][j], fgColor, bgColor);
                    else
                        pixels[i, j] = new Pixel(' ', fgColor, bgColor);
                }
            }
            if (title != "")
            {
                for (int i = 0; i < title.Length; i++)
                {
                    pixels[1, i + 1] = new Pixel(title[i], Color.White, Color.Black);
                }
            }
        }

        /// <param name="fgColor">textcolor in the box</param>
        /// <param name="bgColor">backgroundcolor in the box</param>
        /// <param name="h">height in chars</param>
        /// <param name="w">width in chars</param>
        /// <param name="title">title in top left corner</param>
        public Box(Color fgColor, Color bgColor, int h, int w, string title = "")
        {
            this.h = h;
            this.w = w;
            pixels = new Pixel[h, w];
            this.bgColor = bgColor;

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    pixels[i, j] = new Pixel(' ', Color.Black, bgColor);
                }
            }
            if (title != "")
            {
                for (int i = 0; i < title.Length; i++)
                {
                    pixels[1, i + 1] = new Pixel(title[i], Color.White, Color.Black);
                }
            }
        }

        /// <summary>
        /// Box within a box, used to create a frame or padding around a box.
        /// </summary>
        public Box(Box b, Color fgColor, Color bgColor, int top = 0, int right = 0, int bottom = 0, int left = 0)
        {
            this.h = b.h + top + bottom;
            this.w = b.w + left + right;
            pixels = new Pixel[h, w];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    pixels[i, j] = new Pixel(' ', fgColor, bgColor);
                }
            }
            for (int i = 0; i < b.h; i++)
            {
                for (int j = 0; j < b.w; j++)
                {
                    pixels[i + top, j + left] = b.pixels[i, j];
                }
            }
        }

        /// <summary>
        /// Each line is a separate string. Uses terminal escape codes for color.
        /// </summary>
        public List<string> printVersion()
        {

            List<string> returnVal = new();
            for (int i = 0; i < h; i++)
            {
                Pixel p;
                StringBuilder sb = new StringBuilder(UI.frameWidth + 2);
                for (int j = 0; j < w; j++)
                {
                    p = pixels[i, j];
                    sb.Append(p.kar.ToString().Pastel(p.fgColor).PastelBg(p.bgColor));
                }
                returnVal.Add(sb.ToString());
            }
            return returnVal;
        }

    }
}