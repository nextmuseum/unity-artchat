using System.Threading;
using ARtChat.Templates;
using UnityEngine;
using UniTask = Cysharp.Threading.Tasks.UniTask;

public class ServerMessages : Colleague
{
    private readonly Comment _currentComment;
    private readonly int _initLimit;
    private readonly int _updateLimit;
    
    private int _page;
    private int RequestedAmount => Mathf.Clamp(_initLimit + _updateLimit * _page, 0, _messageCount);
    
    
    private Message[] _currentServerMessages;
    private int _messageCount = 0;

    private readonly CancellationTokenSource _destructionCancellation;

    public ServerMessages(Director director, Comment comment, int initialLimit=15, int updateLim=10) : base(director)
    {
        _currentComment = comment;
        _destructionCancellation = new CancellationTokenSource();
        _initLimit = initialLimit;
        _updateLimit = updateLim;
    }
    
    ~ServerMessages()
    {
        _destructionCancellation.Cancel();
    }

    public void Dispose()
    {
        _destructionCancellation.Cancel();
    }


    public void RequestServerMessages()
    {
        GetServerMessages();
    }

    public Message[] GetCurrentServerMessages()
    {
        return _currentServerMessages;
    }

    public void IncrementPage()
    {
        _page++;
    }
    
    public bool IsLastPageReached()
    {
        return RequestedAmount == _messageCount;
    }
    
    
    
    async void GetServerMessages()
    {
        await UpdateCount();
        await PullCurrentServerMessages();
        NotifyDirector();
    }

    async UniTask UpdateCount()
    {
        _messageCount = await APIManager.GetMessageCount(_currentComment._id, GetLinkedToken());
    }

    CancellationToken GetLinkedToken(float time = 5)
    {
        CancellationToken timer = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
        return CancellationTokenSource.CreateLinkedTokenSource(_destructionCancellation.Token, timer).Token;
    }


    async UniTask PullCurrentServerMessages()
    {
        int limit = RequestedAmount;
        int skip = 0;
        _currentServerMessages = await APIManager.GetMessage(_currentComment.artworkId, _currentComment._id, GetLinkedToken(), 
                                                            limit, skip);
    }
}