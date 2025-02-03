using static System.Console;
using System.Text.RegularExpressions;

namespace CatanConsole
{
    public enum enumCommand { roll, end, village, city, road, get, post, load, save, trade, quit, buycard, playcard };
    public class PlayerInput
    {
        public enumCommand command;
        public List<string> arguments;
        public bool validSyntax;

        public PlayerInput()
        {
            arguments = new List<string>();
            string[] input = ReadLine().Trim().ToLower().Split(' ');

            if (input.Length > 0 && Enum.TryParse(input[0], out enumCommand tempCmd))
            {
                command = tempCmd;
                for (int i = 1; i < input.Length; i++)
                    arguments.Add(input[i]);
                if (arguments.Count > 0) ;
                validSyntax = checkSyntax();
            }
            else
                validSyntax = false;
        }

        private bool checkSyntax()
        {
            bool valid;
            switch (command)
            {
                case enumCommand.roll:
                case enumCommand.end:
                case enumCommand.quit:
                case enumCommand.load:
                case enumCommand.save:
                case enumCommand.buycard:
                    valid = arguments.Count() == 0;
                    break;

                case enumCommand.village:
                case enumCommand.city:
                    valid = arguments.Count() == 1 && int.TryParse(arguments[0], out _);
                    break;

                case enumCommand.road:
                    valid = arguments.Count() == 1 && int.TryParse(arguments[0], out _);
                    break;

                case enumCommand.trade:
                    enumResource resource1, resource2;
                    valid = arguments.Count() == 2
                    && Enum.TryParse(arguments[0], out resource1)
                    && Enum.TryParse(arguments[1], out resource2)
                    && resource1 != resource2;
                    break;
                case enumCommand.playcard:
                    valid = arguments.Count() == 1 &&
                    (arguments[0] == "kngt" || arguments[0] == "vict" || arguments[0] == "rodb" || arguments[0] == "yeop" || arguments[0] == "mono");
                    break;
                default:
                    valid = false;
                    break;
            }
            return valid;
        }
    }
}