using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class EventManager
{
    public static Action<bool> uiCanvasChanged;
    public static Action LoadingAppScene;
}
