using System;
namespace CatanConsole
{
    /// <summary>
    /// 2 six-sided dice
    /// </summary>
    public class Dice
    {
        private Random rnd;
        public Dice(int seed) => rnd = new Random(seed);
        public Dice() => rnd = new Random();
        public (int, int) roll() => (rnd.Next(1, 7), rnd.Next(1, 7));
    }
}

