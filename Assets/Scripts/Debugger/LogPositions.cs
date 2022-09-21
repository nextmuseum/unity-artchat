using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPositions : MonoBehaviour
{
    public List<Transform> objectsToLog;

    private void Update()
    {
        foreach (Transform go in objectsToLog)
        {
            Debug.Log($"Position = {go.position} from {go.name}");
        }
    }
}
