using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class Logger : MonoBehaviour
{
    public int maxMessageCount = 20;
    private TMP_Text textField;

    private int lastMessageCount;
    private static List<string> messages = new List<string>();
    private static StringBuilder stringBuilder = new StringBuilder();

    void Awake()
    {
        if(textField == null) textField = GetComponent<TMP_Text>();

        lock (messages) messages?.Clear();

        Log("Console initialized.");
    }

    void Update()
    {
        lock (messages)
        {
            if(lastMessageCount != messages.Count)
            {
                stringBuilder.Clear();
                var startIndex = Mathf.Max(messages.Count - maxMessageCount, 0);
                for(int i = startIndex; i < messages.Count; ++i)
                {
                    stringBuilder.Append($"{i:1000}> {messages[i]}\n");
                }

                var newText = stringBuilder.ToString();

                if (textField) textField.text = newText;
            }
            lastMessageCount = messages.Count;
        }
    }

    public static void Log(string message)
    {
        lock (messages)
        {
            if (messages == null) messages = new List<string>();
            messages.Add(message);
        }
    }
}
