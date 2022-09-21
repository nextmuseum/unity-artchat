using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Immersal;
using UnityEngine;
using Immersal.AR;
using Immersal.REST;

public class ARSpaceManager : MonoBehaviour
{
    // [SerializeField]
    // private ARMap m_ARMap = null;
    // [SerializeField]
    // private ARSpace m_ARSpace = null;
    [SerializeField]
    public GameObject commentPrefab = null;
    [SerializeField]
    public GameObject worldSpaceCanvas = null;

    private List<Task> m_Jobs = new List<Task>();
    private int m_JobLock = 0;
    private ARMapManager mapManager;
    private ARLocalizer _localizer;

    private int initSecondsToDecay = 10;

    // private Dictionary<string, GameObject> commentIDToObject = new Dictionary<string, GameObject>();
    private List<GameObject> _comments = new List<GameObject>();
    void Awake()
    {
        _localizer = FindObjectOfType<ARLocalizer>();
        mapManager = GetComponent<ARMapManager>();
    }

    private void Start()
    {
        initSecondsToDecay = ImmersalSDK.Instance.secondsToDecayPose;
    }

    void Update()
    {
        if (m_JobLock == 1)
            return;

        if (m_Jobs.Count > 0)
        {
            m_JobLock = 1;
            RunJob(m_Jobs[0]);
        }
    }

    public void AddComment(Comment comment)
    {
        // Debug.Log(
            // $"Comments position = {comment.position} under world space canvas = {worldSpaceCanvas.transform.position} \n" +
            // $"{worldSpaceCanvas.transform.parent.name} = {worldSpaceCanvas.transform.parent.position} and {worldSpaceCanvas.transform.parent.parent.name} = {worldSpaceCanvas.transform.parent.parent.position}");
        MovableComment newMC = Instantiate(commentPrefab, comment.position, Quaternion.Euler(comment.rotation), worldSpaceCanvas.transform).GetComponent<MovableComment>();
        if (!comment.text.Equals("New Comment..."))
        {
            newMC.transform.localPosition = comment.position;
            newMC.transform.localRotation = Quaternion.Euler(comment.rotation);
        }
        newMC.InitComment(comment);
        // commentIDToObject.Add(comment._id, newMC.gameObject);
        _comments.Add(newMC.gameObject);
    }

    public void RefreshesCommentAtIdx(Comment comment, int idx)
    {
        if (idx >= _comments.Count)
            return;
        
        GameObject commentToClear = _comments[idx];
        Destroy(commentToClear);
        
        MovableComment newMC = Instantiate(commentPrefab, comment.position, Quaternion.Euler(comment.rotation), worldSpaceCanvas.transform).GetComponent<MovableComment>();
        if (!comment.text.Equals("New Comment..."))
        {
            newMC.transform.localPosition = comment.position;
            newMC.transform.localRotation = Quaternion.Euler(comment.rotation);
        }
        newMC.InitComment(comment);
        _comments.Insert(idx, newMC.gameObject);
    }

    // public void RemoveCommentWithID(string id)
    // {
    //     if (!commentIDToObject.ContainsKey(id))
    //     {
    //         Debug.Log($"Error Removing Comment; Id {id} not found");
    //         return;
    //     }
    //
    //     GameObject commentToClear = commentIDToObject[id];
    //     commentIDToObject.Remove(id);
    //     Destroy(commentToClear);
    // }

    public void RemoveCommentWithIdx(int idx)
    {
        if (idx >= _comments.Count)
            return;

        GameObject commentToClear = _comments[idx];
        _comments.Remove(commentToClear);
        Destroy(commentToClear);
    }
    
    // public void LoadMap(int jobId)
    // {
    //     JobLoadMapAsync j = new JobLoadMapAsync();
    //     j.id = jobId;
    //     j.OnResult += (SDKMapResult r) =>
    //     {
    //         if (r is SDKMapResult result && result.error == "none")
    //         {
    //             byte[] mapData = Convert.FromBase64String(result.b64);
    //             this.m_ARMap.LoadMap(mapData);
    //         }
    //     };
    //
    //     m_Jobs.Add(j.RunJobAsync());
    // }

    public void UnloadMap()
    {
        // commentIDToObject = new Dictionary<string, GameObject>();
        // m_ARMap.FreeMap();
        // if (worldSpaceCanvas != null)
        // {
            // FindObjectOfType<ARLocalizer>().Reset();
        _localizer.StopLocalizing();
        ImmersalSDK.Instance.secondsToDecayPose = 0;
            
            // Debug.Log($"world = {worldSpaceCanvas.transform.position},  its parent = {worldSpaceCanvas.transform.parent?.position}, its parent = {worldSpaceCanvas.transform.parent.parent?.position}");
        // }
        mapManager.Unload();
    }

    public async UniTask LoadCurrentMap(int mapID)
    {
        // commentIDToObject = new Dictionary<string, GameObject>();
        _localizer.StartLocalizing();
        _comments = new List<GameObject>();
        ImmersalSDK.Instance.secondsToDecayPose = initSecondsToDecay;
        worldSpaceCanvas = await mapManager.Activate(mapID);
        // Debug.Log($"ID is active {worldSpaceCanvas.transform.parent.gameObject.activeSelf}");
        
    }

    public void ActivateMap()
    {
        worldSpaceCanvas.SetActive(true);
    }
    public void DeactivateMap()
    {
        worldSpaceCanvas.SetActive(false);
    }

    // public Task GetLoadMapTask(int jobId)
    // {
    //     JobLoadMapAsync j = new JobLoadMapAsync();
    //     j.id = jobId;
    //     j.OnResult += (SDKMapResult r) =>
    //     {
    //         if (r is SDKMapResult result && result.error == "none")
    //         {
    //             byte[] mapData = Convert.FromBase64String(result.b64);
    //             this.m_ARMap.LoadMap(mapData);
    //         }
    //     };
    //
    //     return j.RunJobAsync();
    // }

    private async void RunJob(Task t)
    {
        await t;
        if (m_Jobs.Count > 0)
            m_Jobs.RemoveAt(0);
        m_JobLock = 0;
    }
}
