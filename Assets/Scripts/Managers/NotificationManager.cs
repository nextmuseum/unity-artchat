using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;
using UnityEngine.Android;

namespace ARtChat
{
    public class NotificationManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_ANDROID

            var channel = new AndroidNotificationChannel()
            {
                Id = "ARtchat",
                Name = "New Messages Channel",
                Importance = Importance.High,
                Description = "Shows up if new messages can be viewed",
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            /*
            var notification = new AndroidNotification();
            notification.Title = "TESTTEST";
            notification.Text = "Du hast " + 0 + " neue Nachrichten in ARt chat";
            notification.SmallIcon = "icon_0";
            notification.LargeIcon = "icon_1";
            notification.FireTime = System.DateTime.Now.AddSeconds(10f);

            int identifier = AndroidNotificationCenter.SendNotification(notification, "ARtchat");
            */
#endif


        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnApplicationFocus(bool focus)
        {
#if UNITY_ANDROID

            if (focus)
            {
                var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
                if (notificationIntentData != null)
                {
                    UIManager.loadWhatsUpPage();
                }
            }
#endif

        }

    }
}
