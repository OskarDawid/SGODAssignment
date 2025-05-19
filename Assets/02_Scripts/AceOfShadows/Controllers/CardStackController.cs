using System.Collections;
using UnityEngine;

public class CardStackController : MonoBehaviour
{
    public CardStackModel model;
    public CardStackView view;
    public int totalCards = 144;
    public float waitForNextCardSeconds = 1f;

    void Start()
    {
        model = new CardStackModel(totalCards);
        view.CreateInitialStack(model.TotalCards);
        view.UpdateCounters(model.CardsInStackA, model.CardsInStackB);
        view.HideCompletionMessage();
        StartCoroutine(MoveCards());
    }

    IEnumerator MoveCards()
    {
        while (model.HasCardsInStackA())
        {
            model.MoveCardToStackB();
            view.MoveCardToStackB(model.CardsInStackB);
            view.UpdateCounters(model.CardsInStackA, model.CardsInStackB);
            yield return new WaitForSeconds(waitForNextCardSeconds);
        }

        view.ShowCompletionMessage();
    }
}
