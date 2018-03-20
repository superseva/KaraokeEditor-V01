using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WordButton : MonoBehaviour {

    public WordData wordData;
    public Text label;
    public Image durationMarker;

    private RectTransform imageRectTransform;
    private float newDurrationLenght;

    void Awake()
    {
        imageRectTransform = durationMarker.GetComponent<RectTransform>();
    }

    public void OnBtnPress()
    {
        //Debug.Log("WORD GameObject : " + gameObject.name);
        UIEventManager.SelectWord(gameObject);
    }

    public void SetWordData(WordData wd)
    {
        wordData = wd;
        label.text = wordData.text.ToString();
    }

    public void SetDurrationMarker(int secondInPixels)
    {
        if (double.IsNaN(wordData.duration))
        {
            wordData.duration = 0f;
        }

        if(wordData.duration <= 0)
        {
            imageRectTransform.sizeDelta = new Vector2(0,2);
            durationMarker.gameObject.SetActive(false);
        }
        else
        {
            durationMarker.gameObject.SetActive(true);
            newDurrationLenght = (float)(wordData.duration * secondInPixels);
            imageRectTransform.sizeDelta = new Vector2(newDurrationLenght, 2);
        }
    }

}
