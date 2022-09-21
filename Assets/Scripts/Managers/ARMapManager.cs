using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class ARMapManager : MonoBehaviour
{
    protected GameObject fallbackCanvasObj;
    public abstract UniTask<GameObject> Activate(int mapID);
    public abstract void Unload();

    public GameObject FallbackCanvas()
    {
        if(fallbackCanvasObj == null)
            CreateFallbackCanvas();
        return fallbackCanvasObj;
    }
    protected void CreateFallbackCanvas()
    {
        fallbackCanvasObj = new GameObject("fallbackCanvas");
        createCanvas(fallbackCanvasObj);
        fallbackCanvasObj = fallbackCanvasObj.transform.GetChild(0).gameObject;
    }
    protected void createCanvas(GameObject parent)
    {
        GameObject canvasObj = new GameObject();
        Canvas canvas = canvasObj.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // canvasObj.transform.parent = parent.transform;
        canvasObj.transform.SetParent(parent.transform, false);
        canvasObj.layer = LayerMask.NameToLayer("UI");

        canvasObj.name = "CommentsCanvas";
    }
}
