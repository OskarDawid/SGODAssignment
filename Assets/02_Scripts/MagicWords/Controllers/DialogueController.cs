using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DialogueController : MonoBehaviour
{
    [Header("References")]
    public DialogueView leftPanel;
    public DialogueView rightPanel;
    public Sprite defaultAvatar;
    public string dataUrl = "https://private-624120-softgamesassignment.apiary-mock.com/v3/magicwords";
    private DialogueModel model;
    private Dictionary<string, Sprite> loadedAvatars = new Dictionary<string, Sprite>();
    private bool leftPanelActive = false;
    private bool rightPanelActive = false;

    void Start()
    {
        model = new DialogueModel();
        leftPanel.gameObject.SetActive(false);
        rightPanel.gameObject.SetActive(false);
        StartCoroutine(InitializeDialogue());
    }

    IEnumerator InitializeDialogue()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(dataUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                model.LoadData(json);

                yield return StartCoroutine(PreloadAvatars(model.Data.avatars));

                foreach (var dialogue in model.Data.dialogue)
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
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                Debug.LogError("Error loading data: " + request.error);
            }
        }
    }

    IEnumerator PreloadAvatars(List<Avatar> avatars)
    {
        HashSet<string> loadedUrls = new HashSet<string>();

        foreach (var avatar in avatars)
        {
            if (loadedUrls.Contains(avatar.url) || loadedAvatars.ContainsKey(avatar.url))
                continue;

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(avatar.url))
            {
                request.timeout = 2;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    loadedAvatars[avatar.url] = sprite;
                }
                else
                {
                    Debug.LogWarning($"Cannot load avatar: {avatar.url}");
                }

                loadedUrls.Add(avatar.url);
            }
        }
    }

    Avatar GetFirstValidAvatar(string characterName)
    {
        List<Avatar> matchingAvatars = model.GetAllAvatarsForCharacter(characterName);
        foreach (var avatar in matchingAvatars)
        {
            if (loadedAvatars.ContainsKey(avatar.url))
            {
                return avatar;
            }
        }

        return new Avatar { name = characterName, url = "default", position = "right" };
    }

    Sprite GetAvatarSprite(Avatar avatar)
    {
        if (avatar.url == "default")
            return defaultAvatar;

        return loadedAvatars.ContainsKey(avatar.url) ? loadedAvatars[avatar.url] : defaultAvatar;
    }

    DialogueView GetActivePanel(string position)
    {
        return position == "left" ? leftPanel : rightPanel;
    }
}
