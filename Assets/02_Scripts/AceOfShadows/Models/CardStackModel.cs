public class CardStackModel
{
    public int TotalCards { get; private set; }
    public int CardsInStackA { get; private set; }
    public int CardsInStackB { get; private set; }

    public CardStackModel(int totalCards)
    {
        TotalCards = totalCards;
        CardsInStackA = totalCards;
        CardsInStackB = 0;
    }

    public void MoveCardToStackB()
    {
        if (CardsInStackA > 0)
        {
            CardsInStackA--;
            CardsInStackB++;
        }
    }

    public bool HasCardsInStackA()
    {
        return CardsInStackA > 0;
    }
}
