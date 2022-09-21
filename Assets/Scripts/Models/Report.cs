using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Report 
{
    public string _id;
    public string commentId;
    public string userId;

    public Report(string _id, string commentId, string userId)
    {
        this._id = _id;
        this.commentId = commentId;
        this.userId = userId;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

