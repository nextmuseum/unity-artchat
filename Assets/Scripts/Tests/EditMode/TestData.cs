using System;
using UnityEngine;
using UnityEditor;

namespace TestUtilities
{
    public static class TestData 
    {
        
        public static Comment TestComment()
        {
            /*
            Comment comment = new Comment(text: "Another good test",
                new Vector3(-0.5455071330070496f, 0.3504205048084259f, 0.9736676812171936f), 
                new Vector3(350.88525390625f, 339.3866271972656f, -0.000000216173020817223f), 
                artworkID: "60b79ec451c8f571c43d339b", 
                userID: "60b781827ccba00015ec5eec",
                _id: "60bde1fed584fd001544312e", 
                date: "2021-06-07T09:08:14.000+00:00");
            return comment;*/
            return null;
        }

        public static Message TestMessage(string content = "")
        {/*
            string text = "Unit_Test Message!!!";
            if (!String.IsNullOrEmpty(content))
                text = content;
            
            Message currentMesssage = new Message(text);
            return currentMesssage;*/
            return null;
        }

        public static Texture2D LoadQRCode(string name)
        {
            Texture2D qrCodeImage = AssetDatabase.LoadAssetAtPath($"Assets/Textures/QR-Codes/ArtworkQRs_Test/{name}", typeof(Texture2D)) as Texture2D;
            return qrCodeImage;
        }
        
    }

}