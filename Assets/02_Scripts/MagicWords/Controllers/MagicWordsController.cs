using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Events;

public class MagicWordsController : MonoBehaviour, IGameController
{
    [SerializeField]
    private Button backButton;

    [Header("References")]
    public DialogueView leftPanel;
    public DialogueView rightPanel;
    public Sprite defaultAvatar;

    private bool leftPanelActive = false;
    private bool rightPanelActive = false;

    void OnEnable()
    {
        InitializePanels();

        if (!DialogueDataCache.IsInitialized)
        {
            StartCoroutine(WaitForDataAndDisplayDialogue());
        }
        else
        {
            StartCoroutine(DisplayDialogue());
        }

        backButton.onClick.AddListener(ExitScene);
    }

    IEnumerator WaitForDataAndDisplayDialogue()
    {
        while (!DialogueDataCache.IsInitialized)
        {
            yield return null;
        }

        StartCoroutine(DisplayDialogue());
    }

    void InitializePanels()
    {
        leftPanel.gameObject.SetActive(false);
        rightPanel.gameObject.SetActive(false);
        leftPanelActive = false;
        rightPanelActive = false;
    }

    IEnumerator DisplayDialogue()
    {
        foreach (var dialogue in DialogueDataCache.CachedModel.Data.dialogue)
        {
            Avatar selectedAvatar = DialogueDataCache.GetFirstValidAvatar(dialogue.name, defaultAvatar);
            Sprite avatarSprite = DialogueDataCache.GetAvatarSprite(selectedAvatar, defaultAvatar);
            DialogueView activePanel = GetActivePanel(selectedAvatar.position);

            if (activePanel == leftPanel && !leftPanelActive)
            {
                leftPanel.gameObject.SetActive(true);
                leftPanelActive = true;
            }
            else if (activePanel == rightPanel && !rightPanelActive)
            {
                rightPanel.gameObject.SetActive(true);
                rightPanelActive = true;
            }

            activePanel.SetDialogue(dialogue, avatarSprite);
            yield return new WaitForSeconds(2f);
        }
    }

    DialogueView GetActivePanel(string position)
    {
        return position == "left" ? leftPanel : rightPanel;
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        CancelInvoke();
        backButton.onClick.RemoveAllListeners();
        InitializePanels();
    }

    public void ExitScene()
    {
        Cleanup();
        EventManager.Broadcast(new EvGameSceneClosed());
    }
}
