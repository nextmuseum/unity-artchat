using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ARtChat
{
    public class LocalMessageGOUpdater
    {
        public enum ConfirmationAction { CONFIRMDELETE, CONFIRMREPORT };

        private readonly GameObject _messageTemplate;
        private readonly GameObject _confirmationBoxTemplate;
        private readonly GameObject _contentContainer;
        private readonly GameObject _confirmationContainer;
        private readonly User _userData;
        private readonly Color _itemHighlight;
        private readonly Color _itemDefault;
        private Action<int> deleteAction;
        private Action<int> reportAction;

        private List<GameObject> _messageGameObjects = new List<GameObject>();
        private GameObject confirmationBox;

        public LocalMessageGOUpdater(GameObject messageTemplate, GameObject contentContainer, User userData, Color itemHighlight, GameObject confirmationBoxTemplate, GameObject confirmationContainer, 
            Action<int> deleteAction, Action<int> reportAction, Color itemDefault)
        {
            _messageTemplate = messageTemplate;
            _contentContainer = contentContainer;
            _userData = userData;
            _itemHighlight = itemHighlight;
            _confirmationBoxTemplate = confirmationBoxTemplate;
            _confirmationContainer = confirmationContainer;
            this.deleteAction = deleteAction;
            this.reportAction = reportAction;
            _itemDefault = itemDefault;
        }

        public void Add(Message message, int index)
        {
            GameObject currentObj = InstantiateMessage(message, index);
            _messageGameObjects.Add(currentObj);
        }
        
        public void AddAll(List<Message> messages)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                Add(messages[i], i);
            }
        }

        public GameObject getGameObjectAt(int index)
        {
            if (_messageGameObjects[index] != null)
                return _messageGameObjects[index];
            Debug.LogError("No messageGO exists with index " + index);
            return null;
        }

        public void RemoveAt(int index)
        {
            GameObject currentObj = _messageGameObjects[index];
            _messageGameObjects.RemoveAt(index);
            Object.Destroy(currentObj);
        }

        public void UpdateAt(int index, Message message)
        {
            GameObject currentItem = _messageGameObjects[index];
            if (currentItem == null)
            {
                currentItem = InstantiateMessage(message, index);
                _messageGameObjects[index] = currentItem;
            }
            else
            {
                UpdateMessageObject(currentItem, message, index);
            }
        }

        public void ClearList()
        {
            foreach (GameObject messageGameObject in _messageGameObjects)
            {
                Object.DestroyImmediate(messageGameObject);
            }
            _messageGameObjects = new List<GameObject>();
        }

        public int Count()
        {
            return _messageGameObjects.Count;
        }


        private GameObject InstantiateMessage(Message message, int index)
        {
            GameObject currentItem = Object.Instantiate(_messageTemplate, _contentContainer.transform);
            UpdateMessageObject(currentItem, message, index);
            return currentItem;
        }
        
        void UpdateMessageObject(GameObject messageObject, Message message, int index)
        {
            if (_userData != null && message.userId.Equals(_userData.userId))
                messageObject.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text = _userData.nickname;
            else
                messageObject.transform.Find("Text - User").GetComponent<TextMeshProUGUI>().text = message.userName;

            messageObject.transform.Find("Text - Comment").GetComponent<TextMeshProUGUI>().text = message.text;
            UIManager.ParseDate(messageObject.transform.Find("Text - Date").GetComponent<TextMeshProUGUI>(), message.date);

            if(_userData != null)
            {
                if (message.reportedByUserId(_userData.userId))
                {
                    messageObject.transform.Find("Report").GetComponent<Button>().AddEventListener(index, ConfirmationAction.CONFIRMREPORT, message, SpawnDeleteConfirmation);
                    messageObject.transform.Find("Report").GetComponent<Image>().color = new Color(221f / 255f, 1f, 6f / 255f);
                }
                else
                {
                    messageObject.transform.Find("Report").GetComponent<Button>().AddEventListener(index, ConfirmationAction.CONFIRMREPORT, message, SpawnDeleteConfirmation);
                    messageObject.transform.Find("Report").GetComponent<Image>().color = Color.white;
                }


                if (message.userId == _userData.userId)
                {
                    messageObject.transform.GetComponent<Image>().color = _itemHighlight;
                    SetTextForCurrentUserMessages(messageObject, Color.white);

                    messageObject.transform.Find("Delete").GetComponent<Button>().AddEventListener(index, ConfirmationAction.CONFIRMDELETE, message, SpawnDeleteConfirmation);
                    messageObject.transform.Find("Delete").gameObject.SetActive(true);
                }
                else
                {
                    messageObject.transform.GetComponent<Image>().color = _itemDefault;
                    SetTextForCurrentUserMessages(messageObject, Color.black);

                    messageObject.transform.Find("Delete").GetComponent<Button>().onClick.RemoveAllListeners();
                    messageObject.transform.Find("Delete").gameObject.SetActive(false);
                }
            }
            
        }
        
        private void SetTextForCurrentUserMessages(GameObject currentItem, Color color)
        {
            TextMeshProUGUI[] textFields = currentItem.GetComponentsInChildren<TextMeshProUGUI>();
            foreach(TextMeshProUGUI textField in textFields)
            {
                textField.color = color;
            }
        }
        
        private void SpawnDeleteConfirmation(int index, ConfirmationAction action, Message message)
        {
            if(confirmationBox != null)
                return;
            confirmationBox = Object.Instantiate(_confirmationBoxTemplate, _confirmationContainer.transform);
            Debug.Log(action);
            switch (action)
            {
                case ConfirmationAction.CONFIRMDELETE:
                    confirmationBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Text löschen?";
                    confirmationBox.transform.GetChild(0).Find("Yes").GetComponent<Button>().AddEventListener(index, deleteAction);
                    confirmationBox.transform.GetChild(0).Find("No").GetComponent<Button>().AddEventListener(index, CancelConfirmation);
                    break;
                case ConfirmationAction.CONFIRMREPORT:
                    string text = (message.reportedByUserId(_userData.userId)) ? "Meldung aufheben?" : "Text melden?";
                    confirmationBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
                    confirmationBox.transform.GetChild(0).Find("Yes").GetComponent<Button>().AddEventListener(index, reportAction);
                    confirmationBox.transform.GetChild(0).Find("No").GetComponent<Button>().AddEventListener(index, CancelConfirmation);
                    break;
            }


            
        }
        
        public void CancelConfirmation(int index)
        {
            if(confirmationBox == null)
                return;

            Object.Destroy(confirmationBox);
            confirmationBox = null;
        }
    }
}
