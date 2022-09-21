using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FluentAssertions;
using UnityEngine.UI;

public class APIManagerTesting
{
    public class AddingMessages
    {
        private User _testingUser;
        private Comment _testingComment;

        private async UniTask Init()
        {
            _testingUser = await TestUtilities.DBSetupHelper.LoginUnitTestUser();
            _testingComment = await TestUtilities.DBSetupHelper.CreateTestComment(_testingUser);
        }

        private async UniTask Cleanup()
        {
            await TestUtilities.DBSetupHelper.DeleteAllMessages(_testingComment, _testingUser);
            await TestUtilities.DBSetupHelper.DeleteAllComments(_testingUser);
        }

        [UnityTest]
        public IEnumerator Adding_A_Message_Should_Increase_Count()
            => UniTask.ToCoroutine(async () =>
            {
                await Init();
                int beforeMessageCount = await GetCurrentMessageCount();
                
                Message currentMesssage = TestUtilities.TestData.TestMessage();
                CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                await APIManager.PutMessage(_testingComment.artworkId, _testingComment._id, currentMesssage, cancellationToken);
                
                int afterMessageCount = await GetCurrentMessageCount();
                
                beforeMessageCount.Should().BeLessThan(afterMessageCount);

                await Cleanup();
            });

        private async UniTask<int> GetCurrentMessageCount()
        {
            CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            int messageCount = await APIManager.GetMessageCount(_testingComment._id, cancellationToken);
            return messageCount;
        }
    }

    public class DeletingMessages
    {
        private User testingUser;
        private Comment testComment;

        public async UniTask InitDB()
        {
            testingUser = await TestUtilities.DBSetupHelper.LoginUnitTestUser();
            testComment = await TestUtilities.DBSetupHelper.CreateTestComment(testingUser);
        }

        public async UniTask CleanDB()
        {
            await TestUtilities.DBSetupHelper.DeleteAllMessages(testComment, testingUser);
            await TestUtilities.DBSetupHelper.DeleteAllComments(testingUser);
        }
        public async UniTask GenerateTestMessages(int amount)
        {
            List<UniTask> jobs = new List<UniTask>();
            for (int i = 0; i < amount; i++)
            {
                string text = $"Unit_Test Message!!! {i}";
                Message currentMesssage = new Message(text);
                CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                jobs.Add(APIManager.PutMessage(testComment.artworkId, testComment._id, currentMesssage, cancellationToken));
            }

            await UniTask.WhenAll(jobs);
            await UniTask.Delay(2000);
        }

        [UnityTest]
        public IEnumerator Deleting_First_Message_Got_Should_Make_Second_First() => UniTask.ToCoroutine(async () =>
        {
            await InitDB();
            await GenerateTestMessages(20);
            
            Message[] allMessagesBefore = await TestUtilities.DBSetupHelper.GetAllMessages(testComment);
            
            CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            await APIManager.DeleteMessage(testComment.artworkId, testComment._id, allMessagesBefore[0]._id,
                cancellationToken);

            Message[] allMessagesAfter = await TestUtilities.DBSetupHelper.GetAllMessages(testComment);

            allMessagesBefore[1].Should().BeEquivalentTo(allMessagesAfter[0]);
            await CleanDB();
        });

    }
}