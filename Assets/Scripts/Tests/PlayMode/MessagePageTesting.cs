using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class MessagePageTesting
    {
        private MessagePage messagePage;
        private User testingUser;
        private Comment testComment;

        public async UniTask InitMessagePage()
        {
            if (messagePage == null)
            {
                GameObject canvasObject = A.GameObject.WithCanvas();
                GameObject messagePageObject = A.Prefab.BuildMessagePage().WithInstance().WithParentObject(canvasObject.transform);
                messagePage = messagePageObject.GetComponent<MessagePage>();
            }
            
            testingUser = await TestUtilities.DBSetupHelper.LoginUnitTestUser();
            testComment = await TestUtilities.DBSetupHelper.CreateTestComment(testingUser);
            messagePage.SetupPage(testComment, testingUser);
        }

        public async UniTask CleanUpMessagePage()
        {
            await TestUtilities.DBSetupHelper.DeleteAllMessages(testComment, testingUser);
            await TestUtilities.DBSetupHelper.DeleteAllComments(testingUser);
        }
        
        public class AddingMessages : MessagePageTesting
        {
            [UnitySetUp]
            public IEnumerator InitScene() => UniTask.ToCoroutine(async () =>
            {
                await InitMessagePage();
            });

            [UnityTearDown]
            public IEnumerator CleanUp() => UniTask.ToCoroutine(async () =>
            {
                await CleanUpMessagePage();
            });

            [UnityTest]
            public IEnumerator After_Initialisation_Member_Should_Not_Be_Null() => UniTask.ToCoroutine(async () => 
                {
                    testingUser.Should().NotBeNull();
                    messagePage.Should().NotBeNull();
                    testComment.Should().NotBeNull();
                }
            );


            [UnityTest]
            public IEnumerator Adding_Message_Using_APIManager_Should_Increase_Count() => UniTask.ToCoroutine(async () =>
                {
                    Message[] messagesBefore = await TestUtilities.DBSetupHelper.GetAllMessages(testComment);
                    Message testMessage = new Message("Unit_Test API Message");

                    CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                    Message postMessage = await APIManager.PutMessage(testComment.artworkId, testComment._id, testMessage, cancellationToken);

                    Message[] messagesAfter = await TestUtilities.DBSetupHelper.GetAllMessages(testComment);

                    if (messagesBefore == null)
                    {
                        messagesAfter.Should().HaveCount(1);
                    }
                    else
                    {
                        messagesAfter.Should().HaveCount(messagesBefore.Length + 1);
                    }
                }
            );

            [UnityTest]
            public IEnumerator Adding_Single_Message_Should_Increase_Message_Count() => UniTask.ToCoroutine(async () =>
                {
                    messagePage.inputText.text = "Unit_Test Message!!!";

                    int lastMessageCount = messagePage.GetMessagesCount();
                    int lastItemCount = messagePage.GetItemsCount();

                    CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                    await messagePage.AddMessageTask(cancellationToken);
                    
                    await UniTask.Delay(2000);
                    // await UniTask.WaitUntil(() => !messagePage.IsLoadingMessagePage);
                    

                    int currentMessageCount = messagePage.GetMessagesCount();
                    int currentItemCount = messagePage.GetItemsCount();

                    int expectedMessageCount = lastMessageCount + 1;
                    int expectedItemCount = lastItemCount + 1;

                    currentItemCount.Should().Be(expectedItemCount);
                    currentMessageCount.Should().Be(expectedMessageCount);
                }
            );

            [UnityTest]
            public IEnumerator Adding_Twenty_Messages_Should_Increase_Count_Up_To_Page_Limit() => UniTask.ToCoroutine(async () =>
            {

                int lastMessageCount = messagePage.GetMessagesCount();
                int lastItemCount = messagePage.GetItemsCount();

                List<UniTask> jobs = new List<UniTask>();
                for (int i = 0; i < 20; i++)
                {
                    messagePage.inputText.text = $"Unit_Test Message!!! {i}";
                    CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                    jobs.Add(messagePage.AddMessageTask(cancellationToken));
                }

                await UniTask.WhenAll(jobs);
                // await UniTask.WaitUntil(() => !messagePage.IsLoadingMessagePage);
                await UniTask.Delay(4000);
                
                // Debug.Log("Done Pushing Messages");
                
                int currentMessageCount = messagePage.GetMessagesCount();
                int currentItemCount = messagePage.GetItemsCount();

                // int expectedMessageCount = lastMessageCount + 20;
                // int expectedItemCount = lastItemCount + 20;
                
                int expectedMessageCount = messagePage.initLimit;
                int expectedItemCount = messagePage.initLimit;
                
                currentItemCount.Should().Be(expectedItemCount);
                currentMessageCount.Should().Be(expectedMessageCount);
            });

        }

        public class DeletingMessage : MessagePageTesting
        {
            [UnitySetUp]
            public IEnumerator InitScene() => UniTask.ToCoroutine(async () =>
            {
                await InitMessagePage();
            });

            [UnityTearDown]
            public IEnumerator CleanUp() => UniTask.ToCoroutine(async () =>
            {
                await CleanUpMessagePage();
            });

            public async UniTask GenerateTestMessages(int amount)
            {
                List<UniTask> jobs = new List<UniTask>();
                for (int i = 0; i < amount; i++)
                {
                    messagePage.inputText.text = $"Unit_Test Message!!! {i}";
                    CancellationToken cancellationToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
                    jobs.Add(messagePage.AddMessageTask(cancellationToken));
                    // await UniTask.Delay(1000);
                }

                await UniTask.WhenAll(jobs);
                await UniTask.Delay(2000);
            }

            [UnityTest]
            public IEnumerator DeletingMessage_Should_Reduce_Count() => UniTask.ToCoroutine(async () =>
            {
                await GenerateTestMessages(5);
                
                int itemCountBefore = messagePage.GetItemsCount();
                int messageCountBefore = messagePage.GetMessagesCount();

                await messagePage.DeleteMessageAtIndex(4);
                await UniTask.Delay(1000);

                int itemCountAfter = messagePage.GetItemsCount();
                int messageCountAfter = messagePage.GetMessagesCount();

                itemCountAfter.Should().Be(itemCountBefore - 1);
                messageCountAfter.Should().Be(messageCountBefore - 1);
                
                // Debugger.Logger.Green("Test Done");
            });

            [UnityTest]
            public IEnumerator Deleting_One_Item_Should_Only_Delete_This_Item() => UniTask.ToCoroutine(async () =>
            {
                await GenerateTestMessages(5);

                List<Message> messagesBefore = messagePage.Messages;

                int removingIndex = 2;
                await messagePage.DeleteMessageAtIndex(removingIndex);
                await UniTask.Delay(1000);

                List<Message> messagesExpected = messagesBefore;
                messagesExpected.RemoveAt(removingIndex);
                
                List<Message> messagesAfter = messagePage.Messages;

                messagesAfter.Should().Equal(messagesExpected);
            });
            
            [UnityTest]
            public IEnumerator Delete_First_Should_Make_Second_First_Message() => UniTask.ToCoroutine(async () =>
            {
                await GenerateTestMessages(5);

                Message secondMessageBefore = messagePage.Messages[1];

                await messagePage.DeleteMessageAtIndex(0);
                await UniTask.Delay(4000);

                Message firstMessageAfter = messagePage.Messages[0];
                
               secondMessageBefore.Should().BeEquivalentTo(firstMessageAfter); 
            });
        }
    }
}
