namespace CatanConsole
{
    // one of the 15 hexagons on the board that produce resources
    public struct Hex
    {
        public enumHexType hexType; // for example LUMBER, DESERT, BRICK etc..
        public int number; // corresponding to dice throw


        public Hex(enumHexType hexType, int number)
        {
            this.hexType = hexType;
            this.number = number;
        }

        /// <summary>
        /// Checks if the coordinate corresponds to a valid hexagon in the board
        /// </summary>
        public static bool hexExists(Location location) => GameLogic.hexCoordinates.Contains(location);
    }
}