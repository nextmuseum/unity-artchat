using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEditor;
using ZXing;

public class CommentPage : MonoBehaviour
{
    public GameObject ItemTemplate;
    public Color32 ItemHighlight;
    public GameObject ListView;
    public GameObject EmptyView;
    public GameObject ArtworkHeader;

    private User userData;
    private Artwork artworkData;
    private List<Comment> comments;
    private List<GameObject> items = new List<GameObject>();

    private bool ShowArtworkDetails = false;

    // Start is called before the first frame update
    void Start()
    {
        /*
        //  Color Manager
        _colorTable = GameObject.Find("ColorManager").GetComponent<colorTable>();
        #region Test Data
        Comment[] commentData = new Comment[3];
        commentData[0] = new Comment(
            "Oh ja, das ist schon sehr cut�.",
            new Vector3(7, 874, 124),
            new Vector3(45, 45, 45),
            "artworkID",
            "userID:Miriam",
            "_id",
            "23.04.2021");
        commentData[1] = new Comment(
            "Das ist doch ein Fisch?!",
            new Vector3(6, 812, 926),
            new Vector3(45, 45, 45),
            "artworkID",
            "userID:Michael",
            "_id",
            "24.04.2021");
        commentData[2] = new Comment(
            "Ziemlich interessant, wenn man bedenk dass ...",
            new Vector3(6, 812, 926),
            new Vector3(45, 45, 45),
            "artworkID",
            "ID_1234",
            "_id",
            "25.04.2021");

        User testUserData = new User("TOKEN_1234","ID_1234","01-01-2022","REFRESH_TOKEN","Fabz","Fabian");

        Artwork testArtworkData = new Artwork(
            "Maria Dudelsause",
            "Blau wie der Ozean",
            "Bliblablubks jafgjwhegfjudeguq wdiuwdhnduw ef wefzgwef uzegf weuf wuegfz usfguwegf wufgwf sdfus gadiugf we",
            "1772",
            "https://picsum.photos/200/300",
            12345,
            "343214",
            "343214",
            "11.11.2011");
        */
        //#endregion

        //UpdateUserData(testUserData);
        //UpdateArtworkData(testArtworkData);
        //UpdateData(commentData);

        //ClearList();
        //LoadHeader();
        //LoadList();
    }

    public void SetupPage(User userData, Artwork artworkData)
    {
        this.userData = userData;
        this.artworkData = artworkData;
        LoadHeader();
        LoadCommentPage();
    }

    public async void LoadCommentPage()
    {
        ClearList();
        clearHeaderImage();

        comments = new List<Comment>();

        //Load Comment
        if (artworkData != null)
        {
            CancellationToken token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            Comment[] commentRes = await APIManager.GetComment(artworkData._id, token);
            if (commentRes != null) comments = commentRes.ToList();
        }

        //Give Array to Exhibition ListView to Render
        UpdateData(comments);
        LoadList();
        UIManager.toggleLoadingScreen(false);
    }

    public void clearHeaderImage()
    {
        ArtworkHeader.transform.Find("Image - Background").gameObject.GetComponent<RawImage>().texture = null;
    }

    public void UpdateUserData(User userData)
    {
        this.userData = userData;
    }

    public void UpdateArtworkData(Artwork artworkHeader)
    {
        this.artworkData = artworkHeader;
    }

    public void UpdateData(List<Comment> commentData)
    {
        comments = commentData;
    }

    public void LoadHeader()
    {
        if (artworkData == null) return;

        ArtworkHeader.transform.Find("Exhibitionname").GetComponent<TextMeshProUGUI>().text = artworkData.exhibitionID;
        ArtworkHeader.transform.Find("Artworkname").GetComponent<TextMeshProUGUI>().text = artworkData.title;
        ArtworkHeader.transform.Find("Artistname").GetComponent<TextMeshProUGUI>().text = artworkData.artist;
        ArtworkHeader.transform.Find("Year").GetComponent<TextMeshProUGUI>().text = artworkData.year;
        ArtworkHeader.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = artworkData.description;
        //ArtworkHeader.transform.Find("DetailView/Text - Title").GetComponent<TextMeshProUGUI>().text = artworkData.title;
        
        StartCoroutine(LoadHeaderImage());
    }

    public void LoadList()
    {
        Debug.Log(comments.Count);
        if (comments.Count == 0)
        {
            ShowEmptyView();
            //_colorTable.ToggleMode();
            return;
        }

        GameObject currentItem;
        for (int i = 0; i < comments.Count; i++)
        {
            currentItem = Instantiate(ItemTemplate, ListView.transform);

            currentItem.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text = comments[i].userName;
            //UIManager.ParseUsername(currentItem.transform.Find("Text - User").GetComponent<TextMeshProUGUI>(), comments[i].userId);
            currentItem.transform.Find("Text - Comment").GetComponent<TextMeshProUGUI>().text = comments[i].text;
            UIManager.ParseDate(currentItem.transform.Find("Text - Date").GetComponent<TextMeshProUGUI>(), comments[i].date);

            currentItem.transform.Find("Report").GetComponent<Image>().color = Color.white;
            //currentItem.GetComponent<Button>().AddEventListener(i, ItemClicked);

            if (userData != null)
            {
                currentItem.GetComponent<Button>().AddEventListener(i, ItemClicked);
                currentItem.transform.Find("Report").GetComponent<Button>().AddEventListener(i, currentItem,ReportItem);

                if (comments[i].userId == userData.userId)//Ist es dein kommentar
                {
                    currentItem.GetComponent<Image>().color = ItemHighlight;
                    currentItem.transform.Find("Delete").GetComponent<Button>().AddEventListener(i, DeleteItem);
                    //currentItem.transform.Find("Report").GetComponent<Button>().AddEventListener(i, ReportItem);
                    currentItem.transform.Find("Delete").gameObject.SetActive(true);
                }
                if (comments[i].reportedByUserId(userData.userId))
                {
                    currentItem.transform.Find("Report").GetComponent<Image>().color = new Color(221f/255f, 1f, 6f/255f);
                }
                
            }

            items.Add(currentItem);
        }

        ShowListView();
    }

    public void ClearList()
    {
        foreach (GameObject item in items)
        {
            if (item != null && !item.name.ToLower().Contains("comment"))
            {
                continue;
            }
            Destroy(item);
        }
        items = new List<GameObject>();
    }

    public void ShowListView()
    {
        //ListView.SetActive(true);
        EmptyView.SetActive(false);
    }

    public void ShowEmptyView()
    {
        //ListView.SetActive(true);
        EmptyView.SetActive(true);
    }

    IEnumerator LoadHeaderImage()
    {
        GameObject itemBackground = ArtworkHeader.transform.Find("Image - Background").gameObject;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(artworkData.imageSrc);

        yield return request.SendWebRequest();

        // if (request.isNetworkError || request.isHttpError)
        if (request.result == UnityWebRequest.Result.ConnectionError 
            || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            itemBackground.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            itemBackground.GetComponent<AspectRatioFitter>().aspectRatio = GetAspecRatio(itemBackground.GetComponent<RawImage>().texture);
        }
    }

    float GetAspecRatio(Texture tex)
    {
        return tex.width / (float)tex.height;
    }

    public void ToggleArtworkDetails()
    {
        ShowArtworkDetails = !ShowArtworkDetails;

        ArtworkHeader.transform.Find("Image - Background").GetComponent<RawImage>().color =
            ShowArtworkDetails ? new Color32(255, 255, 255, 100) : new Color32(255, 255, 255, 255);
        ArtworkHeader.transform.Find("DetailView").gameObject.SetActive(ShowArtworkDetails);
    }
    
    
    void ItemClicked(int itemIndex)
    {
        Debug.Log("Open Chat(MessagePage) Of Selected item: "+itemIndex);
        //UIManager.toggleLoadingScreen(true);
        AppManager _AppManager = GameObject.Find("AppManager").GetComponent<AppManager>();
        _AppManager.LoadMessagePage(comments[itemIndex]);

    }
    
    
    
    async void DeleteItem(int itemIndex)
    {
        if(await APIManager.DeleteComment(artworkData._id, comments[itemIndex]._id))
        {
            Destroy(items[itemIndex]);
        }
    }

    async void ReportItem(int itemIndex, GameObject currentItem)
    {
        //wenn userId in liste der reports drin ist -> dann delete request starten, sonst PUT

        if (comments[itemIndex].reportedByUserId(userData.userId))
        {
            Debug.Log("Delete Report");
            currentItem.transform.Find("Report").GetComponent<Image>().color = Color.white;
            string reportId = comments[itemIndex].getReportById(userData.userId);
            if (reportId != "")
            {
                if (await APIManager.DeleteReport(reportId))
                {
                    Debug.Log("Delete Report Complete");
                    comments[itemIndex].removeReportedByUserId(userData.userId);
                }
                else
                {
                    currentItem.transform.Find("Report").GetComponent<Image>().color = new Color(221f / 255f, 1f, 6f / 255f);
                }
            }
            else
            {
                Debug.LogError("No report from user wirh id " + userData.userId + " found");
            }
        }
        else
        {
            Debug.Log("Put Report");
            currentItem.transform.Find("Report").GetComponent<Image>().color = new Color(221f / 255f, 1f, 6f / 255f);
            Report report = await APIManager.PutReportComment(comments[itemIndex]._id, userData.userId);
            if (report != null)
            {
                Debug.Log("Put Report Complete");
                comments[itemIndex].addReportedByUserId(report);
            }
            else
            {
                currentItem.transform.Find("Report").GetComponent<Image>().color = Color.white;
            }
        }      
    }
}
