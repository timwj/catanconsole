namespace CatanConsole
{
    public partial class GameLogic
    {

        /// <summary>
        /// In the beginning of the game each player gets to place two free villages and roads on the board. 
        /// The player order reverses after the first round of placing for fairness.
        /// </summary>
        private int placeStartingStructures()
        {
            ui.messages.Add("This is the starting phase of the game");

            bool reverseOrder = false; // after each player has put a road and village, the order of players reverses
            for (int r = 0; r < 2; r++)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    ui.drawFrame();
                    if (placeStartingVillage(isSecond: r == 1) == 1 || placeStartingRoad() == 1)  // player pressed quit
                        return 1;
                    ui.drawFrame();
                    end(reverseOrder);
                }
                reverseOrder = !reverseOrder;
                end(reverseOrder);
            }
            return 0;
        }

        private int placeStartingVillage(bool isSecond)
        {
            ui.messages.Add("Place a village");
            while (true) // loop until a village is succesfully placed
            {
                PlayerInput pi = new PlayerInput();
                ui.refreshCommandBox();

                if (pi.command == enumCommand.quit)
                    return 1;

                if (!pi.validSyntax)
                {
                    ui.messages.Add("invalid syntax, check the manual");
                    continue;
                }
                if (pi.command != enumCommand.village)
                {
                    ui.messages.Add("In this part of the game you need to place 1 village");
                    continue;
                }
                if (!inputLogicallyValid(pi, isFree: true))
                {
                    continue;
                }
                int locationNumber = int.Parse(pi.arguments[0]);
                Location l = buildingIntToPoint[locationNumber];
                village(locationNumber, isFree: true);
                if (isSecond)
                    giveStartingResources(l, players[gameState.turn]);
                ui.drawFrame();
                break;
            }
            return 0;
        }

        private int placeStartingRoad()
        {
            ui.messages.Add("Place a road");
            while (true)
            {
                PlayerInput pi = new PlayerInput();
                ui.refreshCommandBox();

                if (pi.command == enumCommand.quit)
                    return 1;
                if (!pi.validSyntax)
                {
                    ui.messages.Add("invalid syntax, check the manual");
                    continue;
                }
                if (pi.command != enumCommand.road)
                {
                    ui.messages.Add("In this part of the game you need to place 1 road");
                    continue;
                }
                if (!inputLogicallyValid(pi, isFree: true))
                {
                    continue;
                }
                road(int.Parse(pi.arguments[0]), isFree: true);
                break;
            }
            return 0;
        }

        private void giveStartingResources(Location secondVillage, Player p)
        {
            List<Location> surroundingHexes = Building.adjacentHexes(secondVillage);
            List<enumResource> surroundingResources = new List<enumResource>();

            foreach (Location h in surroundingHexes)
            {
                enumResource resource = hexTypeToResource[board.hexes[h.y, h.x].hexType];
                if (resource != enumResource.nothing)
                    surroundingResources.Add(resource);
            }
            foreach (enumResource r in surroundingResources)
            {
                p.resources += (r, 1);
                p.resourcesThisTurn += (r, 1);
            }

        }
    }
}