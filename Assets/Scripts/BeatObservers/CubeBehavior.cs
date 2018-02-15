using UnityEngine;
using System.Collections;
using SynchronizerData;
using UnityEngine.UI;

public class CubeBehavior : MonoBehaviour
{

   // private Animator anim;
    private BeatObserver beatObserver;
    public Image img;

    void Start()
    {
       // anim = GetComponent<Animator>();
        beatObserver = GetComponent<BeatObserver>();

        Debug.Log("Modulo : " + (12%5));
    }

    void Update()
    {
        //Debug.Log("beatObserver.beatMask " + beatObserver.beatMask);
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            // anim.SetTrigger("DownBeatTrigger");
            //Debug.Log("klik");
            transform.Rotate(Vector3.forward, 45f);
            if(img.transform.position.x == -64)
            {
                img.transform.position += new Vector3(64, 0, 0);
            }
            img.transform.position += new Vector3(-4, 0, 0);
        }
        if ((beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        {
            //Debug.Log("klok");
            transform.Rotate(Vector3.forward, 45f);
        }
    }
}
