using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordInTrack : MonoBehaviour {

    public WordData wordData;
    public Text textField;

    public Toggle t1;
    public Toggle t2;
    public Toggle t3;

    void Start()
    {
        t1.onValueChanged.AddListener(delegate {
            EvealuateToggleGroup();
        });
        t2.onValueChanged.AddListener(delegate {
            EvealuateToggleGroup();
        });
        t3.onValueChanged.AddListener(delegate {
            EvealuateToggleGroup();
        });
    }

    public void SetWordData(WordData wd)
    {
        wordData = wd;
        textField.text = wordData.text;
        if(wordData.index == 0)
        {
            wordData.index = 1;
            t1.isOn = true;
        }
        else if (wordData.index == 1)
        {
            t1.isOn = true;
        }
        else if (wordData.index == 2)
        {
            t2.isOn = true;
        }
        else if (wordData.index == 3)
        {
            t3.isOn = true;
        }
        else
        {
            wordData.index = 1;
            t1.isOn = true;
        }
    }

    void EvealuateToggleGroup()
    {
        
        if (t1.isOn)
        {
            wordData.index = 1;
        }
        else if(t2.isOn)
        {
            wordData.index = 2;
        }
        else if(t3.isOn)
        {
            wordData.index = 3;
        }
        else
        {
            Debug.Log(0);
        }
    }
}
