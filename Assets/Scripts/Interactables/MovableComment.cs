using System.Collections;
using ARtChat;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Utility;

public class MovableComment : MonoBehaviour
{
    public int charLimit = 500;
    //  Movability
    public float startDragAfter = 0.1f;
    private float timeHold = 0f;

    //  Material
    public Color selectedColor;
    public Color authorColor;
    private Color defaultColor;

    private bool editingMovable = false;
    private bool lockMovable = false;
    private bool editingText = false;

    private Transform cam;
    private float movableDistance;

    //  Comment Item
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI commentText;

    public Comment comment = null;
    private Image commentBackground;
    private BoxCollider commentCollider;

    private TouchScreenKeyboard keyboard;
    private KeyboardAccess keyboardAccess;
    private string lastUpdatedtext = "";

    private GameObject _deleteCommentButton;

    private void Awake()
    {
        cam = Camera.main.transform;
        commentBackground = GetComponent<Image>();
        _deleteCommentButton = GetComponentInChildren<DeleteCommentButton>().gameObject;
    }

    void Update()
    {

        commentUpdate();

        keyboardUpdate();

        editTransform();
    }

    void OnEnable()
    {
        EventManager.uiCanvasChanged += onUICanvasChanged;
    }    

    void OnDisable()
    {
        EventManager.uiCanvasChanged -= onUICanvasChanged;
    }

    void OnMouseDrag()
    {
        if (comment.userId != ARSceneManager.userData.userId)
            return;
        
        timeHold += Time.deltaTime;

        if (timeHold > startDragAfter && !editingMovable)
        {
            movableDistance = Vector3.Dot(transform.position - cam.position, cam.forward) / cam.forward.sqrMagnitude;
            editingMovable = true;
        }
    }

    void OnMouseUp()
    {     
        if(editingMovable)
            FindObjectOfType<TouchManager>().UpdateSelection(this);

        timeHold = 0;
        editingMovable = false;

    }

    public async void InitComment(Comment comment)
    {
        this.comment = comment;
        usernameText.text = comment.userName;
        //UIManager.ParseUsername(usernameText, comment.userId);
        commentText.SetText(comment.text);

        await UniTask.WaitUntil(() => commentText.bounds.size != Vector3.zero);
        commentCollider = gameObject.AddComponent<BoxCollider>();
        commentCollider.center = commentBackground.rectTransform.position;
        commentCollider.size = commentBackground.rectTransform.sizeDelta;

        if (comment.userId == ARSceneManager.userData.userId)
        {
            defaultColor = authorColor;
            _deleteCommentButton.SetActive(true);
        }
        else
        {
            defaultColor = commentBackground.color;
            _deleteCommentButton.SetActive(false);
        }
        // defaultColor = comment.userID == ARSceneManager.userData.userID ? authorColor : commentBackground.color;
        deselect();

        gameObject.SetActive(true);
    }

    void editTransform()
    {
        if (comment.userId != ARSceneManager.userData.userId)
            return;
        
        if (editingMovable && !lockMovable)
        {
            Vector3 projection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, movableDistance));
            transform.position = projection;

            Quaternion newRotation = Quaternion.LookRotation(transform.position - cam.position) * Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 6f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

            comment.position = transform.localPosition;
            comment.rotation = transform.localRotation.eulerAngles;
        }
    }

    public async void Post()
    {
        comment.position = transform.localPosition;
        comment.rotation = transform.localRotation.eulerAngles;

        bool response = await APIManager.PostComment(ARSceneManager.GetArtworkID(), comment._id, comment);
        if (response)
        {
            //comment = response;
            //Debug.Log("Post Successful");
            Hint.Log("Kommentar aktualisiert");
            lastUpdatedtext = comment.text;
            UserSession.addComment(comment, comment.userId, true);
        }
        else
        {
            //Debug.Log("Post Error");
            Hint.Log("Fehler aufgetreten");
        }
    }

    public async void Put()
    {
        comment.position = transform.localPosition;
        comment.rotation = transform.localRotation.eulerAngles;
        Comment tmpComment = await APIManager.PutComment(ARSceneManager.GetArtworkID(), comment);

        if (tmpComment != null)
        {
            comment = tmpComment;
            //Debug.Log("Put Successful");
            Hint.Log("Kommentar veröffentlicht");
            lastUpdatedtext = comment.text;
            UserSession.addComment(tmpComment, tmpComment.userId, true);
        }
        else
        {
            //Debug.Log("Put Error");
            Hint.Log("Fehler aufgetreten");
        }
    }

    public async UniTask<bool> Delete()
    {
        //  Comment not yet in backend
        if(comment._id == null)
        {
            Destroy(gameObject);
            return true;
        }

        if(await APIManager.DeleteComment(ARSceneManager.GetArtworkID(), comment._id))
        {
            //Debug.Log("Delete Successful");
            Hint.Log("Kommentar entfernt");
            UserSession.removeComment(comment, comment.userId);
            Destroy(gameObject);
            return true;
        }
        else
        {
            //Debug.Log("Delete Error");
            Hint.Log("Fehler aufgetreten");
            return false;
        }
    }

    //  Only open keyboard if not in Unity Editor
    public void startTextEdit()
    {
        if (comment._id == null)
        {
            comment.text = "";
        }
    #if !UNITY_EDITOR
    
        keyboard = TouchScreenKeyboard.Open(comment.text, TouchScreenKeyboardType.ASCIICapable, false, false, false, false, "", charLimit);
        StartCoroutine(WaitForKeyboardToClose());
    #else
        if (TouchScreenKeyboard.isSupported)
        {
            Debugger.Logger.Green($"Touchscreen keyboard is supported!");
            keyboard = TouchScreenKeyboard.Open(comment.text, TouchScreenKeyboardType.ASCIICapable, false, false, false, false, "Enter Comment...", charLimit);
        }
        else
        {
            Debugger.Logger.Red($"Touchscreen keyboard not supported");
            keyboardAccess = new EditorTouchScreenKeyboard(comment.text);
            StartCoroutine(WaitForKeyboardAccessToClose());
        }
    #endif
        editingText = true;
    }

    private IEnumerator WaitForKeyboardToClose()
    {
        Debug.Log("Stat Waitng For Keyboard to Close");
        yield return new WaitUntil(() => !keyboard.active);
        keyboard = null;

        Debug.Log("Waiting Done, Posting Comment");
        if (comment._id == null)
        {
            Put();
        }
        else
        {
            Post();
        }
    }
    
    private IEnumerator WaitForKeyboardAccessToClose()
    {
        Debug.Log("Stat Waitng For Keyboard to Close");
        yield return new WaitUntil(() => !keyboardAccess.Active);
        keyboardAccess = null;

        Debug.Log("Waiting Done, Posting Comment");
        if (comment._id == null)
        {
            Put();
        }
        else
        {
            Post();
        }
    }

    //  Add color highlight
    public void select()
    {
        commentBackground.color = selectedColor;
        commentText.color = Color.black;
        usernameText.color = Color.black;

    }

    //  Remove color highlight
    public void deselect()
    {
        commentBackground.color = defaultColor;
        commentText.color = Color.white;
        usernameText.color = Color.white;
        editingText = false;
        // keyboard = null;
        // Debug.Log("Deselecting completed");
    }

    //  Update rendered text of comment
    public void commentUpdate()
    {
        if (comment == null) return;
        if (!editingText) return;

        if (commentText.text != comment.text)
        {
            commentText.SetText(comment.text);
        }
    }

    //  Update text of comment with text from keyboard
    public void keyboardUpdate()
    {
        if (comment == null) return;

        if (keyboard != null)
        {
            if(keyboard.active && keyboard.status == TouchScreenKeyboard.Status.Visible)
            // if (keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                comment.text = keyboard.text;
            }
        }

        if (keyboardAccess != null)
        {
            if (keyboardAccess.Active)
            {
                Debug.Log("updating keyboard text");
                comment.text = keyboardAccess.Text;
                Debug.Log($"{comment.text} = Comment Text");
            }
        }
        // else if (keyboard.status == TouchScreenKeyboard.Status.Done
        // && lastUpdatedtext != comment.text)
        // {
        //
        //     if (comment._id == null)
        //     {
        //         Post();
        //         Debug.Log("Posted Comment");
        //     }
        //     else
        //     {
        //         Put();
        //         Debug.Log("Put Comment");
        //     }
        // }
    }

    private void onUICanvasChanged(bool status)
    {
        lockMovable = status;
        // Debug.Log("The Status for the UI Canvas is" + status);
    }
}
