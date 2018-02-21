using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class TempHelper : MonoBehaviour {

    private string allWordsStr = "Hate to ,  burst your ,  bubble ,  bitch, I'm ,  that ,  weird ,  girl ,  that's ,  runnin' ,  shit, I'm ,  a ,  boss ,  bitch ,  runnin' ,  big ,  shit, Got ,  a ,  compa ,   ny ,   need ,  a ,  couple ,  mil', Ain't ,  no ,  rap ,  talk ,  this ,  my ,  real ,  life, And that's ,  on ,  God ,  I almost ,  died ,  twice, So I ,  go ,  ahead ,  and I ,  get ,  mine, And I ,  cash ,  checks ,  and I ,  get ,  fly, Pelle , Pelle ,  with the ,  BB ,  belt, Skinny , jeans ,  and a ,  studded ,  belt, I've been ,  fly ,  never ,  needed ,  help, I ,  been ,  me ,  ain't ,  nobody ,  else, Skinny ,  jeans ,  and a ,  pair of ,  Vans, Pants ,  sag ,  'til they ,  hit my ,  ass, Lit ,  lit ,  I'm a ,  do my ,  dance, Like a ,  raver ,  bitch ,  goin' ,  in a ,  trance, Av ,  ril ,  I’m a ,  skater ,  boy, Ani ,  me ,  and a ,  lot of ,  toys, My ,  Space ,  made a ,  lot of ,  noise, That's ,  middle ,  school ,  and I'm ,  actin' ,  coy, Back of the ,  class they ,  sending my ,  ass, Roll up my ,  skirt and they ,  think that I'm ,  fast, I got no ,  ass and ,  I got no ,  titties, But ,  all of your ,  dudes they ,  hit me to ,  hit me";
    private string allTimesStr = "00:24.47, 00:24.96, 00:25.50, 00:26.05, 00:26.30, 00:26.47, 00:26.70, 00:27.02, 00:27.29, 00:27.55, 00:27.89, 00:28.23, 00:28.42, 00:28.61, 00:28.97, 00:29.30, 00:29.60, 00:29.95, 00:30.25, 00:30.42, 00:30.66, 00:30.95, 00:31.29, 00:31.51, 00:31.71, 00:32.03, 00:32.35, 00:32.53, 00:32.72, 00:33.05, 00:33.32, 00:33.50, 00:33.68, 00:34.04, 00:34.35, 00:34.65, 00:34.98, 00:35.29, 00:35.61, 00:35.97, 00:36.29, 00:36.66, 00:37.04, 00:37.36, 00:37.72, 00:38.07, 00:38.40, 00:38.76, 00:39.10, 00:39.41, 00:39.75, 00:40.08, 00:40.69, 00:41.07, 00:41.40, 00:41.77, 00:42.12, 00:42.73, 00:43.09, 00:43.42, 00:43.84, 00:44.15, 00:44.78, 00:45.15, 00:45.50, 00:45.87, 00:46.21, 00:46.54, 00:46.83, 00:47.23, 00:47.52, 00:47.80, 00:48.18, 00:48.72, 00:49.17, 00:49.53, 00:49.91, 00:50.23, 00:50.87, 00:51.26, 00:51.58, 00:51.92, 00:52.25, 00:52.87, 00:53.25, 00:53.59, 00:53.93, 00:54.31, 00:54.65, 00:55.01, 00:55.34, 00:55.64, 00:55.99, 00:56.24, 00:56.92, 00:57.32, 00:57.71, 00:58.07, 00:58.38, 00:59.02, 00:59.36, 00:59.73, 01:00.04, 01:00.23, 01:01.03, 01:01.41, 01:01.79, 01:02.15, 01:02.48, 01:02.79, 01:03.13, 01:03.45, 01:03.80, 01:04.11, 01:04.52, 01:05.09, 01:05.58, 01:06.13, 01:06.64, 01:07.16, 01:07.69, 01:08.20, 01:08.69, 01:09.18, 01:09.67, 01:10.21, 01:10.75, 01:11.12, 01:11.42, 01:11.79, 01:12.25, 01:12.71";

    private List<string> words = new List<string>();
    private List<double> times = new List<double>();

    public InputField jsonOutputTxt;


    // Use this for initialization
    void Start () {

        Debug.Log("ALO");
        var song = new SongData();
        song.artist = "superseva";
        song.songname = "myMysic";
        song.bpm = 118;

        



    }

    string[] minsec;
    float minutes;
    string formatTimeToSeconds(string stringTime)
    {
        float result = 0;
        minsec = stringTime.Split(':');
        minutes = float.Parse(minsec[0]) * 60;
        result = minutes + float.Parse(minsec[1]);
        return result.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
