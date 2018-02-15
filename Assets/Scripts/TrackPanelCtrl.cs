using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using LitJson;

public class TrackPanelCtrl : MonoBehaviour {

    public static string SOUND = "Sound";
    public static string JSON = "Json";

    public RawImage rawImg;
    public GameObject songHolder;
    public Text jsonPathText;
    public Text wavPathText;
    public GameObject trackView;
    public Text wordPrefab;
    public GameObject wordsHolder;

    [HideInInspector]
    public AudioSource audioSource;

    [HideInInspector]
    public SongData songDataFromJson;

    //Beat Synch And Counter
    private string songDataJsonString;
    private BeatSynchronizer beatSynchroniser;
    private BeatCounter beatCounter;

    //Tracker
    private float bottomLine = 0;
    private float bottomLineBottomOffset = 200;
    private float secondToPixelSize;
    private float sixteenNoteToPixelSize = 16;
    private float sixteenNoteToSecondsSize;
    private float sixteenNoteToSampleSize;
    private float secondToSampleSize;
    private float onePixelToSeconds;

    string debugString;
    public Text debugText;

    // Use this for initialization
    void Start ()
    {
        trackView.SetActive(false);

        audioSource = songHolder.GetComponent<AudioSource>();
        beatSynchroniser = songHolder.GetComponent<BeatSynchronizer>();
        beatCounter = songHolder.GetComponent<BeatCounter>();

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"), new FileBrowser.Filter("Sound Files", ".wav", ".ogg"));
    }

    public void OpenJsonFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"));
        StartCoroutine(ShowLoadDialogCoroutine(JSON));
    }

    public void OpenSoundFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Sound Files", ".wav", ".ogg"));
        StartCoroutine(ShowLoadDialogCoroutine(SOUND));
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
            if (fileType == JSON)
            {
                songDataJsonString = www.text;
                songDataFromJson = JsonMapper.ToObject<SongData>(www.text);
                jsonPathText.text = filePath;
            }
            else
            {
                audioSource.clip = www.GetAudioClip(false, false);
                wavPathText.text = filePath;
            }
        }
    }

    Texture2D AudioWaveForm(AudioClip aud, int width, int height, Color color)
    {

        int step = Mathf.CeilToInt((aud.samples * aud.channels) / width);

        //Debug.Log(string.Format("STEP : {0}", step));
        //Debug.Log(string.Format("WIDTH : {0}", width));
        float[] samples = new float[aud.samples * aud.channels];

        //workaround to prevent the error in the function getData
        //when Audio Importer loadType is "compressed in memory"
        //string path = AssetDatabase.GetAssetPath(aud);
        //AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
        //AudioImporterLoadType audioLoadTypeBackup = audioImporter.loadType;
        //audioImporter.loadType = AudioImporterLoadType.StreamFromDisc;
        //AssetDatabase.ImportAsset(path);

        //getData after the loadType changed
        aud.GetData(samples, 0);

        //restore the loadType (end of workaround)
       // audioImporter.loadType = audioLoadTypeBackup;
        //AssetDatabase.ImportAsset(path);

        Texture2D img = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] xy = new Color[width * height];
        for (int x = 0; x < width * height; x++)
        {
            xy[x] = new Color(0, 0, 0, 0);
        }

        img.SetPixels(xy);

        int i = 0;
        while (i < width)
        {
            int barHeight = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(samples[i * step]) * height, 0, height));
            int add = samples[i * step] > 0 ? 1 : -1;
            for (int j = 0; j < barHeight; j++)
            {
                img.SetPixel(i, Mathf.FloorToInt(height / 2) - (Mathf.FloorToInt(barHeight / 2) * add) + (j * add), color);
            }
            ++i;
        }

        img.Apply();
        return img;
    }

    public void CreateTracker()
    {
        if(string.IsNullOrEmpty(songDataJsonString) || !audioSource.clip)
        {
            UIEventManager.FireAlert("NOT ALL FILES PRESENT", "ERROR");
            return;
        }

        trackView.SetActive(true);
        beatSynchroniser.bpm = (float)songDataFromJson.bpm;
        beatSynchroniser.startDelay = 1;
        beatCounter.beatValue = SynchronizerData.BeatValue.SixteenthBeat;

        sixteenNoteToSecondsSize = ( 60f / (float)songDataFromJson.bpm ) / 4f;
        onePixelToSeconds = sixteenNoteToSecondsSize / sixteenNoteToPixelSize;
        Debug.Log("sixteenNoteToSecondsSize " + sixteenNoteToSecondsSize);
        Debug.Log("onePixelToSeconds " + onePixelToSeconds);

        //populate words
        float pozX;
        int numOfwords = songDataFromJson.words.Length;
        for(int i = 0; i< numOfwords; i++)
        {
            pozX = float.Parse(songDataFromJson.timestamps[i]) / onePixelToSeconds;
            Debug.Log(pozX);
            Instantiate(wordPrefab, wordsHolder.transform, false);
            wordPrefab.text = songDataFromJson.words[i].ToString();
            
            wordPrefab.rectTransform.anchoredPosition= new Vector3(pozX, 0, 0);
        }


        beatCounter.StartCountingBeats();
        beatSynchroniser.StartSynchronisedMusic();




        rawImg.texture = AudioWaveForm(audioSource.clip, (int)Mathf.Floor(rawImg.rectTransform.rect.width), (int)Mathf.Floor(rawImg.rectTransform.rect.height), new Color(210, 210, 210));

        debugString = string.Empty;
        //float samplesPerQuarterNote = ((float)songDataFromJson.bpm / 60) * 44100;
        float samplesPerQuarterNote = Mathf.CeilToInt(44100/((float)songDataFromJson.bpm / 60));
        float samplesPerBar = samplesPerQuarterNote * 4;
        float barInSong = Mathf.Ceil((float)audioSource.clip.samples / (float)samplesPerBar);
        debugString = string.Format("Song name: {0} \nSong BPM: {1} \nTotal Samples: {2} \nSamlpes per 1/4 note: {3} \nSamples per bar: {4} \nTotal Bar Count: {5} \nSong Time: {6} sec", songDataFromJson.songname, songDataFromJson.bpm, audioSource.clip.samples, samplesPerQuarterNote, samplesPerBar, barInSong, audioSource.clip.length);
        debugText.text = debugString;
    }

    public void prepareSampler()
    {
        
    }

    void ResetFields()
    {
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
