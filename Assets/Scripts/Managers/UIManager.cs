using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    public GameObject loadingPrefab;
    
    private static colorTable _colorTable;

    private static GameObject QuickNavigation;
    // private static GameObject Navigation;
    // private static GameObject NavigationTitle;
    // private static GameObject NavigationLogo;
    // private static GameObject NavigationBack;
    private static GameObject ErrorMessage;
    private static GameObject ErrorType;
    private static GameObject DeletePrompt;
    private static GameObject BadInternetPrompt;

    private static GameObject LandingPage;
    private static GameObject CofirmRegistrationPage;
    // private static GameObject LandingPageLogin;
    // private static GameObject LandingPageRegister;

    private static GameObject LoadingScreen;

    private static GameObject LoginPage;
    private static GameObject LoginPageLogin;
    private static GameObject LoginPageRegister;

    private static GameObject NicknamePage;

    private static GameObject ExhibitionPage;

    private static GameObject ArtworkPage;

    private static GameObject CommentPage;

    private static GameObject WhatsNewPage;

    private static GameObject AccountPage;
    
    private static GameObject MessagePage;

    private static GameObject Guide;

    public void Awake()
    {
        //  Color Manager
        _colorTable = GameObject.Find("ColorManager").GetComponent<colorTable>();

        //  Permanent Elements
        QuickNavigation = transform.Find("Quick Navigation").gameObject;
        QuickNavigation.SetActive(false);

        //Navigation = transform.Find("Navigation").gameObject;
        //NavigationTitle = transform.Find("Navigation/Title").gameObject;
        //NavigationLogo = transform.Find("Navigation/Logo").gameObject;
        //NavigationBack = transform.Find("Navigation/Back").gameObject;
        ErrorMessage = transform.Find("ErrorMessage").gameObject;
        ErrorType = transform.Find("ErrorMessage/Error - Panel/Text - ErrorType").gameObject;
        DeletePrompt = transform.Find("DeletePrompt").gameObject;
        ToogleDeletePrompt(false);
        BadInternetPrompt = transform.Find("BadInternet").gameObject;

        LoadingScreen = transform.Find("LoadingSpinner").gameObject;
        LoadingScreen.SetActive(false);

        //  Landing Page
        LandingPage = transform.Find("LandingPage").gameObject;
        // LandingPageLogin = transform.Find("LandingPage/Login").gameObject;
        // LandingPageRegister = transform.Find("LandingPage/Register").gameObject;

        CofirmRegistrationPage = transform.Find("ConfirmRegistrationPage").gameObject;

        LoginPage = transform.Find("LoginPage").gameObject;
        LoginPageLogin = transform.Find("LoginPage/Login").gameObject;
        LoginPageRegister = transform.Find("LoginPage/Register").gameObject;

        NicknamePage = transform.Find("NicknamePage").gameObject;

        //  Exhibition Page
        ExhibitionPage = transform.Find("ExhibitionPage").gameObject;

        //  Artwork Page
        ArtworkPage = transform.Find("ArtworkPage").gameObject;

        //  Comment Page
        CommentPage = transform.Find("CommentPage").gameObject;


        WhatsNewPage = transform.Find("WhatsNewPage").gameObject;

        //  Account Page
        AccountPage = transform.Find("AccountPage").gameObject;
        
        // ARMessagePage
        MessagePage = transform.Find("ARMessagePage").gameObject;

        // Guide
        Guide = transform.Find("Guide").gameObject;
        Guide.SetActive(false);
        
        CommentPage.GetComponent<RectTransform>().anchoredPosition 
            = new Vector2(0,CommentPage.GetComponent<RectTransform>().anchoredPosition.y);
        Guide.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(0, Guide.GetComponent<RectTransform>().anchoredPosition.y);
        ArtworkPage.GetComponent<RectTransform>().anchoredPosition 
            = new Vector2(0,ArtworkPage.GetComponent<RectTransform>().anchoredPosition.y);

    }

    public void ShowError(string error_description)
    {
        ErrorMessage.SetActive(true);
        ErrorType.GetComponent<TextMeshProUGUI>().text = error_description;
    }

    public static void ShowBadInternetError()
    {
        BadInternetPrompt.SetActive(true);
    }

    /*
    public static async void ParseUsername(TextMeshProUGUI textfield, string userID)
    {
        string accessToken = PlayerPrefs.GetString("AccessToken");
        string username = await APIManager.GetUsername(userID);
        if (username != null) textfield.text = username;
    }
    */

    public void ToggleMessagePageExitButton(bool returnToCommentPage)
    {
        MessagePage.transform.Find("ExitCommentPage").gameObject.SetActive(returnToCommentPage);
        MessagePage.transform.Find("ExitWhatsNewPage").gameObject.SetActive(!returnToCommentPage);
    }

    public void ToogleDeletePrompt(bool show)
    {
        DeletePrompt.SetActive(show);
    }

    public static void ParseDate(TextMeshProUGUI textfield, string dateString)
    {
        DateTime convertedDate = DateTime.Parse(dateString);
        textfield.text = convertedDate.ToString("dd/MM/yy - HH:mm");
    }

    public static void ParseUsernameAccountPage(string nickname)
    {
        AccountPage.transform.Find("ListView/Username").GetComponent<TextMeshProUGUI>().text = nickname;
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(AccountPage.GetComponent<RectTransform>());

        //ParseUsername(AccountPage.transform.Find("ListView/Block/Username").GetComponent<TextMeshProUGUI>(), nickname);
    }

    public static void toggleScannedArtworksButton(bool visible)
    {
        AccountPage.transform.Find("ListView/ButtonScanned").gameObject.SetActive(visible);
        AccountPage.transform.Find("ListView/WhatsNewPage").gameObject.SetActive(visible);
    }

    public static void ErrorRetry()
    {
        ErrorMessage.SetActive(false);
    }

    // public static void loadLandingPage()
    // {
    // QuickNavigation.SetActive(false);
    // Navigation.SetActive(false);

    //     LandingPage.SetActive(true);
    //     LandingPageLogin.SetActive(true);
    //     LandingPageRegister.SetActive(false);

    //     ExhibitionPage.SetActive(false);
 
    //     ArtworkPage.SetActive(false);
    //     CommentPage.SetActive(false);
    //     AccountPage.SetActive(false);
    //     Guide.SetActive(false);
    //     _colorTable.ToggleMode();
    // }

    public static void loadLandingPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // Navigation.SetActive(false);

        LandingPage.SetActive(true);
        CofirmRegistrationPage.SetActive(false);
        // LandingPageLogin.SetActive(true);
        // LandingPageRegister.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false); 
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadCofirmRegistrationPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // Navigation.SetActive(false);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(true);
        // LandingPageLogin.SetActive(true);
        // LandingPageRegister.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadNicknamePage()
    {
        NicknamePage.SetActive(true);
        QuickNavigation.SetActive(false);
        // Navigation.SetActive(false);


        LoginPage.SetActive(false);
        LoginPageLogin.SetActive(true);
        LoginPageRegister.SetActive(false);

        // LandingPageLogin.SetActive(false);
        // LandingPageRegister.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadLoginPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // Navigation.SetActive(false);


        LoginPage.SetActive(true);
        LoginPageLogin.SetActive(true);
        LoginPageRegister.SetActive(false);

        // LandingPageLogin.SetActive(false);
        // LandingPageRegister.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadRegisterPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // Navigation.SetActive(false);


        LoginPage.SetActive(true);
        LoginPageLogin.SetActive(false);
        LoginPageRegister.SetActive(true);

        // LandingPageLogin.SetActive(false);
        // LandingPageRegister.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadAccountPage(bool disableBackButton = false)
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(true);
        // NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Mein Account";
        // NavigationLogo.SetActive(false);
        // NavigationBack.SetActive(!disableBackButton);
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(true);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadExhibitionPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Ausstellungen";
        // NavigationLogo.SetActive(true);
        // NavigationBack.SetActive(false);
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(true);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadArtworkPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        //NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Kunstwerke";
        //NavigationLogo.SetActive(false);
        // NavigationBack.SetActive(true);
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(true);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadCommentPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Kommentare";
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(true);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }
    
    public static void loadWhatsUpPage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Kommentare";
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(true);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();

    }

    public static void loadMessagePage()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Kommentare";
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(true);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(false);
        _colorTable.ToggleMode();
    }

    public static void loadGuide()
    {
        NicknamePage.SetActive(false);
        QuickNavigation.SetActive(false);
        // NavigationTitle.GetComponent<TextMeshProUGUI>().text = "Guide";
        // NavigationLogo.SetActive(false);
        // NavigationBack.SetActive(true);
        // Navigation.SetActive(true);

        LandingPage.SetActive(false);
        CofirmRegistrationPage.SetActive(false);

        ExhibitionPage.SetActive(false);
        MessagePage.SetActive(false);

        ArtworkPage.SetActive(false);
        CommentPage.SetActive(false);
        WhatsNewPage.SetActive(false);
        AccountPage.SetActive(false);
        Guide.SetActive(true);
        _colorTable.ToggleMode();
    }

    public void backButton()
    {
        if (AccountPage.activeSelf)
        {
            loadArtworkPage();
        }

        if (ArtworkPage.activeSelf)
        {
            loadExhibitionPage();
        }

        if (CommentPage.activeSelf)
        {
            loadArtworkPage();
        }

        if (Guide.activeSelf)
        {
            loadAccountPage();
        }

        else return;

    }

    public void loadARScene()
    {
        SceneManager.LoadScene("AR Scene", LoadSceneMode.Single);
    }

    public void loadARScene_OnlyLogin()
    {
        // SceneManager.LoadScene("AR Scene_OnlyLogin", LoadSceneMode.Single);
        StartCoroutine(LoadARSceneAsync());
    }

    public static void toggleLoadingScreen(bool show)
    {
        LoadingScreen.SetActive(show);
    }

    IEnumerator LoadARSceneAsync()
    {
        GameObject loading = Instantiate(loadingPrefab, this.gameObject.transform);
        LoaderUtility.Initialize();
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("AR Scene_OnlyLogin", LoadSceneMode.Single);

        while (!loadSceneAsync.isDone)
        {
            yield return null;
        }
    }
}
