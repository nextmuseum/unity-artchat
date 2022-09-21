using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public class KeyboardAccess
    {
        public enum Status
        {
            Visible
        }

        public Status status;
        public string Text;
        public bool Active = false;


        public KeyboardAccess()
        {
            Active = true;
        }

        ~KeyboardAccess()
        {
        }
    }
}
