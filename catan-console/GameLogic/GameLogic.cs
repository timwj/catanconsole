
// #pragma warning disable 0169, 0414, 8618
using System.Drawing;
using static System.Console;

namespace CatanConsole
{
    /// <summary>
    /// Gamelogic and gameloop happens here. Calls the UI with ui.drawframe(). 
    /// </summary>
    public partial class GameLogic
    {
        public Board board; // Store buildings, roads and resource hexes here.

        // for printing the board and player statistics. Having this in a separate class makes the transition to Unity much easier.
        public UI ui;

        // We use a seed, so the random numbers used are pre-determined, for the purpose of debugging.
        public const int SEED = 501;
        private Dice dice = new Dice();
        public GameState gameState = new GameState(); // contains whose turn it is, dice values etc.

        // Array of all the players, each player is an object containing their name, victory points and resources
        public Player[] players;

        private int currentLargestArmySize = 0;

        public GameLogic(bool loadGame)
        {
            initRoadDicts();
            initBuildingDicts();
            initHexDicts();
            // setupTestData();
            bool randomBoard = false;
            if (!loadGame)
            {
                setupPlayers();
                randomBoard = randomBoardMenu();
            }
            board = new Board(randomBoard: randomBoard);
            ui = new UI(board: board, players: players, gameState: gameState);
            if (!loadGame)
            {
                ui.drawFrame();
                if (placeStartingStructures() == 1) // player typed quit
                    return;
            }
            else if (!load())
                return;
            gameLoop();
        }

        /// <summary>
        /// Checks if this action is allowed (enough resources, right phase of turn, etc.)
        /// </summary>
        /// <param name="pi">The command the player entered, already checked for syntax</param>
        /// <param name="isFree">Are we in a gamephase where building villages and roads is free?</param>
        /// <returns></returns>
        private bool inputLogicallyValid(PlayerInput pi, bool isFree = false)
        {
            bool valid;
            switch (pi.command)
            {
                case enumCommand.roll: valid = isValidRoll(); break;
                case enumCommand.village: valid = isValidVillage(int.Parse(pi.arguments[0]), isFree); break;
                case enumCommand.city: valid = isValidCity(int.Parse(pi.arguments[0])); break;
                case enumCommand.road: valid = isValidRoad(int.Parse(pi.arguments[0]), isFree); break;
                case enumCommand.quit: valid = true; break;
                case enumCommand.get: valid = true; break;
                case enumCommand.post: valid = true; break;
                case enumCommand.load: valid = true; break;
                case enumCommand.save: valid = true; break;
                case enumCommand.trade:
                    enumResource resourceGive = Enum.Parse<enumResource>(pi.arguments[0]);
                    enumResource resourceReceive = Enum.Parse<enumResource>(pi.arguments[1]);
                    valid = isValidTrade(resourceGive, resourceReceive); break;
                case enumCommand.end: valid = isValidEnd(); break;
                case enumCommand.buycard: valid = isValidBuyCard(); break;
                case enumCommand.playcard: valid = isValidPlayCard(pi.arguments[0]); break;
                default: valid = false; break;
            }
            return valid;
        }

        /// <summary>
        /// All the inputs and the main phase game take place here
        /// </summary>
        private void gameLoop()
        {
            ui.messages.Add("This is the main phase of the game");
            gameState.mainPhase = true;

            calculateLongestRoad();  // This can change every turn.
            checkLargestArmy();      // Who has played the most Knight cards (worth 2 VP)
            ui.drawFrame();         // update the screen.

            while (!winCheck())
            {
                PlayerInput pi = new PlayerInput();
                ui.refreshCommandBox();     // after inputting a command, empty the input textbox.

                if (!pi.validSyntax)
                {
                    ui.messages.Add("Invalid syntax for this command, check the manual");
                    continue;
                }
                if (!inputLogicallyValid(pi))
                    continue;

                switch (pi.command)
                {
                    case enumCommand.roll: roll(); break;
                    case enumCommand.village: village(int.Parse(pi.arguments[0])); calculateLongestRoad(); break;
                    case enumCommand.city: city(int.Parse(pi.arguments[0])); break;
                    case enumCommand.road: road(int.Parse(pi.arguments[0])); calculateLongestRoad(); break;
                    case enumCommand.quit: return;
                    case enumCommand.load: load(); checkLargestArmy(); break;
                    case enumCommand.save: save(); break;
                    case enumCommand.trade:
                        enumResource resourceGive = Enum.Parse<enumResource>(pi.arguments[0]);
                        enumResource resourceReceive = Enum.Parse<enumResource>(pi.arguments[1]);
                        trade(resourceGive, resourceReceive); break;
                    case enumCommand.end: end(); break;
                    case enumCommand.buycard: buyCard(); break;
                    case enumCommand.playcard: playCard(pi.arguments[0]); checkLargestArmy(); break;
                }
                ui.drawFrame();
            }
        }

        /// <summary>
        /// First player to 10 victorypoints wins if its his turn.
        /// </summary>
        /// <returns></returns>
        private bool winCheck()
        {
            foreach (Player p in players)
                if (p.victoryPoints >= 10 && gameState.turn == p.number)
                {
                    WriteLine($"Player {p.name} has won the game");
                    ui.pressEnter("Press 'Enter' to go back to the Main Menu");
                    return true;
                }
            return false;
        }

        /// <summary>
        /// When the dice are rolled, all the players that have villages and cities next to a hexagon with that number get resources.
        /// </summary>
        /// <param name="diceNum">the sum of the two dice</param>
        private void giveResources(int diceNum)
        {
            foreach (Player player in players)  // loop over all players computing the number of resources granted
            {
                List<Point> hexesWithNum = board.hexNum(diceNum); // all hexagons with this number
                foreach (Point hexPoint in hexesWithNum)
                {
                    if (hexPoint.Y == board.robber.y && hexPoint.X == board.robber.y)
                        continue;
                    enumResource resourceOfThisHex = hexTypeToResource[board.hexes[hexPoint.Y, hexPoint.X].hexType]; // which resource does this hex produce
                    foreach (Point buildingPoint in board.adjacentBuildings(hexPoint)) // loop over all the building that are adjacent to this hex
                    {
                        Building b = board.buildings[buildingPoint.Y, buildingPoint.X];
                        if (b.owner == player.number)
                            player.resourcesThisTurn += (resourceOfThisHex, b.productionAmount()); // villages produce 1 resource, cities 2. This uses operator overloading
                    }
                }
                player.resources += player.resourcesThisTurn;  // operator overloading, check Resource class
            }
        }

        /// <summary>
        /// If you have played 3 or more knight cards then you get 2 victorypoints for largest army.
        /// If 2 players have the same amount of knight cards then the first player to reach that amounts
        /// Keeps the Largest Army.
        /// </summary>
        private void checkLargestArmy()
        {
            for (int i = 0; i < 2; i++)  // needs 2 passes over the players. This makes logic easier.
            {
                foreach (Player p in players)
                {
                    if (p.knightCardsPlayed >= 3 && (p.knightCardsPlayed > currentLargestArmySize))
                    {
                        currentLargestArmySize = p.knightCardsPlayed;
                        p.largestArmyVP = true;
                    }
                    else if (p.largestArmyVP && currentLargestArmySize == p.knightCardsPlayed)
                    {
                        p.largestArmyVP = true;
                    }
                    else
                    {
                        p.largestArmyVP = false;
                    }
                }
            }

        }
    }
}