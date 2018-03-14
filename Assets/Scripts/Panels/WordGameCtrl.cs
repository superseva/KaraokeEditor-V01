using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGameCtrl : MonoBehaviour {

    public float showTime;
    public float hitTime;
    public float existanceTime;
    public float startX;
    public float startY;

    private void OnEnable()
    {
        Invoke("Destroy", existanceTime + 0.2f);
    }
    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}
