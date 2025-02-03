namespace CatanConsole
{
    /// <summary>
    /// Same as the point structure from System.Drawing but y and x are in correct order in constructor here
    /// </summary>
    /// <value></value>
    public record Location
    {
        public int y;
        public int x;

        public Location(int y, int x)
        {
            this.y = y;
            this.x = x;
        }
    }
}