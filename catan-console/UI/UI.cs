using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Pastel;
using System.Text;
using static System.Console;

namespace CatanConsole
{

    /// <summary>
    /// Responsible for generating board and game statistics in Unicode with color. For drawing to Color terminal. 
    /// Make sure terminal supports UTF-8 and color.
    /// </summary>
    public class UI
    {
        public static Dictionary<enumHexType, string> hexTypeNames = new Dictionary<enumHexType, string>() {
                { enumHexType.lumber,   "LUMBER" },
                { enumHexType.brick,    "BRICK " },
                { enumHexType.wool,     " WOOL " },
                { enumHexType.grain,    "GRAIN " },
                { enumHexType.iron,     " IRON " },
                { enumHexType.desert,   "DESERT" },
                { enumHexType.nonExistent,  "NON-EX" }
        };
        public static Dictionary<enumHexType, Color> hexColors = new Dictionary<enumHexType, Color>() {
                { enumHexType.lumber,   Color.DarkGreen},
                { enumHexType.brick,    Color.LightCoral},
                { enumHexType.wool,     Color.WhiteSmoke},
                { enumHexType.grain,    Color.Gold},
                { enumHexType.iron,     Color.SteelBlue},
                { enumHexType.desert,   Color.SandyBrown},
                { enumHexType.nonExistent,  Color.White}
        };
        public static Dictionary<int, Color> playerColors = new Dictionary<int, Color>() {
            { 0, Color.Red },
            { 1, Color.Green },
            { 2, Color.Yellow },
            { 3, Color.Blue },
            { 4, Color.Magenta },
            { 5, Color.Silver}
        };

        // rectangular boxes that are drawn on the screen, similar to HTML divs
        public Box boxBoard, boxResources, boxMessages, boxDice, boxActions, boxCurrentPlayer, boxLogo, boxResourcesThisTurn, boxMainMenu, boxCommand;
        public static int boardWidth = 54;
        public static int boardHeight = 41;
        public Board board; // Contains the data about buildings, roads, and hex resource types
        public Player[] players; // Contains the amount of resources each player has and their name
        public GameState gameState; // Contains whose turn it is and info about the dice
        public List<Box> leftCol, rightCol;  // We use the left column for the board and the right column for showing messages and statistics.
        public static int leftColWidth = boardWidth + 4, rightColWidth = 70;
        public static int columnSpacing = 1; // number of character of empty space between right and left column
        public static int frameWidth = leftColWidth + rightColWidth + columnSpacing, frameHeight = boardHeight + 2;  // frame is everything that is drawn to the screen
        private List<string> currentFrame; // The most recent frame that has been drawn to the console

        // Loading the sprites into memory is faster than reading from a file every time
        private static Sprite spriteEmptyNode = Sprite.fromFile("emptyNode_4x4.txt");
        private static Sprite spriteVillage = Sprite.fromFile("village_4x4.txt");
        private static Sprite spriteCity = Sprite.fromFile("city_4x4.txt");
        private static Sprite spriteHex = Sprite.fromFile("hex_13x14.txt");
        private static Sprite spriteVerticalRoad = Sprite.fromFile("verticalRoad_1x4.txt");
        private static Sprite spriteHorizontalRoadUp = Sprite.fromFile("horizontalRoadUp_4x1.txt");
        private static Sprite spriteHorizontalRoadDown = Sprite.fromFile("horizontalRoadDown_4x1.txt");
        private static Sprite spriteSea = Sprite.fromFile("sea_1x1.txt", Color.LightBlue);
        private static Sprite spriteLogo = Sprite.fromFile("logo_4x68.txt");

        private static Dictionary<int, char> diceSymbol = new Dictionary<int, char>() {
                {1,'⚀'}, {2,'⚁'}, {3,'⚂'}, {4,'⚃'}, {5,'⚄'}, {6,'⚅'}
        };

        private Location commandCursorPosition = new Location(45, 77);
        // To store all error messages and game events notices. The most recent of these are displayed in a box.
        public ObservableCollection<string> messages;

        // https://stackoverflow.com/questions/24925037/how-can-i-detect-if-liststring-was-changed-and-what-is-the-last-item-that-wa
        void messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            boxMessages = makeBoxMessages();
            drawBox(boxMessages, 23 + (gameState.totalPlayers - 3) * 2, 63);
            SetCursorPosition(commandCursorPosition.x, commandCursorPosition.y);
        }
        public UI(Board board, Player[] players, GameState gameState)
        {
            this.board = board;
            this.gameState = gameState;
            this.players = players;

            this.messages = new ObservableCollection<string>();
            this.messages.CollectionChanged += messages_CollectionChanged;
            messages.Add("Welcome to settlers of Catan");
        }

        // draw the current frame to the console.
        public void drawFrame()
        {
            Clear();
            generateFrame().ForEach(line => WriteLine(line));
            WriteLine();
            SetCursorPosition(commandCursorPosition.x, commandCursorPosition.y);
        }
        public static void drawMainMenu()
        {
            List<string> mainMenuItems = new() { "New Game (N)", "Load Game (L)", "QUIT (Q)" };
            Box boxMainMenu = new Box(Color.White, Color.DarkBlue, 47, 128, title: "Settlers of Catan");
            for (int i = 0; i < mainMenuItems.Count(); i++)
            {
                Sprite s = Sprite.fromLineOfText(mainMenuItems[i]);
                s.putSprite(boxMainMenu, new Point(20, 10 + 2 * i));
            }
            List<string> lines = boxMainMenu.printVersion();
            // List<string> linesa = new() { "a", "dsgsg" };

            foreach (var l in lines)
            {
                WriteLine(l);
            }
            Console.CursorVisible = false;
        }

        public static void drawGameSetup()
        {
            Box box = new Box(Color.White, Color.DarkBlue, 47, 128, title: "Setup new game");
            List<string> lines = box.printVersion();
            foreach (var l in lines)
            {
                WriteLine(l);
            }
            Console.CursorVisible = true;
            SetCursorPosition(20, 10);
        }

        public static void drawSettingsSetup()
        {
            Box box = new Box(Color.White, Color.DarkBlue, 47, 130, title: "Board settings");
            Sprite s = Sprite.fromLineOfText("Random Board? (Y/N)");
            s.putSprite(box, new Point(20, 10));
            List<string> lines = box.printVersion();
            foreach (var l in lines)
            {
                WriteLine(l);
            }
            Console.CursorVisible = false;
        }

        private void drawBox(Box b, int y, int x)
        {
            int cury = y; int curx = x;
            var s = b.printVersion();
            SetCursorPosition(curx, cury);
            for (int i = 0; i < s.Count; i++)
            {
                Write(s[i]);
                if (i != s.Count - 1)
                {
                    SetCursorPosition(curx, ++cury);
                }
            }
        }

        public void refreshCommandBox()
        {
            boxCommand = makeBoxCommand();
            drawBox(boxCommand, 44, 63);
            SetCursorPosition(commandCursorPosition.x, commandCursorPosition.y);
        }

        /// <summary>
        /// Generates the current frame. Called every time somethings changes in the game
        /// </summary>
        /// <returns>each horizontal line is a string</returns>
        private List<string> generateFrame()
        {
            currentFrame = new List<string>();

            boxResources = makeBoxResources();
            boxBoard = makeBoxBoard();
            boxActions = makeBoxActions();
            boxDice = makeBoxDice();
            boxCurrentPlayer = makeBoxCurrentPlayer();
            boxLogo = makeBoxLogo();
            boxMessages = makeBoxMessages();
            boxResourcesThisTurn = makeBoxResourcesGained();
            boxCommand = makeBoxCommand();

            // put all the boxes in a column and convert to a list of strings, each string is a line.
            List<string> leftLines = new List<Box> { boxBoard }.SelectMany(box => box.printVersion()).ToList();
            List<string> rightLines = new List<Box> { boxLogo, boxResources, boxResourcesThisTurn, boxDice, boxMessages, boxActions, boxCurrentPlayer, boxCommand }.SelectMany(box => box.printVersion()).ToList();

            int tallestColumnLines = Math.Max(leftLines.Count, rightLines.Count);

            // Some logic for adding spaces if lines are shorter than the frame
            for (int i = 0; i < tallestColumnLines; i++)
            {
                string line = i < leftLines.Count ? leftLines[i] : new string(' ', leftColWidth);  // if there are no more lines in left column put spaces instead
                line += new string(' ', columnSpacing);  // add spaces between the columns
                line += i < rightLines.Count ? rightLines[i] : new string(' ', rightColWidth); // add the contents from the right column to the line
                currentFrame.Add(line);
            }
            return currentFrame;
        }

        /// <summary>
        /// Contains info about game events and errors.
        /// </summary>
        private Box makeBoxMessages()
        {
            int boxHeight = frameHeight - 37 - 2 * gameState.totalPlayers + 13;
            Box box = new Box(Color.White, Color.Gray, boxHeight, 70, "Messages");
            List<string> messagesToShow = new();
            for (int i = 0; i < boxHeight - 2; i++)
            {
                if (messages.Count > i)
                    messagesToShow.Add(messages.Count() - i + ": " + messages[messages.Count() - i - 1]);
            }
            for (int i = 0; i < messagesToShow.Count(); i++)
            {
                Sprite msg = Sprite.fromLineOfText(messagesToShow[i], Color.Black, Color.Gray);
                msg.putSprite(box, new Point(11, i + 1));
            }
            return box;
        }

        private Box makeBoxCommand()
        {
            Box box = new Box(Color.White, Color.LightGreen, 3, 70, "Your command");
            return box;
        }

        private Box makeBoxActions()
        {
            Box box = new Box(Color.White, Color.LightSlateGray, 5, 70, "Actions");
            Sprite s = Sprite.fromLineOfText("village X   city X   road X   roll   end   buycard", Color.Black, Color.LightSlateGray);
            Sprite z = Sprite.fromLineOfText("trade {Resource give} {resource get}   playcard {cardname}", Color.Black, Color.LightSlateGray);
            Sprite w = Sprite.fromLineOfText("load   save   quit", Color.Black, Color.LightSlateGray);
            s.putSprite(box, new Point(12, 1));
            z.putSprite(box, new Point(12, 2));
            w.putSprite(box, new Point(12, 3));

            return box;
        }
        // Box with settlers of Catan Logo in ASCI art
        private Box makeBoxLogo()
        {
            Box box = new Box(Color.White, Color.DarkRed, h: 6, w: 70);
            spriteLogo.putSprite(box, new Point(1, 1));
            return box;
        }

        // This box contains a table with the amount of resources each player gained this or the last turn
        private Box makeBoxResourcesGained()
        {

            Box box = new Box(Color.Black, Color.Pink, players.Length + 4, 70, "RESOURCES GAINED THIS TURN                 DEVELOPMENT CARDS     ");

            Sprite tableHeader = Sprite.fromLineOfText("Lumbr Brick Grain Wool  Iron - kngt vict rodb yeop mono", Color.White, Color.Gray);
            tableHeader.putSprite(box, new Point(10, 2));

            for (int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                Sprite playerSprite = Sprite.fromLineOfText(p.name, playerColors[p.number], Color.Pink);
                playerSprite.putSprite(box, new Point(1, i + 3));

                string resourceAmounts = $@"{p.resourcesThisTurn.lumber}     {p.resourcesThisTurn.brick}     {p.resourcesThisTurn.grain}     {p.resourcesThisTurn.wool}     {p.resourcesThisTurn.iron}      ";

                string devCards = i == gameState.turn ? $"{p.knightCards}    {p.victorypointCards}    {p.roadbuildingCards}    {p.yearofplentyCards}    {p.monopolyCards}" : " ";

                Sprite resourceSprite = Sprite.fromLineOfText(resourceAmounts + devCards, Color.Black, Color.Pink);
                resourceSprite.putSprite(box, new Point(10, i + 3));
            }
            return box;
        }

        // This box contains a table with the amount of resources each player has
        private Box makeBoxResources()
        {
            Box box = new Box(Color.White, Color.Tan, players.Length + 4, 70, "TOTAL RESOURCES AND VICTORYPOINTS");

            Sprite tableHeader = Sprite.fromLineOfText("Lumbr Brick Grain Wool  Iron  3:1  VPTS LROAD ARMY", Color.White, Color.Gray);
            tableHeader.putSprite(box, new Point(10, 2));

            for (int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                Sprite playerSprite = Sprite.fromLineOfText(p.name, playerColors[p.number], Color.Tan);
                playerSprite.putSprite(box, new Point(1, i + 3));

                string harborLumber = p.harbors.Contains(harborType.lumber) ? "*" : " ";
                string harborBrick = p.harbors.Contains(harborType.brick) ? "*" : " ";
                string harborGrain = p.harbors.Contains(harborType.grain) ? "*" : " ";
                string harborWool = p.harbors.Contains(harborType.wool) ? "*" : " ";
                string harborIron = p.harbors.Contains(harborType.iron) ? "*" : " ";
                string harborThreeOne = p.harbors.Contains(harborType.threeForOne) ? "*" : " ";

                string stats = $"{p.victoryPoints}    {p.longestRoad + (p.longestRoadVP ? "*" : " ")}    {p.knightCardsPlayed + (p.largestArmyVP ? "*" : "")}";

                string resourceAmounts;

                if (i == gameState.turn)
                    resourceAmounts = $@"{p.resources.lumber}{harborLumber}    {p.resources.brick}{harborBrick}    {p.resources.grain}{harborGrain}    {p.resources.wool}{harborWool}    {p.resources.iron}{harborIron}    {harborThreeOne}    ";
                else
                    resourceAmounts = $" {harborLumber}      {harborBrick}      {harborGrain}      {harborWool}      {harborIron} {harborThreeOne}   ";

                Sprite resourceSprite = Sprite.fromLineOfText(resourceAmounts + stats, Color.Black, Color.Tan);
                resourceSprite.putSprite(box, new Point(10, i + 3));
            }
            return box;
        }

        // Shows whose turn it is
        private Box makeBoxCurrentPlayer()
        {Player currentPlayer = players[gameState.turn];
            Color currentPlayerColor = playerColors[currentPlayer.number];
            Box box = new Box(Color.Black, Color.NavajoWhite, 3, 70, "Current Player");

            Sprite playerName = Sprite.fromLineOfText(" " + currentPlayer.name + " ", Color.Black, currentPlayerColor);
                playerName.putSprite(box, new Point(20, 1));

                return box;
            }

            // A box with information about the latest dice roll
            private Box makeBoxDice()
            {
                Color boxColor = Color.LightCoral;
                Box box = new Box(Color.Black, boxColor, 3, 70, "Dice");

                if (gameState.diceRolled)
                {
                    Sprite diceSprite = Sprite.fromLineOfText($"D1: {gameState.d1} {UI.diceSymbol[gameState.d1]}   D2: {gameState.d2} {UI.diceSymbol[gameState.d1]}  SUM = {gameState.d1 + gameState.d2}", Color.Black, boxColor);
                    diceSprite.putSprite(box, new Point(8, 1));
                }
                Sprite diceRolled = Sprite.fromLineOfText($"The dice have {(gameState.diceRolled ? "" : "NOT ")} been rolled", Color.Black, boxColor);
                diceRolled.putSprite(box, new Point(30, 2));
                return box;
            }

            // To make the user press a key to contintue the game. Good for making user read instructions before continuing
            public void pressEnter(string msg = "Press 'ENTER' to continue")
            {
                Console.WriteLine(msg.Pastel(Color.White).PastelBg(Color.Black));
                Console.ReadKey();
            }

            // Generates the board. Order of the statements matters.
            private Box makeBoxBoard()
            {
                Box boardBox = new Box(Color.White, Color.DimGray, boardHeight, boardWidth);
                drawHexes(boardBox);
                drawBuildings(boardBox);
                drawRoads(boardBox);
                drawSea(boardBox);

                Box paddingBox = new Box(boardBox, Color.White, Color.DarkBlue, 3, 4, 3, 4); // draw blue frame around board for aesthetic reasons
                return paddingBox;
            }

            // draws the hexagons to the board
            private void drawHexes(Box box)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {   // Check if the coordinate is valid; does this hex exist?
                        Location l = new Location(i, j);
                        if (Hex.hexExists(l))
                        {
                            Hex hexData = board.hexes[i, j];
                            int lastDiceRoll = gameState.d1 + gameState.d2;
                            HexSprite s;
                            int hexUINumber = GameLogic.hexPointToInt[new Location(i, j)];
                            if (board.robber == l)
                                s = new HexSprite(hexData, spriteHex, lastDiceRoll, knight: true);
                            else
                                s = new HexSprite(hexData, spriteHex, lastDiceRoll, knight: false, showNumbers: gameState.placingRobber, location: hexUINumber);
                            Point position = new Point(5 * j, 14 * (i / 4) + 7 * (i % 4 / 3));  // This works to place our hexagon sprites in a hexagon grid.

                            s.putSprite(box, position, overWrite: false); // the sprite rectangles overlap a bit, overwrite:false 'removes' the empty spaces in the hexagon corners
                        }
                    }
                }
            }

            // draws the buildings to the board. These can be villages, cities or empty places where its possible to place a building. Sometimes building is not allowed 
            // because of the distance rule.
            private void drawBuildings(Box box)
            {
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        // BuildingExists checks if the coordinates are valid
                        if (Building.validBuildingCoordinates(new Location(i, j)))
                        {
                            Building building = board.buildings[i, j];
                            Color buildingColor = playerColors[building.owner];
                            string buildingType = building.ToString(); // building is abstract class thats why we need to look up which type of building it is

                            Sprite s = buildingType switch
                            {
                                "emptybuilding" => spriteEmptyNode,
                                "village" => spriteVillage,
                                "city" => spriteCity,
                                _ => spriteEmptyNode
                            };
                            s = new Sprite(s);


                            int yOffset = i % 2 == 1 ? 2 : 0;
                            Point position = new Point(5 * j, 7 * (i / 2) + yOffset);  // by trial and error, this gives the location in pixels of the building


                            s.colorSprite(playerColors[board.buildings[i, j].owner]); // give the building the color of the player that owns it.
                            if (buildingType == "emptybuilding" || buildingType == "village")
                            {
                                Location location = new Location(i, j);
                                if ((board.allowedDistanceRule(location)
                                && (board.allowedConnectedVillage(location, players[gameState.turn]) || !gameState.mainPhase)
                                || buildingType == "village"))
                                {
                                    s.addText(GameLogic.buildingPointToInt[new Location(i, j)].ToString("D2"), new Point(1, 1), Color.LightGray);
                                }
                                else if (buildingType != "city") // This means because of the distance rule you cannot build here, also cities dont need to show coordinates
                                {
                                    s.addText("  ", new Point(1, 1), Color.LightGray);
                                    s.addText("  ", new Point(1, 2), Color.LightGray);
                                    s.colorSprite(Color.DimGray);
                                }
                            }
                            if (board.harbors[i, j] != harborType.notHarbor && board.buildings[i, j].ToString() == "emptybuilding")
                                s.colorSprite(Color.SteelBlue);

                            if (board.harbors[i, j] != harborType.notHarbor)
                            {
                                string htext = board.harbors[i, j] switch
                                {
                                    harborType.lumber => "LU",
                                    harborType.brick => "BR",
                                    harborType.wool => "WO",
                                    harborType.grain => "GR",
                                    harborType.iron => "IR",
                                    harborType.threeForOne => "TH"
                                };
                                s.addText(htext, new Point(1, 2), Color.Purple);
                            }
                            s.putSprite(box, position);
                        }
                    }
                }
            }


            private void drawRoads(Box box)
            {
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        Location thisLocation = new Location(i, j);
                        Road road = board.roads[i, j];
                        bool inLongestRoad = board.longestRoad.Contains(thisLocation);
                        if (!Road.validRoadCoordinates(thisLocation))
                            continue;

                        string selectionNumber = GameLogic.roadPointToInt[thisLocation].ToString("D2");
                        char firstDigit = selectionNumber[0];
                        char secondDigit = selectionNumber[1];

                        Sprite s;
                        Point position;
                        if (i % 2 == 1)
                        {
                            s = new Sprite(spriteVerticalRoad);
                            if (road.owner == Road.NOROADHEREYET)
                            {
                                s.addText(firstDigit, new Point(1, 0));
                                s.addText(secondDigit, new Point(2, 0));
                            }
                            else
                            {
                                if (inLongestRoad)
                                    s.addText(" *| ", new Point(0, 0));
                                else
                                    s.addText(" || ", new Point(0, 0));
                            }
                            position = new Point(5 * j, 6 + 7 * (i / 2));
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                if (i % 4 == 0)
                                {
                                    s = new Sprite(spriteHorizontalRoadUp);
                                    if (road.owner == Road.NOROADHEREYET)
                                    {
                                        s.addText(firstDigit, new Point(0, 1));
                                        s.addText(secondDigit, new Point(0, 2));
                                    }
                                    else
                                    {
                                        s.addText("/", new Point(0, 1));
                                        s.addText("/", new Point(0, 2));
                                        if (inLongestRoad)
                                        {
                                            s.addText("*", new Point(0, 1));

                                        }
                                        s.addText(" ", new Point(0, 0));
                                        s.addText(" ", new Point(0, 3));

                                    }
                                }
                                else
                                {
                                    s = new Sprite(spriteHorizontalRoadDown);
                                    if (road.owner == Road.NOROADHEREYET)
                                    {
                                        s.addText(firstDigit, new Point(0, 1));
                                        s.addText(secondDigit, new Point(0, 2));
                                    }
                                    else
                                    {
                                        if (inLongestRoad)
                                        {
                                            s.addText("*", new Point(0, 1));
                                            s.addText("\\", new Point(0, 2));

                                        }
                                        else
                                        {
                                            s.addText("\\", new Point(0, 1));
                                            s.addText("\\", new Point(0, 2));
                                        }
                                        s.addText(" ", new Point(0, 0));
                                        s.addText(" ", new Point(0, 3));
                                    }
                                }
                                position = new Point(4 + 10 * (j / 2), 1 + 7 * (i / 2));
                            }
                            else
                            {
                                if (i % 4 == 0)
                                {
                                    s = new Sprite(spriteHorizontalRoadDown);
                                    if (road.owner == Road.NOROADHEREYET)
                                    {
                                        s.addText(firstDigit, new Point(0, 1));
                                        s.addText(secondDigit, new Point(0, 2));
                                    }
                                    else
                                    {
                                        if (inLongestRoad)
                                        {
                                            s.addText("*", new Point(0, 1));
                                            s.addText("\\", new Point(0, 2));
                                        }
                                        else
                                        {
                                            s.addText("\\", new Point(0, 1));
                                            s.addText("\\", new Point(0, 2));
                                        }
                                        s.addText(" ", new Point(0, 0));
                                        s.addText(" ", new Point(0, 3));
                                    }
                                }
                                else
                                {
                                    s = new Sprite(spriteHorizontalRoadUp);
                                    if (road.owner == Road.NOROADHEREYET)
                                    {
                                        s.addText(firstDigit, new Point(0, 1));
                                        s.addText(secondDigit, new Point(0, 2));
                                    }
                                    else
                                    {
                                        if (inLongestRoad)
                                        {
                                            s.addText("*", new Point(0, 1));
                                            s.addText("/", new Point(0, 2));
                                        }
                                        else
                                        {
                                            s.addText("/", new Point(0, 1));
                                            s.addText("/", new Point(0, 2));
                                        }
                                        s.addText(" ", new Point(0, 0));
                                        s.addText(" ", new Point(0, 3));
                                    }
                                }
                                position = new Point(9 + 10 * (j / 2), 1 + 7 * (i / 2));
                            }
                        }
                        s.colorSprite(playerColors[road.owner]);
                        s.putSprite(box, position);
                    }
                }
            }

            // draws the sea that surrounds the catan island
            private void drawSea(Box box)
            {
                // fill board horizontally with sea sprites from the sides
                for (int p = 0; p < box.h; p++)
                {
                    // fill from left
                    for (int q = 0; q < boardWidth && box.pixels[p, q].kar == ' '; q++)
                        spriteSea.putSprite(box, new Point(q, p));

                    // fill from right
                    for (int q = 0; q < boardWidth && box.pixels[p, boardWidth - q - 1].kar == ' '; q++)
                        spriteSea.putSprite(box, new Point(boardWidth - q - 1, p));
                }

                // fill vertically
                for (int q = 0; q < boardWidth; q++)
                {
                    // fill from top
                    for (int p = 0; p < boardHeight && box.pixels[p, q].kar == ' '; p++)
                        spriteSea.putSprite(box, new Point(q, p));

                    // fill from bottom
                    for (int p = 0; p < boardHeight && box.pixels[boardHeight - p - 1, q].kar == ' '; p++)
                        spriteSea.putSprite(box, new Point(q, boardHeight - p - 1));

                }
            }
        }
    }



