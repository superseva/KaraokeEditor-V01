using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTab : MonoBehaviour {

    [HideInInspector]
    public GameObject wordGO;
    [HideInInspector]
    public WordData wordData;

    public InputField textField;
    public InputField timeField;
    public TimetrackerPanelCtrl timetrackCtrl;


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
        wordGO = word;
        wordData = wordGO.GetComponent<WordButton>().wordData;
        textField.text = wordData.text;
        timeField.text = wordData.time;
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


}
