using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PullToRefresh : MonoBehaviour
{
    ScrollRect scrollRect;
    public UnityEvent action;
    
    bool allowSnapBack = false;
    TextMeshProUGUI refreshText;

    float startPositionY;
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        GetComponentInParent<WhatsNewPage>().onLoadFinishedListener += OnLoadingFinished;
        refreshText = transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if(scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        startPositionY = scrollRect.content.position.y;
    }

    private void OnDestroy()
    {
        GetComponentInParent<WhatsNewPage>().onLoadFinishedListener -= OnLoadingFinished;
    }

    // Update is called once per frame
    void Update()
    {
        float normalizedScrollValue = (startPositionY - scrollRect.content.position.y) / Screen.height;

        if (allowSnapBack)
        {
            if (normalizedScrollValue <= 0.01f)
                allowSnapBack = false;
        }

        if (!allowSnapBack && normalizedScrollValue > 0f)
        {
            refreshText.gameObject.SetActive(true);
            refreshText.transform.GetChild(0).gameObject.SetActive(false);
            refreshText.fontSize = Mathf.Clamp((600f * (normalizedScrollValue)),0f,54f);
        }

        if(!allowSnapBack && normalizedScrollValue >= 0.1f && Input.touchCount == 0)
        {
            //scrollRect.verticalNormalizedPosition = 1.299f;
            refreshText.transform.GetChild(0).gameObject.SetActive(true);
            refreshText.gameObject.SetActive(true);
            //refreshText.fontSize = 54f;
            allowSnapBack = true;
            scrollRect.StopMovement();
            scrollRect.enabled = false;
            action?.Invoke();
        }

        if (normalizedScrollValue <= 0 && scrollRect.vertical)
        {
            refreshText.gameObject.SetActive(false);
            refreshText.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnLoadingFinished()
    {
        scrollRect.enabled = true;
        scrollRect.vertical = false;
        Invoke("UnblockScrolling", 1f);
    }

    public void UnblockScrolling()
    {
        refreshText.transform.GetChild(0).gameObject.SetActive(false);
        refreshText.gameObject.SetActive(false);
        scrollRect.vertical = true;
    }
}
