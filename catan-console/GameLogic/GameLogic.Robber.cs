using static System.Console;
using System.Drawing;
namespace CatanConsole
{
    public partial class GameLogic
    {
        /// <summary>
        /// When 7 is rolled, the robber comes and any player with more than 7 resources loses half of them randomly
        /// </summary>
        private void stealResources()
        {
            Random rnd = new Random();
            foreach (Player p in players)
            {
                if (p.resources.totalResources >= 8)
                {
                    int numDiscard = p.resources.totalResources / 2; // rounds down 

                    ui.messages.Add($"Player {p.name} lost {numDiscard} resources to the robber");
                    for (int i = 0; i < numDiscard; i++)
                    {
                        enumResource rs = (enumResource)rnd.Next(0, 5);
                        switch (rs)
                        {
                            case enumResource.lumber: if (p.resources.lumber == 0) i--; else p.resources.lumber -= 1; break;
                            case enumResource.brick: if (p.resources.brick == 0) i--; else p.resources.brick -= 1; break;
                            case enumResource.wool: if (p.resources.wool == 0) i--; else p.resources.wool -= 1; break;
                            case enumResource.grain: if (p.resources.grain == 0) i--; else p.resources.grain -= 1; break;
                            case enumResource.iron: if (p.resources.iron == 0) i--; else p.resources.iron -= 1; break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Put the robber on a new hex when you roll 7.
        /// </summary>
        private void replaceRobber()
        {
            int previousRobber = hexPointToInt[board.robber];
            ui.messages.Add("Type the new location for the robber");
            gameState.placingRobber = true;
            ui.drawFrame();
            while (true)
            {
                string input = ReadLine().Trim();
                int location;
                if (int.TryParse(input, out location) && 0 <= location && location <= 18)
                {
                    if (location == previousRobber)
                    {
                        ui.messages.Add("The robber can not stay at the same location");
                        ui.drawFrame();
                        continue;
                    }
                    if (location == 7)
                    {
                        ui.messages.Add("The robber can not move to the desert.");
                        ui.drawFrame();
                        continue;
                    }
                    board.robber = hexIntToPoint[location];
                    ui.messages.Add($"Player {players[gameState.turn].name} moved the robber.");
                    gameState.placingRobber = false;
                    return;
                }
                else
                {
                    ui.messages.Add("Invalid location for the robber! try again..");
                    ui.drawFrame();
                }
            }
        }
    }
}