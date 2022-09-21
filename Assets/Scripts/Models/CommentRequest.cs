using UnityEngine;

[System.Serializable]
public class CommentRequest
{
    public string text;
    public Vector3 position;
    public Vector3 rotation;

    public CommentRequest(string text, Vector3 position, Vector3 rotation)
    {
        this.text = text;
        this.position = position;
        this.rotation = rotation;
    }

    public CommentRequest(Comment comment)
    {
        this.text = comment.text;
        this.position = comment.position;
        this.rotation = comment.rotation;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
