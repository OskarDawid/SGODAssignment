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
    public string dataUrl = "https://private-624120-softgamesassignment.apiary-mock.com/v3/magicwords";

    private static DialogueModel cachedModel;
    private static Dictionary<string, Sprite> cachedAvatars = new Dictionary<string, Sprite>();
    private bool leftPanelActive = false;
    private bool rightPanelActive = false;

    void OnEnable()
    {
        InitializePanels();

        if (cachedModel == null)
        {
            StartCoroutine(InitializeDialogue());
        }
        else
        {
            StartCoroutine(DisplayDialogue());
        }

        backButton.onClick.AddListener(ExitScene);
    }

    IEnumerator InitializeDialogue()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(dataUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                cachedModel = new DialogueModel();
                cachedModel.LoadData(request.downloadHandler.text);
                yield return StartCoroutine(PreloadAvatars(cachedModel.Data.avatars));
                InitializePanels();
                StartCoroutine(DisplayDialogue());
            }
            else
            {
                Debug.LogError("Error loading data: " + request.error);
            }
        }
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
        foreach (var dialogue in cachedModel.Data.dialogue)
        {
            Avatar selectedAvatar = GetFirstValidAvatar(dialogue.name);
            Sprite avatarSprite = GetAvatarSprite(selectedAvatar);
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

    IEnumerator PreloadAvatars(List<Avatar> avatars)
    {
        HashSet<string> loadedUrls = new HashSet<string>();

        foreach (var avatar in avatars)
        {
            if (cachedAvatars.ContainsKey(avatar.url))
                continue;

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(avatar.url))
            {
                request.timeout = 2;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    cachedAvatars[avatar.url] = sprite;
                }
                else
                {
                    Debug.LogWarning($"Cannot load avatar: {avatar.url}");
                }
            }
        }
    }

    Avatar GetFirstValidAvatar(string characterName)
    {
        List<Avatar> matchingAvatars = cachedModel.GetAllAvatarsForCharacter(characterName);
        foreach (var avatar in matchingAvatars)
        {
            if (cachedAvatars.ContainsKey(avatar.url))
            {
                return avatar;
            }
        }

        return new Avatar { name = characterName, url = "default", position = "right" };
    }

    Sprite GetAvatarSprite(Avatar avatar)
    {
        return avatar.url == "default" ? defaultAvatar : cachedAvatars.GetValueOrDefault(avatar.url, defaultAvatar);
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