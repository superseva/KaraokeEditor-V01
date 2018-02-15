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
    public Text wordPrefab;
    public GameObject wordsHolder;

    private AudioSource audioSource;
    private string songDataJsonString;
    private float songCurrentTime = 0.0f;
    private float songPercent = 0;
    

    //SONG VARS
    private float secondInPixels = 100f;
    private int wavImageHeight = 250;
   

    // Use this for initialization
    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        //FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"), new FileBrowser.Filter("Sound Files", ".wav", ".ogg"));
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

        int songPixelSize = Mathf.CeilToInt(audioSource.clip.length * secondInPixels);
        wavImage.rectTransform.sizeDelta = new Vector2(songPixelSize, wavImageHeight);
        wavImage.texture = AudioDrawer.CreateAudioTexture(audioSource.clip, songPixelSize, wavImageHeight, Color.grey);

        float pozX = 0;
        float pozY = 0;
        int numOfwords = songDataFromJson.words.Length;
        for (int i = 0; i < numOfwords; i++)
        {
            pozX = float.Parse(songDataFromJson.timestamps[i]) * secondInPixels;
            Debug.Log(pozX);
            Instantiate(wordPrefab, wordsHolder.transform, false);
            wordPrefab.text = songDataFromJson.words[i].ToString();
            pozY = (i % 3) * 20;

            wordPrefab.rectTransform.anchoredPosition = new Vector3(pozX, pozY, 0);
        }
    }


    public void PlaySong()
    {
        if (!audioSource.clip)
            return;

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
            
            songCurrentTime = (float)audioSource.timeSamples / 44100f;
            songPercent = songCurrentTime / audioSource.clip.length;
            nextX = Mathf.FloorToInt(0 - (wavImage.rectTransform.rect.width * songPercent));
            wavImage.rectTransform.anchoredPosition = new Vector2(nextX, wavImage.rectTransform.anchoredPosition.y);
            wordsHolder.transform.position = new Vector3(nextX, wordsHolder.transform.position.y, 0);

            yield return new WaitForSeconds(30 / 1000f);
        }
    }

    void ResetFields()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
