using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Immersal.AR;

public class ObjectBasedMapManager : ARMapManager
{
    private GameObject currentActiveMap;
    private Dictionary<int, GameObject> iDToObject = new Dictionary<int, GameObject>();

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        
        ARMap[] aRMaps = GetComponentsInChildren<ARMap>(true);
        ARMap.mapHandleToMap.Clear();

        foreach(ARMap aRMap in aRMaps)
        {
            aRMap.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            
            // aRMap.LoadMap();
            // Debug.Log($"map id = {aRMap.mapHandle} from object = {aRMap.gameObject.name}");
            iDToObject.Add(aRMap.mapId, aRMap.gameObject);

            if(aRMap.GetComponentInChildren<Canvas>() == null)
            {
                createCanvas(aRMap.gameObject);
            }
            aRMap.gameObject.SetActive(false);
            // aRMap.FreeMap();
        }

        // foreach (var map in ARMap.mapHandleToMap)
        // {
        //     Debug.Log($"mapHandles = {map.Key};;; {map.Value}");
        // }
        
        foreach (var mapid in Immersal.ImmersalSDK.Instance.Localizer.mapIds)
        {
            Debug.Log(mapid.id);
        }
        // Debugger.Logger.Red("MapID Count Is: " + iDToObject.Count.ToString());
        // Logger.Log("MapID Count Is: " + iDToObject.Count.ToString());
    }
    public override async UniTask<GameObject> Activate(int mapID)
    {
        if(!iDToObject.ContainsKey(mapID))
        {
            Debugger.Logger.Red("MapID Not Found. - using Fallback");
            currentActiveMap = FallbackCanvas();
            return currentActiveMap;
        }

        if(currentActiveMap != null)
        {
            currentActiveMap.SetActive(false);
        }

        GameObject aRMap = iDToObject[mapID];

        // currentActiveMap.GetComponent<ARMap>().LoadMap();
        currentActiveMap = aRMap;
        currentActiveMap.SetActive(true);
        currentActiveMap.GetComponent<ARMap>().LoadMap(null, mapID);
        // currentActiveMap.GetComponent<ARMap>().LoadMap();

        // Debug.Log($"Activated Map {currentActiveMap.name}");
        return currentActiveMap.transform.GetChild(0).gameObject;
    }

    public override void Unload()
    { 
        // currentActiveMap.GetComponent<ARMap>().FreeMap();
        currentActiveMap?.SetActive(false); 
    }
    
}
