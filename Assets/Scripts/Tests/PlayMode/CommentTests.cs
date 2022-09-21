using System.Collections;
using FluentAssertions;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class CommentTests
    {
        public class CreationTests
        {
            
            [UnityTest]
            public IEnumerator Adding_Comment_Should_Increase_Count() => UniTask.ToCoroutine(async () =>
                {
                    User testUser = await TestUtilities.DBSetupHelper.LoginUnitTestUser();
                    Comment[] firstComments = await TestUtilities.DBSetupHelper.GetAllComments();
                    Comment testComment = await TestUtilities.DBSetupHelper.CreateTestComment(testUser);
                    Comment[] commentsAfterAdded = await TestUtilities.DBSetupHelper.GetAllComments();

                    testComment.Should().NotBeNull();
                    commentsAfterAdded.Should().NotBeNull();

                    if (firstComments == null)
                    {
                        commentsAfterAdded.Length.Should().Be(1);
                    }
                    else
                    {
                        commentsAfterAdded.Length.Should().Be(firstComments.Length + 1);
                    }
                    
                    await TestUtilities.DBSetupHelper.DeleteAllComments(testUser);
                }
            );
        }
    }
}
