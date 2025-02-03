namespace CatanConsole
{
    /// <summary>
    /// For storing some data about the turn and the dice and how many players there are.
    /// </summary>
    public class GameState
    {
        // Value of the first and second dice thrown. Is (0,0) if the dice have not been thrown yet at the beginning of the game.
        public int d1;
        public int d2;
        public int totalTurns; // not used yet, maybe later.

        // Whose turn is it, from 0 to (number of players - 1). Example: If turn =0 then its Piet's turn, if its 1 then its Klaas' turn.
        public int turn;

        // This turn, has the dice already been rolled? Some actions are only allowed after a dice is rolled. Will be replaced when state machine implemented.
        public bool diceRolled;
        public int totalPlayers;

        public int longestRoadLength;

        public int playerNumLongestRoad;
        public bool placingRobber;

        public bool mainPhase;
        public CardStack cardStack;

        public GameState()
        {
            mainPhase = false;
            longestRoadLength = 0;
            playerNumLongestRoad = -1;
            turn = 0;
            d1 = 0;
            d2 = 0;
            diceRolled = false;
            totalTurns = 0;
            placingRobber = false;
            cardStack = new CardStack();
            cardStack.shuffleCards();
        }

        public GameState(int d1, int d2, int totalTurns, int turn, bool diceRolled, int totalPlayers, int longestRoadLength, int playerNumLongestRoad, bool placingRobber, CardStack cardStack, bool mainPhase)
        {
            this.mainPhase = mainPhase;
            this.turn = turn;
            this.d1 = d1;
            this.d2 = d2;
            this.diceRolled = diceRolled;
            this.totalTurns = totalTurns;
            this.totalPlayers = totalPlayers;
            this.longestRoadLength = longestRoadLength;
            this.playerNumLongestRoad = playerNumLongestRoad;
            this.placingRobber = placingRobber;
            this.cardStack = cardStack;
        }
    }
}