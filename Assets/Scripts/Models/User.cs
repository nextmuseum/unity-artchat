using UnityEngine;

[System.Serializable]
public class User
{
    public string accessToken;
    public string accessExpires;
    public string refreshToken;
    public string userId;
    public string nickname;
    public bool emailVerified;

    public string nickNameSuggestion;

    public User(string accessToken, string userID, string accessExpires, string refreshToken, string nickname, string nickNameSuggestion, bool emailVerified)
    {
        this.accessToken = accessToken;
        this.userId = userID;
        this.accessExpires = accessExpires;
        this.refreshToken = refreshToken;
        this.nickname = nickname;
        this.nickNameSuggestion = nickNameSuggestion;
        this.emailVerified = emailVerified;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
