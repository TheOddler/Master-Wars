using UnityEngine;
using System.Collections;

public class Crosshair_DrawScript : MonoBehaviour {
	public Texture _crosshair;
	public int _size = 8;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(Screen.width/2.0f - _size/2.0f, Screen.height/2.0f - _size/2.0f, _size, _size), _crosshair);
	}
}
