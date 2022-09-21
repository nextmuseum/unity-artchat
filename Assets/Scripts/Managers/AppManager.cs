using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using ARtChat;
using ARtChat.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.ComponentModel;
using SimpleJSON;

public class AppManager : MonoBehaviour
{
    [Header("Login/Register")]
    public bool onlyLogin = false;
    public bool deleteUserOnStart = false;

    public GameObject nicknameRegister;
    public GameObject usernameLogin;
    public GameObject passwordLogin;
    public GameObject usernameRegister;
    public GameObject passwordRegister;
    public GameObject passwordConfirmRegister;
    public GameObject ErrorMessage_UsernameExists;
    public GameObject deletInProgressHint;

    [Header("Components")]
    public UIManager _UIManager;
    public ExhibitionPage _ExhibitionPage;
    public ArtworkPage _ArtworkPage;
    public CommentPage _CommentPage;
    public WhatsNewPage _WhatsNewPage;
    public GameObject messagePage;

    [Header("Prefab/ Templates")]
    public GameObject loadingTemplate;

    public GameObject internetErrorTemplate;
    
    private User userData;
    private UserActivity[] userActivity;

    private GameObject loadingInstance = null;
        
    private InternetChecker _internetChecker = new InternetChecker();
    private GameObject connectionErrorPrompt;

    private CancellationTokenSource destroyCancellationTokenSource = new CancellationTokenSource();

    private void Start()
    {
        UIManager.toggleLoadingScreen(true);
        Application.targetFrameRate = 60;

#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
            

        InitializeApp();
        CheckConnectivity();
    }

    private void OnDestroy()
    {
        destroyCancellationTokenSource.Cancel();
        destroyCancellationTokenSource.Dispose();
    }

    private async void CheckConnectivity()
    {
        CancellationToken timerToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5f);
        CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timerToken, destroyCancellationTokenSource.Token);
        bool connectionAvailable = await _internetChecker.IsInternetAvailable(linkedSource.Token);

        if (connectionAvailable)
        {
            Debug.Log($"Internet is available -> Starting app!");
            // InstantiateInternetErrorPrompt();
            return;
        }
        
        Debug.Log("Internet not Found, trying again...");
        InstantiateInternetErrorPrompt();
    }

    private void InstantiateInternetErrorPrompt()
    {
        connectionErrorPrompt = Instantiate(internetErrorTemplate, _UIManager.gameObject.transform);
        connectionErrorPrompt.transform.Find("Confirmation/Reload").GetComponent<Button>().onClick.AddListener(OnInternetErrorReload);
        connectionErrorPrompt.transform.Find("Confirmation/Cancel").GetComponent<Button>().onClick.AddListener(OnInternetErrorCancel);
    }

    private void OnInternetErrorReload()
    {
        InitializeApp();
        Destroy(connectionErrorPrompt);
    }

    private void OnInternetErrorCancel()
    {
        Destroy(connectionErrorPrompt);
    }

    async void InitializeApp()
    {
        // if(LoaderUtility.GetActiveLoader() != null)
        //     LoaderUtility.Deinitialize();
        //  Delete Local Data if Requested
#if UNITY_EDITOR
        if (deleteUserOnStart) DataManager.DeleteUser();
#endif

        //  Load Local Data from Memory
        userData = DataManager.LoadUser();

        if (userData != null)
        {
            deletInProgressHint.SetActive(deletionListContains(userData.userId));


            Debug.Log(userData.ToString());
#if !UNITY_EDITOR
            if (!userData.emailVerified)
            {
                UIManager.toggleLoadingScreen(false);
                UIManager.loadCofirmRegistrationPage();
                return;
            }

#endif
            UserSession.AccessToken = userData.accessToken;
            UserSession.AccessExpires = userData.accessExpires;
            UserSession.RefreshToken = userData.refreshToken;

            GameObject.FindObjectOfType<CommentWatcher>().setUserData(userData);
            //await GameObject.FindObjectOfType<CommentWatcher>().Refresh();

            UIManager.toggleLoadingScreen(false);

            if (userData.nickname.Equals(""))
            {
                //set nickname for comments & chats
                nicknameRegister.GetComponent<TMP_InputField>().text = userData.nickNameSuggestion;
                UIManager.loadNicknamePage();
            }
            else
            {
                if (onlyLogin)
                {                    
                    userActivity = await GetCurrentUserActivity();
                    LoadAccountPage(userActivity.Length);
                }

            }
        }
        else
        {

#if UNITY_EDITOR
            //* TODO: Debug Acc 
            authorizeUser("Auth0 JSON answer values: Accesstoken","expires ins seconds", "refreshtoken");
           

#else
            ///TODO
            //Application.OpenURL(APIManager.loginUrl);
            //  Load LandingPage
            UIManager.toggleLoadingScreen(false);
            UIManager.loadLandingPage();
#endif
        }

    }

    public void LogIn()
    {
        UIManager.toggleLoadingScreen(true);
        FindObjectOfType<DeepLinkManager>().ShowWebsite(APIManager.logoutUrl);
        //Application.OpenURL(APIManager.logoutUrl);
    }

    public void LoadAccountPage(int activityLength)
    {
        UIManager.toggleScannedArtworksButton(activityLength > 0);
        UIManager.ParseUsernameAccountPage(userData.nickname);
        UIManager.loadAccountPage(onlyLogin);
    }

    public void LoadExhibitionPage()
    {
        UIManager.ParseUsernameAccountPage(userData.nickname);
        UIManager.loadExhibitionPage();
        _ExhibitionPage.SetupPage(userData);
    }

    public void LoadArtworkPage(UserActivity userActivity)
    {
        UIManager.loadArtworkPage();
        _ArtworkPage.SetupPage(userData, userActivity);
    }
    public void LoadArtworkPage()
    {
        LoadArtworkPageAsync();
    }

    public async void reconfigureFollowingComments()
    {
        UIManager.toggleLoadingScreen(true);
        CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(50f);
        CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);

        List<Comment> followingComments = new List<Comment>();

        try
        {
            if (userActivity != null && userActivity.Length > 0)
            {
                //get all comments
                foreach (UserActivity activity in userActivity)
                {
                    foreach (string artworkId in activity.artwork)
                    {
                        Comment[] result = await APIManager.GetComment(artworkId, combined.Token);
                        if (result != null && result.Length > 0)
                        {
                            foreach (Comment comment in result)
                            {
                                // add all comments which belong to you
                                if (comment.userId.Equals(userData.userId))
                                {
                                    CancellationTokenSource combined2 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
                                    comment.messageCount = await APIManager.GetMessageCount(comment._id, combined2.Token);
                                    if (!followingComments.Contains(comment))
                                        followingComments.Add(comment);
                                }
                                else
                                {
                                    CancellationTokenSource combined3 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
                                    //check if you engaged with comments, you are no owning
                                    Message[] messages = await APIManager.GetMessage(artworkId, comment._id, combined3.Token, 100);
                                    if (messages != null && messages.Length > 0)
                                    {
                                        foreach (Message message in messages)
                                        {
                                            if (message.userId.Equals(userData.userId))
                                            {
                                                if (!followingComments.Contains(comment))
                                                {
                                                    comment.messageCount = messages.Length;
                                                    followingComments.Add(comment);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //overwrite all comments
            UserSession.refreshComments(followingComments, userData.userId);
            await GameObject.FindObjectOfType<CommentWatcher>().Refresh();
            UIManager.toggleLoadingScreen(false);
            _WhatsNewPage.SetUpPage();
        }
        catch(Exception e)
        {
            Debug.LogError(e.StackTrace);
            UIManager.toggleLoadingScreen(false);
        }
    }

    private void LoadArtworkPageAsync()
    {
        UserActivity combinedActivity = CombineActivities(userActivity);
        UIManager.loadArtworkPage();
        _ArtworkPage.SetupPage(userData, combinedActivity);
    }


    public void LoadMessagePageOfSelection()
    {
        //buttonFunctionAR.changeARMessagePageStatus();
        //sceneManager.LoadMessagePage(currentSelection.comment);
    }
    
    public async void LoadMessagePage(Comment comment, bool backToCommentgae = true)
    {
        // await LoadingMessagePage(comment);
        UIManager.loadMessagePage();
        _UIManager.ToggleMessagePageExitButton(backToCommentgae);
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
    
    private UserActivity CombineActivities(UserActivity[] activities)
    {
        List<string> allArtworks = new List<string>();

        foreach (UserActivity userActivity in activities)
        {
            foreach (string s in userActivity.artwork)
            {
                if(allArtworks.Contains(s)) continue;
                allArtworks.Add(s);
            }
        }
        
        UserActivity combinedActivity = new UserActivity("", allArtworks.ToArray());
        return combinedActivity;
    }

    private async UniTask<UserActivity[]> GetCurrentUserActivity()
    {
        UserActivity[] activities = await APIManager.GetUserActivity(userData.userId);
        return activities;
    }

    public void LoadCommentPage(Artwork artworkData)
    {
        UIManager.loadCommentPage();
        _CommentPage.SetupPage(userData, artworkData);
    }

    public void LoadWhatsUpPage()
    {
        UIManager.toggleLoadingScreen(true);
        UIManager.loadWhatsUpPage();
        if (UserSession.loadFollowingComments(userData.userId).Length == 0)
        {
            reconfigureFollowingComments();
            _WhatsNewPage.Init(userData, false);
            //_WhatsNewPage.SetUpPage();
        }
        else
        {
            _WhatsNewPage.Init(userData, true);
        }

    }

    public void authorizeNewUser(string accessToken, string accessExpires, string refreshToken)
    {
    }

    public void toggleLoadingScreen(bool visible)
    {
        UIManager.toggleLoadingScreen(visible);
    }

    public async void authorizeUser(string accessToken, string accessExpires, string refreshToken)
    {

        float starttime = Time.time;
        userData = await APIManager.LoginUser(accessToken, accessExpires, refreshToken, destroyCancellationTokenSource.Token);
        
        Logger.Log($"Time To Login = {Time.time - starttime}");

        if (userData != null)
        {
            GameObject.FindObjectOfType<CommentWatcher>().setUserData(userData);
            //await GameObject.FindObjectOfType<CommentWatcher>().Refresh();
            deletInProgressHint.SetActive(deletionListContains(userData.userId));

            UIManager.toggleLoadingScreen(false);

#if !UNITY_EDITOR
            if (!userData.emailVerified)
            {
                UIManager.loadCofirmRegistrationPage();
                return;
            }
#endif
            if (userData.nickname.Equals("")){
                //set nickname for comments & chats
                nicknameRegister.GetComponent<TMP_InputField>().text = userData.nickNameSuggestion;
                UIManager.loadNicknamePage();
                UIManager.toggleLoadingScreen(false);
            }
            else 
            {
                DataManager.SaveUser(userData);
                if (onlyLogin)
                {                   
                    userActivity = await GetCurrentUserActivity();
                    LoadAccountPage(userActivity.Length);
                    UIManager.toggleLoadingScreen(false);
                }
                else
                {
                    LoadExhibitionPage();
                    UIManager.toggleLoadingScreen(false);
                }
            }
        }
        else
        {
            Debug.Log("userdate == null");
            GameObject.FindObjectOfType<DeepLinkManager>().ShowWebsite(APIManager.logoutUrl);
            //Application.OpenURL(APIManager.logoutUrl);
        }
    }

    public async void confirmNickName(TMP_InputField textfield)
    {
        userData.nickname = textfield.text;
        bool success = await APIManager.SaveUserNickname(userData.nickname, destroyCancellationTokenSource.Token);
        if (!success)
            Debug.LogError("Failed to save nickname in database");
        else
        {
            DataManager.SaveUser(userData);
            userActivity = await GetCurrentUserActivity();
            LoadAccountPage(userActivity.Length);
        }
    }

    public void Delete(TMP_InputField inputField)
    {
        try
        {
            //TODO mailaccount
            string userName = "mail@mail.mail";
            string userPassword= "mailpassword";
            SmtpClient mailServer = new SmtpClient("mail.gmx.net", 587);
            mailServer.EnableSsl = true;
            mailServer.Credentials = new NetworkCredential(userName, userPassword) as ICredentialsByHost;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                return true;
            };

            MailMessage msg = new MailMessage(userName, "artchat@gmx.de");
            msg.Subject = "User Loeschung";
            msg.Body = "Hallo,\nIch möchte meinen Account Löschen.\n\nMeine UserID = "+userData.userId+"\n\nMein Nickname = "+userData.nickname+" \n";
            if (!inputField.text.Equals(""))
            {
                msg.Body += "\n\nDer Grund der Löschung ist:\n";
                msg.Body += inputField.text;
            }

            msg.Body += "\n\n\nVielen Dank für die coole App und viele Grüße\n\n" + userData.nickname;

            mailServer.SendCompleted += new SendCompletedEventHandler( (sender, args) => {
                addToDeletionList(userData.userId);
                deletInProgressHint.SetActive(true);

            });
            mailServer.SendAsync(msg, "");

            Debug.Log("SimpleEmail: Sending Email.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("SimpleEmail: " + ex);
        }
    }

    private void addToDeletionList(string accountId)
    {
        string deletionJSONString = PlayerPrefs.GetString("DeletionList", "[]");
        JSONArray deletionJSON = JSON.Parse(deletionJSONString).AsArray;
        if (!deletionJSONString.Contains(accountId))
        {
            deletionJSON.Add(accountId);
        }
        PlayerPrefs.SetString("DeletionList", deletionJSON.ToString());
        PlayerPrefs.Save();
    }

    private bool deletionListContains(string accountId)
    {
#if UNITY_EDITOR
        return false;
#endif
        return PlayerPrefs.GetString("DeletionList", "[]").Contains(accountId);
    }


    public void Logout()
    {
        APIManager.LogOutUser();

        DataManager.DeleteUser();
        userData = null;
        UIManager.loadLandingPage();

        UserSession.AccessToken = "";
        UserSession.AccessExpires = "";
        UserSession.RefreshToken = "";
    }

    private void InstantiateLoading()
    {
        if(loadingInstance != null)
            return;
        loadingInstance = Instantiate(loadingTemplate, _UIManager.gameObject.transform);
    }

    private void DeleteLoading()
    {
        if(loadingInstance == null)
            return;

        Destroy(loadingInstance);
        loadingInstance = null;
    }
}
