#nullable enable 

using System.Drawing;

namespace CatanConsole
{
    public class Board
    {
        public Hex[,] hexes = new Hex[10, 9];
        public Building[,] buildings = new Building[12, 11]; // can be a village, city or empty
        public Road[,] roads = new Road[11, 11];
        public List<Location> longestRoad = new();

        public Location robber;

        public harborType[,] harbors = new harborType[12, 11];

        /// <summary>
        /// Create the standard board, this one is fair for beginners. Resources are uniformly distributed 
        /// </summary>
        public Board(bool randomBoard = false)
        {
            initHexes(randomBoard);
            initRoads();
            initBuildings();
        }

        /// <summary> 
        /// Initialise hexes, this is specific for the standard (beginners) board 
        /// </summary>
        private void initHexes(bool random = false)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 9; j++)
                    hexes[i, j] = new Hex(enumHexType.nonExistent, 13); // default value for hex that doesn't exist in our board

            List<enumHexType> listOfHexTypes = new List<enumHexType> { // the standard 'fair' permuation
                enumHexType.lumber, enumHexType.grain, enumHexType.grain, enumHexType.brick, enumHexType.iron,
                enumHexType.brick, enumHexType.wool, enumHexType.desert, enumHexType.lumber, enumHexType.grain,
                enumHexType.lumber, enumHexType.grain, enumHexType.brick, enumHexType.wool, enumHexType.wool,
                enumHexType.iron, enumHexType.iron, enumHexType.grain, enumHexType.lumber};

            List<int> diceNumbers = new List<int> { // the standard 'fair' permutation
                11, 12, 9, 4, 6,
                5, 10, 3, 11, 4,
                8, 8, 10, 9, 3,
                5, 2, 6
            };

            List<int> resourcePermutation = Enumerable.Range(0, 19).ToList();
            List<int> dicePermutation = Enumerable.Range(0, 18).ToList();
            if (random)
            {
                Random rng = new Random();
                resourcePermutation = resourcePermutation.OrderBy(a => rng.Next()).ToList();
                dicePermutation = dicePermutation.OrderBy(a => rng.Next()).ToList();
            }

            int d = 0; // for looping over the dice numbers permutation
            for (int i = 0; i < GameLogic.hexCoordinates.Count(); i++)
            {
                Location l = GameLogic.hexCoordinates[i];
                enumHexType thisHexType = listOfHexTypes[resourcePermutation[i]];
                if (thisHexType == enumHexType.desert)
                {
                    hexes[l.y, l.x] = new Hex(enumHexType.desert, 14);
                    robber = l;
                }
                else
                    hexes[l.y, l.x] = new Hex(thisHexType, diceNumbers[dicePermutation[d++]]);
            }
        }

        /// <summary>
        /// Initialise roads, the same for every board 
        /// </summary>
        private void initRoads()
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    // Road.BUITENBORD means no road can exist on that specific coordinate (invalid coordinates)
                    roads[i, j] = new Road(Road.validRoadCoordinates(new Location(i, j)) ? Road.NOROADHEREYET : Road.INVALIDCOORDINATES);
                }
            }
        }

        private void initBuildings()
        {
            // Initialise buildings, the same for every board
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 11; j++)
                {   // If the coordinate is valid we create a an empty building entry
                    harbors[i, j] = harborType.notHarbor;
                    if (Building.validBuildingCoordinates(new Location(i, j)))
                    {
                        // EmptyBuilding represents an empty habitable spot
                        buildings[i, j] = new EmptyBuilding();
                    }
                }
            }

            harbors[0, 3] = harborType.threeForOne; harbors[1, 2] = harborType.threeForOne;
            harbors[0, 5] = harborType.grain; harbors[1, 6] = harborType.grain;
            harbors[2, 8] = harborType.iron; harbors[3, 9] = harborType.iron;

            harbors[5, 10] = harborType.threeForOne; harbors[6, 10] = harborType.threeForOne;
            harbors[8, 9] = harborType.wool; harbors[9, 8] = harborType.wool;
            harbors[11, 5] = harborType.threeForOne; harbors[10, 6] = harborType.threeForOne;

            harbors[10, 2] = harborType.threeForOne; harbors[11, 3] = harborType.threeForOne;
            harbors[7, 1] = harborType.brick; harbors[8, 1] = harborType.brick;
            harbors[3, 1] = harborType.lumber; harbors[4, 1] = harborType.lumber;
        }

        ///<summary> gives a list of all the coordinates of hexes that have the number num, used when rolling a dice to
        /// determine who will get resources this turn <summary>
        public List<Point> hexNum(int num)
        {
            List<Point> p = new List<Point>();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 9; j++)
                    if (hexes[i, j].number == num)
                        p.Add(new Point(j, i));
            return p;
        }

        /// Gives a list of all the buildings coordinates that are adjacent to a hex and exist.
        public List<Point> adjacentBuildings(Point hex)
        {
            List<Point> adjacent = new List<Point>(); // y and x coordinate of an adjacent building
            List<(int, int)> offsetList = new List<(int, int)> { (0, 0), (-1, 1), (0, 2), (1, 2), (2, 1), (1, 0) };
            foreach (var v in offsetList)
            {
                (int, int) location = (hex.Y + v.Item1, hex.X + v.Item2);
                adjacent.Add(new Point(location.Item2, location.Item1));
            }
            return adjacent;
        }

        /// <summary>
        /// For the distance rule. Buildings must be placed 2 roads apart.
        /// Here we check the condition that all surrounding buildings of a building are empty (or outside the board) which is equivalent
        /// </summary>
        /// <param name="p">The new building that we want to place</param>
        /// /// <returns></returns>
        public bool allowedDistanceRule(Location p)
        {
            int x = p.x; int y = p.y;
            List<Location> neighbours;

            if (y % 2 == 0) // even node
            {
                neighbours = new List<Location>() { new Location(y + 1, x - 1), new Location(y - 1, x), new Location(y + 1, x + 1) };
            }
            else
            {
                neighbours = new List<Location>() { new Location(y - 1, x - 1), new Location(y - 1, x + 1), new Location(y + 1, x) };
            }
            foreach (Location nb in neighbours)
            {
                if (!Building.validBuildingCoordinates(nb))
                {
                    continue;
                }
                if (!(buildings[nb.y, nb.x].ToString() == "emptybuilding"))
                {
                    return false;
                }
            }
            return true;
        }

        public bool allowedConnectedVillage(Location location, Player player)
        {
            List<Location> adjacentRoadsToVillage = Building.adjacentRoadsToBuilding(location);
            bool valid = false;
            foreach (Location rd in adjacentRoadsToVillage)
            {
                if (roads[rd.y, rd.x].owner == player.number)
                    valid = true;
            }
            return valid;
        }

    }
}


