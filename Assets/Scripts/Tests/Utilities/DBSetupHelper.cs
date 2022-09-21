using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TestUtilities
{
    public class DBSetupHelper
    {
        public const string ArtworkID = "60efe3678ff8372a39c2b8f1";
        public static async UniTask<User> LoginUnitTestUser()
        {
            /*CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            string testUsername = "Unit_Test";
            string testPw = "tmp";
            User userData = await APIManager.LoginUser(testUsername, testPw, cancellationToken);
            // Debug.Log(userData);
            if(userData == null)
            {
                //await APIManager.RegisterUser(testUsername, testPw, cancellationToken);
                //userData = await APIManager.LoginUser(testUsername, testPw, cancellationToken);
            }
            
            return userData;*/
            return null;
        }

        public static async UniTask<Comment> CreateTestComment(User _user)
        {
            Comment testComment = new Comment("Unit_Test Comment", Vector3.zero, Vector3.zero);
            Comment response = await APIManager.PutComment(ArtworkID, testComment);
            
            if (response == null)
                return null;
            return response;
        }

        public static async UniTask DeleteAllComments(User _user)
        {
            List<UniTask> jobs = new List<UniTask>();
            Comment[] allUnitTestComments = await GetAllComments();
            foreach (Comment unitTestComment in allUnitTestComments)
            {
                jobs.Add(APIManager.DeleteComment(ArtworkID, unitTestComment._id));
            }
            await UniTask.WhenAll(jobs);
        }

        public static async UniTask<Comment[]> GetAllComments()
        {
            CancellationToken ctx = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            Comment[] allUnitTestComments = await APIManager.GetComment(ArtworkID, ctx);
            return allUnitTestComments;
        }

        public static async UniTask<Message[]> GetAllMessages(Comment comment)
        {
            CancellationToken ctx = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            Message[] allMessages = await APIManager.GetMessage(ArtworkID, comment._id, ctx, limit:15);
            return allMessages;
        }

        public static async UniTask DeleteAllMessages(Comment comment, User user)
        {
            // Debugger.Logger.Blue("Deleting All Messages!!!");
            Message[] allMessages = await GetAllMessages(comment);
            if (allMessages == null)
                return;

            List<UniTask> jobs = new List<UniTask>();
            for (int i = 0; i < allMessages.Length; i++)
            {
                CancellationToken ctx = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                jobs.Add(APIManager.DeleteMessage(comment.artworkId, comment._id, allMessages[i]._id, ctx));
            }

            await UniTask.WhenAll(jobs);
        }
        
    }
}
