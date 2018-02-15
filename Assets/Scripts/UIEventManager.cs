using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour {

    public delegate void AlertAction(string msg, string title);
    public static event AlertAction OnAlert;

    public static void FireAlert(string msg, string title)
    {
        if (OnAlert != null)
        {
            OnAlert(msg, title);
        }
    }
}
