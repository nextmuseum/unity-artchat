using UnityEngine;

[System.Serializable]
public class Exhibition
{
    public string title;
    public string description;
    public string imageSrc;
    public string beginning;
    public string end;
    public string museum;
    public string city;
    public string _id;
    public string date;

    public Exhibition(
        string title,
        string description,
        string imageSrc,
        string beginning,
        string end,
        string museum,
        string city,
        string _id,
        string date)
    {
        this.title = title;
        this.description = description;
        this.imageSrc = imageSrc;
        this.beginning = beginning;
        this.end = end;
        this.museum = museum;
        this.city = city;
        this._id = _id;
        this.date = date;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
