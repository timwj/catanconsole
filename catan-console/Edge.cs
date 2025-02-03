namespace CatanConsole
{
    /// <summary>
    /// Shows which nodes does a road connect, used graph algorithm for finding longest road.
    /// </summary>
    public struct Edge
    {
        public Location node1;
        public Location node2;

        public Edge(Location rd)
        {
            int o = Road.orientation(rd);
            if (o == 0)
            {// hor up
                node1 = new Location(rd.y + 1, rd.x);
                node2 = new Location(rd.y, rd.x + 1);
            }
            else if (o == 1)
            {
                node1 = new Location(rd.y, rd.x);
                node2 = new Location(rd.y + 1, rd.x + 1);
            }
            else
            {
                node1 = new Location(rd.y, rd.x);
                node2 = new Location(rd.y + 1, rd.x);
            }
        }

        public override string ToString()
        {
            return $"({node1.y}, {node1.x}) - ({node2.y}, {node2.x})";
        }
    }
}