using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TMP_Text))]
public class DialogueTyper : MonoBehaviour
{
    private TMP_Text textBox;

    [SerializeField] private string textToShow;

    private int currentVisibleCharacterIndex;

    private Coroutine typeWriterCoroutine;
    
    private WaitForSeconds delay;
    private WaitForSeconds interpunctuationDelayWait;

    private float characterPerSecond = 20f;
    private float interpunctuationDelay = 0.5f;

    private void Awake()
    {
        textBox =  GetComponent<TMP_Text>();
        delay = new WaitForSeconds(1 / characterPerSecond);
        interpunctuationDelayWait = new WaitForSeconds(interpunctuationDelay);
    }

    private void OnEnable()
    {
        SetText(textToShow);
    }

    private void Start()
    {
        //SetText(testText);
    }

    private void SetText(string text)
    {
        textBox.text = text;
        textBox.maxVisibleCharacters = 0;
        currentVisibleCharacterIndex = 0;

        typeWriterCoroutine = StartCoroutine(TypeWriter());
    }

    IEnumerator TypeWriter()
    {
        TMP_TextInfo textInfo = textBox.textInfo;

        while (currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            char character = textInfo.characterInfo[currentVisibleCharacterIndex].character;
            textBox.maxVisibleCharacters++;

            if ((character == '.' || character == '!'))
            {
                yield return interpunctuationDelayWait;
            }
            else
            {
                yield return delay;
            }
            currentVisibleCharacterIndex++;
        }
    }
}
