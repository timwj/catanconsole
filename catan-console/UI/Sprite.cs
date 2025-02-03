using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

namespace CatanConsole
{
    /// <summary>
    /// A sprite is a rectangular block of unicode characters, where each character has a text color and a background color.
    /// </summary>
    public class Sprite
    {
        public static string spritesFolder = "Sprites";

        public Pixel[,] pixels;
        public Size size;

        // this constructor is neccesary for the named constructor idion
        private Sprite() { }

        /// <summary>
        /// copy constructor - With Color
        /// </summary>
        /// <param name="s">sprite to be copied</param>
        public Sprite(Sprite s)
        {
            pixels = s.pixels.Clone() as Pixel[,];
            size = s.size;
        }

        // text file constructor - Without Color, named constructor idiom
        public static Sprite fromFile(string path, Color? c = null)
        {
            Sprite sprite = new Sprite();
            string[] lines = File.ReadAllLines(Path.Combine(spritesFolder, path));

            sprite.size = new Size(lines[0].Length, lines.Length);

            sprite.pixels = new Pixel[sprite.size.Height, sprite.size.Width];
            for (int i = 0; i < sprite.size.Height; i++)
                for (int j = 0; j < sprite.size.Width; j++)
                    sprite.pixels[i, j] = new Pixel(lines[i][j]);
            return sprite;
        }

        // make a sprite from a single line of text - named constructor idion - With Color
        public static Sprite fromLineOfText(string s, Color? foreGround = null, Color? backGround = null)
        {
            Sprite sprite = new Sprite();
            sprite.size = new Size(s.Length, 1);
            sprite.pixels = new Pixel[sprite.size.Height, sprite.size.Width];

            for (int j = 0; j < sprite.size.Width; j++)
                sprite.pixels[0, j] = new Pixel(s[j], foreGround ?? Color.White, backGround ?? Color.Black);
            return sprite;
        }

        // replaces each character old in the rectangle with character nw
        public void replaceChar(char old, char nw, Rectangle r)
        {
            for (int i = r.Top; i <= r.Bottom; i++)
                for (int j = r.Left; j <= r.Right; j++)
                    if (pixels[i, j].kar == old)
                        pixels[i, j].kar = nw;
        }

        // each character in the rectangle that is the same as k will change color to become newColor.
        public void colorFill(Rectangle r, Color newColor, char k = ' ')
        {
            for (int i = r.Top; i <= r.Bottom; i++)
                for (int j = r.Left; j <= r.Right; j++)
                    if (pixels[i, j].kar == k)
                        pixels[i, j].fgColor = newColor;
        }

        // Gives the entire sprite the same color.
        public void colorSprite(Color c)
        {
            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                    pixels[i, j].fgColor = c;
        }

        // add a string of text to a sprite
        public void addText(string text, Point p, Color? c = null)
        {
            for (int i = 0; i < text.Length; i++)
                pixels[p.Y, p.X + i] = new Pixel(text[i], c ?? Color.White);
        }

        public void addText(char text, Point p, Color? c = null) => pixels[p.Y, p.X] = new Pixel(text, c ?? Color.White); // add 1 char of text to a sprite

        // Plaatst de sprite in een box, overwrite bepaalt of spaties in een sprite de bestaande pixel overschrijven of niet
        public void putSprite(Box box, Point p, bool overWrite = true)
        {
            for (int a = 0; a < size.Height; a++)
                for (int b = 0; b < size.Width; b++)
                    if (overWrite || (box.pixels[p.Y + a, p.X + b].kar == ' '))
                        box.pixels[p.Y + a, p.X + b] = pixels[a, b];
        }
    }

}
