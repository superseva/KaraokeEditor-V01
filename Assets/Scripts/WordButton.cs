using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour {

    public WordData wordData;
    public Text label;

    public void OnBtnPress()
    {
        Debug.Log("WORD GameObject : " + gameObject.name);
        UIEventManager.SelectWord(gameObject);
    }

}
