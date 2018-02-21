using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventManager : MonoBehaviour {

    public delegate void AlertAction(string msg, string title);
    public static event AlertAction OnAlert;

    public delegate void SelectWordAction(GameObject wordGO);
    public static event SelectWordAction OnSelectWord;

    public static void FireAlert(string msg, string title)
    {
        if (OnAlert != null)
        {
            OnAlert(msg, title);
        }
    }

    public static void SelectWord(GameObject wordGO)
    {
        if (OnSelectWord != null)
        {
            OnSelectWord(wordGO);
        }
    }
}
