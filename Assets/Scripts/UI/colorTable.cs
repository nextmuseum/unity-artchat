using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class colorTable : MonoBehaviour
{

    /* -- Image / Raw Image Tags --
    Button;
    nav_bar;
    logo;
    background;
    image_opaque
    input_field
    image_transparent
    comment
    nextMuseum
    highlighted
    toggle
    error_background
    outline


    -- Text Tags --
    Header;
    secondary_text;
    primary_text;
    button_text

    */

    public UIManager UIManager;

    public ColorMode DarkMode;
    public ColorMode LightMode;
    public ColorMode NextMuseumMode;
    public ColorMode FunkyMode;
    
    public List<GameObject> canvasList;

    void Awake()
    {
        #region LightMode
        LightMode = new ColorMode();
        LightMode.white = new Color32(255, 255, 255, 255);
        LightMode.black = new Color32(0, 0, 0, 255);
        LightMode.nextMuseumPlatform = new Color32(165, 229, 221, 255);
        LightMode.buttonColor = new Color32(85, 151, 154, 255);
        LightMode.otherComment = new Color32(205, 238, 234, 255);
        LightMode.background = new Color32(199, 221, 212, 255);
        LightMode.navBar = new Color32(255, 255, 255, 255);
        LightMode.primaryText = new Color32(20, 20, 20, 255);
        LightMode.secondaryText = new Color32(37, 39, 46, 255);
        LightMode.buttonText = new Color32(255, 255, 255, 255);
        LightMode.imgTransparent = new Color32(255, 255, 255, 100);
        LightMode.imgFilled = new Color32(255, 255, 255, 250);
        LightMode.inputField = new Color32(245, 245, 245, 255);
        LightMode.highlighted = new Color32(237, 69, 50, 255);
        LightMode.logo = new Color32(53, 96, 102, 255);
        LightMode.header = new Color32(0, 67, 87, 255);
        LightMode.toggleColor = new Color32(245, 245, 245, 255);
        LightMode.errorBackground = new Color32(205, 238, 234, 255);
        LightMode.outline = new Color32(255, 255, 255, 255);
        LightMode.FAB = new Color32(85, 151, 154, 255);


        #endregion

        #region DarkMode
        DarkMode = new ColorMode();
        DarkMode.white = new Color32(255, 255, 255, 255);
        DarkMode.black = new Color32(0, 0, 0, 255);
        DarkMode.nextMuseumPlatform = new Color32(165, 229, 221, 255);
        DarkMode.buttonColor = new Color32(3, 218, 198, 255);
        DarkMode.otherComment = new Color32(48, 58, 66, 255);
        DarkMode.background = new Color32(29, 32, 37, 255);
        DarkMode.navBar = new Color32(40, 46, 53, 255);
        DarkMode.primaryText = new Color32(203, 210, 217, 255);
        DarkMode.secondaryText = new Color32(203, 210, 217, 255);
        DarkMode.buttonText = new Color32(37, 39, 46, 255);
        DarkMode.imgTransparent = new Color32(255, 255, 255, 100);
        DarkMode.imgFilled = new Color32(255, 255, 255, 255);
        DarkMode.inputField = new Color32(50, 63, 75, 255);
        DarkMode.highlighted = new Color32(255, 228, 255, 255);
        DarkMode.logo = new Color32(165, 229, 221, 255);
        DarkMode.header = new Color32(160, 224, 216, 255);
        DarkMode.toggleColor = new Color32(45, 45, 45, 255);
        DarkMode.errorBackground = new Color32(48, 58, 66, 255);
        DarkMode.outline = new Color32(255, 255, 255, 255);
        DarkMode.FAB = new Color32(255, 228, 255, 255);
        #endregion


        #region NextMuseumMode
        NextMuseumMode = new ColorMode();
        NextMuseumMode.white = new Color32(255, 255, 255, 255);
        NextMuseumMode.black = new Color32(0, 0, 0, 255);
        NextMuseumMode.nextMuseumPlatform = new Color32(203, 255, 243, 255);    // NM plattform t�rkis
        NextMuseumMode.buttonColor = new Color32(255, 255, 255, 255);
        NextMuseumMode.otherComment = new Color32(69, 27, 246, 255);            // NM kommentar blau
        NextMuseumMode.background = new Color32(203, 255, 243, 255);            // NM event flieder
        NextMuseumMode.navBar = new Color32(255, 255, 255, 255);                // NM event flieder
        NextMuseumMode.primaryText = new Color32(0, 0, 0, 255);                 // NM black
        NextMuseumMode.secondaryText = new Color32(213, 213, 213, 255);         // NM grey
        NextMuseumMode.buttonText = new Color32(203, 255, 243, 255);            // NM plattform t�rkis
        NextMuseumMode.imgTransparent = new Color32(255, 255, 255, 100);
        NextMuseumMode.imgFilled = new Color32(255, 255, 255, 255);
        NextMuseumMode.inputField = new Color32(203, 255, 243, 255);            // NM plattform t�rkis
        NextMuseumMode.highlighted = new Color32(255, 78, 0, 255);              // NM curation orange
        NextMuseumMode.logo = new Color32(0, 0, 0, 255);                        // NM black
        NextMuseumMode.header = new Color32(0, 0, 0, 255);                      // NM black    
        NextMuseumMode.toggleColor = new Color32(45, 45, 45, 255);
        NextMuseumMode.errorBackground = new Color32(213, 213, 213, 255);       // NM grey
        NextMuseumMode.outline = new Color32(0, 0, 0, 255);
        NextMuseumMode.FAB = new Color32(255, 255, 255, 255);
        NextMuseumMode.navBackground = new Color32(203, 255, 243, 120);
        NextMuseumMode.uiBox = new Color32(69, 27, 246, 255);
        #endregion

        #region FunkyMode
        FunkyMode = new ColorMode();
        FunkyMode.white = RandomColor();
        FunkyMode.black = RandomColor();
        FunkyMode.nextMuseumPlatform = RandomColor();
        FunkyMode.buttonColor = RandomColor();
        FunkyMode.otherComment = RandomColor();
        FunkyMode.background = RandomColor();
        FunkyMode.navBar = RandomColor();
        FunkyMode.primaryText = RandomColor();
        FunkyMode.secondaryText = RandomColor();
        FunkyMode.buttonText = RandomColor();
        FunkyMode.imgTransparent = new Color32(255,255,255,100);
        FunkyMode.imgFilled = RandomColor();
        FunkyMode.inputField = RandomColor();
        FunkyMode.highlighted = RandomColor();
        FunkyMode.logo = RandomColor();
        FunkyMode.header = RandomColor();
        FunkyMode.toggleColor = RandomColor();
        FunkyMode.errorBackground = RandomColor();
        FunkyMode.outline = RandomColor();
        FunkyMode.FAB = RandomColor();
        #endregion
        
    }
    void Start()
    {
        if(SceneManager.GetSceneByName("AR Scene_OnlyLogin").isLoaded)
        {
            Debug.Log("Stuff is colored now");
            
            CheckState(NextMuseumMode);
        }

        else
        {
            ToggleMode();
        }
    }

    private Color RandomColor()
    {
      Color32 randColor = new Color32(
      (byte)Random.Range(0, 255),
      (byte)Random.Range(0, 255),
      (byte)Random.Range(0, 255),
      255
      );
      return randColor;
    }
    public void ToggleMode()
    {
        SetMode(NextMuseumMode);
       
    }

    public void CheckState(ColorMode mode)
    {
        foreach (GameObject canvas in canvasList)
        {
            Transform[] allUiObjects = canvas.GetComponentsInChildren<Transform>(true);

            List<GameObject> falseObjects = new List<GameObject>();
            
            foreach (Transform uiObject in allUiObjects)
            {
                if (uiObject.gameObject.activeSelf == false)
                {
                    falseObjects.Add(uiObject.gameObject);
                    uiObject.gameObject.SetActive(true);
                }
            }
            SetMode(mode);

            foreach (GameObject falseObject in falseObjects)
            {
                falseObject.gameObject.SetActive(false);
            }
        }
    }
    
    public void SetMode(ColorMode mode)
    {
        //Images
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject elem in buttons)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.buttonColor;
            else if (elem.TryGetComponent(out RawImage rawImage)) rawImage.GetComponent<RawImage>().color = mode.buttonColor;
            else elem.GetComponent<TextMeshProUGUI>().color = mode.buttonText;

            if (elem.TryGetComponent<Outline>(out Outline _outline)) _outline.effectColor = mode.outline;
        }

        GameObject[] navbar = GameObject.FindGameObjectsWithTag("nav_bar");
        foreach (GameObject elem in navbar) elem.GetComponent<Image>().color = mode.navBar;

        GameObject[] logos = GameObject.FindGameObjectsWithTag("logo");
        foreach (GameObject elem in logos) elem.GetComponent<Image>().color = mode.logo;

        GameObject[] background = GameObject.FindGameObjectsWithTag("background");
        foreach (GameObject elem in background) elem.GetComponent<Image>().color = mode.background;

        GameObject[] opaqueImages = GameObject.FindGameObjectsWithTag("image_opaque");
        foreach (GameObject elem in opaqueImages)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.imgFilled;
            else elem.GetComponent<RawImage>().color = mode.imgFilled;
        }

        GameObject[] inputFields = GameObject.FindGameObjectsWithTag("input_field");
        foreach (GameObject elem in inputFields) elem.GetComponent<Image>().color = mode.inputField;

        GameObject[] transparentImages = GameObject.FindGameObjectsWithTag("image_transparent");
        foreach (GameObject elem in transparentImages)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.imgTransparent;
            else elem.GetComponent<RawImage>().color = mode.imgTransparent;
        }

        GameObject[] comments = GameObject.FindGameObjectsWithTag("comment");
        foreach (GameObject elem in comments) elem.GetComponent<Image>().color = mode.otherComment;

        GameObject[] nextMuseumColored = GameObject.FindGameObjectsWithTag("nextMuseum");
        foreach (GameObject elem in nextMuseumColored)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.nextMuseumPlatform;
            else elem.GetComponent<RawImage>().color = mode.nextMuseumPlatform;
        }

        GameObject[] highlightedObjects = GameObject.FindGameObjectsWithTag("highlighted");
        foreach (GameObject elem in highlightedObjects)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.highlighted;
            else elem.GetComponent<TextMeshProUGUI>().color = mode.highlighted;
        }

        GameObject[] errorBackground = GameObject.FindGameObjectsWithTag("error_background");
        foreach (GameObject elem in errorBackground) elem.GetComponent<Image>().color = mode.errorBackground;

        GameObject[] FAB = GameObject.FindGameObjectsWithTag("FAB");
        foreach (GameObject elem in FAB)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.FAB;
            else elem.GetComponent<RawImage>().color = mode.FAB;

            if (elem.TryGetComponent<Outline>(out Outline _outline)) _outline.effectColor = mode.outline;
        }

        //Text
        GameObject[] header = GameObject.FindGameObjectsWithTag("Header");
        foreach (GameObject elem in header) elem.GetComponent<TextMeshProUGUI>().color = mode.header;

        GameObject[] secondary_text = GameObject.FindGameObjectsWithTag("secondary_text");
        foreach (GameObject elem in secondary_text) elem.GetComponent<TextMeshProUGUI>().color = mode.secondaryText;

        GameObject[] primary_text = GameObject.FindGameObjectsWithTag("primary_text");
        foreach (GameObject elem in primary_text) elem.GetComponent<TextMeshProUGUI>().color = mode.primaryText;

        GameObject[] button_text = GameObject.FindGameObjectsWithTag("button_text");
        foreach (GameObject elem in button_text)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.buttonText;
            else if(elem.TryGetComponent(out RawImage rawImage)) rawImage.GetComponent<RawImage>().color = mode.buttonText;
            else elem.GetComponent<TextMeshProUGUI>().color = mode.buttonText;
        }
        
        GameObject[] nav_background = GameObject.FindGameObjectsWithTag("nav_background");
        foreach (GameObject elem in nav_background)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.navBackground;
            else elem.GetComponent<TextMeshProUGUI>().color = mode.navBackground;
        }
        
        GameObject[] ui_box = GameObject.FindGameObjectsWithTag("ui_box");
        foreach (GameObject elem in ui_box)
        {
            if (elem.TryGetComponent(out Image image)) image.GetComponent<Image>().color = mode.uiBox;
            else if(elem.TryGetComponent(out RawImage rawImage)) rawImage.GetComponent<RawImage>().color = mode.uiBox;
            else elem.GetComponent<TextMeshProUGUI>().color = mode.uiBox;
        }

    }

}
