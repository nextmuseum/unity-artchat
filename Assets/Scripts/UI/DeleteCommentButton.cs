using UnityEngine;
using UnityEngine.UI;

namespace ARtChat
{
    public class DeleteCommentButton : MonoBehaviour
    {
        private Button _deleteButton;
        private MovableComment _thisMovableComment;
        private TouchManager _touchManager;

        private void Awake()
        {
            _deleteButton = GetComponent<Button>();
            _touchManager = FindObjectOfType<TouchManager>();
            _thisMovableComment = transform.parent.GetComponent<MovableComment>();
        }

        private void OnEnable()
        {
            _deleteButton.onClick.AddListener(DeleteThisComment);
        }

        private void OnDisable()
        {
            _deleteButton.onClick.RemoveListener(DeleteThisComment);
        }

        private void DeleteThisComment()
        {
            _touchManager.DeleteComment(_thisMovableComment);
        }
    }
}
