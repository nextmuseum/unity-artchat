using UnityEngine;
using System.Collections;
using System.Net.Mime;
using TMPro;

public class ColorChanger : MonoBehaviour {
 
    public Color[] colors;
         
    public int currentIndex = 0;
    private int nextIndex;
     
    public float changeColourTime = 2.0f;
     
    private float lastChange = 0.0f;
    private float timer = 0.0f;

    private TMP_Text texttenderer;
    private UnityEngine.UI.Image imagerenderer;
    
    void Start() {
        if (colors == null || colors.Length < 2)
            Debug.Log ("Need to setup colors array in inspector");
         
        nextIndex = (currentIndex + 1) % colors.Length; 
        
        if(this.GetComponent<TMP_Text>() != null)
            texttenderer = this.GetComponent<TMP_Text>();
        else
            imagerenderer = this.GetComponent<UnityEngine.UI.Image>();
    }
     
    void Update() {
         
        timer += Time.deltaTime;
         
        if (timer > changeColourTime) {
            currentIndex = (currentIndex + 1) % colors.Length;
            nextIndex = (currentIndex + 1) % colors.Length;
            timer = 0.0f;
             
        }
        if(this.texttenderer != null)
            texttenderer.color = Color.Lerp (colors[currentIndex], colors[nextIndex], timer / changeColourTime );
        else
            imagerenderer.color = Color.Lerp (colors[currentIndex], colors[nextIndex], timer / changeColourTime );
    }
}