using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewPlayer : MonoBehaviour {

    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public AudioClip audioClip;
    [HideInInspector]
    public SongData songData;

    public GameObject gamePanel;
    public GameObject wordPreab;

    public Image playBtn;
    public Image stopBtn;

    public float timeonScreen = 3f;

    private float startX;
    private float startY;
    private float endX;
    private float endY;
    private float distance;

    private float currentTime;
    private float sampleRate = 44100f;
    private int listIndex = 0;
    
    private float nextShowTime = 0;
    private float nextHitTime = 0;
    private List<GameObject> wordsCollection = new List<GameObject>();

	// Use this for initialization
	void Start () {

        startX = gamePanel.GetComponent<RectTransform>().position.x;
        startY = gamePanel.GetComponent<RectTransform>().position.y;
        endY = gamePanel.GetComponent<RectTransform>().position.y - gamePanel.GetComponent<RectTransform>().rect.height;

        distance = gamePanel.GetComponent<RectTransform>().rect.height;

        //GameObject go = Instantiate(wordPreab, gamePanel.transform);
        //go.GetComponent<RectTransform>().position = new Vector3(startX, endY, 0);

        //go.GetComponent<RectTransform>().position += new Vector3(0, -gamePanel.GetComponent<RectTransform>().rect.height, 0);
        //Debug.Log(gamePanel.GetComponent<RectTransform>().rect);
        //Debug.Log(go.GetComponent<RectTransform>().anchoredPosition);
    }

    public void PreviewSong(AudioSource aSrc, SongData sdata, float speed)
    {
        audioSource = aSrc;
        audioClip = aSrc.clip;
        songData = sdata;
        timeonScreen = speed;

        listIndex = 0;
        
        GenerateWords();

        nextShowTime = float.Parse(songData.wordsList[0].time) - timeonScreen;

        PlaySong();
    }

    void GenerateWords()
    {
        GameObject go;
        WordGameCtrl goCtrl;
        string wordText;
        string trimWord;
        for (int i  = 0; i < songData.wordsList.Length; i++)
        {
            go = Instantiate(wordPreab, gamePanel.transform);
            trimWord = songData.wordsList[i].text.TrimEnd();
            wordText = trimWord.Replace( " ", "\n");
            go.GetComponentInChildren<Text>().text = wordText;
            goCtrl = go.GetComponent<WordGameCtrl>() as WordGameCtrl;
            goCtrl.startX = GetStartX(songData.wordsList[i].index);
            goCtrl.startY = 0;
            goCtrl.showTime = float.Parse(songData.wordsList[i].time) - timeonScreen;
            goCtrl.hitTime = float.Parse(songData.wordsList[i].time);
            goCtrl.existanceTime = timeonScreen;
            Debug.Log(goCtrl.showTime + ",  " + goCtrl.hitTime);
            go.GetComponent<RectTransform>().localPosition = new Vector3(goCtrl.startX, 0, 0);
            wordsCollection.Add(go);
            go.SetActive(false);
        }
    }

    

    float GetStartX(int index)
    {
        float i = 0f;
        if (index == 1)
        {
            i = (gamePanel.GetComponent<RectTransform>().rect.width/4);
        }else if (index == 2)
        {
            i = gamePanel.GetComponent<RectTransform>().rect.width / 2;
        }
        else if (index == 3)
        {
            i = gamePanel.GetComponent<RectTransform>().rect.width - (gamePanel.GetComponent<RectTransform>().rect.width / 4);
        }
        else
        {
            i = gamePanel.GetComponent<RectTransform>().rect.width / 2;
        }
        return i;
    }

    void PlaySong()
    {
        audioSource.Play();
        //forward to the first word
        float firstWordShowTime = wordsCollection[0].GetComponent<WordGameCtrl>().showTime;
        audioSource.timeSamples = Mathf.CeilToInt((sampleRate) * (firstWordShowTime - 1));
        playBtn.gameObject.SetActive(false);
        stopBtn.gameObject.SetActive(true);
    }

    public void StopPreview()
    {
        audioSource.Stop();
        wordsCollection.Clear();
        foreach (Transform child in gamePanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        listIndex = 0;
        playBtn.gameObject.SetActive(true);
        stopBtn.gameObject.SetActive(false);
    }

    // Update is called once per frame
    GameObject currentWord;
	void Update () {
        if (!audioSource)
            return;

        if (!audioSource.isPlaying)
            return;

        currentTime = audioSource.timeSamples / sampleRate;

        if (currentTime >= nextShowTime)
        {
            currentWord = wordsCollection[listIndex];
            currentWord.SetActive(true);

            listIndex++;
            nextShowTime = wordsCollection[listIndex].GetComponent<WordGameCtrl>().showTime;
        }

        WordGameCtrl wordCtrl;
        float rangeTime;
        float percentTime;
        float newY;
        foreach(GameObject gWord in wordsCollection)
        {
            if (gWord.activeSelf)
            {
                wordCtrl = gWord.GetComponent<WordGameCtrl>();
                rangeTime = wordCtrl.hitTime - wordCtrl.showTime;
                percentTime = (currentTime - wordCtrl.showTime) / rangeTime;
                newY = 0 - (distance * percentTime);
                gWord.GetComponent<RectTransform>().localPosition = new Vector3(wordCtrl.startX, newY, 0);

            }
        }
        
        
	}
}
