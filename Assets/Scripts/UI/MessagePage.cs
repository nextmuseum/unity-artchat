using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System;
using System.Threading;
using ARtChat;
using ARtChat.Templates;
using Cysharp.Threading.Tasks;

public class MessagePage : MonoBehaviour, Director
{
    private List<Message> messages;
    private Comment data;
    private User userData;

    public GameObject mainMessage;
    public GameObject loadNextPageButton;
    public GameObject loadingAnimation;
    public GameObject loadingPrefab;
    public GameObject messageTemplate;
    public GameObject confirmationTemplate;
    public GameObject contentContainer;
    public GameObject newMessagesArrivedButton;
    public Color32 ItemHighlight;
    public Color32 itemDefault;
    public int initLimit = 15;
    public int updateLimit = 10;
    public TMP_InputField inputText;
    
    public bool IsLoadingMessagePage => isLoadingMessagePage;
    public List<Message> Messages => messages;

    private ServerMessages _serverMessages;
    private LocalMessageGOUpdater _messageGOUpdater;
    private Action _lastPageReached;

    private GameObject confirmationBox = null;
    private Coroutine coroutineChecker = null;
    private GameObject _loadingSign;

    private bool pausingPullingPages = false;
    private UniTask loadingMessagePage;
    private bool isLoadingMessagePage = false;

    private CancellationTokenSource destroyCancellation = new CancellationTokenSource();
    
    public void SetupPage(Comment comment, User user)
    {
        if (comment._id == null)
        {
            Debug.LogError("Could Not Setup Message page - Invalid Comment", this);
            return;
        }

        _serverMessages = new ServerMessages(this, comment, initLimit, updateLimit);
        _messageGOUpdater?.ClearList();
        _messageGOUpdater = new LocalMessageGOUpdater(
            messageTemplate,
            contentContainer,
            user,
            ItemHighlight, 
            confirmationTemplate,
            transform.parent.gameObject,
            DeleteItem,
            ReportItem,
            itemDefault
        ); 
        
        data = comment;
        userData = user;
        messages = new List<Message>();

        loadNextPageButton.transform.parent.gameObject.SetActive(true);
        
        LoadMainMessage();

        ActivateLoadingPageButton();
        _serverMessages.RequestServerMessages();
    }

    private void OnDisable()
    {
        if (_serverMessages != null)
        {
            _serverMessages.Dispose();
            _serverMessages = null;
        }
    }

    private void OnApplicationQuit()
    {
        TriggerDestroyCancellation();
    }

    private void TriggerDestroyCancellation()
    {
        if (destroyCancellation == null) return;
        destroyCancellation.Cancel();
        destroyCancellation.Dispose();
        destroyCancellation = null;
    }

    public void LoadNextPage()
    {
        _serverMessages.IncrementPage();
        ActivateLoadingPageButton();
    }

    private void ActivateNextPageButton()
    {
        if (loadingAnimation == null || loadNextPageButton == null) return;
        loadNextPageButton.SetActive(true);
        loadingAnimation.SetActive(false);
        //UIManager.toggleLoadingScreen(false);
    }

    private void ActivateLoadingPageButton()
    {
        loadNextPageButton.SetActive(false);
        loadingAnimation.SetActive(true);
    }

    private void LastPageReached()
    {
        if(loadNextPageButton == null) return;
        loadNextPageButton.transform.parent.gameObject.SetActive(false);
    }

    private void CheckMessages(List<Message> checkMessage)
    {
        if (InitializeMessages(checkMessage)) return;

        UpdateLocalMessages(checkMessage);
        AppendMessages(checkMessage);
        RemoveMessages(checkMessage);

    }

    private bool InitializeMessages(List<Message> checkMessage)
    {
        if (messages.Count == 0)
        {
            messages = checkMessage;
            _messageGOUpdater.AddAll(messages);
            return true;
        }

        return false;
    }

    private void UpdateLocalMessages(List<Message> checkMessage)
    {
        int minLenght = checkMessage.Count;
        minLenght = Math.Min(minLenght, messages.Count);
        for (int i = 0; i < minLenght; i++)
        {
            bool areEqual = messages[i].GetHash().Equals(checkMessage[i].GetHash());

            if (areEqual)
            {
                continue;
            }

            if (i == 0 && checkMessage[i].userId != userData.userId)
                NewMessagesArrived();

            messages[i] = checkMessage[i];
            _messageGOUpdater.UpdateAt(i, messages[i]);
        }
    }

    private void AppendMessages(List<Message> checkMessage)
    {
        if (checkMessage.Count <= messages.Count) return;
        
        for (int i = messages.Count; i < checkMessage.Count; i++)
        {
            messages.Add(checkMessage[i]);
            _messageGOUpdater.Add(messages[i], i);
        }
    }
    
    private void RemoveMessages(List<Message> checkMessage)
    {
        if (checkMessage.Count < messages.Count)
        {
            for (int i = messages.Count - 1; i >= checkMessage.Count; i--)
            {
                _messageGOUpdater.RemoveAt(i);
                messages.RemoveAt(i);
            }

            DestroyLoadingSign();
        }
    }


    private void NewMessagesArrived()
    {
        if(contentContainer.GetComponent<RectTransform>().anchoredPosition.y > 400)
            newMessagesArrivedButton.SetActive(true);
    }
    
    protected void LoadMainMessage()
    {
        mainMessage.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text = data.userName;
        mainMessage.transform.Find("Text - Comment").GetComponent<TextMeshProUGUI>().text = data.text;

        if (data.date != null)
        {
            UIManager.ParseDate(mainMessage.transform.Find("Text - Date").GetComponent<TextMeshProUGUI>(), data.date);
        }
    }

    async void DeleteItem(int itemIndex)
    {
        CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellation.Token, ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5));
        await DeletingItem(itemIndex, linked.Token);
        data.messageCount--;
        UserSession.addComment(data, userData.userId, true);
    }

    async void ReportItem(int itemIndex)
    {
        await ReportingItem(itemIndex);
    }

    async UniTask DeletingItem(int itemIndex, CancellationToken cancellationToken)
    {
        _messageGOUpdater.CancelConfirmation(itemIndex);
        SpawnLoadingSign();

        try
        {
            await APIManager.DeleteMessage(messages[itemIndex].artworkId, messages[itemIndex].commentId,
                messages[itemIndex]._id, cancellationToken);
        }
        catch (Exception e)
        {
            DestroyLoadingSign();
        }
    }

    async UniTask ReportingItem(int itemIndex)
    {
        _messageGOUpdater.CancelConfirmation(itemIndex);
        SpawnLoadingSign();

        try
        {
            if (messages[itemIndex].reportedByUserId(userData.userId))
            {
                _messageGOUpdater.getGameObjectAt(itemIndex).transform.Find("Report").GetComponent<Image>().color = Color.white;

                if (await APIManager.DeleteReport(messages[itemIndex].getReportById(userData.userId)))
                {
                    messages[itemIndex].removeReportedByUserId(userData.userId);
                }
                else
                {
                    _messageGOUpdater.getGameObjectAt(itemIndex).transform.Find("Report").GetComponent<Image>().color = new Color(221f / 255f, 1f, 6f / 255f);
                }

            }
            else
            {
                _messageGOUpdater.getGameObjectAt(itemIndex).transform.Find("Report").GetComponent<Image>().color = new Color(221f / 255f, 1f, 6f / 255f);

                Report report = await APIManager.PutReportMessage(messages[itemIndex]._id);
                if (report != null)
                {
                    Debug.Log("Put Report Complete");
                    messages[itemIndex].addReportedByUserId(report);
                }
                else
                {
                    _messageGOUpdater.getGameObjectAt(itemIndex).transform.Find("Report").GetComponent<Image>().color = Color.white;
                }
            }
            DestroyLoadingSign();

        }
        catch (Exception e)
        {
            DestroyLoadingSign();
        }
    }

    private void SpawnLoadingSign()
    {
        if(_loadingSign != null)
            return;
        Transform canvas = GetComponentInParent<Canvas>().transform;
        _loadingSign = Instantiate(loadingPrefab, canvas);
    }

    private void DestroyLoadingSign()
    {
        if (_loadingSign == null)
            return;
        Destroy(_loadingSign);
    }

    public async void AddMessage()
    {
        CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellation.Token, ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5));
        await AddMessageTask(linked.Token);
        data.messageCount++;
        UserSession.addComment(data,userData.userId, true);
    }

    public async UniTask AddMessageTask(CancellationToken token)
    {
        string text = inputText.text;
        if(text.Equals(String.Empty))
            return;
            
        Message message = new Message(text);
        message.artworkId = data.artworkId;
        message.commentId = data._id;
        message.userId = userData.userId;
        DateTime localDate = DateTime.Now;
        message.date = localDate.ToString();

        await APIManager.PutMessage(message.artworkId, message.commentId, message, token);

        inputText.text = "";
    }

    public int GetMessagesCount()
    {
        return messages.Count;
    }

    public int GetItemsCount()
    {
        return _messageGOUpdater.Count();
    }

    public async UniTask DeleteMessageAtIndex(int index)
    {
        CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellation.Token, ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5));
        await DeletingItem(index, linked.Token);
    }

    public void OnNotified(Colleague colleague)
    {
        if (colleague == _serverMessages)
            HandleServerMessagesNotified();
    }

    private void HandleServerMessagesNotified()
    {
        Message[] messageRes = _serverMessages.GetCurrentServerMessages();
        
        if(messageRes != null)
        {
            CheckMessages(messageRes.ToList());
            
            loadNextPageButton.transform.parent.SetAsLastSibling();
            ActivateNextPageButton();
            if (_serverMessages.IsLastPageReached())
            {
                LastPageReached();
            }
        }
        else
        {
            _messageGOUpdater.ClearList();
            DestroyLoadingSign();
            ActivateNextPageButton();
            LastPageReached();
            messages.Clear();
        }

        _serverMessages.RequestServerMessages();
    }
}