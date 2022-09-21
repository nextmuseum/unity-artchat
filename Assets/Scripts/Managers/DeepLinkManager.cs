using System;
using UnityEngine;
using ARtChat;


public class DeepLinkManager : MonoBehaviour
{
    public UniWebView webView;
    ///public WebAuthentification webAuth;
    public static DeepLinkManager Instance { get; private set; }
    public AppManager appMananger;

    public string deeplinkURL;
    //public TextMeshProUGUI link;
    //public TextMeshProUGUI anwswer;
    private void Awake()
    {
        
       Application.deepLinkActivated += onDeepLinkActivated;

    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= onDeepLinkActivated;
    }

    private void OnApplicationFocus(bool focus)
    {

    }

    public void ShowWebsite(string url)
    {
#if UNITY_ANDROID || UNITY_IOS
        Application.OpenURL(APIManager.logoutUrl);
        return;
#endif
        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();

        //string USER_AGENT_FAKE = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36";
        string USER_AGENT_FAKE = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148 Chrome/92.0.4515.107 Safari/537.36";
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.BackgroundColor = new Color(1f, 228f/255f, 1f);
        webView.SetBackButtonEnabled(true);
        webView.SetShowToolbar(true,false,true,true);
        webView.SetShowToolbarNavigationButtons(true);
        webView.SetToolbarDoneButtonText("  ");

        webView.SetUserAgent(USER_AGENT_FAKE);
        //loading old links...
        webView.CleanCache();
        UniWebView.ClearCookies();

        webView.Load(url);
        webView.Show(true);

        //callback which indicates page loading if finished
        webView.OnPageFinished += (view, statusCode, url) => {
            Debug.Log(statusCode);
            Debug.Log(url);
        };

        //cleaning
        webView.OnShouldClose += (view) => {
            //webView = null;
            return false;
        };

    }

    public void hideWebView()
    {
        webView.Hide(true);
    }

    private void onDeepLinkActivated(string url)
    {

        Debug.Log(">>>>>deepLink callback with url = "+url);
        if (webView != null)
        {
            webView.Stop();
            webView.Hide(true);
            Debug.Log(">>>>>DeeplinkwebView.Hide(true" + webView.gameObject.name);
            
            GameObject.DestroyImmediate(webView.gameObject);
            Debug.Log(">>>>>DestroyImmediate(webView.gameObject)");
            webView = null;
            
        }
        Debug.Log("appmanager = "+appMananger);
        appMananger.toggleLoadingScreen(true);

        Debug.Log("Deeplink called");

        // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
        deeplinkURL = url;
        //Debug
        //link.text = url;
        string data = url.Split('?')[1];
        string[] dataValues = data.Split('&');

        string accessToken = dataValues[0].Split('=')[1];
        string accessExpires = dataValues[2].Split('=')[1];
        string refreshToken = dataValues[3].Split('=')[1];

        Debug.Log(accessToken + " \n " + accessExpires + " \n" + refreshToken);

        DateTime tokenExpirationTime = DateTime.Now.AddSeconds(double.Parse(accessExpires));

        UserSession.AccessToken = accessToken;
        UserSession.AccessExpires = tokenExpirationTime.ToString();
        UserSession.RefreshToken = refreshToken;

        appMananger.authorizeUser(accessToken, accessExpires, refreshToken);
        Debug.Log("after authorization");
        //save expiration date and time

    }

}
