using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUICtrl : MonoBehaviour {

    public GameObject alertPanel;
    public Text alertMessageTxt;
    public Text alertTitleTxt;

    // Use this for initialization
    void Start () {
        UIEventManager.OnAlert -= PopMeUp;
        UIEventManager.OnAlert += PopMeUp;

        alertPanel.SetActive(false);
    }

    // Update is called once per frame
    void PopMeUp(string msg, string title) {
        alertPanel.SetActive(true);
        alertMessageTxt.text = msg;
        alertTitleTxt.text = title;
       // Debug.Log("alert: " + msg);
    }
}
