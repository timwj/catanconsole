using static System.Console;

namespace CatanConsole
{
    public partial class GameLogic
    {
        /// <summary>
        /// Only works if each player has at least 1 road. Crashes otherwise. Don't call when players don't have roads.
        /// </summary>

        /// <summary>
        /// The longest road is worth 2 Victorypoints. 
        /// This entire class builds up a graph with nodes and edges for each connected road a player has.
        /// The graph is built with Breadth first search using a Queue.
        /// 
        /// Then a depth first search is performed, starting from each edge, to find the longest connected road.
        /// The edges that make up this longest road and the length are saved.
        /// Since the longest road can contain cycles, some adjustments to a normal depth first search were made.
        /// </summary>
        private void calculateLongestRoad()
        {
            // When the road lengths of players are equal, the player who first reached
            // that length get the 2 victorypoints.
            int previousPlayerNumLongest = gameState.playerNumLongestRoad;

            foreach (Player p in players)
            {
                p.playersLongestRoad = findPlayersLongestRoad(p.number);
                if (p.playersLongestRoad.Count > gameState.longestRoadLength)
                {
                    gameState.longestRoadLength = p.playersLongestRoad.Count;
                    gameState.playerNumLongestRoad = p.number;
                }
                p.longestRoadVP = false;
            }
            // stores a list of the edges that make the longest road. Used in the UI to mark the edges with a  '*'
            board.longestRoad = players[gameState.playerNumLongestRoad].playersLongestRoad.Select(i => Road.coordFromEdge(i)).ToList();

            // A minimum length of 5 edges is required to get the victorypoints
            if (gameState.longestRoadLength >= 5)
                players[gameState.playerNumLongestRoad].longestRoadVP = true;

        }

        /// <summary>
        /// Finds locations of all edges owned by this player that are connected to this edge.
        /// </summary>
        /// <param name="road">starting edge from where to search for connected edges</param>
        /// <param name="playerNum">only add connected edges owned by this player</param>
        /// <returns></returns>
        private List<Location> makeRoadSet(Location road, int playerNum)
        {
            // marked means this edge is already visited. Neccesary cause may contain cycles. Would loop infinetely otherwise.
            bool[,] marked = new bool[11, 11];
            for (int i = 0; i < 11; i++)
                for (int j = 0; j < 11; j++)
                    marked[i, j] = false;
            List<Location> set = new() { road };
            marked[road.y, road.x] = true;

            // This does a standard Breadth first search to find connected edges.
            Queue<Location> q = new();
            q.Enqueue(road);
            while (q.Any())
            {
                var cur = q.Dequeue();
                List<Location> nb = Road.neighbours(cur);

                foreach (var n in nb)
                {

                    int ny = n.y; int nx = n.x;
                    if (board.roads[ny, nx].owner == playerNum && !marked[ny, nx])
                    {
                        marked[ny, nx] = true;
                        set.Add(n);
                        q.Enqueue(n);
                    }
                }
            }
            return set;
        }

        /// <summary>
        /// This contains all the 'administration', creating the graphs, then performing depth first search
        /// while keeping track of the longest path so far.
        /// </summary>
        /// <param name="playerNum">Finds the longest path of this player</param>
        /// <returns></returns>
        private List<Edge> findPlayersLongestRoad(int playerNum)
        {
            // connected road graphs, each player should have 1 or 2 in base Catan (because of adjacency rule of new roads)
            List<List<Edge>> edgeSets = makeEdgeSets(playerNum);

            // all the villages and buildings that are nodes in this graph
            List<List<Location>> nodeSets = new();

            // find for each edge the number of edges it is directly connected to. Used to find endpoints of the graph.
            List<int[]> nConnectionSet = new();

            // Is this edge part of a cycle?
            List<bool> isLoop = new List<bool>();

            // Endpoints of the graph, only connected to 1 other edge.
            List<List<int>> startEdges = new();

            // Keep track of already visited edges for depth first search
            List<Edge> visited = new();

            // Keep track of the longest path found so far.
            List<Edge> longestPath = new();

            // Current path where we are searching in depth first search
            List<Edge> currentPath = new();

            // create graph(s) and info about endpoints
            for (int i = 0; i < edgeSets.Count; i++)
            {
                nodeSets.Add(makeNodeSet(edgeSets[i]));
                nConnectionSet.Add(makeConnectionSet(edgeSets[i]));
                startEdges.Add(new List<int>());
                for (int q = 0; q < nConnectionSet[i].Length; q++)
                {
                    // This edge is part of a cycle. So it has no endpoints and we need to perform a depth first search starting from each edge.
                    // Take a loop at figure 8 graph (double loop, two overlapping hexagons.
                    if (!nConnectionSet[i].Contains(1))
                        startEdges[i].Add(q);
                    else if (nConnectionSet[i][q] == 1)  // This edge is an endpoint of the graph
                        startEdges[i].Add(q);
                }

                // Only performing depth first search on endpoints saves a lot of calculations
                foreach (int startEdge in startEdges[i])
                {
                    Edge se = edgeSets[i][startEdge]; // this is the edge that is connected to an endpoint node.

                    Location startNode; // need to determine which part of the edge is connected to the endpoint node.
                    int left = 0; int right = 0;
                    for (int j = 0; j < edgeSets[i].Count; j++)
                    {
                        if (edgeSets[i][j].node1 == se.node1 || edgeSets[i][j].node2 == se.node1)
                            left++;
                        if (edgeSets[i][j].node1 == se.node2 || edgeSets[i][j].node2 == se.node2)
                            right++;
                    }
                    startNode = left == 1 ? se.node1 : se.node2;

                    // Start searching recursively with length 1 of path
                    depthFirstSearch(se, startNode, edgeSets[i], 1, visited, currentPath, longestPath);
                }
            }
            return longestPath;
        }

        /// <summary>
        /// Overhead function for creating the 1 or 2 connected graphs of roads a player has. Not the algorithm itself.
        /// </summary>
        private List<List<Edge>> makeEdgeSets(int playerNum)
        {
            // Make a list of all the roads on the board this player has.
            List<Location> thisPlayersRoads = new();
            for (int i = 0; i < 11; i++)
                for (int j = 0; j < 11; j++)
                    if (board.roads[i, j].owner == playerNum)
                        thisPlayersRoads.Add(new Location(i, j));

            // start at a random road
            Location someRoadFirstSet = thisPlayersRoads[0];
            // search from this road to which other roads is it connected?
            List<Location> firstRoadSet = makeRoadSet(someRoadFirstSet, playerNum);

            // all the roads that are not part of the first connected graph
            List<Location> remainder = thisPlayersRoads.Except<Location>(firstRoadSet).ToList();

            // Roads are (y,x) values in an array, need to convert to Edge(NodeFrom, NodeTo) datastructure
            // so that graph algorithms will be easier.
            List<Edge> firstEdgeSet = firstRoadSet.Select(rd => new Edge(rd)).ToList();

            // Sometimes a player doesn't have a second connected graph
            bool existsSecondSet = remainder.Count > 0;
            List<List<Edge>> returnValue = new List<List<Edge>>() { firstEdgeSet };
            if (existsSecondSet)
            {
                List<Edge> secondEdgeSet = makeRoadSet(remainder[0], playerNum).Select(rd => new Edge(rd)).ToList();
                returnValue.Add(secondEdgeSet);
            }
            return returnValue;
        }

        /// <summary>
        /// Get the locations of nodes that are part of this road network.
        /// </summary>
        private List<Location> makeNodeSet(List<Edge> edges)
        {
            List<Location> nodes = new();
            foreach (Edge edge in edges)
            {
                if (!nodes.Contains(edge.node1))
                    nodes.Add(edge.node1);
                if (!nodes.Contains(edge.node2))
                    nodes.Add(edge.node2);
            }
            return nodes;
        }

        /// <summary>
        /// For finding endpoints. Endpoints are connected only on 1 side.
        /// </summary>
        private int[] makeConnectionSet(List<Edge> edges)
        {
            int[] nConnections = new int[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                for (int j = 0; j < edges.Count; j++)
                    if (i != j)
                        nConnections[i] += truth(edges[j].node1 == edges[i].node1,
                        edges[j].node1 == edges[i].node2,
                        edges[j].node2 == edges[i].node1,
                        edges[j].node2 == edges[i].node2);
            return nConnections;
        }

        /// <summary>
        /// Helper function. Count how many booleans in an array are true.
        /// </summary>
        private static int truth(params bool[] booleans) => booleans.Count(b => b);

        /// <summary>
        /// Find the longest path in a graph. Recursive implementation.
        /// </summary>
        /// <param name="ed">Edge from where to start searching</param>
        /// <param name="nodeFrom">Node from where to start searching, must be connected to 'ed'</param>
        /// <param name="network">All the edges in this connected graph</param>
        /// <param name="length">Length of the current path</param>
        /// <param name="visited">Already visited edges, must keep track because graph can contain cycles</param>
        /// <param name="currentPath">Keep track to prevent going back in the same direction</param>
        /// <param name="longestPath">Longest path found so far</param>
        private void depthFirstSearch(Edge ed, Location nodeFrom, List<Edge> network, int length, List<Edge> visited, List<Edge> currentPath, List<Edge> longestPath)
        {
            currentPath.Add(ed);

            // Where to go first, don't backtrack to starting position
            Location nodeTo = nodeFrom == ed.node1 ? ed.node2 : ed.node1;
            visited.Add(ed);

            // Find new edges to go to in next step of algorithm
            List<Edge> edgeNeighbours = new();
            for (int i = 0; i < network.Count; i++)
            {
                if (!visited.Contains(network[i]))
                {
                    if (network[i].node1 == nodeTo)
                        edgeNeighbours.Add(network[i]);

                    if (network[i].node2 == nodeTo)
                        edgeNeighbours.Add(network[i]);
                }
            }

            // keep track of longest path found so far
            if (currentPath.Count > longestPath.Count)
            {
                longestPath.Clear();
                currentPath.ForEach(item => longestPath.Add(item));
            }

            // perform depth first search on each adjacent road. 
            // T-Junction with 0,1 or 2 options depeding on whether near edge of the board or some roads already visited.
            for (int i = 0; i < edgeNeighbours.Count; i++)
                depthFirstSearch(edgeNeighbours[i], nodeTo, network, length + 1, visited, currentPath, longestPath);
            visited.Remove(ed);
            currentPath.Remove(ed);
        }

    }
}