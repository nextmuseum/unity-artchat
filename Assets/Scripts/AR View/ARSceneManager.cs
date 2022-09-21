using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Immersal;
using Immersal.AR;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    public static User userData;

    public GameObject crosshair;
    public GameObject icons;
    public GameObject addCommentButton;
    public GameObject pleaseScanImmersalText;
    public GameObject loadingPrefab;

    public GameObject messagePage;

    private static Artwork currentArtwork;
    private List<Comment> comments = new List<Comment>();

    private ARSpaceManager m_SpaceManager;
    private ARLocalizer m_Localizer;

    private GameObject loadingScreen;
    private GameObject screenSpaceCanvas;

    private CancellationTokenSource destroyCancellationTokenSource = new CancellationTokenSource();

    void Awake()
    {
        m_SpaceManager = FindObjectOfType<ARSpaceManager>();
        m_Localizer = FindObjectOfType<ARLocalizer>();
        
        if(icons != null)
            screenSpaceCanvas = icons.GetComponentInParent<Canvas>().gameObject;

        #if UNITY_EDITOR
        userData = DataManager.LoadUser();
#endif

#if !UNITY_EDITOR
        userData = DataManager.LoadUser();
        if(userData == null) SceneManager.LoadScene("App UI_OnlyLogin", LoadSceneMode.Single);
#endif
    }

    private void OnEnable()
    {
        EventManager.LoadingAppScene += OnSceneUnload;
    }
    
    private void OnDisable()
    {
        EventManager.LoadingAppScene -= OnSceneUnload;
    }

    private void OnApplicationQuit()
    {
       destroyCancellationTokenSource.Cancel(); 
       destroyCancellationTokenSource.Dispose();
       UnloadScene();
    }

    private void OnSceneUnload()
    {
       destroyCancellationTokenSource.Cancel(); 
       destroyCancellationTokenSource.Dispose();
        UnloadScene();
    }


    private async void LoginTestUser()
    {
        
        string testUsername = "Test1";
        string testPw = "test";
        userData = await APIManager.LoginUser(ARtChat.UserSession.AccessToken, testUsername, testPw, destroyCancellationTokenSource.Token);
        // Debug.Log(userData);
        if(userData == null)
        {
            //await APIManager.RegisterUser(testUsername, testPw, destroyCancellationTokenSource.Token);
            //userData = await APIManager.LoginUser(testUsername, testPw, destroyCancellationTokenSource.Token);
        }
        DataManager.SaveUser(userData);
        
    }

    public async void StartLoadingScene(string artworkID)
    {
        CancellationToken token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
        CancellationTokenSource linked =
            CancellationTokenSource.CreateLinkedTokenSource(token, destroyCancellationTokenSource.Token);
        try
        {
            await LoadScene(linked.Token, artworkID);
        }
        catch (Exception e)
        {
            Debugger.Logger.Red($"Loading Page Failed - timeout {e.Message}");
            DisableLoadingScreen();
            m_Localizer.StopLocalizing();
        }
    }
    public async UniTask LoadScene(CancellationToken token, string artworkID)
    {
        //  GET artwork ID through QR code and GET artwork from API

        // Debug.Log("Loading Scene...");

        //m_Localizer.StartLocalizing();
        Debug.Log($"Current Artwork ID = {artworkID}");
        Logger.Log($"Scanned QR-Code: {artworkID} ");
        
        crosshair.SetActive(false);
        ShowLoadingScreen();

        float startTime = Time.time;
        currentArtwork = await APIManager.GetArtwork(artworkID, token);
        // Logger.Log($"Time To load Artwork Data = {Time.time-startTime}");
        if (currentArtwork == null)
        {
            // m_QRReader.setDefaultColor();
            DisableLoadingScreen();
            crosshair.SetActive(true);
            m_Localizer.StopLocalizing();
            Debug.LogError("Artwork == null");
            return;
        }
        

        //  GET map from immersal API and load map
        //await m_SpaceManager.GetLoadMapTask(currentArtwork.mapID);
        await m_SpaceManager.LoadCurrentMap(currentArtwork.mapID);
        
        m_SpaceManager.DeactivateMap();

        await UniTask.WaitUntil(() => userData != null, cancellationToken: token);

        //  POST user activity
        await APIManager.PostUserActivity(userData.userId, new UserActivityRequest(currentArtwork.exhibitionID, currentArtwork._id), token);

        //  Deactivate crosshair
        // m_QRReader.setDefaultColor();

        //  GET comments and place in map
        startTime = Time.time;
        Comment[] serverComments = await APIManager.GetComment(currentArtwork._id, token);
        if (serverComments == null)
            comments = null;
        else
        {
            comments = serverComments.ToList();
        }
        // Logger.Log($"Time To Load Comments = {Time.time - startTime}");
        if(comments != null) foreach (Comment c in comments) m_SpaceManager.AddComment(c);

        DisableLoadingScreen();
        
        //  Activate Comment UI
        icons.SetActive(true);
        addCommentButton.SetActive(false);
        pleaseScanImmersalText.SetActive(true);
        
        
        //  Activate Comment UI
        ActivateAddingCommentAfterTrackingSuccess();
    }

    private async void ActivateAddingCommentAfterTrackingSuccess()
    {
        await UniTask.WaitUntil(() => ImmersalSDK.Instance.TrackingQuality > 0, cancellationToken:destroyCancellationTokenSource.Token);
        pleaseScanImmersalText.SetActive(false);
        addCommentButton.SetActive(true);
        m_SpaceManager.ActivateMap();
    }
    
    public async UniTask LoadScene(string artworkID, CancellationToken token)
    {
        crosshair.SetActive(false);
        // Debug.Log($"Current Artwork ID = {artworkID}");
        currentArtwork = await APIManager.GetArtwork(artworkID, token);
        if (currentArtwork == null) return;

        await m_SpaceManager.LoadCurrentMap(currentArtwork.mapID);

        await UniTask.WaitUntil(() => userData != null, cancellationToken: token);

        //  POST user activity
        await APIManager.PostUserActivity(userData.userId, new UserActivityRequest(currentArtwork.exhibitionID, currentArtwork._id), token);

        //  Deactivate crosshair

        //  GET comments and place in map
        Comment[] serverComments = await APIManager.GetComment(currentArtwork._id, token);
        if (serverComments == null)
            comments = null;
        else
        {
            comments = serverComments.ToList();
        }
        
        if(comments != null) foreach (Comment c in comments) m_SpaceManager.AddComment(c);

        //  Activate Comment UI
        icons.SetActive(true);

        StartCoroutine(PullComments());
    }

    private void ShowLoadingScreen()
    {
        loadingScreen = Instantiate(loadingPrefab, screenSpaceCanvas.transform);
    }

    private void DisableLoadingScreen()
    {
        if (loadingScreen == null)
        {
            return;
        }
        Destroy(loadingScreen); 
    }

    public void UnloadScene()
    {
        StopAllCoroutines();
        currentArtwork = null;
        comments = null;
        foreach (MovableComment mc in FindObjectsOfType<MovableComment>()) Destroy(mc.gameObject);
        m_SpaceManager.UnloadMap();
        crosshair.SetActive(true);
        icons.SetActive(false);
        pleaseScanImmersalText.SetActive(false);
    }



    public async void LoadMessagePage(Comment comment)
    {
        // await LoadingMessagePage(comment);
        messagePage.GetComponent<MessagePage>().SetupPage(comment, userData);
    }

    public async UniTask LoadingMessagePage(Comment comment)
    {
        CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
        CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
        await UniTask.WaitUntil(() => comment._id != null, cancellationToken: combined.Token);

        if (comment._id == null)
        {
            Debug.LogError("Could not Load Message Page - Please Upload Comment beforehand");
            return;
        }
        
        
        messagePage.GetComponent<MessagePage>().SetupPage(comment, userData);
    }

    public void CreateNewComment()
    {
        Transform camTransform = Camera.main.transform;

        Quaternion newRotation = Quaternion.LookRotation((camTransform.position + camTransform.forward) - camTransform.position);
        Comment comment = new Comment("New Comment...", camTransform.position + camTransform.forward, newRotation.eulerAngles);
        comment.userId = userData.userId;
        m_SpaceManager.AddComment(comment);
    }

    public static string GetArtworkID()
    {
        return currentArtwork._id;
    }

    private IEnumerator PullComments()
    {
        while (true)
        {
            yield return pullingComments().ToCoroutine();
            async UniTask pullingComments()
            {
                CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
                Comment[] serverComments = await APIManager.GetComment(currentArtwork._id, linkedSource.Token);
                if (serverComments != null)
                    CheckComments(serverComments);
            }
            ;
            yield return new WaitForSeconds(2f);
        }
    }

    private void CheckComments(Comment[] serverComments)
    {
        if (comments == null || comments.Count == 0)
        {
            comments = serverComments.ToList();
            RefreshAllCommentObjects();
            return;
        }

        int minLength = serverComments.Length;
        minLength = Math.Min(serverComments.Length, comments.Count);

        for (int i = 0; i < minLength; i++)
        {
            bool areEqual = comments[i].GetHash().Equals(serverComments[i].GetHash());
            if(areEqual)
                continue;

            comments[i] = serverComments[i];
            RefreshCommentWithIdx(i);
        }

        if (serverComments.Length > comments.Count)
        {
            for (int i = comments.Count; i < serverComments.Length; i++)
            {
                comments.Add(serverComments[i]);
                m_SpaceManager.AddComment(serverComments[i]);
            }
        }

        if (serverComments.Length < comments.Count)
        {
            for (int i = comments.Count-1; i > serverComments.Length; i--)
            {
                RemoveAtIndex(i);
                comments.RemoveAt(i);
            }
        }

    }

    private void RefreshAllCommentObjects()
    {
        foreach (MovableComment mc in FindObjectsOfType<MovableComment>()) Destroy(mc.gameObject);
        if(comments != null) foreach (Comment c in comments) m_SpaceManager.AddComment(c);
    }

    private void RefreshCommentWithIdx(int idxToClear)
    {
        m_SpaceManager.RefreshesCommentAtIdx(comments[idxToClear], idxToClear);
    }

    private void RemoveAtIndex(int idx)
    {
        m_SpaceManager.RemoveCommentWithIdx(idx);
    }
}