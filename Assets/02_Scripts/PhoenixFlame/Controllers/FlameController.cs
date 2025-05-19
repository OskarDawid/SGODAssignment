using UnityEngine;
using UnityEngine.UI;

public class FlameController : MonoBehaviour
{
    public Button colorChangeButton;
    public Animator flameAnimator;

    void Start()
    {
        if (colorChangeButton != null)
        {
            colorChangeButton.onClick.RemoveAllListeners();
            colorChangeButton.onClick.AddListener(OnColorChangeClicked);
        }
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
}
