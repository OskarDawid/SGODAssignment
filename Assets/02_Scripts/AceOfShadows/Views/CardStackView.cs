using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardStackView : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform stackA;
    public Transform stackB;
    public TextMeshProUGUI stackACounter;
    public TextMeshProUGUI stackBCounter;
    public TextMeshProUGUI completionMessage;
    public float cardSpacing = 0.2f;
    public float cardMoveDuration = 1f;
    public float arcHeight = 100f;
    public float dropOffset = 20f;

    public void CreateInitialStack(int totalCards)
    {
        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, stackA);
            card.transform.localPosition = new Vector3(0, i * cardSpacing, 0);
        }
    }

    public void MoveCardToStackB(int cardsInStackB)
    {
        if (stackA.childCount == 0) return;

        Transform card = stackA.GetChild(stackA.childCount - 1);
        Vector3 targetPosition = new Vector3(0, cardsInStackB * cardSpacing, 0);
        Vector3 arcPosition = targetPosition + Vector3.up * arcHeight;
        Vector3 overshootPosition = targetPosition + Vector3.up * dropOffset;
        card.SetParent(stackB);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(card.DOLocalMove(arcPosition, cardMoveDuration / 2).SetEase(Ease.OutQuad));
        sequence.Append(card.DOLocalMove(overshootPosition, cardMoveDuration / 4).SetEase(Ease.OutQuad));
        sequence.Append(card.DOLocalMove(targetPosition, cardMoveDuration / 4));
        sequence.Play();
    }

    public void UpdateCounters(int cardsInStackA, int cardsInStackB)
    {
        stackACounter.text = $"Stack A: {cardsInStackA}";
        stackBCounter.text = $"Stack B: {cardsInStackB}";
    }

    public void ShowCompletionMessage()
    {
        completionMessage.text = "All animations are finished!";
        completionMessage.gameObject.SetActive(true);
    }

    public void HideCompletionMessage()
    {
        completionMessage.gameObject.SetActive(false);
    }
}
