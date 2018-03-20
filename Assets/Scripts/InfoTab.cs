using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InfoTab : MonoBehaviour {

    [HideInInspector]
    public GameObject wordGO;
    [HideInInspector]
    public WordData wordData;

    public InputField textField;
    public InputField timeField;
    public TimetrackerPanelCtrl timetrackCtrl;
    public InputField durationField;
    
    void OnEnable()
    {
        UIEventManager.OnSelectWord += ShowWordSelected;
    }

    void OnDisable()
    {
        UIEventManager.OnSelectWord -= ShowWordSelected;
    }

    void ShowWordSelected(GameObject word)
    {
        //DESELECT OLD ONE
        if (wordGO)
        {
            wordGO.GetComponent<WordButton>().label.color = new Color(212, 212, 212);
        }


        wordGO = word;
        wordData = wordGO.GetComponent<WordButton>().wordData;
        wordGO.GetComponent<WordButton>().label.color = new Color(0,225,100);
        textField.text = wordData.text;
        timeField.text = wordData.time;
        if (double.IsNaN(wordData.duration))
        {
            wordData.duration = 0;
        }
        durationField.text = wordData.duration.ToString();
    }

    public void ApplyTextChange()
    {
        if (!wordGO)
            return;
        wordData.text = textField.text;
        timetrackCtrl.ChangeWordText(wordGO);
    }

    public void ApplyTimeChange()
    {
        if (!wordGO)
            return;
        wordData.time = timeField.text;
        timetrackCtrl.ChangeWordTime(wordGO);
    }

    public void Add10Ms()
    {
        if (!wordGO)
            return;
        float newTime = float.Parse(wordData.time) + 0.01f;
        wordData.time = newTime.ToString("F2");
        timetrackCtrl.ChangeWordTime(wordGO);
        timeField.text = wordData.time;
    }

    public void Sub10Ms()
    {
        if (!wordGO)
            return;
        if (float.Parse(wordData.time) > 0) { 
            float newTime = float.Parse(wordData.time) - 0.01f;
            wordData.time = newTime.ToString("F2");
            timetrackCtrl.ChangeWordTime(wordGO);
            timeField.text = wordData.time;
        }
        
    }

    public void IncreaseDuration()
    {
        if (!wordGO)
            return;

        wordData.duration += 0.1f;
        wordData.duration = (double)System.Math.Round(wordData.duration, 2);
        wordGO.GetComponent<WordButton>().SetDurrationMarker(TimetrackerPanelCtrl.SECOND_IN_PIXELS);
        durationField.text = wordData.duration.ToString();
    }

    public void DecreaseDuration()
    {
        if (!wordGO)
            return;

        if(wordData.duration>0)
            wordData.duration -= 0.1f;

        wordData.duration = (double)System.Math.Round(wordData.duration, 2);

        wordGO.GetComponent<WordButton>().SetDurrationMarker(TimetrackerPanelCtrl.SECOND_IN_PIXELS);
        durationField.text = wordData.duration.ToString();
    }

    public void RemoveDuration()
    {
        if (!wordGO)
            return;
        wordData.duration = 0;

        wordGO.GetComponent<WordButton>().SetDurrationMarker(TimetrackerPanelCtrl.SECOND_IN_PIXELS);
        durationField.text = wordData.duration.ToString();
    }


}
