using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Debugger
{
    public static class Logger 
    {
        public static void Red(string logMessage)
        {
            Debug.Log($"<color=red> {logMessage} </color>");
        }
        public static void Green(string logMessage)
        {
            Debug.Log($"<color=green> {logMessage} </color>");
        }
        public static void Blue(string logMessage)
        {
            Debug.Log($"<color=blue> {logMessage} </color>");
        }
    }
}
