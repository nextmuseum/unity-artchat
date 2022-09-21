using System.Collections;
using ARtChat.Utility;
using FluentAssertions;
using NUnit.Framework;
using TestUtilities;
using UnityEngine.TestTools;
using UniTask = Cysharp.Threading.Tasks.UniTask;

public class ServerMessagesTesting
{
    public class RequestServerMessageMethod
    {
        private User testUser;
        private Comment testComment;

        private async UniTask SetupTest()
        {
            testUser = await DBSetupHelper.LoginUnitTestUser();
            testComment = await DBSetupHelper.CreateTestComment(testUser);
        }

        public async UniTask TearingDownTests()
        {
            await DBSetupHelper.DeleteAllComments(testUser);
        }

        [UnityTest]
        public IEnumerator _0_Requesting_Should_Notify_Director() => UniTask.ToCoroutine(
            async () =>
            {
                await SetupTest();
                
                DummyDirector dummyDirector = new DummyDirector();
                ServerMessages serverMessages = new ServerMessages(dummyDirector, testComment);

                serverMessages.RequestServerMessages();
                await UniTask.Delay(5000);

                dummyDirector.CountNotified.Should().BeGreaterThan(0);

                await TearingDownTests();
            });
    }
}