using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButtonCtrl : MonoBehaviour {

	public void CloseMe()
    {
        gameObject.SetActive(false);
    }
}
