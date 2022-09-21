using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ArtworkPage : MonoBehaviour
{
    public GameObject ItemTemplate;
    public GameObject DividerTemplate;

    public GameObject ListView;

    private User userData;
    private UserActivity userActivity;

    private List<Artwork> artworks;
    private List<GameObject> items = new List<GameObject>();
    private List<GameObject> dividers = new List<GameObject>();

    private List<Exhibition> _currentExhibitions;

    void Start()
    {
        #region Test Data
        Artwork[] artworkData = new Artwork[3];
        artworkData[0] = new Artwork(
            "Maria Dudelsause",
            "Blau wie der Ozean",
            "Bliblablubks jafgjwhegfjudeguq wdiuwdhnduw ef wefzgwef uzegf weuf wuegfz usfguwegf wufgwf sdfus gadiugf we",
            "1800",
            "https://picsum.photos/200/300",
            12345,
            "343214",
            "343214",
            "11.11.2011");

        artworkData[1] = new Artwork(
           "Marijherththsause",
           "Blau wieargerr Ozean",
           "Bliblablubks jafgjarguq wdiuwdhnduw ef wefzgwef uzegf weuf wuegfz usfguwegf wufgwf sdfus gadiugf we",
           "1800",
            "https://picsum.photos/200/300",
            12345,
            "343214",
            "343214",
            "11.11.2011");

        artworkData[2] = new Artwork(
           "Marthgwrtlsause",
           "Blautrher Ozean",
           "Bliblablubks jafgjwhegfjudeguq wdiuwdhnduw ef wefzgwef uzegf weuf wuegfz usfguwegf wufgwf sdfus gadiugf we",
           "1800",
            "https://picsum.photos/200/300",
            12345,
            "343214",
            "343214",
            "11.11.2011");
        #endregion

        //UpdateData(artworkData);
        //ClearList();
        //LoadList();
    }

    public void SetupPage(User userData, UserActivity userActivity)
    {
        this.userData = userData;
        this.userActivity = userActivity;
        LoadArtworkPage();
    }

    public async void LoadArtworkPage()
    {
        artworks = new List<Artwork>();
        _currentExhibitions = new List<Exhibition>();
        ClearList();

        //Load Artwork
        if (userActivity.artwork.Length != 0)
        {
            ListView.transform.Find("EmptyView").gameObject.SetActive(false);

            foreach (string artworkID in userActivity.artwork)
            {
                CancellationToken token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                Artwork art = await APIManager.GetArtwork(artworkID, token);
                if (art != null) artworks.Add(art);

                await UpdateCurrentExhibitions(art);
            }
        }
        else
        {
            ListView.transform.Find("EmptyView").gameObject.SetActive(true);
        }

        //Give Array to Exhibition ListView to Render
        UpdateData(artworks);
        LoadList();
        UIManager.toggleLoadingScreen(false);
    }

    private async UniTask UpdateCurrentExhibitions(Artwork art)
    {
        bool exhibitionAlreadyInList = false;
        foreach (Exhibition exhibition in _currentExhibitions)
        {
            if (exhibition._id == art.exhibitionID)
                exhibitionAlreadyInList = true;
        }

        if (!exhibitionAlreadyInList)
        {
            Exhibition exhibition = await APIManager.GetExhibition(art.exhibitionID);
            if (exhibition != null)
            {
                _currentExhibitions.Add(exhibition);
            }
        }
    }

    public void UpdateData(List<Artwork> artworkData)
    {
        artworks = artworkData;
    }

    public void LoadList()
    {
        if (artworks.Count == 0) return;

        GameObject currentItem;
        GameObject currentDivider;
        float xFlip = -1;
            
        for (int i = 0; i < artworks.Count; i++)
        {
            currentItem = Instantiate(ItemTemplate, ListView.transform);
            currentItem.GetComponent<Button>().AddEventListener(i, ItemClicked);

            UpdateArtworkTexts(currentItem, artworks[i]);
            // if(i != artworks.Count-1)
            // {
            //     currentDivider = Instantiate(DividerTemplate, ListView.transform);
            //     currentDivider.transform.localScale = new Vector3(xFlip,1,1);
            //     xFlip = -xFlip;
            //     dividers.Add(currentDivider);
            // }

            items.Add(currentItem);

            StartCoroutine(LoadImage(i));
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(ListView.GetComponent<RectTransform>());
    }

    private void UpdateArtworkTexts(GameObject currentItem, Artwork currentArtwork)
    {
        int i;
        TMP_Text titleText = currentItem.transform.Find("Title").GetComponent<TMP_Text>();
        TMP_Text exhibition = currentItem.transform.Find("Exhibition").GetComponent<TMP_Text>();
        TMP_Text yearText = currentItem.transform.Find("Year").GetComponent<TMP_Text>();

        Exhibition exhibitionOfArtwork = _currentExhibitions.First(ce => ce._id == currentArtwork.exhibitionID);
        
        titleText.text = currentArtwork.title;
        yearText.text = currentArtwork.year;
        exhibition.text = exhibitionOfArtwork.title;
    }

    public void ClearList()
    {
        foreach (GameObject item in items) Destroy(item);
        items = new List<GameObject>();

        foreach (GameObject divider in dividers) Destroy(divider);
        dividers = new List<GameObject>();
    }

    IEnumerator LoadImage(int itemIndex)
    {
        GameObject itemBackground = items[itemIndex].transform.Find("ImageAnchor/Image - Background").gameObject;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(artworks[itemIndex].imageSrc);

        yield return request.SendWebRequest();

        // if (request.isNetworkError || request.isHttpError)
        if(request.result == UnityWebRequest.Result.ConnectionError ||
           request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            itemBackground.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            itemBackground.GetComponent<AspectRatioFitter>().aspectRatio = GetAspecRatio(itemBackground.GetComponent<RawImage>().texture);
            itemBackground.transform.parent.GetChild(1).gameObject.SetActive(false);

        }
    }

    float GetAspecRatio(Texture tex)
    {
        return tex.width / (float)tex.height;
    }

    void ItemClicked(int itemIndex)
    {
        UIManager.toggleLoadingScreen(true);
        var _AppManager = GameObject.Find("AppManager").GetComponent<AppManager>();
        _AppManager.LoadCommentPage(artworks[itemIndex]);
    }
}
