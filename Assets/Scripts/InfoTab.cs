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
        wordData.text = textField.text;
        timetrackCtrl.ChangeWordText(wordGO);
    }

    public void ApplyTimeChange()
    {
        wordData.time = timeField.text;
        timetrackCtrl.ChangeWordTime(wordGO);
    }
}
