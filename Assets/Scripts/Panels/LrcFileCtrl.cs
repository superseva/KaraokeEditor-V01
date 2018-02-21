using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LitJson;
using System.Text;

public class LrcFileCtrl : MonoBehaviour {

    public static string LYRICS = "lyrics";
    public static string TIMESTAMPS = "timestamps";

    string lyricsString;
    //string[] songWords = new string[] { };
    //string[] songTimestamps;
    MatchCollection matchWords;
    MatchCollection matchTimestamps;
    WordData[] wordsList;

    //UI Parts
    public Text lyricsPathTxt;

    public InputField jsonOutputTxt;
    public InputField songNameTxt;
    public InputField songArtist;
    public InputField songBPMTxt;

    // Use this for initialization
    void Start()
    {
        //FileBrowser.SetFilters(true, new FileBrowser.Filter("Lrc Files", ".lrc"));
    }

    public void OpenLrcFileBrowser()
    {
        
        FileBrowser.SetDefaultFilter(".lrc");
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(false, null, "Open", "Load");

        if (FileBrowser.Success)
        {
            StartCoroutine(LoadFile(FileBrowser.Result));
        }
        else
        {
            UIEventManager.FireAlert("Error opening file", "ERROR");
            ResetFields();
        }
    }

    IEnumerator LoadFile(string filePath)
    {
        string url = string.Format("file:///{0}", filePath);
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            lyricsPathTxt.text = filePath;
            lyricsString = www.text;
        }
    }

    public void MergeLyricsAndTimes()
    {
        if (string.IsNullOrEmpty(lyricsString))
        {
            UIEventManager.FireAlert("Not all files prsent !", "ALERT");
            return;
        }

        // remove first 5 lines and the last one;
        string[] remN = lyricsString.Split('\n');
        List<string> strLst = new List<string>(remN);
        strLst.RemoveRange(0, 5);
        strLst.RemoveRange(strLst.Count-2, 2);
        string strFromLst = String.Join(" ", strLst.ToArray());

        // remove line breaks
        string stringOneLine = Regex.Replace(strFromLst, @"\r\n?|\n", String.Empty);

        // replace < and > for [ and ] and add one "[" at the end
        string strClean = StringExtention.clean(stringOneLine) + "[";
        jsonOutputTxt.text = strClean;

        // pull out all the timestamps from mm:ss.ss
        string patternTimestamps = @"\d\:\d{1,2}.\d{1,2}";
        matchTimestamps = Regex.Matches(strClean, patternTimestamps);

        // pull the words between ] and [
        string patternWords = @"\](.*?)\[";
        matchWords = Regex.Matches(strClean, patternWords);

        if (matchWords.Count != matchTimestamps.Count)
        {
            UIEventManager.FireAlert("Count doesn't match \n" + "lyrics lines: " + matchWords.Count + " - vs - " + "timestamps lines: " + matchTimestamps.Count, "WORD COUNT MISSMATCH");
            return;
        }

        //Debug.Log("All is OK... proceed");

        //songWords = new string[matchWords.Count];
        int i = 0;
        int w = matchWords.Count;
       // for (i = 0; i < w; i++)
        //{
            //songWords[i] = matchWords[i].ToString().TrimStart(']', ' ').TrimEnd('[');
       // }

        //songTimestamps = new string[matchTimestamps.Count];
       // int n = matchTimestamps.Count;
        //for (i = 0; i < n; i++)
        //{
            //songTimestamps[i] = formatTimeToSeconds(matchTimestamps[i].ToString());
        //}

        WordData word;
        wordsList = new WordData[matchWords.Count];
        for(i=0; i<w; i++)
        {
            word = new WordData();
            word.text = matchWords[i].ToString().TrimStart(']', ' ').TrimEnd('[');
            word.time = formatTimeToSeconds(matchTimestamps[i].ToString());
            word.index = i;
            wordsList[i] = word;

        }

        //// Proceed to Save
        SaveToJson();
    }

    public void ResetFields()
    {
        lyricsPathTxt.text = string.Empty;
        lyricsString = string.Empty;
    }

    // SAVING PART
    public void SaveToJson()
    {
        if(matchWords.Count < 1 || matchTimestamps.Count< 1)
        {
            UIEventManager.FireAlert("Please merge lyrics with text first", "NO LYRICS OR TIMESTAMPS");
            return;
        }
        if (string.IsNullOrEmpty(songNameTxt.text) || string.IsNullOrEmpty(songArtist.text) || string.IsNullOrEmpty(songBPMTxt.text))
        {
            UIEventManager.FireAlert("Please populate song name, artist and BPM", "NO SONG INFO FOUND");
            return;
        }

        SongData song = new SongData();
        song.songname = songNameTxt.text;
        song.bpm = int.Parse(songBPMTxt.text);
        song.artist = songArtist.text;

        //song.words = songWords;
        //song.timestamps = songTimestamps;
        song.wordsList = wordsList;

        string jsonToSave = JsonMapper.ToJson(song);
        jsonOutputTxt.text = jsonToSave;

        FileBrowser.SetDefaultFilter(".json");
        StartCoroutine(ShowSaveDialogCoroutine(jsonToSave.ToString()));

        //File.WriteAllText(Application.persistentDataPath + "/" + song.artist + "_" + song.songname+".json", jsonToSave.ToString());

        //UIEventManager.FireAlert("File Saved to " + Application.persistentDataPath + "/" + song.artist + "_" + song.songname + ".json", "SUCCESS");
    }

    IEnumerator ShowSaveDialogCoroutine(string jsonToSaveString)
    {
        yield return FileBrowser.WaitForSaveDialog(false, null);
        if (FileBrowser.Success)
        {
            File.WriteAllText(FileBrowser.Result, jsonToSaveString);
            UIEventManager.FireAlert("Saved to: " + FileBrowser.Result, "SAVE SUCCESS");
        }
    }

    // HELPERS
    // Create seconds as floats.ToString() from the format <mm:ss.sss>
    // Need to return ToString because JSON parser bug 
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
}

public static class StringExtention
{
    public static string clean(this string s)
    {
        StringBuilder sb = new StringBuilder(s);
        sb.Replace("<", "[");
        sb.Replace(">", "]");
        sb.Replace(",", "");
        sb.Replace("  ", " ");
        return sb.ToString();
    }
}