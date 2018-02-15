using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Texture2DHelper : MonoBehaviour {

    public RawImage img;

	// Use this for initialization
	void Start () {
        Texture2D texture = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = ((x & y) != 0 ? new Color(0,0,0,0) : Color.gray);
                //Color color = new Color(120,42,120,100);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        img.texture = texture;
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
