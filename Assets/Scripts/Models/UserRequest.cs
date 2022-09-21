using UnityEngine;

[System.Serializable]
public class UserRequest
{
    public string username;
    public string password;

    public UserRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
