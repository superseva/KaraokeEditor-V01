using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using LitJson;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

public class LevelPanelCtrl : MonoBehaviour {

    [HideInInspector]
    public SongData songDataFromJson;

    public Text jsonPathText;
    public Text wavPathText;
    public GameObject contentBox;
    public GameObject wordLinePrefab;
    public InputField speedField;

    private string songDataJsonString;
    private AudioSource audioSource;
    private PreviewPlayer songPlayer;

    // Use this for initialization
    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        songPlayer = gameObject.GetComponent<PreviewPlayer>();
    }

    public void OpenJsonFileBrowser()
    {
        FileBrowser.SetDefaultFilter(".json");
        StartCoroutine(ShowLoadDialogCoroutine("json"));
    }

    public void OpenSoundFileBrowser()
    {
        FileBrowser.SetDefaultFilter(".wav");
        StartCoroutine(ShowLoadDialogCoroutine("sound"));
    }

    IEnumerator ShowLoadDialogCoroutine(string fileType)
    {
        yield return FileBrowser.WaitForLoadDialog(false, null, "Open " + fileType, "Load");

        if (FileBrowser.Success)
        {
            StartCoroutine(LoadFile(FileBrowser.Result, fileType));
        }
    }

    IEnumerator LoadFile(string filePath, string fileType)
    {
        string url = string.Format("file:///{0}", filePath);
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (fileType == "json")
            {
                songDataJsonString = www.text;
                songDataFromJson = JsonMapper.ToObject<SongData>(www.text);
                jsonPathText.text = filePath;
                GenerateTextLines();
            }
            else if (fileType == "sound")
            {
                audioSource.clip = www.GetAudioClip(false, false);
                wavPathText.text = filePath;
            }
        }
    }

    void OnDisable()
    {
        DestroyTrackLines();
        jsonPathText.text = String.Empty;
        wavPathText.text = String.Empty;
        audioSource.clip = null;
        songDataJsonString = null;
        songDataJsonString = null;
    }

    void DestroyTrackLines()
    {
        foreach (Transform child in contentBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        contentBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        contentBox.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
    }

    void GenerateTextLines()
    {
        DestroyTrackLines();
        int wordsCount = songDataFromJson.wordsList.Length;
        int contentHeight = 0;
        for(int i = 0; i < wordsCount; i++)
        {
            GameObject go = Instantiate(wordLinePrefab, contentBox.transform);
            go.GetComponent<WordInTrack>().SetWordData(songDataFromJson.wordsList[i]);
            contentHeight = i * 40;
        }
        contentHeight += 80;
        contentBox.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contentHeight);


        if (songDataFromJson.timeOnScreen.Equals(null))
        {
            songDataFromJson.timeOnScreen = 1;
        }
        speedField.text = songDataFromJson.timeOnScreen.ToString();
    }

    public void RandomizeNoRepeat()
    {
        int wordsCount = songDataFromJson.wordsList.Length;
        int lastIndex = Mathf.FloorToInt(UnityEngine.Random.Range(1,3.9f));
        float randomPick;
        for (int i = 0; i < wordsCount; i++)
        {
            randomPick = UnityEngine.Random.value;

            if (lastIndex == 1)
            {
                songDataFromJson.wordsList[i].index = (randomPick < 0.5f) ? 2 : 3;   
            }else if(lastIndex == 2)
            {
                songDataFromJson.wordsList[i].index = (randomPick < 0.5f) ? 1 : 3;
            }
            else if (lastIndex == 3)
            {
                songDataFromJson.wordsList[i].index = (randomPick < 0.5f) ? 1 : 2;
            }

            lastIndex = songDataFromJson.wordsList[i].index;
            //songDataFromJson.wordsList[i].index = Mathf.FloorToInt(UnityEngine.Random.Range(1, 3.9f));
        }
        GenerateTextLines();
    }

    public void RandomizeRepeat()
    {
        int wordsCount = songDataFromJson.wordsList.Length;
        for (int i = 0; i < wordsCount; i++)
        {
            songDataFromJson.wordsList[i].index = Mathf.FloorToInt(UnityEngine.Random.Range(1, 3.9f));
        }
        GenerateTextLines();
    }

    public void PreviewSong()
    {

        

        if (songDataFromJson!=null && audioSource.clip!=null)
            songPlayer.PreviewSong(audioSource, songDataFromJson, float.Parse(speedField.text));
        else
            UIEventManager.FireAlert("Load JSON and WAV!", "ERROR");
    }

    // SAVING SONG

    public void OpenSaveDialog()
    {

        if (string.IsNullOrEmpty(songDataJsonString))
        {
            UIEventManager.FireAlert("NO DATA TO SAVE", "SAVE ERROR");
            return;
        }

        FileBrowser.SetDefaultFilter(".json");
        StartCoroutine(ShowSaveDialogCoroutine());
    }

    IEnumerator ShowSaveDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForSaveDialog(false, null);

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        //Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        if (FileBrowser.Success)
        {
            songDataFromJson.timeOnScreen = float.Parse(speedField.text);
            string jsonToSave = JsonMapper.ToJson(songDataFromJson);
            //jsonOutputTxt.text = jsonToSave;
            string pathToSave = (IsStringEndsWith(FileBrowser.Result, ".json")) ? FileBrowser.Result : FileBrowser.Result + ".json";
            File.WriteAllText(FileBrowser.Result, jsonToSave.ToString());

            UIEventManager.FireAlert("Saved to: " + FileBrowser.Result, "SAVE SUCCESS");
        }
    }

    public static bool IsStringEndsWith(string a, string b)
    {
        int ap = a.Length - 1;
        int bp = b.Length - 1;

        while (ap >= 0 && bp >= 0 && a[ap] == b[bp])
        {
            ap--;
            bp--;
        }
        return (bp < 0 && a.Length >= b.Length) ||

                (ap < 0 && b.Length >= a.Length);
    }
}
