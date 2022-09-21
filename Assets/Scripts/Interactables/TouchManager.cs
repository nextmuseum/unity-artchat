using System.Collections;
using System.Collections.Generic;
using ARtChat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TouchManager : MonoBehaviour
{
    [Header("General")]
    public Camera cam;
    public LayerMask layerMask;

    [Header("UI")]
    public GameObject buttonGroup_NoSelection;
    public GameObject buttonGroup_Selection;
    public GameObject buttonGroup_Selection_NotAuthor;
    public GameObject backButton;
    public Button[] messagePageButton;

    private MovableComment currentSelection;

    private ARSceneManager sceneManager;
    private ButtonFunctionAR buttonFunctionAR;
    private bool lockSelection = false;

    private LayerMask uiMask;


    private void OnEnable() 
    {
       EventManager.uiCanvasChanged += OnUIStatusChanged;
    }

    private void OnDisable() 
    {
       EventManager.uiCanvasChanged -= OnUIStatusChanged;
    }
    void Start()
    {
        uiMask = LayerMask.GetMask("UI");
        sceneManager = FindObjectOfType<ARSceneManager>();
        buttonFunctionAR = FindObjectOfType<ButtonFunctionAR>();

        messagePageButton = new Button[2];
        messagePageButton[0] = buttonGroup_Selection.transform.Find("Comment").GetComponent<Button>();
        messagePageButton[1] = buttonGroup_Selection_NotAuthor.transform.Find("Comment").GetComponent<Button>();
        // messagePageButton[0]
    }
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if(lockSelection)
            return;

        // if(EventSystem.current.IsPointerOverGameObject()) return;
        // if(EventSystem.current.currentSelectedGameObject != null) return;
        if(IsPointerOverUIObjectV2(Input.mousePosition))
            return;
        
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, 50f, layerMask))
        {
            // Debug.Log("Deselecting, cause new Selection");
            if (currentSelection != null)
            {
                if (currentSelection.comment.userId == ARSceneManager.userData.userId)
                {
                   currentSelection.GetComponent<ShowARFloorPlaneConnection>() .Hide();
                }
                currentSelection.deselect();
            }
            
            currentSelection = hitInfo.transform.GetComponent<MovableComment>();
            Debug.Log(currentSelection.ToString());
            currentSelection.select();
            buttonGroup_NoSelection.SetActive(false);
            
            if (currentSelection.comment.userId == ARSceneManager.userData.userId)
            { 
                buttonGroup_Selection.SetActive(true); 
                buttonGroup_Selection_NotAuthor.SetActive(false); 
                hitInfo.transform.GetComponent<ShowARFloorPlaneConnection>().Show();
            }
            else
            {
                buttonGroup_Selection_NotAuthor.SetActive(true);
                buttonGroup_Selection.SetActive(false);
            }
            Debug.Log("Current selection != null");
            StartCoroutine(ActivateMessagePageButtonIfCommentIsPosted());
            backButton.SetActive(false);
        }
        else
        {
            // Debug.Log("Calling Is Pointer over ui object");
            if (IsPointerOverUIObject()) return;
            // Debug.Log($"current selection is {currentSelection}");
            if (currentSelection == null) return;
            // Debug.Log("Deselecting...");
            currentSelection.deselect();
            currentSelection.GetComponent<ShowARFloorPlaneConnection>() .Hide();
            currentSelection = null;
            buttonGroup_NoSelection.SetActive(true);
            buttonGroup_Selection.SetActive(false);
            buttonGroup_Selection_NotAuthor.SetActive(false);
            backButton.SetActive(true);
            // Debug.Log("Activating Buttons");
            Debug.Log("Current selection == null");

        }
    }

    public void EditSelection()
    {
        currentSelection.startTextEdit();
    }

    public void UpdateSelection(MovableComment caller)
    {
        if (currentSelection != null && caller.comment._id == currentSelection.comment._id)
            UpdateSelection();
    }

    public void UpdateSelection()
    {
        if (currentSelection.comment._id != null) currentSelection.Post();
        else currentSelection.Put();
    }

    public async void DeleteSelection()
    {
        if(await currentSelection.Delete())
        {
            StopAllCoroutines();
            currentSelection = null;
            buttonGroup_NoSelection.SetActive(true);
            buttonGroup_Selection.SetActive(false);
            buttonGroup_Selection_NotAuthor.SetActive(false);
        }
    }

    public async void DeleteComment(MovableComment _movableComment)
    { 
        bool success = await _movableComment.Delete();
        if (!success) return;
        
        StopAllCoroutines();
        currentSelection = null;
        buttonGroup_NoSelection.SetActive(true);
        buttonGroup_Selection.SetActive(false);
        buttonGroup_Selection_NotAuthor.SetActive(false);
    }

    public void LoadMessagePageOfSelection()
    {
        buttonFunctionAR.changeARMessagePageStatus();
        sceneManager.LoadMessagePage(currentSelection.comment);
    }

    // http://answers.unity.com/answers/1115473/view.html
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private bool IsPointerOverUIObjectV2(Vector2 pos)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if(results.Count <= 1)
            return false;

        bool overAInteractableComment = false;
        bool overAnotherUIObject = false;
        foreach(RaycastResult res in results)
        {
            // Skip interactable Items -> Comment Objects which are 3D placed Canvas Objects
            if(res.gameObject.layer == LayerMask.NameToLayer("Interactible"))
            {
                overAInteractableComment = true;
                continue;
            }

            if(res.gameObject.GetComponent<CanvasRenderer>())
            {
                overAnotherUIObject = true;
            }
        }

        return overAInteractableComment && overAnotherUIObject;
    }

    private IEnumerator ActivateMessagePageButtonIfCommentIsPosted()
    {
        Debug.Log("Page Button Length"+messagePageButton.Length);
        foreach (Button button in messagePageButton)
        {
            Debug.Log(button.name);
            button.interactable = false;
        }
        Debug.Log("currentSelection = " + currentSelection);
        Debug.Log("currentSelection = " + currentSelection.ToString());
        yield return new WaitUntil(() => (currentSelection) != null && currentSelection.comment._id != null);
        foreach (Button button in messagePageButton)
        {
            button.interactable = true;
        }
    }

    private void OnUIStatusChanged(bool status)
    {
        lockSelection = status;
    }
}