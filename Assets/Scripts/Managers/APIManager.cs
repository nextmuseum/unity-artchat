using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Proyecto26;
using System.Text;
using System.Threading;
using SimpleJSON;
using ARtChat;
using System.Collections.Generic;
using System;
using System.Globalization;

public class APIManager
{
    public static string baseURL = "https://YOUR-URL.heroku.com/";

    public static string loginUrl = baseURL+"auth/login-unity";
    public static string logoutUrl = baseURL+"auth/logout-unity";

    public static DateTime dateOfLegacyAccounts = DateTime.Parse("2022-03-23");

    #region USER

    public static async UniTask<bool> refreshAccessToken(string refreshToken, CancellationToken token)
    {
        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}auth/renew-token?refresh_token={refreshToken}", UnityWebRequest.kHttpVerbGET);
        Debug.Log(request.uri);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log("Request answer =============== " + request.downloadHandler.text);
            JSONNode jsonObject = JSON.Parse(request.downloadHandler.text).AsObject;

            string accessToken = jsonObject["access_token"].Value;
            string accessExpires = jsonObject["expires_in"].Value;

            System.DateTime tokenExpirationTime = System.DateTime.Now.AddSeconds(double.Parse(accessExpires));

            UserSession.AccessToken = accessToken;
            UserSession.AccessExpires = tokenExpirationTime.ToString();

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("#########" + e.Message);
            return false;
        }
    }

    public static void LogOutUser()
    {
        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest(logoutUrl, UnityWebRequest.kHttpVerbGET);
        Debug.Log(request.uri);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        #endregion

        try
        {
            request.SendWebRequest();
            Debug.Log("Request answer =============== " + request.downloadHandler.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError("#########" + e.Message);
        }
    }

    public static async UniTask<User> LoginUser(string accessToken, string accessExpires, string refreshToken, CancellationToken token)
    {
#if UNITY_EDITOR
        System.DateTime tokenExpirationTimeEditor = System.DateTime.Now.AddSeconds(-double.Parse(accessExpires));
        UserSession.AccessToken = accessToken;
        UserSession.AccessExpires = tokenExpirationTimeEditor.ToString();
        UserSession.RefreshToken = refreshToken;
        await tokenIsValid();
        accessToken = UserSession.AccessToken;
#endif


        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/users/me", UnityWebRequest.kHttpVerbGET);
        Debug.Log(request.uri);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log("Request answer =============== "+request.downloadHandler.text);
            JSONNode jsonObject = JSON.Parse(request.downloadHandler.text).AsObject;

            string userId = jsonObject["identities"].AsArray[0].AsObject["user_id"].Value;
            string nickname = "";
            string nickNameSuggestion = jsonObject["nickname"].Value;
            bool emailVerified;
            if (!jsonObject["appdata"].IsNull)
            {
                DateTime date = DateTime.Parse(jsonObject["appdata"].AsObject["date"].Value).Date;
                //all legacy accounts dont have to verify mail
                emailVerified = jsonObject["email_verified"].AsBool || date < dateOfLegacyAccounts;

                nickname = jsonObject["appdata"].AsObject["userName"].Value;

            }
            else
            {
                emailVerified = jsonObject["email_verified"].AsBool;
            }

            Debug.Log("AC = "+accessToken);
            Debug.Log("UID = "+userId);
            Debug.Log("AE = "+accessExpires);
            Debug.Log("RT = "+refreshToken);
            Debug.Log("NN = "+nickname);
            Debug.Log("NNS = "+nickNameSuggestion);

            System.DateTime tokenExpirationTime = System.DateTime.Now.AddSeconds(double.Parse(accessExpires));

#if UNITY_EDITOR

            UserSession.AccessToken = accessToken;
            UserSession.AccessExpires = tokenExpirationTime.ToString();
            UserSession.RefreshToken = refreshToken;
#endif

            User newUser = new User(accessToken, userId, tokenExpirationTime.ToString(), refreshToken, nickname, nickNameSuggestion, emailVerified);

            Debug.Log("New User = "+newUser);

            return newUser;
        }
        catch (System.Exception e)
        {
            Debug.LogError("#########"+e.Message);
            return null;
        }
    }

    public static async UniTask<bool> SaveUserNickname(string nickname, CancellationToken token)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        JSONNode nicknameObj = JSON.Parse("{}");
        nicknameObj["userName"] = nickname;

        #region RequestBuild
        string url = $"{baseURL}api/users/me/appdata";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(nicknameObj.ToString());

        UnityWebRequest request = UnityWebRequest.Put(url, data);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            if (request.downloadHandler.text.Equals("")) 
                Debug.Log("Successfully changed nickname");    
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public static async UniTask<bool> PostUserActivity(string userID, UserActivityRequest userActivityRequest, CancellationToken token)
    {
        await tokenIsValid();

        string accessToken = UserSession.AccessToken;
        string requestBody = JsonUtility.ToJson(userActivityRequest);

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/users/{UnityWebRequest.EscapeURL(userID)}/activity", UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        Debug.Log("Post User activity " + requestBody);
        Debug.Log("Post User activity " + request.url);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }
    public static async UniTask<UserActivity[]> GetUserActivity(string userID)
    {
        await tokenIsValid();

        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/users/{UnityWebRequest.EscapeURL(userID)}/activity", UnityWebRequest.kHttpVerbGET);
        Debug.Log($"{baseURL}api/users/{userID}/activity");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest();
            string result = "{}";
            if (request.downloadHandler.text != "")
            {
                result = request.downloadHandler.text;
            }
            else
            {
                return new UserActivity[0];
            }
            
            return JsonHelper.ArrayFromJson<UserActivity>(request.downloadHandler.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }
    
    public static async UniTask<string> GetUsername(string userID)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/users/{UnityWebRequest.EscapeURL(userID)}/name", UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        Debug.Log(request.uri);
        #endregion

        try
        {
            await request.SendWebRequest();
            JSONNode json = JSON.Parse("request.downloadHandler.text");
            return json["nickname"];
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }
    #endregion

    #region EXHIBITION
    public static async UniTask<Exhibition> GetExhibition(string exhibitionID)
    {
        await tokenIsValid();

        #region RequestBuild
        string accessToken = UserSession.AccessToken;

        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/exhibitions/{exhibitionID}", UnityWebRequest.kHttpVerbGET);
        Debug.Log($"{baseURL}exhibitions/{exhibitionID}");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            return JsonUtility.FromJson<Exhibition>(request.downloadHandler.text);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }
    #endregion

    #region ARTWORK

    public static async UniTask<Artwork> GetArtwork(string artworkID, CancellationToken token)
    {
        await tokenIsValid();

        #region RequestBuild
        string accessToken = UserSession.AccessToken;

        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}", UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        Debug.Log($"{baseURL}api/artworks/{artworkID}");
        Debug.Log(request.uri);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log("Text = "+request.downloadHandler.text);
            return JsonUtility.FromJson<Artwork>(request.downloadHandler.text);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }
    #endregion

    #region COMMENT
    public static async UniTask<Comment[]> GetComment(string artworkID, CancellationToken token)
    {
        await tokenIsValid();

        #region RequestBuild
        string accessToken = UserSession.AccessToken;
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments", UnityWebRequest.kHttpVerbGET);
        request.url += "/?limit=0";

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log("Comments = " + request.downloadHandler.text);
            Comment[] result = JsonHelper.ArrayFromJson<Comment>(request.downloadHandler.text);
            foreach(Comment c in result)
                Debug.Log(c.ToString());
            return result;           
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return new Comment[0];
        }
    }

    public static async UniTask<Comment[]> GetComment(string[] comments, CancellationToken token)
    {
        await tokenIsValid();

        #region RequestBuild
        string accessToken = UserSession.AccessToken;
        string requestString = $"{baseURL}api/comments?ids=" + comments[0];
        for (int i = 1; i < comments.Length; i++)
        {
            requestString += "," + comments[i];
        }
        requestString += "&limit=0";
        Debug.Log(requestString);

        UnityWebRequest request = new UnityWebRequest(requestString, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log("Comments = " + request.downloadHandler.text);
            Comment[] result = JsonHelper.ArrayFromJson<Comment>(request.downloadHandler.text);
            foreach (Comment c in result)
                Debug.Log(c.ToString());
            return result;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return new Comment[0];
        }
    }

    public static async UniTask<bool> tokenIsValid()
    { 
        System.DateTime accessTokenExpiration = System.DateTime.Parse(UserSession.AccessExpires);
        if (accessTokenExpiration <= System.DateTime.Now)
        {
            CancellationToken token = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            CancellationTokenSource linked =
                CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationTokenSource().Token);
            await refreshAccessToken(UserSession.RefreshToken, linked.Token);

        }
        return true;
    }

    public static async UniTask<bool> PostComment(string artworkID, string commentID, Comment comment)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        string requestBody = JsonUtility.ToJson(new CommentRequest(comment));

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments/{commentID}", UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        #endregion

        try
        {
            await request.SendWebRequest();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public static async UniTask<bool> DeleteReport(string reportID)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/reports/{reportID}", UnityWebRequest.kHttpVerbDELETE);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        Debug.Log(request.uri);
        #endregion

        try
        {
            await request.SendWebRequest();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public static async UniTask<Report> PutReportComment(string commentID, string userId)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        JSONNode properties = JSONNode.Parse("{}");
        properties["commentId"] = commentID;
        properties["userId"] = userId;
        string requestBody = properties.ToString();

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/comments/{commentID}/reports", UnityWebRequest.kHttpVerbPUT);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        Debug.Log(request.uri);
        Debug.Log(requestBody);
        #endregion

        try
        {
            await request.SendWebRequest();
            Debug.Log("Request answer = "+request.downloadHandler.text);
            return JsonUtility.FromJson<Report>(request.downloadHandler.text);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    //if comment moved it gets updated in the database
    public static async UniTask<Comment> PutComment(string artworkID, Comment comment)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        string requestBody = JsonUtility.ToJson(new CommentRequest(comment));

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments", UnityWebRequest.kHttpVerbPUT);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        Debug.Log(requestBody.ToString() + " to URL " + request.url);
        #endregion

        try
        {
            await request.SendWebRequest();
            return JsonUtility.FromJson<Comment>(request.downloadHandler.text);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public static async UniTask<bool> DeleteComment(string commentID)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/comments/{commentID}", UnityWebRequest.kHttpVerbDELETE);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);

        #endregion

        try
        {
            await request.SendWebRequest();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }
    public static async UniTask<bool> DeleteComment(string artworkID, string commentID)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments/{commentID}", UnityWebRequest.kHttpVerbDELETE);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        
        #endregion

        try
        {
            await request.SendWebRequest();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    #endregion

    #region MESSAGE_PAGE
    public static async UniTask<Message[]> GetMessage(string artworkID, string commentID, CancellationToken token, int limit = 15, int skip = 0)
    {
        await tokenIsValid();

        #region RequestBuild0
        string accessToken = UserSession.AccessToken;
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments/{commentID}/messages", UnityWebRequest.kHttpVerbGET);
        request.url += $"/?limit={limit}";
        request.url += $"&skip={skip}";
        request.url += "&sort=-1";
        Debug.Log(request.url);

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            Debug.Log(request.downloadHandler.text);
            return JsonHelper.ArrayFromJson<Message>(request.downloadHandler.text);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            return new Message[0];
        }
    }
    public static async UniTask<Message> PutMessage(string artworkID, string commentID, Message message, CancellationToken token)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        string requestBody = JsonUtility.ToJson(new MessageRequest(message));

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments/{commentID}/messages", UnityWebRequest.kHttpVerbPUT);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        // Debug.Log(requestBody);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            Debug.Log("++++++" + requestBody);
            await request.SendWebRequest().WithCancellation(token);
            return JsonUtility.FromJson<Message>(request.downloadHandler.text);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public static async UniTask<bool> DeleteReportMessage(string messageID)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/messages/{messageID}/reports", UnityWebRequest.kHttpVerbDELETE);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async UniTask<Report> PutReportMessage(string messageID)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/messages/{messageID}/reports", UnityWebRequest.kHttpVerbPUT);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest();
            return JsonUtility.FromJson<Report>(request.downloadHandler.text);
        }
        catch(System.Exception e)
        {
            Debug.Log(request.uri);
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            return null;
        }
    }

    public static async UniTask<int> GetMessageCount(string commentID, CancellationToken token)
    {
        await tokenIsValid();

        #region RequestBuild0
        string accessToken = UserSession.AccessToken;

        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/comments/{commentID}/messages", UnityWebRequest.kHttpVerbGET);
        request.url += "/?count=1";

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            int count = 0;
            int.TryParse(request.downloadHandler.text, out count);
            return count;
        }
        catch
        {
            return 0;
        }
    }

    public static async UniTask<bool> DeleteMessage(string artworkID, string commentID, string messageID, CancellationToken token)
    {
        await tokenIsValid();
        string accessToken = UserSession.AccessToken;

        #region RequestBuild
        UnityWebRequest request = new UnityWebRequest($"{baseURL}api/artworks/{artworkID}/comments/{commentID}/messages/{messageID}", UnityWebRequest.kHttpVerbDELETE);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        #endregion

        try
        {
            await request.SendWebRequest().WithCancellation(token);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }


    #endregion
}
