using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class PanelCtrl : MonoBehaviour {

    //public GameObject[] panels;
    public List<GameObject> panels;
    public GameObject helpPanel;

    void Start ()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Lrc Files", ".lrc"), new FileBrowser.Filter("Json", ".json"), new FileBrowser.Filter("Sound Files", ".wav", ".ogg"));
        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }
    }

    public void ToglePanels(string panelName)
    {
        foreach(GameObject go in panels)
        {
            if (go.name != panelName)
            {
                if(go.activeSelf)
                    go.SetActive(false);
            }
            else
            {
                if(!go.activeSelf)
                    go.SetActive(true);
            }
        }
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(!helpPanel.activeSelf);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
