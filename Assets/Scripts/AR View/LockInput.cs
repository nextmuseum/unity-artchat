using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LockInput : MonoBehaviour
{
    private IEnumerator lockJob = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool _lock = false;

    private void OnEnable()
    {

        if (lockJob != null)
        {
            Debug.Log("LOCKINPUT Kill previous Lock");

            StopCoroutine(lockJob);
            lockJob = null;
        }

        _lock = false;
        this.GetComponent<Button>().interactable = true;

    }

    public void Lock()
    {
        if (_lock == false)
        {
            Debug.Log("LOCKINPUT Start Lock");
            this.GetComponent<Button>().interactable = false;
            _lock = true;

            if (lockJob != null) {
                Debug.Log("LOCKINPUT Kill previous Lock");

                StopCoroutine(lockJob);
                lockJob = null;
            }
            lockJob = LockThis();
            StartCoroutine(lockJob);
        }
    }

    private IEnumerator LockThis()
    {
        yield return new WaitForSeconds(4.0f);
        _lock = false;
        this.GetComponent<Button>().interactable = true;
        lockJob = null;
        Debug.Log("LOCKINPUT Stop Lock");

    }
}
