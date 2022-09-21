using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

namespace ARtChat
{
    public class CommentWatcher : MonoBehaviour
    {
        public WhatsNewPage whatsNewPage;
        public Dictionary<string, int> commentMessageCount;
        public Dictionary<string, List<Comment>> artworkMapper;

        public Comment[] comments;
        public Comment[] databaseComments;

        private CancellationTokenSource destroyCancellationTokenSource = new CancellationTokenSource();

        public delegate void OnLoadItemsFinishedDelegate();
        public OnLoadItemsFinishedDelegate onLoadItemsFinishedListener;

        private int newMessagesCount;
        private User userData;

        public void Init()
        {
            Refresh();
            //refreshRoutine();
        }

        public void setUserData(User _userData)
        {
            this.userData = _userData;
        }

        public void refreshRoutine()
        {
            RefreshCoroutine();
        }

        public async void RefreshCoroutine()
        {
#if UNITY_ANDROID

            while (true)
            {
                await Refresh();
                //notification

                string[] titles = new string[] { "Gro?artige Neuigkeiten", "...wusstest du schon?", "Tolle Nachrichten", "Das solltest du nicht verpassen" };
                if (newMessagesCount > 0 && !whatsNewPage.gameObject.activeSelf)
                {
                    int index = UnityEngine.Random.Range(0, 3);
                    var notification = new AndroidNotification();
                    notification.Title = titles[index];
                    notification.Text = "Du hast " + newMessagesCount + " neue Nachrichten in ARt chat";
                    notification.SmallIcon = "icon_0";
                    notification.LargeIcon = "icon_1";
                    notification.FireTime = System.DateTime.Now.AddSeconds(2f);

                    AndroidNotificationCenter.SendNotification(notification, "ARtchat");
                }
                Debug.Log("REFRESh finished");
                await Task.Delay(TimeSpan.FromSeconds(10f));
            }
#endif
        }

        public Comment getCommentById(string id)
        {
            foreach (Comment c in comments)
                if (c._id.Equals(id))
                    return c;
            return null;
        }

        async public UniTask Refresh()
        {
            newMessagesCount = 0;
            commentMessageCount = new Dictionary<string, int>();
            artworkMapper = new Dictionary<string, List<Comment>>();

            //list of updated comments
            List<Comment> refreshList = new List<Comment>();
            comments = UserSession.loadFollowingComments(userData.userId);

            if (comments.Length > 0)
            {
                foreach (Comment oldComments in comments)
                {
                    commentMessageCount[oldComments._id] = oldComments.messageCount;
                }

                string[] commentIds = new string[comments.Length];
                for (int i = 0; i < comments.Length; i++)
                    commentIds[i] = comments[i]._id;

                CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(50f);
                CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
                databaseComments = await APIManager.GetComment(commentIds, combined.Token);

                for (int j = 0; j < databaseComments.Length; j++)
                {
                    try
                    {
                        CancellationTokenSource combinedMessageCount = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
                        databaseComments[j].messageCount = await APIManager.GetMessageCount(databaseComments[j]._id, combinedMessageCount.Token);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(databaseComments[j] + " not null?");
                        Debug.LogError(e.StackTrace);
                        UIManager.toggleLoadingScreen(false);
                        UIManager.loadAccountPage();
                        UIManager.ShowBadInternetError();
                    }

                    //check if message count changed since last update from database
                    if(databaseComments[j].messageCount != commentMessageCount[databaseComments[j]._id]){
                        //TODO prompt new message here
                        databaseComments[j].messageDiff = databaseComments[j].messageCount - commentMessageCount[databaseComments[j]._id];
                        //count how many comments got updated mesages
                        newMessagesCount++;
                    }

                    //check if artworkname got already pulled from database
                    if (databaseComments[j].artworkName == "" || databaseComments[j].artworkName == null)
                    {
                        try
                        {
                            CancellationTokenSource combinedArtwork = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationTokenSource.Token);
                            Artwork artwork = await APIManager.GetArtwork(databaseComments[j].artworkId, combinedArtwork.Token);
                            databaseComments[j].artworkName = artwork.title;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(databaseComments[j] + " not null? && " + combined + " not null?");
                            Debug.LogError(e.StackTrace);
                            UIManager.toggleLoadingScreen(false);
                            UIManager.loadAccountPage();
                            UIManager.ShowBadInternetError();
                        }
                    }

                    //sort comments by artworkName
                    if (artworkMapper.ContainsKey(databaseComments[j].artworkName))
                    {
                        if (!artworkMapper[databaseComments[j].artworkName].Contains(databaseComments[j]))
                        {
                            artworkMapper[databaseComments[j].artworkName].Add(databaseComments[j]);
                        }
                    }
                    else
                    {
                        artworkMapper[databaseComments[j].artworkName] = new List<Comment>();
                        artworkMapper[databaseComments[j].artworkName].Add(databaseComments[j]);
                    }

                    refreshList.Add(databaseComments[j]);

                    //remove from list for cleanup
                    comments[j] = null;
                }
                comments = refreshList.ToArray();
            }
            onLoadItemsFinishedListener?.Invoke();
        }
    }
}
