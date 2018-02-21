using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using LitJson;

public class TimetrackerPanelCtrl : MonoBehaviour {

    [HideInInspector]
    public SongData songDataFromJson;

    public Text jsonPathText;
    public Text wavPathText;
    public RawImage wavImage;
    public GameObject wavimagesPanel;
    public RawImage wavImagePrefab;
    public Text wordPrefab;
    public GameObject wordsHolder;
    public Toggle halfSpeedToggle;
    public int markerX = 100;
    public float songPitch = 1;

    private AudioSource audioSource;
    private string songDataJsonString;
    private float songCurrentTime = 0.0f;
    private float songPercent = 0;
    private float sampleRate = 44100f;
    private int songPixelSize;


    //SONG VARS
    public int secondInPixels = 100;
    public int wavImageHeight = 250;
    public int secondsPerWavImage = 60;
   

    // Use this for initialization
    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        //FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"), new FileBrowser.Filter("Sound Files", ".wav", ".ogg"));

        halfSpeedToggle.onValueChanged.AddListener(delegate {
            ToggleHalfSpeed(halfSpeedToggle);
        });
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
            UIEventManager.FireAlert("Error opening file", "ERROR");
            ResetFields();
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

    public void GenerateTrack()
    {
        if (string.IsNullOrEmpty(songDataJsonString) || !audioSource.clip)
        {
            UIEventManager.FireAlert("NOT ALL FILES PRESENT", "ERROR");
            return;
        }

        songPixelSize = Mathf.CeilToInt(audioSource.clip.length * secondInPixels);
        int numOfChuncks = Mathf.CeilToInt(audioSource.clip.length / secondsPerWavImage);
        int samplesInChunk = (int)sampleRate * secondsPerWavImage;
        int chunkPixelSize = secondsPerWavImage * secondInPixels;
        int startSample = 0;
        int endSample = 0;
        RawImage wavImageGO;
        for (int c=0; c < numOfChuncks; c++)
        {
            startSample = samplesInChunk * c;
            endSample = Mathf.Min( samplesInChunk * (c + 1), audioSource.clip.samples);
            chunkPixelSize = Mathf.FloorToInt((endSample - startSample) / (sampleRate / secondInPixels));
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
        for (int i = 0; i < numOfwords; i++)
        {
            pozX = float.Parse(songDataFromJson.wordsList[i].time) * secondInPixels;
            //Debug.Log(pozX);
            Instantiate(wordPrefab, wordsHolder.transform, false);
            wordPrefab.text = songDataFromJson.wordsList[i].text.ToString();
            pozY = (i % 3) * 20;

            wordPrefab.rectTransform.anchoredPosition = new Vector3(pozX, pozY, 0);
        }
    }


    public void PlaySong()
    {
        if (!audioSource.clip)
            return;

        audioSource.pitch = songPitch;
        audioSource.PlayScheduled(songCurrentTime);
        StartCoroutine(UpdateTrackPosition());
    }

    public void PauseSong()
    {
        audioSource.Pause();
    }

    private int nextX = 0;
    IEnumerator UpdateTrackPosition()
    {
        //songCurrentTime = audioSource.timeSamples / 44100;
        //Debug.Log(songCurrentTime);
        // yield return null;
        while (audioSource.isPlaying)
        {
            
            songCurrentTime = (float)audioSource.timeSamples / sampleRate;
            songPercent = songCurrentTime / audioSource.clip.length;
            nextX = Mathf.FloorToInt(0 - (songPixelSize * songPercent)) + markerX;
            //wavImage.rectTransform.anchoredPosition = new Vector2(nextX, wavImage.rectTransform.anchoredPosition.y);
            wavimagesPanel.transform.position = new Vector3(nextX, wavimagesPanel.transform.position.y, 0);
            wordsHolder.transform.position = new Vector3(nextX, wordsHolder.transform.position.y, 0);

            yield return new WaitForSeconds(30 / 1000f);
        }
    }

    void ResetFields()
    {

    }

    void ToggleHalfSpeed(Toggle change)
    {
        songPitch = (halfSpeedToggle.isOn) ? 0.5f : 1f;
        StartCoroutine(ChangeSongSpeed());
    }

    IEnumerator ChangeSongSpeed()
    {
        PauseSong();
        yield return new WaitForSeconds(0.3f);
        PlaySong();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
