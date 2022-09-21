using UnityEngine;

[System.Serializable]
public class Artwork
{
    public string artist;
    public string title;
    public string description;
    public string year;
    public string imageSrc;
    public int mapID;

    public string exhibitionID;
    public string _id;
    public string date;

    public Artwork(string artist, string title, string description, string year, string imageSrc, int mapID, string exhibitionID, string _id, string date)
    {
        this.artist = artist;
        this.title = title;
        this.description = description;
        this.year = year;
        this.imageSrc = imageSrc;
        this.mapID = mapID;
        this.exhibitionID = exhibitionID;
        this._id = _id;
        this.date = date;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
