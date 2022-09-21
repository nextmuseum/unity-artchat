using ARtChat;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WhatsNewPage : MonoBehaviour
{
    public CommentWatcher commentWatcher;

    public GameObject emptyView;
    public GameObject ItemTemplate;
    public GameObject HeaderTemplate;
    public Transform ListView;
    public Color32 ItemHighlight;


    private List<GameObject> items;
    private User userData;

    public delegate void OnLoadFinishedDelegate();
    public OnLoadFinishedDelegate onLoadFinishedListener;

    private void Awake()
    {
        commentWatcher.onLoadItemsFinishedListener += SetUpPage;
    }

    private void OnDestroy()
    {
        commentWatcher.onLoadItemsFinishedListener -= SetUpPage;
    }

    // Start is called before the first frame update
    public void Init(User userData, bool refreshComments)
    {
        transform.GetChild(2).GetComponent<PullToRefresh>().action = new UnityEngine.Events.UnityEvent();
        transform.GetChild(2).GetComponent<PullToRefresh>().action.AddListener(() => { commentWatcher.Refresh(); });
        this.userData = userData;
        if(refreshComments)
            commentWatcher.Refresh();
    }

    // Update is called once per frame
    public void SetUpPage()
    {
        items = new List<GameObject>();

        //clear list
        if (ListView.childCount > 0)
        {
            //spare empty view and start with 1
            for (int i = 2; i < ListView.childCount; i++)
            {
                Destroy(ListView.GetChild(i).gameObject);
            }
        }

        if (commentWatcher.comments.Length == 0)
        {
            emptyView.SetActive(true);
            UIManager.toggleLoadingScreen(false);
            //_colorTable.ToggleMode();
            return;
        }



        emptyView.SetActive(false);

        int itemIndex = 0;

        foreach (KeyValuePair<string, List<Comment>> entry in commentWatcher.artworkMapper)
        {
            GameObject Header = Instantiate(HeaderTemplate, ListView);
            Header.GetComponentInChildren<TextMeshProUGUI>().text = entry.Key;

            for(int i = 0; i < entry.Value.Count; i++)
            {
                Debug.LogWarning(itemIndex);
                GameObject currentItem;
                currentItem = Instantiate(ItemTemplate, ListView);

                currentItem.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text = entry.Value[i].userName;
                //UIManager.ParseUsername(currentItem.transform.Find("Text - User").GetComponent<TextMeshProUGUI>(), comments[i].userId);
                currentItem.transform.Find("Text - Comment").GetComponent<TextMeshProUGUI>().text = entry.Value[i].text;
                UIManager.ParseDate(currentItem.transform.Find("Text - Date").GetComponent<TextMeshProUGUI>(), commentWatcher.comments[i].date);

                currentItem.transform.Find("Report").GetComponent<Image>().color = Color.white;
                //currentItem.GetComponent<Button>().AddEventListener(i, ItemClicked);

                if (entry.Value[i].messageDiff != 0)
                {
                    currentItem.transform.Find("NewMessages").GetComponentInChildren<TextMeshProUGUI>().text = entry.Value[i].messageDiff.ToString();
                    currentItem.transform.Find("NewMessages").gameObject.SetActive(true);
                }

                if (userData != null)
                {
                    int finalItemIndex = itemIndex;
                    if (!entry.Value[i].deleted)
                    {
                        currentItem.GetComponent<Button>().AddEventListener(finalItemIndex, entry.Value[i]._id, ItemClicked);
                        Debug.LogWarning("Select item with Index " + finalItemIndex);
                        currentItem.transform.Find("Report").GetComponent<Button>().AddEventListener(finalItemIndex, entry.Value[i]._id, currentItem, ReportItem);
                        Debug.LogWarning("Report item with Index " + finalItemIndex);
                    }
                    else
                    {
                        currentItem.transform.Find("Report").gameObject.SetActive(false);
                        currentItem.transform.Find("Delete").gameObject.SetActive(false);
                    }

                    if (entry.Value[i].userId == userData.userId)//Ist es dein kommentar
                    {
                        currentItem.GetComponent<Image>().color = ItemHighlight;
                        currentItem.transform.Find("Delete").GetComponent<Button>().AddEventListener(finalItemIndex, entry.Value[i]._id, DeleteItem);
                        Debug.LogWarning("Delete item with Index " + finalItemIndex);
                        //currentItem.transform.Find("Report").GetComponent<Button>().AddEventListener(i, ReportItem);
                        currentItem.transform.Find("Delete").gameObject.SetActive(true);
                    }
                    if (entry.Value[i].reportedByUserId(userData.userId))
                    {
                        currentItem.transform.Find("Report").GetComponent<Image>().color = new Color(221f / 255f, 1f, 6f / 255f);
                    }

                }

                items.Add(currentItem);
                itemIndex++;
            }
        }
        onLoadFinishedListener?.Invoke();

        UIManager.toggleLoadingScreen(false);
    }

    async void DeleteItem(int itemIndex, string itemId)
    {
        if (await APIManager.DeleteComment(commentWatcher.getCommentById(itemId)._id))
        {
            Destroy(items[itemIndex]);
            items.Remove(items[itemIndex]);
            UserSession.removeComment(commentWatcher.getCommentById(itemId), userData.userId);

        }
    }

    void ItemClicked(int itemIndex, string itemId)
    {
        Debug.Log("Open Chat(MessagePage) Of Selected item: " + itemIndex);
        //UIManager.toggleLoadingScreen(true);
        AppManager _AppManager = GameObject.Find("AppManager").GetComponent<AppManager>();
        UserSession.addComment(commentWatcher.getCommentById(itemId), userData.userId, true);
        items[itemIndex].transform.Find("NewMessages").gameObject.SetActive(false);
        _AppManager.LoadMessagePage(commentWatcher.getCommentById(itemId),false);

    }

    async void ReportItem(int itemIndex, string itemId, GameObject currentItem)
    {
        //wenn userId in liste der reports drin ist -> dann delete request starten, sonst PUT

        if (commentWatcher.getCommentById(itemId).reportedByUserId(userData.userId))
        {
            Debug.Log("Delete Report");
            currentItem.transform.Find("Report").GetComponent<Image>().color = Color.white;
            string reportId = commentWatcher.getCommentById(itemId).getReportById(userData.userId);
            if (reportId != "")
            {
                if (await APIManager.DeleteReport(reportId))
                {
                    Debug.Log("Delete Report Complete");
                    commentWatcher.getCommentById(itemId).removeReportedByUserId(userData.userId);
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
            Report report = await APIManager.PutReportComment(commentWatcher.getCommentById(itemId)._id, userData.userId);
            if (report != null)
            {
                Debug.Log("Put Report Complete");
                commentWatcher.getCommentById(itemId).addReportedByUserId(report);
            }
            else
            {
                currentItem.transform.Find("Report").GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void ClearList()
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
}

