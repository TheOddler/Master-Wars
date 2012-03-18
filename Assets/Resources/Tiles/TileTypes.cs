using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public struct TileInfo
{
	public TileInfo(ushort number, string tag, GUIContent guiContent, string resourcePath)
	{
		Number = number;
		Tag = tag;
		GuiContent = guiContent;
		ResourcePath = resourcePath;
	}

	public ushort Number;
	public string Tag;
	public GUIContent GuiContent;
	public string ResourcePath;
}

static public class TileTypes// : MonoBehaviour
{
	//Make TileInfoList, TerraintInfoList & BuildingsInfoList
	//TileInfoList will be filled automaticly, it contains both the info in the TerrainList and BuildingsList
	//The info in TileInfoList will always be ordered the same as Terrain & Buildings List, with the info from the TerrainList FIRST, followed by the BuildingsList
	private static List<TileInfo> _tileInfoList;
	private static List<TileInfo> _terrainInfoList;
	private static List<TileInfo> _buildingsInfoList;
	
	// The GUIContent list will be filled in automaticly based on the InfoLists, idem for Terrain & Buildings
	private static GUIContent[] _tileGUIContentList;
	private static GUIContent[] _terrainGUIContentList;
	private static GUIContent[] _buildingsGUIContentList;
	
	//
	//Fill Info Lists & extract GUI Arrays
	//-------------------------------------------------------------------------
	static TileTypes()
	{
		//
		//Fill in TerrainInfoList & BuildginsInfoList
		//-------------------------------------------
		_terrainInfoList= new List<TileInfo>
		{
			new TileInfo(0, "Blank", 		new GUIContent("Blank"), 	"Tiles/BlankTile/BlankTile"),
			
			new TileInfo(101, "Plains",		new GUIContent("Plains"),	"Tiles/Terrain/Plains/Plains"),
			new TileInfo(102, "Forest",		new GUIContent("Forest"),	"Tiles/Terrain/Forest/Forest"),
			new TileInfo(103, "Mountain",		new GUIContent("Mountain"),	"Tiles/Terrain/Mountain/Mountain"),
			new TileInfo(104, "Road",			new GUIContent("Road"),		"Tiles/Terrain/Road/Road"),
			new TileInfo(105, "Bridge",		new GUIContent("Bridge"),	"Tiles/Terrain/Bridge/Bridge"),
			new TileInfo(106, "River",		new GUIContent("River"),	"Tiles/Terrain/River/River"),
			new TileInfo(107, "Beach",		new GUIContent("Beach"),	"Tiles/Terrain/Beach/Beach"),
			new TileInfo(108, "Sea",			new GUIContent("Sea"),		"Tiles/Terrain/Sea/Sea"),
			new TileInfo(109, "Reef",			new GUIContent("Reef"),		"Tiles/Terrain/Reef/Reef"),
		};
		_buildingsInfoList= new List<TileInfo>
		{
			new TileInfo(201, "HQ",			new GUIContent("HQ"),		"Tiles/Buildings/HQ/HQ"),
			new TileInfo(202, "City",		new GUIContent("City"),		"Tiles/Buildings/City/City"),
			new TileInfo(203, "Factory",	new GUIContent("Factory"),	"Tiles/Buildings/Factory/Factory"),
			new TileInfo(204, "Airport",	new GUIContent("Airport"),	"Tiles/Buildings/Airport/Airport"),
			new TileInfo(205, "Port",		new GUIContent("Port"),		"Tiles/Buildings/Port/Port"),
		};
		
		//
		//Fill TileInfoList based on TerrainList and BuildingsList
		//--------------------------------------------------------
		_tileInfoList= new List<TileInfo>();
		_tileInfoList.AddRange(_terrainInfoList); // ALWAIS ADD TERRAIN FIRST !
		_tileInfoList.AddRange(_buildingsInfoList);
		
		
		//
		//Create GUIContent lists
		//-----------------------
		_tileGUIContentList = new GUIContent[_tileInfoList.Count];
		for(int i = 0; i < _tileInfoList.Count; ++i)
		{
			_tileGUIContentList[i] = _tileInfoList[i].GuiContent;
		}
		_terrainGUIContentList = new GUIContent[_terrainInfoList.Count];
		for(int i = 0; i < _terrainInfoList.Count; ++i)
		{
			_terrainGUIContentList[i] = _terrainInfoList[i].GuiContent;
		}
		_buildingsGUIContentList = new GUIContent[_buildingsInfoList.Count];
		for(int i = 0; i < _buildingsInfoList.Count; ++i)
		{
			_buildingsGUIContentList[i] = _buildingsInfoList[i].GuiContent;
		}
	}
	
	//
	//Getters for GUIContentLists
	//--------------------------------------------------------
	public static GUIContent[] TilesGUIContentList{get{return _tileGUIContentList;}}
	public static GUIContent[] TerrainGUIContentList{get{return _terrainGUIContentList;}}
	public static GUIContent[] BuildingsGUIContentList{get{return _buildingsGUIContentList;}}
	
	
	//
	//Getters for TileInfo
	//-------------------------------------------------------------
	//Tile
	public static TileInfo GetTileInfo(int id)
	{
		if(id >= _tileInfoList.Count)
			return _tileInfoList[0];
		
		return _tileInfoList[id];
	}
	public static TileInfo GetTileInfo(string tag)
	{
		foreach(TileInfo inf in _tileInfoList)
		{
			if(inf.Tag == tag)
				return inf;
		}
		return _tileInfoList[0];
	}
	public static TileInfo GetTileInfo(ushort number)
	{
		foreach(TileInfo info in _tileInfoList)
		{
			if(info.Number == number)
				return info;
		}
		return _tileInfoList[0];
	}
	//Terrain
	public static TileInfo GetTerrainTileInfo(int id)
	{
		if(id >= _terrainInfoList.Count)
			return _tileInfoList[0];
		
		return _terrainInfoList[id];
	}
	public static TileInfo GetTerrainTileInfo(string tag)
	{
		foreach(TileInfo inf in _terrainInfoList)
		{
			if(inf.Tag == tag)
				return inf;
		}
		return _tileInfoList[0];
	}
	//Building
	public static TileInfo GetBuildingTileInfo(int id)
	{
		if(id >= _buildingsInfoList.Count)
			return _tileInfoList[0];
		
		return _buildingsInfoList[id];
	}
	public static TileInfo GetBuildingTileInfo(string tag)
	{
		foreach(TileInfo inf in _buildingsInfoList)
		{
			if(inf.Tag == tag)
				return inf;
		}
		return _tileInfoList[0];
	}
	
	public static int TerrainIDToListID(int id)
	{
		if(id < 0 || id > _terrainInfoList.Count)
			return -1;
		return id;
	}
	public static int BuildingsIDToListID(int id)
	{
		id += _terrainInfoList.Count;
		if(id < _terrainInfoList.Count || id > _buildingsInfoList.Count+_terrainInfoList.Count)
			return -1;
		return id;
	}
	public static int ListIDToTerrainID(int id)
	{
		if(id < 0 || id > _terrainInfoList.Count)
			return -1;
		return id;
	}
	public static int ListIDToBuildingsID(int id)
	{
		id -= _terrainInfoList.Count;
		if(id < 0 || id > _buildingsInfoList.Count)
			return -1;
		return id;
	}
	
	public static int NumberOfTileTypes()
	{
		return _tileGUIContentList.Count();
	}
	public static int NumberOfTerrainTypes()
	{
		return _terrainInfoList.Count();
	}
	public static int NumberOfBuildingTypes()
	{
		return _buildingsGUIContentList.Count();
	}
}
