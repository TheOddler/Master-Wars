using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Unity Growl, By Pablo Bollansée
//This has nothing to do with Growl, I just love that app, the only popups I don't really hate.

public class UnityGrowl : MonoBehaviour
{
	public GUISkin _GUISkin;
	public enum position{TopLeft, TopRight, BottomLeft, BottomRight}
	public position _position = position.TopRight;
	
	static position _pos;
	static GUIStyle _style;
	static List<Popup> _popups = new List<Popup>();
	
	void Start()
	{
		_pos = _position;
		if(_GUISkin)
		{
			_style = _GUISkin.FindStyle("popup") ?? _GUISkin.FindStyle("textArea");
		}
	}
	void Update()
	{
		_popups.ForEach(p => p.Life -= Time.deltaTime);
		_popups.RemoveAll(IsDead);
	}
	private bool IsDead(Popup popup)
	{
		if(popup.Life <= 0)
			return true;
		
		return false;
	}
	void OnGUI()
	{
		if(_style  == null)
		{
			_style = GUI.skin.FindStyle("box");
		}
		
		//Fullscreen Area to make the FlexibleSpaces work
		GUILayout.BeginArea(new Rect(0,0,Screen.width, Screen.height));
		
		GUILayout.BeginHorizontal();
		if(_pos == position.BottomRight || _pos == position.TopRight)
			GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		
			if(_pos == position.BottomLeft || _pos == position.BottomRight)
			{
				GUILayout.FlexibleSpace();
				for(int i = _popups.Count-1; i >= 0; --i)
				{
					GUILayout.Box(_popups[i].Message, _style);
				}
			}
			else
			{
				for(int i = 0, l = _popups.Count; i < l; ++i)
				{
					GUILayout.Box(_popups[i].Message, _style);
				}
			}
		
		GUILayout.EndVertical();
		if(_pos == position.BottomLeft || _pos == position.TopLeft)
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}
	
	static public void Show(string message)
	{
		ShowPopup(message, 3);
	}
	static public void Show(string message, float life)
	{
		ShowPopup(message, life);
	}
	
	
	static public void RemoveAll()
	{
		HideAllPopups();
	}
	
	private static void ShowPopup(string message, float life)
	{
		_popups.Add(new Popup(message, life));
	}
	private static void HideAllPopups()
	{
		_popups.Clear();
	}
}

internal class Popup
{
	public string Message{get;set;}
	public float Life{get;set;}
	
	public Popup(string message, float life)
	{
		Message = message;
		Life = life;
	}
}
	