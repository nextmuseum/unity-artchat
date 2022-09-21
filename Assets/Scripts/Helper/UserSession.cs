using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARtChat
{
    public class UserSession : MonoBehaviour
    {
        public static string AccessToken;
        public static string AccessExpires;
        public static string RefreshToken;

        public string _AccessToken;
        public string _AccessExpires;
        public string _RefreshToken;

        public void Start()
        {
            DontDestroyOnLoad(this);
        }

        /*
        public void Update()
        {
            _AccessToken = AccessToken;
            _AccessExpires = AccessExpires;
            _RefreshToken = RefreshToken;
        }
        */

        /// <summary>
        /// adds new comment to engagement list. if comment already is saved, it will be ignored
        /// </summary>
        /// <param name="comment"></param>

        public static void addComment(Comment comment, string userId, bool shouldOverride = false)
        {
            List<Comment> followingComments = loadFollowingCommentsAsList(userId);
            bool inserted = false;
            for (int i = 0; i < followingComments.Count; i++)
            {
                if (comment._id.Equals(followingComments[i]._id))
                {
                    if (shouldOverride)
                    {
                        followingComments[i] = comment;
                        inserted = true;
                    }
                    break;
                }
            }
            if (!inserted)
                followingComments.Add(comment);

            refreshComments(followingComments, userId);

        }

        public static void removeComment(Comment comment, string userId)
        {
            List<Comment> followingComments = loadFollowingCommentsAsList(userId);
            if (followingComments.Contains(comment))
                followingComments.Remove(comment);
            refreshComments(followingComments, userId);

        }

        public static void refreshComments(List<Comment> followingComments, string userId)
        {
            string commentString = Proyecto26.JsonHelper.ArrayToJsonString(followingComments.ToArray());
            SimpleJSON.JSONNode root = SimpleJSON.JSON.Parse(commentString);
            Debug.Log(commentString);
            PlayerPrefs.SetString("FollowingComments_" + userId, root["Items"].AsArray.ToString());
            PlayerPrefs.Save();
        }

        public static void addComments(Comment[] comment, string userId)
        {
            List<Comment> followingComments = JsonUtility.FromJson<List<Comment>>(PlayerPrefs.GetString("FollowingComments_" + userId, "[]"));
            foreach (Comment c in comment)
            {
                if (!followingComments.Contains(c))
                {
                    followingComments.Add(c);
                }
            }
            PlayerPrefs.SetString("FollowingComments_" + userId, JsonUtility.ToJson(followingComments));
            PlayerPrefs.Save();
        }

        public static void addComments(List<Comment> comment, string userId)
        {
            List<Comment> followingComments = JsonUtility.FromJson<List<Comment>>(PlayerPrefs.GetString("FollowingComments_" + userId, "[]"));
            foreach (Comment c in comment)
            {
                if (!followingComments.Contains(c))
                {
                    followingComments.Add(c);
                }
            }
            PlayerPrefs.SetString("FollowingComments_" + userId, JsonUtility.ToJson(followingComments));
            PlayerPrefs.Save();
        }



        public static Comment[] loadFollowingComments(string userId)
        {
            string savedCommentsString = PlayerPrefs.GetString("FollowingComments_" + userId, "[]");
            Debug.Log(savedCommentsString);
            if (savedCommentsString.Equals("[]"))
            {
                return new Comment[0];
            }
            else
            {
                SimpleJSON.JSONArray comments = SimpleJSON.JSON.Parse(savedCommentsString).AsArray;
                Comment[] savedComments = new Comment[comments.Count];
                for (int i = 0; i < comments.Count; i++)
                {
                    savedComments[i] = new Comment(comments[i].AsObject);
                }
                return savedComments;
            }
        }

        public static List<Comment> loadFollowingCommentsAsList(string userId)
        {
            string savedCommentsString = PlayerPrefs.GetString("FollowingComments_" + userId, "[]");
            Debug.Log(savedCommentsString);
            if (savedCommentsString.Equals("[]"))
            {
                return new List<Comment>();
            }
            else
            {
                SimpleJSON.JSONArray comments = SimpleJSON.JSON.Parse(savedCommentsString).AsArray;
                List<Comment> savedComments = new List<Comment>();
                for (int i = 0; i < comments.Count; i++)
                {
                    savedComments.Add(new Comment(comments[i].AsObject));
                }
                return savedComments;
            }
        }

    }
}
