using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SongData
{
    public string songname;
    public int bpm;
    public string artist;
    public WordData[] wordsList;
    public float mp3dealy = 0;
    //public string[] words;
    //public string[] timestamps;
}
