using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class EditorTouchScreenKeyboard : KeyboardAccess
    {
        private string _alphabet = "abcdefghijklmnopqrstuvwxyz";
        public EditorTouchScreenKeyboard(String initialText) : base()
        {
            Text = initialText;
            MonoUtilities.Instance.StartCoroutine(RecordKeyboardInput());
        }

        IEnumerator RecordKeyboardInput()
        {
            while (Active)
            {
                // Debug.Log("checking keyboard input");
                foreach (char c in _alphabet)
                {
                    if (Input.GetKeyDown(c.ToString()))
                    {
                        // Debug.Log($"{c} was pressed");
                        Text += c;
                        Debug.Log($"Text = {Text}");
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Text += " ";
                }
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    Text = Text.Remove(Text.Length-1);
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    // Debug.Log($"Return was pressed");
                    Active = false;
                }
                yield return null;
            }
        }
    }
}
