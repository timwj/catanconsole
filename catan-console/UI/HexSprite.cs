using System.Drawing;
namespace CatanConsole
{
    /// <summary>
    /// special sprite used for drawing the hexagon tiles.
    /// </summary>
    public class HexSprite : Sprite
    {
        public HexSprite(Hex h, Sprite s, int lastDiceRoll, bool knight = false, bool showNumbers = false, int location = 0) : base(s) // calls copy constructor of Sprite
        {
            Color textColor = knight ? Color.DimGray : (h.number == lastDiceRoll ? Color.Gold : Color.White);

            addText(UI.hexTypeNames[h.hexType], new Point(4, 7), textColor);  // draws LUMBER or GRAIN in the hexSprite

            if (h.number != 14)
            {
                addText($"({h.number:D2})", new Point(5, 6), textColor);   // draws dice number corresponding to this hexagon
            }
            replaceChar(' ', '#', new Rectangle(new Point(4, 4), new Size(5, 4))); // fill with dithered texxture

            if (knight)
            {
                addText("ROBBER", new Point(4, 5), Color.Gray);
            }

            else if (showNumbers)
            {
                addText($"[{location}]", new Point(5, 5), textColor);
            }
            colorFill(new Rectangle(new Point(4, 4), new Size(5, 4)), UI.hexColors[h.hexType], '#');
        }
    }
}