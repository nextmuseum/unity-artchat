using System.Collections;
using System.Collections.Generic;
using ARtChat;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class CommentMessageCountDisplayTesting
    {
        [UnityTest]
        public IEnumerator Update_Should_Change_Text()
            => UniTask.ToCoroutine( async () =>
        {
            GameObject testComment = A.Prefab.BuildComment();
            CommentMessageCountDisplay commentMessageCountDisplay =
                testComment.GetComponentInChildren<CommentMessageCountDisplay>();
            TMP_Text textMeshPro = commentMessageCountDisplay.GetComponentInChildren<TMP_Text>();
            
            string beforeUpdate = textMeshPro.text;
            commentMessageCountDisplay.PerformUpdate();
            string afterUpdate = textMeshPro.text;

            beforeUpdate.Should().NotMatch(afterUpdate);
        });
}
}
