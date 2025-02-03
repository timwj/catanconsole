using Newtonsoft.Json;
namespace CatanConsole
{
    /// <summary>
    /// Models one of the 3-5 players of the game. Each player has resources, a name and a number
    /// </summary>
    public class Player
    {
        public int number;
        public string name;

        public ResourceClass resources = new ResourceClass(); // initialises resources to 0

        // Keeps track of the resources the player has gained from the latest dice roll
        public ResourceClass resourcesThisTurn = new ResourceClass();

        public int nVillages = 0;  // number of villages this player has built
        public int nCities = 0; // number of cities this player has built
        public int nRoads = 0; // number of roads this player has built

        public int knightCards = 0;
        public int victorypointCards = 0;
        public int roadbuildingCards = 0;
        public int yearofplentyCards = 0;
        public int monopolyCards = 0;

        public int victorypointsFromCards = 0;
        public int knightCardsPlayed = 0;

        public List<harborType> harbors = new();

        // [JsonIgnore]
        public List<Edge> playersLongestRoad = new();

        public Player(string name) => this.name = name;

        public bool longestRoadVP = false;
        public bool largestArmyVP = false;

        public int victoryPoints
        {
            get { return nVillages * 1 + nCities * 2 + (longestRoadVP ? 2 : 0) + victorypointsFromCards + (largestArmyVP ? 2 : 0); } // roads are not worth victorypoints
        }
        public int longestRoad
        {
            get { return playersLongestRoad.Count; }
        }

        // For initialising through Json deserialise, not sure if really neccesary
        public Player() { }
    }
}

