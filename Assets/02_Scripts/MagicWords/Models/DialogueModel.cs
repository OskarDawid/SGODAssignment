using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    public string text;
}

[System.Serializable]
public class Avatar
{
    public string name;
    public string url;
    public string position;
}

[System.Serializable]
public class DialogueData
{
    public List<Dialogue> dialogue;
    public List<Avatar> avatars;
}

public class DialogueModel
{
    public DialogueData Data { get; private set; }

    public void LoadData(string json)
    {
        Data = JsonUtility.FromJson<DialogueData>(json);
    }

    public List<Avatar> GetAllAvatarsForCharacter(string characterName)
    {
        List<Avatar> avatars = new List<Avatar>();

        foreach (var avatar in Data.avatars)
        {
            if (avatar.name == characterName)
            {
                avatars.Add(avatar);
            }
        }

        if (avatars.Count == 0)
        {
            avatars.Add(new Avatar
            {
                name = "Unknown",
                url = "https://via.placeholder.com/100",
                position = "left"
            });
        }

        return avatars;
    }
}
