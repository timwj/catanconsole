using System;
using static System.Console;
namespace CatanConsole
{
    public class DevelopmentCard
    {
        // 14 knight cards, 5 victory point cards, 2 road building, 2 year of plenty, and 2 monopoly
        public string cardType;

        public DevelopmentCard(string cardType)
        {
            this.cardType = cardType;
        }

        public override string ToString()
        {
            return cardType;
        }
        public static ResourceClass cost = new ResourceClass(0, 0, 1, 1, 1);

    }
}
