using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;
using UnityEngine.UI;

public class BarImageBehavior : MonoBehaviour {

    public GameObject wordsHolder;

    private BeatObserver beatObserver;

	// Use this for initialization
	void Start () {
        beatObserver = GetComponent<BeatObserver>();
    }
	
	// Update is called once per frame
	void Update () {
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            if (this.transform.position.x == -64)
            {
                this.transform.position += new Vector3(64, 0, 0);
            }
            this.transform.position += new Vector3(-16, 0, 0);
            wordsHolder.transform.position += new Vector3(-16, 0, 0);
        }
    }
}
