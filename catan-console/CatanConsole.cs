using static System.Console;
namespace CatanConsole
{
    public class CatanConsole
    {
        static int Main(string[] args)
        {   // Needed for some terminals to correctly display unicode.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            WriteLine($"Console dimensions: {Console.WindowWidth} x {Console.WindowHeight}  W x H");

            if (Console.WindowWidth < 132)
            {
                WriteLine("Console window too narrow, it needs to be 132 character wide minimum");
                // return 1;
            }
            if (Console.WindowHeight < 49)
            {
                WriteLine("Console window height too small, it needs to be 50 characters high minimum ");
                // return 1;
            }

            while (true)
            {
                UI.drawMainMenu();

                var input = ReadKey().Key;
                switch (input)
                {
                    case ConsoleKey.Q: return 0;
                    case ConsoleKey.L:
                        GameLogic game = new GameLogic(loadGame: true);
                        break;
                    case ConsoleKey.N:
                        game = new GameLogic(loadGame: false);
                        break;
                }
            }

        }
    }
}

