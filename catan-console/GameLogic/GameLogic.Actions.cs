using static System.Console;
using System.Drawing;
using Newtonsoft.Json;
namespace CatanConsole
{
    // Here all the commands a player can enter during the game. Validation takes place elsewhere so we assume valid input.
    public partial class GameLogic
    {
        /// <summary>
        /// Roll the dice
        /// </summary>
        private void roll()
        {
            (gameState.d1, gameState.d2) = dice.roll();
            int sum = gameState.d1 + gameState.d2;
            gameState.diceRolled = true;
            ui.messages.Add($"Player {players[gameState.turn].name} has rolled the dice");

            // Rolling 7 activates the robber.
            if (sum == 7)
            {
                stealResources();
                replaceRobber();
                stealResourceKnight(board.robber);
            }
            else
            {
                giveResources(sum);
            }
        }

        /// <summary>
        /// Build a village.
        /// </summary>
        /// <param name="locationNumber">The number as shown in the UI</param>
        /// <param name="isFree">In the beginning of the game placing 2 villages is free</param>
        private void village(int locationNumber, bool isFree = false)
        {
            Location location = buildingIntToPoint[locationNumber];
            Player player = players[gameState.turn];

            board.buildings[location.y, location.x] = new Village(owner: player.number);
            player.harbors.Add(board.harbors[location.y, location.x]);

            player.nVillages++;
            // Check if it is the first game phase, in which players can put villages without using resources.
            if (!isFree)
                player.resources -= Village.cost;

            ui.messages.Add($"Player {player.name} built a new village");
        }

        /// <summary>
        /// Build a city
        /// </summary>
        /// <param name="locationNumber">As shown in the UI</param>
        private void city(int locationNumber)
        {
            Location location = buildingIntToPoint[locationNumber];
            Player player = players[gameState.turn];

            board.buildings[location.y, location.x] = new City(owner: player.number);
            player.nCities++;
            player.resources -= City.cost;

            ui.messages.Add($"Player {player.name} built a new city");

        }

        /// <summary>
        /// Build a road
        /// </summary>
        /// <param name="locationNumber">As shown in the UI</param>
        /// <param name="isFree">In the beginning, and with the development card roadbuilding, 2 roads are for free</param>
        private void road(int locationNumber, bool isFree = false)
        {
            Location location = roadIntToPoint[locationNumber];
            Player player = players[gameState.turn];

            board.roads[location.y, location.x] = new Road(player.number);
            player.nRoads++;

            if (!isFree)
                player.resources -= Road.cost;
            ui.messages.Add($"Player {player.name} built a new road");
        }

        /// <summary>
        /// Trade resources with the bank (called maritime trade in official game)
        /// </summary>
        /// <param name="resourceGive">You will give this to the bank</param>
        /// <param name="resourceReceive">You will this resource from the bank</param>
        private void trade(enumResource resourceGive, enumResource resourceReceive)
        {
            Player player = players[gameState.turn];

            // Standard rate is 4:1, with a settlement on a threeForOne harbor the bank rate is 3:1
            int rate = player.harbors.Contains(harborType.threeForOne) ? 3 : 4;

            // With a specialised harbor, the rate is even better namely 2:1
            foreach (var h in player.harbors)
                if (h != harborType.threeForOne && h != harborType.notHarbor && harborTypeToResource[h] == resourceGive)
                    rate = 2;

            player.resources -= (resourceGive, rate);
            player.resources += (resourceReceive, 1);
            ui.messages.Add($"Player {players[gameState.turn].name} traded {rate} {resourceGive} for 1 {resourceReceive}");
        }


        /// <summary>
        /// It's now the next players turn. 
        /// </summary>
        /// <param name="reverse">Reverse order of players. Used in the beginning of the game, there for 1 round the order reverses</param>
        private void end(bool reverse = false)
        {
            gameState.turn = (gameState.turn + (reverse ? -1 : 1)) % players.Length;

            if (gameState.turn < 0) // this is neccesary because when reversing turn can become < 0 and then modulo doesn't work as expected.
                gameState.turn += players.Length;

            gameState.diceRolled = false;

            foreach (Player player in players)  // set all the earned resources this turn to 0, since its a new turn
                player.resourcesThisTurn = new ResourceClass();
        }

        /// <summary>
        /// Save the game to a file (new folder with 3 files). Has a dialog where player enters the name for the savegame.
        /// </summary>
        private void save()
        {
            Clear();
            WriteLine("Type the name for this savegame\n");
            WriteLine();
            string fileName = ReadLine().Trim();
            ui.messages.Add(($"Trying to save game as {fileName} ..."));

            GameData gd = new GameData(board, gameState, players);
            try
            {
                gd.saveGame(fileName);
            }
            catch (Exception e)
            {
                ui.messages.Add("Unknow error, cannot save game, try another name, do you have write permissions?");
                return;
            }
            ui.messages.Add(($"Succesfully saved game as {fileName}"));
        }

        /// <summary>
        /// Load game from a file (folder with 3 files)
        /// </summary>
        /// <returns>Returns whether loading was succesful</returns>
        private bool load()
        {
            Clear();
            // Show a menu with all filenames from which you can make a selection, for a better user-experience, instead of having to fill in a filename by yourself.
            WriteLine("Type the name of the file you want to open\n");

            // This makes a list of all the savegames.
            Directory.GetDirectories("saves").Select(d => Path.GetRelativePath("saves", d)).ToList<string>().ForEach(line => WriteLine(line));
            WriteLine();
            string fileName = ReadLine().Trim();

            // When loading from main menu the ui variable doesn't exist.
            if (ui != null)
                ui.messages.Add(($"Trying to load game {fileName}..."));

            GameData gd;
            try
            {
                gd = new GameData(fileName);
            }
            catch (Exception e)
            {
                ui.messages.Add("Can not open that file");
                return false;
            }
            (board, players, gameState) = (gd.board, gd.players, gd.gameState);
            ui = new UI(board, players, gameState);
            ui.messages.Add(($"Succesfully loaded game {fileName}"));

            return true;
        }

        /// <summary>
        /// Buy a random development card.
        /// </summary>
        private void buyCard()
        {
            Player player = players[gameState.turn];
            DevelopmentCard card = gameState.cardStack.pullCard();
            System.Console.WriteLine(card.cardType);
            switch (card.cardType)
            {
                case "knight": player.knightCards++; break;
                case "victorypoint": player.victorypointCards++; break;
                case "roadbuilding": player.roadbuildingCards++; break;
                case "yearofplenty": player.yearofplentyCards++; break;
                case "monopoly": player.monopolyCards++; break;
            }

            player.resources -= DevelopmentCard.cost;
            ui.messages.Add($"Player {player.name} bought a development card");
        }

        /// <summary>
        /// Play a development card that you own.
        /// </summary>
        /// <param name="cardType">Short version of the name of the card</param>
        private void playCard(string cardType)
        {
            System.Console.WriteLine("in playCard");
            Player player = players[gameState.turn];
            switch (cardType)
            {
                case "kngt":
                    player.knightCards--;
                    player.knightCardsPlayed++;
                    replaceRobber();
                    stealResourceKnight(board.robber);
                    break;
                case "vict":
                    player.victorypointCards--;
                    player.victorypointsFromCards++;
                    break;
                case "rodb":
                    player.roadbuildingCards--;
                    playRoadBuilding();
                    break;
                case "yeop":
                    player.yearofplentyCards--;
                    playYearOfPlenty();
                    break;
                case "mono":
                    player.monopolyCards--;
                    playMonopoly();
                    break;
            }
            ui.messages.Add($"Player {player.name} played a {cardType} card");
        }

        /// <summary>
        /// When the player plays the Road Building Development Card, they get to place two roads.
        private void playRoadBuilding()
        {
            for (int i = 0; i < 2; i++)
            {
                ui.drawFrame();
                ui.messages.Add("Choose a place for a road");
                while (true)
                {
                    PlayerInput pi = new PlayerInput();

                    if (!pi.validSyntax)
                    {
                        ui.messages.Add("Invalid syntax for this command, check the manual");
                        continue;
                    }

                    if (pi.command != enumCommand.road)
                        continue;

                    if (!inputLogicallyValid(pi, isFree: true))
                        continue;

                    road(int.Parse(pi.arguments[0]), isFree: true); calculateLongestRoad();
                    break;
                }
            }
        }

        /// <summary>
        /// When the player draws the Year of Plenty Development Card, they get to take two resources of any type from the bank.
        /// </summary>
        private void playYearOfPlenty()
        {
            ui.drawFrame();
            resourcePicker();
            resourcePicker();
        }

        /// <summary>
        /// When the player plays the Monopoly Development Card, they announce a resource type.
        /// All the other players have to give all of the resources of that type to the player who drew the card.
        /// </summary>
        private void playMonopoly()
        {
            ui.drawFrame();
            ui.messages.Add("Choose 1 resource: lumber, brick, grain, wool, iron");
            bool correctInput = false;
            while (!correctInput)
            {
                ui.refreshCommandBox();
                string resChoice = ReadLine().Trim().ToLower();
                switch (resChoice)
                {
                    case "lumber":
                        foreach (Player p in players)
                        {
                            if (Array.IndexOf(players, p) != gameState.turn)
                            {
                                players[gameState.turn].resources.lumber += players[Array.IndexOf(players, p)].resources.lumber;
                                players[Array.IndexOf(players, p)].resources.lumber = 0;
                            }
                        }
                        correctInput = true;
                        return;
                    case "brick":
                        foreach (Player p in players)
                        {
                            if (Array.IndexOf(players, p) != gameState.turn)
                            {
                                players[gameState.turn].resources.brick += players[Array.IndexOf(players, p)].resources.brick;
                                players[Array.IndexOf(players, p)].resources.brick = 0;
                            }
                        }
                        correctInput = true;
                        return;
                    case "wool":
                        foreach (Player p in players)
                        {
                            if (Array.IndexOf(players, p) != gameState.turn)
                            {
                                players[gameState.turn].resources.wool += players[Array.IndexOf(players, p)].resources.wool;
                                players[Array.IndexOf(players, p)].resources.wool = 0;
                            }
                        }
                        correctInput = true;
                        return;
                    case "grain":
                        foreach (Player p in players)
                        {
                            if (Array.IndexOf(players, p) != gameState.turn)
                            {
                                players[gameState.turn].resources.grain += players[Array.IndexOf(players, p)].resources.grain;
                                players[Array.IndexOf(players, p)].resources.grain = 0;
                            }
                        }
                        correctInput = true;
                        return;
                    case "iron":
                        foreach (Player p in players)
                        {
                            if (Array.IndexOf(players, p) != gameState.turn)
                            {
                                players[gameState.turn].resources.iron += players[Array.IndexOf(players, p)].resources.iron;
                                players[Array.IndexOf(players, p)].resources.iron = 0;
                            }
                        }
                        correctInput = true;
                        return;
                }
            }
        }

        /// <summary>
        /// When the player plays the Knight development card, he gets to steal 1 resource from a player
        /// who has a building adjactent to the hex where the robber was placed.
        /// </summary>
        /// <param name="location"></param>
        private void stealResourceKnight(Location location)
        {
            // Make a list of all the buildings adjacent to the new robber location and check who owns these buildings.
            List<Point> adjacentBuildings = board.adjacentBuildings(new Point(location.x, location.y));
            List<Player> adjacentPlayers = new();
            foreach (Point building in adjacentBuildings)
            {
                if (Building.validBuildingCoordinates(new Location(building.Y, building.X)))
                {
                    Building b = board.buildings[building.Y, building.X];
                    // Don't add players twice, also don't add the player who played the knight card. Can't steal from yourself.
                    if (b.owner != Building.NOBUILDINGHEREYET && !adjacentPlayers.Contains(players[b.owner]) && b.owner != gameState.turn)
                    {
                        adjacentPlayers.Add(players[b.owner]);
                    }
                }

            }
            if (adjacentPlayers.Count() == 0)
            {
                ui.messages.Add("No adjacent players to steal a resource from");
                return;
            }
            stealOneResource(selectPlayerToStealFrom(adjacentPlayers));
        }

        /// <summary>
        /// Player needs to choose which player to steal a resource from.
        /// </summary>
        /// <param name="adjacentPlayers">Players adjacent to new location of the robber</param>
        /// <returns></returns>
        private int selectPlayerToStealFrom(List<Player> adjacentPlayers)
        {
            ui.messages.Add("Which player do you want to steal a resource from?");
            ui.messages.Add("Type the number of one of the players above");
            string playerNames = "| ";
            foreach (Player p in adjacentPlayers)
            {
                playerNames += p.number + ": " + p.name + " | ";
            }
            ui.messages.Add(playerNames);

            List<int> validNums = new();
            foreach (Player p in adjacentPlayers)
                validNums.Add(p.number);
            while (true)
            {
                int input;
                if (!int.TryParse(ReadLine().Trim(), out input))
                {
                    ui.messages.Add("Invalid. Type the number of the player.");
                    continue;
                }
                if (!validNums.Contains(input))
                {
                    ui.messages.Add("Invalid. Type number of one of players in list");
                    continue;
                }
                return input;
            }
        }

        /// <summary>
        /// Steal one random resource from a player
        /// </summary>
        /// <param name="playerNum"></param>
        private void stealOneResource(int playerNum)
        {
            Player playerLose = players[playerNum];
            Random rng = new Random();

            int rdnum = rng.Next(0, playerLose.resources.total);
            // we put all the resources the player has in a list.
            List<string> resources = new List<string>();
            for (int i = 0; i < playerLose.resources.lumber; i++)
                resources.Add("lumber");
            for (int i = 0; i < playerLose.resources.brick; i++)
                resources.Add("brick");
            for (int i = 0; i < playerLose.resources.wool; i++)
                resources.Add("wool");
            for (int i = 0; i < playerLose.resources.iron; i++)
                resources.Add("iron");
            for (int i = 0; i < playerLose.resources.grain; i++)
                resources.Add("grain");

            // take a random resource from this list.
            string stolenResource = resources[rdnum];
            enumResource rs;
            Enum.TryParse(stolenResource, out rs);
            Player playerReceive = players[gameState.turn];

            // subtract from player who loses and add to player who steals.
            switch (rs)
            {
                case enumResource.lumber: playerReceive.resources.lumber += 1; playerLose.resources.lumber -= 1; break;
                case enumResource.brick: playerReceive.resources.brick += 1; playerLose.resources.brick -= 1; break;
                case enumResource.wool: playerReceive.resources.wool += 1; playerLose.resources.wool -= 1; break;
                case enumResource.grain: playerReceive.resources.grain += 1; playerLose.resources.grain -= 1; break;
                case enumResource.iron: playerReceive.resources.iron += 1; playerLose.resources.iron -= 1; break;
            }
            ui.messages.Add($"You stole 1 {stolenResource}");
        }


        /// <summary>
        /// Function for Year of Plenty Development card:
        /// Allows user to choose a resource type to get one from.
        /// Called twice by playYearOfPlenty.
        /// </summary>
        private void resourcePicker()
        {
            ui.drawFrame();
            bool correctInput = false;
            Player player = players[gameState.turn];
            ui.messages.Add("Choose 1 resource: lumber, brick, grain, wool, iron");
            while (!correctInput)
            {
                ui.refreshCommandBox();
                string resChoice = ReadLine().Trim().ToLower();
                switch (resChoice)
                {
                    case "lumber":
                        ui.messages.Add($"You got 1 {resChoice}");
                        player.resources.lumber++;
                        return;
                    case "brick":
                        ui.messages.Add($"You got 1 {resChoice}");
                        player.resources.brick++;
                        return;
                    case "wool":
                        ui.messages.Add($"You got 1 {resChoice}");
                        player.resources.wool++;
                        return;
                    case "grain":
                        ui.messages.Add($"You got 1 {resChoice}");
                        player.resources.grain++;
                        return;
                    case "iron":
                        ui.messages.Add($"You got 1 {resChoice}");
                        player.resources.iron++;
                        return;
                }
            }
        }
    }
}
