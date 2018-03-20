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

public class TimetrackerPanelCtrl : MonoBehaviour {

    public static int SECOND_IN_PIXELS = 100;

    [HideInInspector]
    public SongData songDataFromJson;

    public Text jsonPathText;
    public Text wavPathText;
    public InputField timer;
    public RawImage wavImage;
    public GameObject wavimagesPanel;
    public RawImage wavImagePrefab;
    public Image wordPrefabClickable;
    public GameObject wordsHolder;
    public Toggle halfSpeedToggle;
    public InfoTab infoTab;
    public Slider speedSlider;
    public int markerX = 100;
    public float songPitch = 1;

    private AudioSource audioSource;
    private string songDataJsonString;
    private float songCurrentTime = 0.0f;
    private float songPercent = 0;
    private float sampleRate = 44100f;
    private int songPixelSize;

    //SONG VARS
    //public int secondInPixels = 100;
    public int wavImageHeight = 250;
    public int secondsPerWavImage = 60;

    private bool musicPaused = true;

    // Use this for initialization
    void Start ()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        //FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"), new FileBrowser.Filter("Sound Files", ".wav", ".ogg"));

        halfSpeedToggle.onValueChanged.AddListener(delegate {
            ToggleHalfSpeed(halfSpeedToggle);
        });
        
    }


    void OnDisable()
    {
        StopAllCoroutines();
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
        else
        {
            //UIEventManager.FireAlert("Error opening file", "ERROR");
            //ResetFields();
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
            }
            else if(fileType == "sound")
            {
                audioSource.clip = www.GetAudioClip(false, false);
                wavPathText.text = filePath;
            }
        }
    }

    public void OnGenerateTrackButton()
    {
        if (string.IsNullOrEmpty(songDataJsonString) || !audioSource.clip)
        {
            UIEventManager.FireAlert("NOT ALL FILES PRESENT", "ERROR");
            return;
        }

        // Clear the visual representatives for words and wav
        ResetTrack();
        GenerateTrack();
    }

    public void GenerateTrack()
    {
        if (string.IsNullOrEmpty(songDataJsonString) || !audioSource.clip)
        {
            UIEventManager.FireAlert("NOT ALL FILES PRESENT", "ERROR");
            return;
        }

        //songPixelSize = Mathf.CeilToInt(audioSource.clip.length * secondInPixels);
        songPixelSize = Mathf.CeilToInt(audioSource.clip.length * TimetrackerPanelCtrl.SECOND_IN_PIXELS);
        int numOfChuncks = Mathf.CeilToInt(audioSource.clip.length / secondsPerWavImage);
        int samplesInChunk = (int)sampleRate * secondsPerWavImage;
        //int chunkPixelSize = secondsPerWavImage * secondInPixels;
        int chunkPixelSize = secondsPerWavImage * TimetrackerPanelCtrl.SECOND_IN_PIXELS;
        int startSample = 0;
        int endSample = 0;
        RawImage wavImageGO;
        for (int c=0; c < numOfChuncks; c++)
        {
            startSample = samplesInChunk * c;
            endSample = Mathf.Min( samplesInChunk * (c + 1), audioSource.clip.samples);
            //chunkPixelSize = Mathf.FloorToInt((endSample - startSample) / (sampleRate / secondInPixels));
            chunkPixelSize = Mathf.FloorToInt((endSample - startSample) / (sampleRate / TimetrackerPanelCtrl.SECOND_IN_PIXELS));
            wavImageGO = Instantiate(wavImagePrefab, wavimagesPanel.transform, false) as RawImage;
            wavImageGO.rectTransform.sizeDelta = new Vector2(chunkPixelSize, wavImageHeight);
            wavImageGO.texture = AudioDrawer.CreateAudioTexturePiece(audioSource.clip, chunkPixelSize, wavImageHeight, Color.grey, startSample, endSample);
        }
        
        //wavImage.rectTransform.sizeDelta = new Vector2(songPixelSize, wavImageHeight);
        //wavImage.texture = AudioDrawer.CreateAudioTexture(audioSource.clip, songPixelSize, wavImageHeight, Color.grey);
       // wavImage.texture = AudioDrawer.CreateAudioTexturePiece(audioSource.clip, songPixelSize, wavImageHeight, Color.grey, 0, audioSource.clip.samples);

        float pozX = 0;
        float pozY = 0;
        int numOfwords = songDataFromJson.wordsList.Length;
        Image wordGO;
        Button btn;
        WordButton wordButtonCls;
        for (int i = 0; i < numOfwords; i++)
        {
           // pozX = float.Parse(songDataFromJson.wordsList[i].time) * secondInPixels;
            pozX = float.Parse(songDataFromJson.wordsList[i].time) * TimetrackerPanelCtrl.SECOND_IN_PIXELS;
            wordGO = Instantiate(wordPrefabClickable, wordsHolder.transform, false);
            btn = wordGO.transform.GetComponent<Button>();
            wordButtonCls = wordGO.transform.GetComponent<WordButton>();
            wordButtonCls.SetWordData(songDataFromJson.wordsList[i]);
            wordButtonCls.SetDurrationMarker(TimetrackerPanelCtrl.SECOND_IN_PIXELS);
            //wordButtonCls.wordData = songDataFromJson.wordsList[i];
            //wordButtonCls.label.text = wordButtonCls.wordData.text.ToString();
            wordGO.name = string.Format("{0}_{1}", songDataFromJson.wordsList[i].text, songDataFromJson.wordsList[i].time);
            pozY = (150) + ((i % 3) * 20);
            wordGO.rectTransform.anchoredPosition = new Vector3(pozX, pozY, 0);
            wordGO.tag = "word";
        }
    }

    public void PlaySong()
    {
        if (!audioSource.clip)
            return;

        musicPaused = false;
        audioSource.pitch = songPitch;
        audioSource.PlayScheduled(songCurrentTime);
        //StopAllCoroutines();
        StopCoroutine(UpdateTrackPosition());
        StartCoroutine(UpdateTrackPosition());
    }

    public void PlayFromTime()
    {
        if (!audioSource.clip)
            return;

        musicPaused = false;
        audioSource.pitch = songPitch;
        songCurrentTime = float.Parse(timer.text);
        audioSource.timeSamples = Mathf.CeilToInt(songCurrentTime * sampleRate);
        audioSource.PlayScheduled(songCurrentTime);
        //StopAllCoroutines();
        StopCoroutine(UpdateTrackPosition());
        StartCoroutine(UpdateTrackPosition());
    }

    public void PauseSong()
    {
        musicPaused = true;
        audioSource.Pause();
    }

    void ToggleHalfSpeed(Toggle change)
    {
        songPitch = (halfSpeedToggle.isOn) ? 0.5f : 1f;
        speedSlider.value = (halfSpeedToggle.isOn) ? 50 : 100;
        audioSource.pitch = songPitch;
        // StartCoroutine(ChangeSongSpeed());
    }

    IEnumerator ChangeSongSpeed()
    {
        PauseSong();
        yield return new WaitForSeconds(0.3f);
        PlaySong();
    }

    public void OnSpeedSliderChange()
    {
        Debug.Log("change");
        songPitch = speedSlider.value / 100;
        audioSource.pitch = songPitch;
    }

    private int nextX = 0;
    IEnumerator UpdateTrackPosition()
    {
        //songCurrentTime = audioSource.timeSamples / 44100;
        //Debug.Log(songCurrentTime);
        // yield return null;
        while (audioSource.isPlaying)
        {
            //songCurrentTime = (float)audioSource.timeSamples / sampleRate;
            //timer.text = songCurrentTime.ToString("F2");
            //songPercent = songCurrentTime / audioSource.clip.length;
            //nextX = Mathf.FloorToInt(0 - (songPixelSize * songPercent)) + markerX;
            //wavimagesPanel.transform.position = new Vector3(nextX, wavimagesPanel.transform.position.y, 0);
            //wordsHolder.transform.position = new Vector3(nextX, wordsHolder.transform.position.y, 0);
            SetTtrackPosition();
            yield return new WaitForSeconds(30 / 1000f);
        }   
    }

    public void Rewind1Sec()
    {
        if (!audioSource.clip)
            return;
        audioSource.timeSamples = Mathf.Max(0, audioSource.timeSamples - (int)sampleRate);
        SetTtrackPosition();
    }

    public void Forward1Sec()
    {
        if (!audioSource.clip)
            return;
        audioSource.timeSamples = Mathf.Min(audioSource.clip.samples, audioSource.timeSamples + (int)sampleRate);
        SetTtrackPosition();
    }

    void SetTtrackPosition()
    {
        if (!audioSource.clip)
            return;
        songCurrentTime = (float)audioSource.timeSamples / sampleRate;
        timer.text = songCurrentTime.ToString("F2");
        songPercent = songCurrentTime / audioSource.clip.length;
        nextX = Mathf.FloorToInt(0 - (songPixelSize * songPercent)) + markerX;
        wavimagesPanel.transform.position = new Vector3(nextX, wavimagesPanel.transform.position.y, 0);
        wordsHolder.transform.position = new Vector3(nextX, wordsHolder.transform.position.y, 0);
    }

    public void ZoomTrack(int newPixelSize = 100)
    {
        PauseSong();

        TimetrackerPanelCtrl.SECOND_IN_PIXELS = newPixelSize;
        //secondInPixels = newPixelSize;
        DestroyGraphics();
        GenerateTrack();
        SetTtrackPosition();
    }

    public void ResetTrack()
    {
        //StopAllCoroutines();
        audioSource.Stop();
        musicPaused = true;
        nextX = 0;
        audioSource.timeSamples = 0;
        songCurrentTime = 0;
        timer.text = songCurrentTime.ToString("F2");
        DestroyGraphics();

        wavimagesPanel.transform.position = new Vector3(0, wavimagesPanel.transform.position.y, 0);
        wordsHolder.transform.position = new Vector3(0, wordsHolder.transform.position.y, 0);
    }

    void DestroyGraphics()
    {
        foreach (Transform child in wordsHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in wavimagesPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ChangeWordTime(GameObject wordGO)
    {
        Debug.Log("Parse time: " + float.Parse(wordGO.GetComponent<WordButton>().wordData.time) * TimetrackerPanelCtrl.SECOND_IN_PIXELS);
        float pozX = float.Parse(wordGO.GetComponent<WordButton>().wordData.time) * TimetrackerPanelCtrl.SECOND_IN_PIXELS;
        float pozY = wordGO.GetComponent<Image>().rectTransform.anchoredPosition.y;
        wordGO.GetComponent<Image>().rectTransform.anchoredPosition = new Vector3(pozX, pozY, 0);
        //wordGO.transform.position = new Vector3(pozX, wordGO.transform.position.y, 0);
        UpdateWordGOName(wordGO);
    }

    public void ChangeWordText(GameObject wordGO)
    {
        wordGO.transform.GetComponent<WordButton>().label.text = wordGO.transform.GetComponent<WordButton>().wordData.text;
        UpdateWordGOName(wordGO);
    }

    void UpdateWordGOName(GameObject wordGO)
    {
        wordGO.name = string.Format("{0}_{1}", wordGO.GetComponent<WordButton>().wordData.text, wordGO.GetComponent<WordButton>().wordData.time);
    }

    

    // SAVING SONG

    public void OpenSaveDialog()
    {

        if(wordsHolder.transform.childCount<1 || wavimagesPanel.transform.childCount <1)
        {
            UIEventManager.FireAlert("Generate track first", "SAVE ERROR");
            return;
        }


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
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {

            if (!audioSource.clip)
                return;

            if (!musicPaused)
            {
                PauseSong();
                Debug.Log("Pause");
            }
            else
            {
                PlaySong();
                Debug.Log("Play");
            }
        }

        
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rewind1Sec();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Forward1Sec();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            infoTab.Add10Ms();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            infoTab.Sub10Ms();
        }

    }


}
