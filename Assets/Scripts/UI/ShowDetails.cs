using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShowDetails : MonoBehaviour
{
    private bool showDetails = false;
    public void toggleDetails()
    {
        showDetails = !showDetails;

        transform.Find("Image - Background").GetComponent<RawImage>().color = 
            showDetails ? new Color32(255,255,255,100) : new Color32(255, 255, 255, 255);
        transform.Find("Text - Artist, Year").gameObject.SetActive(showDetails);
        transform.Find("Text - Title").gameObject.SetActive(showDetails);
        transform.Find("Text - Description").gameObject.SetActive(showDetails);

    }

    
}
