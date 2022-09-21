using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserActivityRequest
{
    public string exhibitionId;
    public string artwork;

    public UserActivityRequest(string exhibitionID, string artwork)
    {
        this.exhibitionId = exhibitionID;
        this.artwork = artwork;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}
