using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


[RequireComponent(typeof(TextMeshPro))]
public class DisplayTMPCharacterCount : MonoBehaviour
{
    public TMP_InputField countingInputField;
    private TextMeshProUGUI textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        // Debug.Log(textMeshPro);
        countingInputField.onValueChanged.AddListener(OnInputChanged);

        OnInputChanged(countingInputField.text);
    }

    private void OnInputChanged(string value)
    {
        int currentCount = value.Count();
        int maxCount = countingInputField.characterLimit;
        textMeshPro.text = $"{currentCount}/{maxCount}";
    }

}
