using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ExhibitionPage : MonoBehaviour
{
    public GameObject ItemTemplate;

    public GameObject ListView;
    public GameObject EmptyView;

    private User userData;
    private UserActivity[] userActivities;

    private List<Exhibition> exhibitions;
    private List<GameObject> items = new List<GameObject>();

    void Start()
    {
        #region Test Data
        List<Exhibition> exhibitionData = new List<Exhibition>();
        exhibitionData.Add(new Exhibition(
            "#cute-Inseln der Gl�ckseligkeit",
            "Die Neue Ausstellung 1",
            "https://picsum.photos/200/300",
            "10.10.2020",
            "11.11.2021",
            "NRW Forum",
            "Doedeldorf",
            "id-123123123123",
            "09.10.2020"));

        exhibitionData.Add(new Exhibition(
            "Vergangenheit Heute",
            "Die Neue Ausstellung 2",
            "https://picsum.photos/400/300",
            "10.10.2020",
            "11.11.2021",
            "Strauch Museum",
            "K�ln",
            "id-123123123123",
            "09.10.2020"));

        exhibitionData.Add(new Exhibition(
            "Willkommen Forum",
            "Die Neue Ausstellung 3",
            "https://picsum.photos/400/800",
            "10.10.2020",
            "11.11.2021",
            "Strauch Museum",
            "K�ln",
            "id-123123123123",
            "09.10.2020"));
        #endregion

        //UpdateData(exhibitionData);
        //ClearList();
        //LoadList();
    }

    public void SetupPage(User userData)
    {
        this.userData = userData;
        LoadExhibitionPage();
    }

    public async void LoadExhibitionPage()
    {
        exhibitions = new List<Exhibition>();

        //Load User Activity
        if (userData == null) return;
        userActivities = await APIManager.GetUserActivity(userData.userId);

        //Load Exhibitions
        if (userActivities.Length != 0)
        {
            foreach (UserActivity userAct in userActivities)
            {
                Exhibition exh = await APIManager.GetExhibition(userAct.exhibitionId);
                if (exh != null) exhibitions.Add(exh);
            }
        }

        //Give Array to Exhibition ListView to Render
        UpdateData(exhibitions);
        ClearList();
        LoadList();
    }

    public void UpdateData(List<Exhibition> exhibitionData)
    {
        exhibitions = exhibitionData;
    }

    public void LoadList()
    {
        if (exhibitions.Count == 0)
        {
            ShowEmptyView();
            return;
        }

        GameObject currentItem;
        for (int i = 0; i < exhibitions.Count; i++)
        {
            currentItem = Instantiate(ItemTemplate, ListView.transform);

            currentItem.transform.Find("Text - Museum").GetComponent<TextMeshProUGUI>().text = exhibitions[i].museum;
            currentItem.transform.Find("Text - Exhibition").GetComponent<TextMeshProUGUI>().text = exhibitions[i].title;
            currentItem.transform.Find("Text - Date").GetComponent<TextMeshProUGUI>().text = $"{exhibitions[i].beginning} - {exhibitions[i].end}";

            currentItem.GetComponent<Button>().AddEventListener(i, ItemClicked);

            items.Add(currentItem);

            StartCoroutine(LoadImage(i));
        }

        ShowListView();
    }

    public void ClearList()
    {
        foreach (GameObject item in items) Destroy(item);
        items = new List<GameObject>();
    }

    public void ShowListView()
    {
        ListView.SetActive(true);
        EmptyView.SetActive(false);
    }

    public void ShowEmptyView()
    {
        ListView.SetActive(false);
        EmptyView.SetActive(true);
    }

    IEnumerator LoadImage(int itemIndex)
    {
        GameObject itemBackground = items[itemIndex].transform.Find("Image - Background").gameObject;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(exhibitions[itemIndex].imageSrc);

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
        return tex.width / (float) tex.height;
    }

    void ItemClicked(int itemIndex)
    {
        var _AppManager = GameObject.Find("AppManager").GetComponent<AppManager>();
        _AppManager.LoadArtworkPage(userActivities[itemIndex]);
    }
}
