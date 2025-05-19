using UnityEngine;
using UnityEngine.UI;
using static Events;

public class PhoenixFlameController : MonoBehaviour, IGameController
{
    [SerializeField]
    private Button backButton;

    [SerializeField]
    private Button colorChangeButton;
    [SerializeField]
    private Animator flameAnimator;

    void OnEnable()
    {
        if (colorChangeButton != null)
        {
            colorChangeButton.onClick.AddListener(OnColorChangeClicked);
        }

        backButton.onClick.AddListener(ExitScene);
    }

    private void OnColorChangeClicked()
    {
        if (flameAnimator != null)
        {
            colorChangeButton.interactable = false;
            flameAnimator.SetTrigger("ColorChange");

            float animationDuration = GetCurrentAnimationLength("FlameAnimation");

            DelayedCall.Instance.AddTimer(() => {
                colorChangeButton.interactable = true;
            }, animationDuration);
        }
    }

    private float GetCurrentAnimationLength(string animationName)
    {
        AnimatorStateInfo stateInfo = flameAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName))
        {
            return stateInfo.length;
        }

        return flameAnimator.runtimeAnimatorController.animationClips[0].length;
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        CancelInvoke();

        if (colorChangeButton != null)
        {
            colorChangeButton.interactable = true;
            colorChangeButton.onClick.RemoveAllListeners();
        }

        if (flameAnimator != null)
        {
            flameAnimator.Rebind();
            flameAnimator.Update(0f);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
    }

    public void ExitScene()
    {
        Cleanup();
        EventManager.Broadcast(new EvGameSceneClosed());
    }
}
