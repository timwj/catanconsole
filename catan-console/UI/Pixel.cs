using System.Drawing;

namespace CatanConsole
{
    /// <summary>
    /// Not a real pixel but a unicode character and text and background color in the terminal.
    /// </summary>
    public struct Pixel
    {
        private char _kar;
        private Color _fgColor; // textcolor
        private Color _bgColor; // backgroundcolor

        public char kar
        {
            get { return _kar; }
            set { if (value <= sbyte.MaxValue) _kar = value; }  // only ASCII allowed (some terminals dont support Unicode
        }
        public Color fgColor { get { return _fgColor; } set { _fgColor = value; } } // not neccesary to do input validation, all Colors are valid. Assigning types that are not Color will result in compilation error anyway.
        public Color bgColor { get { return _bgColor; } set { _bgColor = value; } }

        public Pixel(char kar, Color fgColor, Color bgColor)
        {
            this._kar = kar;
            this._fgColor = fgColor;
            this._bgColor = bgColor;
        }
        public Pixel(char kar, Color fgColor)
        {
            this._kar = kar;
            this._fgColor = fgColor;
            this._bgColor = Color.Black;
        }
        public Pixel(char kar)
        {
            this._kar = kar;
            this._fgColor = Color.White;
            this._bgColor = Color.Black;
        }

    }
}
