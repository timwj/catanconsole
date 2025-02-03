using static System.Console;
using System.Drawing;

namespace CatanConsole
{
    public enum enumResource { lumber = 0, brick = 1, wool = 2, grain = 3, iron = 4, nothing = 5 };

    public enum enumHexType { lumber = 0, brick = 1, wool = 2, grain = 3, iron = 4, desert = 5, nonExistent = 6 };

    public enum harborType { lumber = 0, brick = 1, wool = 2, grain = 3, iron = 4, threeForOne = 5, notHarbor = 6 };

    public partial class GameLogic
    {
        // To convert a (x,y) road coordinate to a 2 digit number (07,10) in docs (bord.pdf) -> 71 (in game UI)  and (2,0) => 0
        public static Dictionary<Location, int> roadPointToInt;

        // To convert a road number in the UI to a number corresponding to bord.pdf   example: 71(in UI) -> (07,10) 
        public static Dictionary<int, Location> roadIntToPoint;

        // same for building. Converts a UI coordinate to an internal coordinate. These 2 dictionaries correspond to a bijective function
        public static Dictionary<int, Location> buildingIntToPoint;
        public static Dictionary<Location, int> buildingPointToInt;
        public static Dictionary<int, Location> hexIntToPoint;
        public static Dictionary<Location, int> hexPointToInt;

        // Which resource does a hex produce
        public static Dictionary<enumHexType, enumResource> hexTypeToResource = new Dictionary<enumHexType, enumResource>()
        {
            {enumHexType.lumber, enumResource.lumber},
            {enumHexType.brick, enumResource.brick},
            {enumHexType.wool, enumResource.wool},
            {enumHexType.grain, enumResource.grain},
            {enumHexType.iron, enumResource.iron},
            {enumHexType.desert, enumResource.nothing }
        };

        public static Dictionary<harborType, enumResource> harborTypeToResource = new Dictionary<harborType, enumResource>()
        {
            {harborType.lumber, enumResource.lumber},
            {harborType.brick, enumResource.brick},
            {harborType.wool, enumResource.wool},
            {harborType.grain, enumResource.grain},
            {harborType.iron, enumResource.iron},
        };

        /// <summary>
        /// These are all the valid locations of hexes in the array. Check Bord.pdf in documentation folder.
        /// </summary>
        public static List<Location> hexCoordinates = new List<Location> {
                new Location(1,2),
                new Location(1,4),
                new Location(1,6),
                new Location(3,1),
                new Location(3,3),
                new Location(3,5),
                new Location(3,7),
                new Location(5,0),
                new Location(5,2),
                new Location(5,4),
                new Location(5,6),
                new Location(5,8),
                new Location(7,1),
                new Location(7,3),
                new Location(7,5),
                new Location(7,7),
                new Location(9,2),
                new Location(9,4),
                new Location(9,6),
        };

        /// <summary>
        /// This dictionary is a bijection between a 1 digit number in the UI and a 2 digit array index. 
        /// </summary>
        private void initRoadDicts()
        {
            roadPointToInt = new();
            int num = 0;
            for (int i = 0; i < 11; i++)
                for (int j = 0; j < 11; j++)
                    if (Road.validRoadCoordinates(new Location(i, j)))
                        roadPointToInt[new Location(i, j)] = num++;

            roadIntToPoint = roadPointToInt.ToDictionary((i) => i.Value, (i) => i.Key); // the reverse dictionary, possible because its a bijection
        }

        /// <summary>
        /// This dictionary is a bijection between a 1 digit number in the UI and a 2 digit array index. 
        /// </summary>
        private void initBuildingDicts()
        {
            buildingPointToInt = new();
            int num = 0;
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 12; j++)
                    if (Building.validBuildingCoordinates(new Location(i, j)))
                        buildingPointToInt[new Location(i, j)] = num++;


            buildingIntToPoint = buildingPointToInt.ToDictionary((i) => i.Value, (i) => i.Key); // the reverse dictionary, possible because its a bijection
        }

        /// <summary>
        /// This dictionary is a bijection between a 1 digit number in the UI and a 2 digit array index. 
        /// </summary>
        private void initHexDicts()
        {
            hexPointToInt = new();
            int num = 0;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 9; j++)
                    if (Hex.hexExists(new Location(i, j)))
                        hexPointToInt[new Location(i, j)] = num++;

            hexIntToPoint = hexPointToInt.ToDictionary((i) => i.Value, (i) => i.Key); // the reverse dictionary, possible because its a bijection
        }

    }
}