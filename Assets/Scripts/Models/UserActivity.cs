using UnityEngine;

[System.Serializable]
public class UserActivity
{
    public string exhibitionId;
    public string[] artwork;

    public UserActivity(string exhibitionID, string[] artwork)
    {
        this.exhibitionId = exhibitionID;
        this.artwork = artwork;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
