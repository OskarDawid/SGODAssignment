using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueView : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text messageText;
    public Image avatarImage;

    public void SetDialogue(Dialogue dialogue, Sprite avatarSprite)
    {
        nameText.text = dialogue.name;
        messageText.text = ParseTextWithEmojis(dialogue.text);

        if (avatarSprite != null)
        {
            avatarImage.sprite = avatarSprite;
            avatarImage.SetNativeSize();
            avatarImage.color = Color.white;
        }
        else
        {
            avatarImage.sprite = null;
            avatarImage.color = Color.gray;
        }
    }

    private string ParseTextWithEmojis(string text)
    {
        return text
            .Replace("{satisfied}", "\U0001F60A")
            .Replace("{intrigued}", "\U0001F914")
            .Replace("{neutral}", "\U0001F610")
            .Replace("{affirmative}", "\U0001F44D")
            .Replace("{laughing}", "\U0001F606")
            .Replace("{win}", "\U0001F3C6");
    }
}
