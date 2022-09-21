using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
    public int maxMessageCount = 1;
    public TMP_Text textField;
    public GameObject textWrapper;

    private int lastMessageCount;
    private static List<string> messages = new List<string>();
    private static StringBuilder stringBuilder = new StringBuilder();

    void Awake()
    {
        if (textField == null) textField = GetComponent<TMP_Text>();

        lock (messages) messages?.Clear();
    }

    void Update()
    {
        lock (messages)
        {
            if (lastMessageCount != messages.Count)
            {
                stringBuilder.Clear();
                var startIndex = Mathf.Max(messages.Count - maxMessageCount, 0);
                for(int i = startIndex; i < messages.Count; ++i)
                {
                    stringBuilder.Append($"{messages[i]}\n");
                }

                var newText = stringBuilder.ToString();

                if (textField) textField.text = newText;

                StartCoroutine(ShowHint(2.0f));
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

    IEnumerator ShowHint(float waitTime)
    {
        textWrapper.SetActive(true);

        yield return new WaitForSeconds(waitTime);

        textWrapper.SetActive(false);
    }
}
