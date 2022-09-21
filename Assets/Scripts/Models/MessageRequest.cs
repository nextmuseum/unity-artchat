using UnityEngine;

[System.Serializable]
public class MessageRequest
{
    public string text;

    public MessageRequest(string text)
    {
        this.text = text;
    }

    public MessageRequest(Message message)
    {
        this.text = message.text;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
