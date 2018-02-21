using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class LrcToJson : MonoBehaviour {

    void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("LRC", ".lrc"));
        FileBrowser.SetDefaultFilter(".txt");
    }

    // Use this for initialization
    public string OpenBrowser()
    {
        string result = string.Empty;
        return result;
    }


}
