using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Comment
{
    public string text;
    public Vector3 position;
    public Vector3 rotation;

    public string artworkId;
    public string userId;
    public string userName;
    public string _id;
    public string date;

    public bool isOwner;

    //helper variables for WhatsNewPage
    public int messageCount;
    public int messageDiff = 0;
    public bool deleted = false;
    public string artworkName = "";

    public Report[] reports;


    public Comment(
        string text,
        Vector3 position,
        Vector3 rotation)
    {
        this.text = text;
        this.position = position;
        this.rotation = rotation;
        this.artworkName = "";
    }

    public Comment(
        string text,
        Vector3 position,
        Vector3 rotation,
        string artworkID,
        string userID,
        string userName,
        string _id,
        string date,
        Report[] reports) : this(text, position, rotation)
    {
        this.artworkId = artworkID;
        this.userId = userID;
        this.userName = userName;
        this._id = _id;
        this.date = date;
        this.reports = reports;
    }

    public Comment(
    string text,
    Vector3 position,
    Vector3 rotation,
    string artworkID,
    string userID,
    string userName,
    string _id,
    string date,
    int _messageCount,
    Report[] reports) : this(text, position, rotation, artworkID, userID, userName, _id, date, reports)
    {
        this.messageCount = _messageCount;
    }

    //bugy aus den playerprefs zu ziehen...
    public Comment(JSONNode commentJSON)
    {
        this.text = commentJSON["text"].Value;
        this.position = new Vector3(commentJSON["position"]["x"].AsFloat, commentJSON["position"]["y"].AsFloat, commentJSON["position"]["z"].AsFloat);
        this.rotation = new Vector3(commentJSON["rotation"]["x"].AsFloat, commentJSON["rotation"]["y"].AsFloat, commentJSON["rotation"]["z"].AsFloat); ;
        this.artworkId = commentJSON["artworkId"].Value;
        this.userId = commentJSON["userId"].Value;
        this.userName = commentJSON["userName"].Value;
        this._id = commentJSON["_id"].Value;
        this.date = commentJSON["date"].Value;
        this.artworkName = commentJSON["artworkName"].Value;

        if (!commentJSON["isOwner"].IsNull)
            this.isOwner = commentJSON["isOwner"].AsBool;
        if (!commentJSON["messageCount"].IsNull)
            this.messageCount = commentJSON["messageCount"].AsInt;
        if (!commentJSON["artworkName"].IsNull)
            this.artworkName = commentJSON["artworkName"].Value;
    }

    public bool reportedByUserId(string userId)
    {
        if (reports != null) {
            foreach (Report report in reports)
                if (report.userId.Equals(userId))
                    return true;
        }
        return false;
    }

    public void removeReportedByUserId(string userId)
    {
        if (reports != null)
        {
            List<Report> reports = new List<Report>();
            foreach (Report report in reports)
                if (!report.userId.Equals(userId))
                    reports.Add(report);

            this.reports = reports.ToArray();
        }
    }

    public void addReportedByUserId(Report _report)
    {
        List<Report> reports = new List<Report>();

        foreach (Report report in reports)
                reports.Add(report);

        reports.Add(_report);

        this.reports = reports.ToArray();
    }

    public string getReportById(string userId)
    {
        if (reports != null)
        {
            foreach (Report report in reports)
                if (report.userId.Equals(userId))
                    return report._id;
        }
        return "";
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
    
    
    public string GetHash()
    {
        using(SHA256 hash = SHA256.Create())
        {
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(text + date + userId));
            
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
