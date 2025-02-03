namespace CatanConsole
{
    public abstract class Building
    {
        // We have 5 players, numbered 0 to 4. We pick a dummy integer 5 for describing a vacant spot.
        public const int NOBUILDINGHEREYET = 5;

        /// <summary>
        /// since the board is the shape of a rough hexagon, and we store coordinates in a square array, some array
        /// indices are not used and thus invalid. 
        /// </summary>
        public static bool validBuildingCoordinates(Location location)
        {
            int y = location.y; int x = location.x;
            return y switch
            {
                0 or 11 => (x == 3 || x == 5 || x == 7),
                1 or 2 or 9 or 10 => (x == 2 || x == 4 || x == 6 || x == 8),
                3 or 4 or 7 or 8 => (x == 1 || x == 3 || x == 5 || x == 7 || x == 9),
                5 or 6 => (x == 0 || x == 2 || x == 4 || x == 6 || x == 8 || x == 10),
                _ => false
            };
        }
        public int owner
        {
            get; set;
        }
        public abstract int productionAmount();  // can't get the syntax for property in abstact class correct, really tried.

        public static List<Location> adjacentHexes(Location bd)
        {
            List<Location> adjacent = new();
            if (bd.y % 2 == 0)
            {
                if (Hex.hexExists(new Location(bd.y - 1, bd.x - 2)))
                    adjacent.Add(new Location(bd.y - 1, bd.x - 2));

                if (Hex.hexExists(new Location(bd.y - 1, bd.x)))
                    adjacent.Add(new Location(bd.y - 1, bd.x));

                if (Hex.hexExists(new Location(bd.y + 1, bd.x - 1)))
                    adjacent.Add(new Location(bd.y + 1, bd.x - 1));
            }
            else
            {
                if (Hex.hexExists(new Location(bd.y, bd.x - 2)))
                    adjacent.Add(new Location(bd.y, bd.x - 2));

                if (Hex.hexExists(new Location(bd.y, bd.x)))
                    adjacent.Add(new Location(bd.y, bd.x));

                if (Hex.hexExists(new Location(bd.y - 2, bd.x - 1)))
                    adjacent.Add(new Location(bd.y - 2, bd.x - 1));
            }
            return adjacent;
        }

        public static List<Location> adjacentRoadsToBuilding(Location bd)
        {
            List<Location> adjacentRoadsToBuilding = new();
            int y = bd.y;
            int x = bd.x;
            List<Location> nb;
            if (y % 2 == 0)
            {
                nb = new List<Location> { new Location(y - 1, x), new Location(y, x - 1), new Location(y, x) };
            }
            else
            {
                nb = new List<Location> { new Location(y - 1, x - 1), new Location(y, x), new Location(y - 1, x) };
            }
            foreach (Location n in nb)
            {
                if (Road.validRoadCoordinates(n))
                {
                    adjacentRoadsToBuilding.Add(n);
                }
            }
            return adjacentRoadsToBuilding;
        }
    }
    public class EmptyBuilding : Building
    {
        public override int productionAmount() => 0;
        public EmptyBuilding() => this.owner = NOBUILDINGHEREYET;

        public override string ToString() => "emptybuilding";
    }

    public class Village : Building
    {
        public static ResourceClass cost = new ResourceClass(1, 1, 1, 1, 0);
        public override int productionAmount() => 1;
        public Village(int owner) => this.owner = owner;

        public override string ToString() => "village";
    }

    public class City : Building
    {
        public static ResourceClass cost = new ResourceClass(0, 0, 0, 2, 3);
        public override int productionAmount() => 2;
        public City(int owner) => this.owner = owner;

        public override string ToString() => "city";
    }

}
