using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageScrollButton : MonoBehaviour
{

    public RectTransform scrollContent;
    public float timeForScrolling = 1f;

    [Tooltip("float value to overshoot init Pos. Used to create Bouncy visual effect with help from scrollrect")]
    public float overshoot = 100f;

    private Coroutine animationRoutine = null;
    private Vector2 initPos = Vector2.zero;

    private void Start()
    {
        // initPos = scrollContent.anchoredPosition - new Vector2(0, overshoot);
        initPos = new Vector2(0, overshoot);
        // Debug.Log(scrollContent.anchoredPosition);
    }
    public void ScrollToFirst()
    {
        if(animationRoutine == null)
            animationRoutine = StartCoroutine(AnimateScrolling(initPos));
    }

    private IEnumerator AnimateScrolling(Vector2 targetPos)
    {
        bool isAnimating = true;
        Vector2 startPos = scrollContent.anchoredPosition;
        float t = 0;
        float startTime = Time.time;
        Debug.Log($"startPos = {startPos} and targetPos = {targetPos}");

        while(isAnimating)
        {
            float currentTime = Time.time;
            t = (currentTime - startTime) / timeForScrolling;
            t = Mathf.Clamp01(t);

            Vector2 pos = Vector2.Lerp(startPos, targetPos, t);
            scrollContent.anchoredPosition = pos;

            if(t >= 0.99f)
            {
                scrollContent.anchoredPosition = targetPos;
                isAnimating = false;
                animationRoutine = null;
                gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }

    }
}
