using System.Collections.Generic;
namespace CatanConsole
{
    public partial class GameLogic
    {

        // All these functions check if a command is allowed. Assumes valid syntax which is checked elsewhere.

        private bool isValidRoll()
        {
            if (gameState.diceRolled)
            {
                ui.messages.Add("Dice already rolled!");
                return false;
            }
            return true;
        }

        private bool isValidVillage(int locationNumber, bool isFree)
        {
            if (!gameState.diceRolled && gameState.mainPhase)
            {
                ui.messages.Add("Roll the dice first!");
                return false;
            }
            Player player = players[gameState.turn];

            Location location;
            if (!buildingIntToPoint.TryGetValue(locationNumber, out location))
            {
                ui.messages.Add("Invalid coordinate for a village");
                return false;
            }
            if (board.buildings[location.y, location.x].owner != Building.NOBUILDINGHEREYET)
            {
                ui.messages.Add("Space already occupied");
                return false;
            }
            if (!board.allowedDistanceRule(location))
            {
                ui.messages.Add("Not allowed to build here because distance rule");
                return false;
            }
            if (!villageConnected(location) && !isFree)
            {
                ui.messages.Add("Not allowed to build here. Must be connected to road");
                return false;
            }
            if (!(player.resources >= Village.cost) && !isFree)
            {
                ui.messages.Add("Not enough resources to build a village");
                return false;
            }
            return true;
        }

        private bool villageConnected(Location location)
        {
            return board.allowedConnectedVillage(location, players[gameState.turn]);
        }

        private bool isValidCity(int locationNumber)
        {
            if (!gameState.diceRolled)
            {
                ui.messages.Add("Roll the dice first!");
                return false;
            }
            Player player = players[gameState.turn];
            Location location;

            if (!buildingIntToPoint.TryGetValue(locationNumber, out location))
            {
                ui.messages.Add("Invalid coordinate for a city");
                return false;
            }
            if (!(player.resources >= City.cost))
            {
                ui.messages.Add("Not enough resources!");
                return false;
            }
            Building current = board.buildings[location.y, location.x];
            if (current.ToString() != "village")
            {
                ui.messages.Add("Can only place city on an existing village");
                return false;
            }
            if (current.owner != player.number)
            {
                ui.messages.Add("Can only place city on a village owned by you!");
                return false;
            }
            return true;
        }

        private bool isValidRoad(int locationNumber, bool isFree)
        {
            if (!gameState.diceRolled && gameState.mainPhase)
            {
                ui.messages.Add("Roll the dice first!");
                return false;
            }
            Location location;
            Player player = players[gameState.turn];

            if (!roadIntToPoint.TryGetValue(locationNumber, out location))
            {
                ui.messages.Add("Invalid coordinate for a road");
                return false;
            }
            if (board.roads[location.y, location.x].owner != Road.NOROADHEREYET)
            {
                ui.messages.Add("Can't build a road here, space already occupied!");
                return false;
            }
            if (!roadHasAdjacentStructureSameOwner(location, player))
            {
                ui.messages.Add("A road must be adjacent to one of your structures");
                return false;
            }
            if (!(players[gameState.turn].resources >= Road.cost) && !isFree) // in the beginning game phase 2 roads are free
            {
                ui.messages.Add("Not enough resources!");
                return false;
            }
            System.Console.WriteLine("valid road");
            return true;
        }

        private bool isValidEnd()
        {
            if (gameState.diceRolled)
            {
                return true;
            }
            else
            {
                ui.messages.Add("roll the dice first");
                return false;
            }
        }

        private bool isValidTrade(enumResource resourceGive, enumResource resourceReceive)
        {
            if (!gameState.diceRolled)
            {
                ui.messages.Add("Roll the dice first!");
                return false;
            }
            Player player = players[gameState.turn];
            int rate = player.harbors.Contains(harborType.threeForOne) ? 3 : 4;

            foreach (var h in player.harbors)
                if (h != harborType.threeForOne && h != harborType.notHarbor && harborTypeToResource[h] == resourceGive)
                    rate = 2;

            if (player.resources >= (new ResourceClass(resourceGive, rate)))
            {
                return true;
            }
            else
            {
                ui.messages.Add("Not enough resources for this trade");
                return false;
            }
        }

        private bool isValidBuyCard()
        {
            if (!gameState.diceRolled)
            {
                ui.messages.Add("Roll the dice first!");
                return false;
            }
            Player player = players[gameState.turn];

            if (player.resources >= DevelopmentCard.cost)
            {
                return true;
            }
            else
            {
                ui.messages.Add("Not enough resources to buy a development card");
                return false;
            }
        }

        private bool isValidPlayCard(string cardType)
        {
            if (!gameState.diceRolled && cardType != "kngt")   // There is a weird rule that you can only play knight cards before you roll
            {
                ui.messages.Add("Roll the dice first!");
                return false;
            }
            Player player = players[gameState.turn];
            bool valid = false;
            switch (cardType)
            {
                case "kngt": valid = player.knightCards > 0; break;
                case "vict": valid = player.victorypointCards > 0; break;
                case "rodb": valid = player.roadbuildingCards > 0; break;
                case "yeop": valid = player.yearofplentyCards > 0; break;
                case "mono": valid = player.monopolyCards > 0; break;
            }
            if (!valid)
            {
                ui.messages.Add("You don't own this development card");

            }
            return valid;
        }


        /// <summary>
        /// This rule says that newly placed roads must be directly connected to a road or a city or a village that is owned by
        /// the same player.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        bool roadHasAdjacentStructureSameOwner(Location location, Player player)
        {
            bool valid = false;
            List<Location> adjacentRoads = Road.neighbours(location);

            Edge e = new Edge(location);  // find the two nodes that this road connects.
            List<Location> adjacentBuildings = new() { e.node1, e.node2 };

            foreach (Location r in adjacentRoads)
            {
                if (board.roads[r.y, r.x].owner == player.number)
                    valid = true;
            }
            foreach (Location b in adjacentBuildings)
            {
                if (board.buildings[b.y, b.x].owner == player.number)
                    valid = true;
            }
            return valid;

        }


    }
}