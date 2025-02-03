using static System.Console;
namespace CatanConsole
{
    public partial class GameLogic
    {
        /// <summary>
        /// Player can select here whether he wants a fair board or a randomly generated board
        /// </summary>
        private bool randomBoardMenu()
        {
            UI.drawSettingsSetup();

            while (true)
            {
                var input = ReadKey().Key;
                switch (input)
                {
                    case ConsoleKey.Y: return true;
                    case ConsoleKey.N: return false;
                }
            }
        }

        /// <summary>
        /// Enter player names here
        /// </summary>
        private void setupPlayers()
        {
            // SetCursorPosition
            UI.drawGameSetup();

            List<Player> playersTemp = new();
            string input;
            gameState.totalPlayers = 0;
            while (gameState.totalPlayers < 3)
            {
                SetCursorPosition(20, 10 + gameState.totalPlayers);
                Write($"Player {gameState.totalPlayers + 1} name : ");
                Player p = new Player(ReadLine().Trim());
                p.number = gameState.totalPlayers++;
                playersTemp.Add(p);
            }
            while (gameState.totalPlayers < 5)
            {
                SetCursorPosition(20, 8);
                Write("Type 'start' to start game (minimum 3 players, max 5)");
                SetCursorPosition(20, 10 + gameState.totalPlayers);
                Write($"Player {gameState.totalPlayers + 1} name : ");

                input = ReadLine().Trim();
                if (input != "start")
                {
                    Player p = new Player(input);
                    p.number = gameState.totalPlayers++;
                    playersTemp.Add(p);
                }
                else break;
            }
            players = playersTemp.ToArray();
        }
    }
}