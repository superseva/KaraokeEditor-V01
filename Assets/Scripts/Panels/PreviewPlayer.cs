using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPlayer : MonoBehaviour {

    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public AudioClip audioClip;
    [HideInInspector]
    public SongData songData;

	// Use this for initialization
	void Start () {
		
	}

    public void PreviewSong(AudioSource aSrc, SongData sdata)
    {
        audioSource = aSrc;
        audioClip = aSrc.clip;
        songData = sdata;
        PlaySong();
    }

    void PlaySong()
    {
        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
