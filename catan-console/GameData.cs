using Newtonsoft.Json;
using System.Text;
using System.Net;
namespace CatanConsole
{
    /// <summary>
    /// Class helps with converting game data to and from json. Can write game data to disk or to a web server.
    /// Used for saving and loading games and multiplayer. 
    /// (multiplayer just sends a savegame to the webserver where other players gets it with GET request)
    /// </summary>
    class GameData
    {
        public Board board;
        public GameState gameState;
        public Player[] players;

        public GameData(Board board, GameState gameState, Player[] players)
        {
            this.board = board;
            this.gameState = gameState;
            this.players = players;
        }

        /// <summary>
        /// Load a game from a file
        /// </summary>
        public GameData(string fileName)
        {
            string loadPathString = Path.Combine("saves", fileName);
            if (!Directory.Exists(loadPathString))
            {
                throw new Exception();
            }
            try
            {
                players = JsonConvert.DeserializeObject<List<Player>>(File.ReadAllText(Path.Combine(loadPathString, "players.json"))).ToArray();

                board = JsonConvert.DeserializeObject<Board>(File.ReadAllText(Path.Combine(loadPathString, "board.json")), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                gameState = JsonConvert.DeserializeObject<GameState>(File.ReadAllText(Path.Combine(loadPathString, "gamestate.json")));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Save game to a file.
        /// </summary>
        /// <param name="saveName"></param>
        public void saveGame(string saveName)
        {
            string pathString = Path.Combine("saves", saveName);
            try
            {
                System.IO.Directory.CreateDirectory(pathString);

                string boardJson = JsonConvert.SerializeObject(board, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                File.WriteAllText(Path.Combine(pathString, "board.json"), boardJson);

                File.WriteAllText(Path.Combine(pathString, "gamestate.json"), JsonConvert.SerializeObject(gameState));

                string playerJson = JsonConvert.SerializeObject(players, Formatting.Indented);
                File.WriteAllText(Path.Combine(pathString, "players.json"), playerJson);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
