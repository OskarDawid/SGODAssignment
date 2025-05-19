using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Events;

public class AceOfShadowsController : MonoBehaviour, IGameController
{
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private CardStackModel model;
    [SerializeField]
    private CardStackView aceOfShadowsView;
    [SerializeField]
    private int totalCards = 144;
    [SerializeField]
    private float waitForNextCardSeconds = 1f;


    void OnEnable()
    {
        model = new CardStackModel(totalCards);
        aceOfShadowsView.CreateInitialStack(model.TotalCards);
        aceOfShadowsView.UpdateCounters(model.CardsInStackA, model.CardsInStackB);
        aceOfShadowsView.HideCompletionMessage();
        StartCoroutine(MoveCards());

        backButton.onClick.AddListener(ExitScene);
    }

    IEnumerator MoveCards()
    {
        while (model.HasCardsInStackA())
        {
            model.MoveCardToStackB();
            aceOfShadowsView.MoveCardToStackB(model.CardsInStackB);
            aceOfShadowsView.UpdateCounters(model.CardsInStackA, model.CardsInStackB);
            yield return new WaitForSeconds(waitForNextCardSeconds);
        }

        aceOfShadowsView.ShowCompletionMessage();
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        CancelInvoke();

        backButton.onClick.RemoveAllListeners();

        aceOfShadowsView.UpdateCounters(0, 0);
        aceOfShadowsView.ClearScene();
    }

    public void ExitScene()
    {
        Cleanup();
        EventManager.Broadcast(new EvGameSceneClosed());
    }
}
