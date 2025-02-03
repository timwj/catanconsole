namespace CatanConsole
{
    public class Road
    {
        public const int NOROADHEREYET = 5, INVALIDCOORDINATES = -1;
        public int owner;
        public static ResourceClass cost = new ResourceClass(1, 1, 0, 0, 0);

        /// <summary>
        /// Because the roads are the edges of hexagons, and we save them in a 2D square array,
        /// Not all array elements are used. Some are empty, this wastes storage but the indexes
        /// remain more logical for use in algorithms. This function detects if the array element
        /// corresponds to a road that exists in the board.
        /// </summary>
        /// <param name="y">y position in array (height)</param>
        /// <param name="x">x position in array (width)</param>
        /// <returns></returns>
        public static bool validRoadCoordinates(Location location)
        {
            int y = location.y; int x = location.x;
            return y switch
            {
                0 or 10 => (2 <= x && x <= 7),
                1 or 9 => (2 <= x && x <= 8 && x % 2 == 0),
                2 or 8 => (1 <= x && x <= 8),
                3 or 7 => (1 <= x && x <= 9 && x % 2 == 1),
                4 or 6 => (0 <= x && x <= 9),
                5 => (0 <= x && x <= 10 && x % 2 == 0),
                _ => false
            };

        }
        public override string ToString() => "road";
        public Road(int owner) => this.owner = owner;


        public static List<Location> neighbours(Location l)
        {
            List<Location> nb = new List<Location>();
            List<Location> retList = new List<Location>();
            int y = l.y; int x = l.x;

            if ((y % 4 == 0 && x % 2 == 0) || (y % 4 == 2 && x % 2 == 1))
                nb = new List<Location> { new Location(y, x + 1), new Location(y + 1, x), new Location(y, x - 1), new Location(y - 1, x + 1) };
            else if ((y % 4 == 0 && x % 2 == 1) || (y % 4 == 2 && x % 2 == 0))
                nb = new List<Location> { new Location(y, x + 1), new Location(y + 1, x + 1), new Location(y, x - 1), new Location(y - 1, x) };
            else
                nb = new List<Location> { new Location(y + 1, x), new Location(y + 1, x - 1), new Location(y - 1, x - 1), new Location(y - 1, x) };
            for (int i = 0; i < nb.Count; i++)
                if (Road.validRoadCoordinates(nb[i]))
                    retList.Add(nb[i]);
            return retList;
        }

        public static int orientation(Location location)
        {
            int y = location.y; int x = location.x;
            if ((y % 4 == 0 && x % 2 == 0) || (y % 4 == 2 && x % 2 == 1))
                return 0;
            else if ((y % 4 == 0 && x % 2 == 1) || (y % 4 == 2 && x % 2 == 0))
                return 1;
            else return 2;
        }

        /// <summary>
        /// For longest road graph algorithm, finds which 2 buildings (nodes) this road connects
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static Location coordFromEdge(Edge edge)
        {
            if (edge.node1.y == edge.node2.y + 1 && edge.node1.x == edge.node2.x - 1) // horizontal up
                return new Location(edge.node1.y - 1, edge.node1.x);
            else if (edge.node1.y == edge.node2.y - 1 && edge.node1.x == edge.node2.x - 1) // horizontal down
                return new Location(edge.node1.y, edge.node1.x);
            else  // vertical
                return new Location(edge.node1.y, edge.node1.x);
        }

    }
}