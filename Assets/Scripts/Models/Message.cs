using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public class Message
{
    public string text;

    public string artworkId;
    public string commentId;
    public string userId;
    public string userName;
    public string _id;
    public string date;

    public Report[] reports;

    public Message(string text)
    {
        this.text = text;
    }

    public Message(string _text, string _artworkId, string _commentId, string _userId, string _userName, string __id, string _date, Report[] _reports)
    {
        this.text = _text;
        this.artworkId = _artworkId;
        this.commentId = _commentId;
        this.userId = _userId;
        this.userName = _userName;
        this._id = __id;
        this.date = _date;
        this.reports = _reports;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }

    public bool reportedByUserId(string userId)
    {
        foreach (Report report in reports)
            if (report.userId.Equals(userId))
                return true;
        return false;
    }

    public void removeReportedByUserId(string userId)
    {
        List<Report> reports = new List<Report>();

        foreach (Report report in reports)
            if (!report.userId.Equals(userId))
                reports.Add(report);

        this.reports = reports.ToArray();
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
        foreach (Report report in reports)
            if (report.userId.Equals(userId))
                return report._id;
        return "";
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
