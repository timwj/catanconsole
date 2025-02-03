namespace CatanConsole
{
    /// <summary>
    /// Stack of 25 development cards
    /// </summary>
    public class CardStack
    {
        List<DevelopmentCard> cards;

        public CardStack()
        {
            makeStack();
        }

        public void makeStack()
        {
            cards = new();
            for (int i = 0; i < 14; i++)
            {
                cards.Add(new DevelopmentCard("knight"));
            }
            for (int i = 0; i < 5; i++)
            {
                cards.Add(new DevelopmentCard("victorypoint"));
            }
            for (int i = 0; i < 2; i++)
            {
                cards.Add(new DevelopmentCard("roadbuilding"));
            }
            for (int i = 0; i < 2; i++)
            {
                cards.Add(new DevelopmentCard("yearofplenty"));
            }
            for (int i = 0; i < 2; i++)
            {
                cards.Add(new DevelopmentCard("monopoly"));
            }
        }

        public override string ToString()
        {
            string stackList = "";
            for (int i = 0; i < cards.Count; i++)
            {
                stackList += $"{i}: {cards[i]}\n";
            }
            return stackList;
        }

        public void shuffleCards()
        {
            Random rng = new Random();
            cards = cards.OrderBy(a => rng.Next()).ToList();
        }

        /// <summary>
        /// Take 1 card from the top of the stack
        /// </summary>
        public DevelopmentCard pullCard()
        {
            if (cards.Count() == 0)
            {
                makeStack();
                shuffleCards();
            }
            DevelopmentCard pulled;
            pulled = cards[0];
            cards.RemoveAt(0);
            return pulled;

        }

    }
}