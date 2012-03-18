using UnityEngine;
using System.Collections;

static public class TeamsInfo
{
	public enum Colour
	{
		Neutral = 0, Black, Blue, Green, Red, White, Yellow
	};
	private static GUIContent[] _colourGUIContentList;

	static TeamsInfo()
	{
		//Keep in sync with the Colours
		_colourGUIContentList = new GUIContent[7]
		{
			new GUIContent("Neu"),
			
			new GUIContent("Bla"),
			new GUIContent("Blu"),
			new GUIContent("Gre"),
			new GUIContent("Red"),
			new GUIContent("Whi"),
			new GUIContent("Yel"),
		};
	}
	
	public static GUIContent[] ColourGUIContentList{get{return _colourGUIContentList;}}
	public static int NumberOfTeamColors
	{
		get{return _colourGUIContentList.Length;}
	}
}
