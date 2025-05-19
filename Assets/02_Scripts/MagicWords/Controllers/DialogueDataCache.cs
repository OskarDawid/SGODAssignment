using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class DialogueDataCache
{
    private static DialogueModel cachedModel;
    private static Dictionary<string, Sprite> cachedAvatars = new Dictionary<string, Sprite>();
    private static bool isInitialized = false;

    public static bool IsInitialized => isInitialized;
    public static DialogueModel CachedModel => cachedModel;
    public static Dictionary<string, Sprite> CachedAvatars => cachedAvatars;

    public static IEnumerator Initialize(string dataUrl, Sprite defaultAvatar)
    {
        if (isInitialized)
            yield break;

        using (UnityWebRequest request = UnityWebRequest.Get(dataUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                cachedModel = new DialogueModel();
                cachedModel.LoadData(request.downloadHandler.text);
                yield return PreloadAvatars(cachedModel.Data.avatars, defaultAvatar);
                isInitialized = true;
                Debug.Log("Dialogue data loaded and cached.");
            }
            else
            {
                Debug.LogError("Error loading data: " + request.error);
            }
        }
    }

    private static IEnumerator PreloadAvatars(List<Avatar> avatars, Sprite defaultAvatar)
    {
        HashSet<string> loadedUrls = new HashSet<string>();

        foreach (var avatar in avatars)
        {
            if (cachedAvatars.ContainsKey(avatar.url) || loadedUrls.Contains(avatar.url))
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
                    loadedUrls.Add(avatar.url);
                }
                else
                {
                    Debug.LogWarning($"Cannot load avatar: {avatar.url}");
                }
            }
        }
    }

    public static Avatar GetFirstValidAvatar(string characterName, Sprite defaultAvatar)
    {
        List<Avatar> matchingAvatars = cachedModel.GetAllAvatarsForCharacter(characterName);
        foreach (var avatar in matchingAvatars)
        {
            if (avatar.url != "default" && !cachedAvatars.ContainsKey(avatar.url))
                continue;

            return avatar;
        }

        return new Avatar { name = characterName, url = "default" };
    }


    public static Sprite GetAvatarSprite(Avatar avatar, Sprite defaultAvatar)
    {
        return avatar.url == "default" ? defaultAvatar : cachedAvatars.GetValueOrDefault(avatar.url, defaultAvatar);
    }
}