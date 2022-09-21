using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ARtChat.Utility;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARtChat
{
    public class CommentMessageCountDisplay : MonoBehaviour
    {
        public float updateTime = 5f;
        public int maxCount = 100;
        public string maxCountDisplay = "99+";
        
        private TMP_Text _tmpText;
        private MovableComment _thisMovableComment;
        private Image _bgImage;
        private GameObject[] _children;
        private int messageCount = 0;
        private float lastTime = 0;

        private CancellationTokenSource _destroyCancellation;
        private void Awake()
        {
            _tmpText = GetComponentInChildren<TMP_Text>();
            _thisMovableComment = GetComponentInParent<MovableComment>();
            _destroyCancellation = new CancellationTokenSource();
            _bgImage = GetComponent<Image>();

            FindAndInitChildren();
        }

        private void FindAndInitChildren()
        {
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            if (childTransforms.Length <= 1) return;

            List<GameObject> theChildren = new List<GameObject>();
            for (int i = 0; i < childTransforms.Length; i++)
            {
                if(childTransforms[i].gameObject == this.gameObject)
                    continue;
                theChildren.Add(childTransforms[i].gameObject);
            }

            _children = theChildren.ToArray();
        }

        private void Start()
        {
            DeactivateDisplay();
            StartCoroutine(ContinuouslyUpdate());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            if (_destroyCancellation == null) return;
            _destroyCancellation.Cancel();
            _destroyCancellation = null;
        }

        private IEnumerator ContinuouslyUpdate()
        {
            while (true)
            {
                float timeSinceUpdate = Time.time - lastTime;
                if (timeSinceUpdate < updateTime) yield return null;
                
                yield return PerformUpdate().ToCoroutine();

                if (messageCount == 0)
                {
                    DeactivateDisplay();
                }
                else if (!_bgImage.enabled)
                {
                    ActivateDisplay();
                }
            } 
        }

        private void DeactivateDisplay()
        {
            _bgImage.enabled = false;
            foreach (GameObject child in _children)
            {
               child.SetActive(false);
            }
        }
        private void ActivateDisplay()
        {
            _bgImage.enabled = true;
            foreach (GameObject child in _children)
            {
               child.SetActive(true);
            }
        }

        public async UniTask PerformUpdate()
        {
            await UpdateCount();
            UpdateText();
        }
        private async UniTask UpdateCount()
        {
            CancellationToken timedToken = ARtChat.Utility.UniTaskHelper.GetTimeoutTokenFromSeconds(5);
            CancellationTokenSource linkedToken = CancellationTokenSource.CreateLinkedTokenSource(timedToken, _destroyCancellation.Token);
            messageCount = await APIManager.GetMessageCount(_thisMovableComment.comment._id, linkedToken.Token);
        }

        private void UpdateText()
        {
            if (messageCount >= maxCount)
            {
                _tmpText.text = maxCountDisplay;
                return;
            }
            _tmpText.text = messageCount.ToString();
        }
    }
}
