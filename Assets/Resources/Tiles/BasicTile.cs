using UnityEngine;
using System.Collections;

public class BasicTile : MonoBehaviour {
	//Col and Row
	private int _collumn, _row;
	//Colour
	TeamsInfo.Colour _teamColour = TeamsInfo.Colour.Neutral;
	
	public int Collumn
	{
		get{return _collumn;}
		set{_collumn = value;}
	}
	public int Row
	{
		get{return _row;}
		set{_row = value;}
	}
	public TeamsInfo.Colour Colour
	{
		get{return _teamColour;}
		set{_teamColour = value;}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
