using UnityEngine;
using System.Collections;
using System.IO;

public struct RectSize
{
	public RectSize(int w, int h)
	{
		width = w;
		height = h;
	}
	public int width;
	public int height;
}

public class Board_Editing_Script : MonoBehaviour {
	//An optional script that can be placed on the board if you want to be able to edit it.
	//This script requires that there is a Board_Script on the board, otherwise it won't know what to edit.
	//This script will also draw the editing gui.
	
	//Get some info about the board
	Board_Script _boardScript;
	
	//GUI
	public GUISkin _GUISkin;
	public Texture _mouseNone;
	public Texture _mouseRight;
	public Texture _mouseLeft;
	public Texture _mouseBoth;
	Vector2 _toolbarPosition = new Vector2(10,10);
	//Vector2 _GUIPosition = new Vector2(10,40);
	bool _mouseIsOnGUI = false;
	
	//New - 0
	const int newID = 0;
	bool _drawNewWindow = true;
	Rect _newWindowRect = new Rect(10, 40, 220, 260);
	const int _minSize = 8;
	const int _maxSize = 52;
	int _numColumns = _minSize, _numRows = _minSize;
	int _newListEntry = 0;
	TileInfo _newType;// = TileType.Blank;
	
	//Change - 1
	const int changeID = 1;
	bool _drawChangeWindow = false;
	Rect _changeWindowRect = new Rect(10, 40, 220, 370);
	TileInfo _changeType;// = TileType.Blank;
	int _changeTypeEntry = 0;
	int _changePositionEntry = 4; //mid (0-based)
	public Texture UL, UM, UR, L, M, R, LL, LM, LR;
	GUIContent[] _changePositionGUIContent;
	int _changeNewNumCols = _minSize, _changeNewNumRows = _minSize;
	
	//Fill - 2
	const int fillID = 2;
	bool _drawFillWindow = false;
	Rect _fillWindowRect = new Rect(10, 40, 220, 180);
	int _fillListEntry = 0;
	TileInfo _fillType;// = TileType.Blank;
	
	//Terrain & Buildings - 3
	const int terrainAndBuildingsID = 3;
	bool _drawTerrainAndBuildingsWindow = false;
	Rect _terrainAndBuildingsWindowRect = new Rect(10, 40, 220, 350);
	int _leftListEntry = 0;
	TileInfo _leftType;// = TileType.Blank;
	int _rightListEntry = 1;
	TileInfo _rightType;// = TileType.Blank;
	TeamsInfo.Colour _selectedBuildingsColour = TeamsInfo.Colour.Neutral;
	
	//Units - 4
	const int unitsID = 4;
	bool _drawUnitsWindow = false;
	Rect _unitsWindowRect = new Rect(10, 40, 220, 100);
	bool _editingUnits = false;
	
	//Save/Load - 5
	const int saveLoadID = 5;
	bool _drawSaveLoadWindow = false;
	Rect _saveLoadWindowRect = new Rect(10, 40, 370, 240);
	FileBrowser _saveDirectoryBrowser;
	string _saveDirectoryPath;
	string _saveName = "Name";
	string _saveDescription = "Description";
	FileBrowser _loadFileBrowser;
	string _loadFilePath;
	string[] _simpleLoadFilesArray;
	bool _simpleSelectingFileToLoad;
	int _simpleLoadFileID;
	Vector2 _simpleLoadScrollPosition = Vector2.zero;
	RectSize _nonBrowsingSize = new RectSize(370, 240);
	RectSize _browsingSize = new RectSize(520, 500);
	const string LevelFileExtension = "mwm";
	bool _simpleSaveLoadMode = true;
	
	
	void Awake()
	{
		//Find the Board_Script
		_boardScript = GetComponent<Board_Script>();
	}
	void Start ()
	{
		//Set default values
		_newType = TileTypes.GetTerrainTileInfo(0);
		_changeType = TileTypes.GetTerrainTileInfo(0);
		_fillType = TileTypes.GetTerrainTileInfo(0);
		_leftType = TileTypes.GetTerrainTileInfo(0);
		_rightType = TileTypes.GetTerrainTileInfo(1);
		
		//Fill In the icons
		_changePositionGUIContent = new GUIContent[]
		{
			new GUIContent(UL),new GUIContent(UM),new GUIContent(UR),
			new GUIContent(L),new GUIContent(M),new GUIContent(R),
			new GUIContent(LL),new GUIContent(LM),new GUIContent(LR),
		};
	}
	void Update ()
	{
		//Draw
		if (Input.GetAxisRaw("CameraMove") == 0 && _mouseIsOnGUI == false
			&& _editingUnits == false && _drawTerrainAndBuildingsWindow == true)
		{
			if(Input.GetMouseButton(0)) //left
			{
				int col, row;
				if( RayCastTile(out col, out row) )
				{
					ChangeTileToType(_leftType, _selectedBuildingsColour, col, row);
				}
			}
			else if(Input.GetMouseButton(1))//right
			{
				int col, row;
				if( RayCastTile(out col, out row) )
				{
					ChangeTileToType(_rightType, _selectedBuildingsColour, col, row);
				}
			}
		}
	}
	
	void DrawToolbar()
	{
		SmartToolbarButton(new Rect(_toolbarPosition.x, _toolbarPosition.y, 96, 20), ref _drawNewWindow, "New");
		SmartToolbarButton(new Rect(_toolbarPosition.x+100, _toolbarPosition.y, 96, 20), ref _drawChangeWindow, "Change");
		SmartToolbarButton(new Rect(_toolbarPosition.x+200, _toolbarPosition.y, 96, 20), ref _drawFillWindow, "Fill");
		SmartToolbarButton(new Rect(_toolbarPosition.x+300, _toolbarPosition.y, 196, 20), ref _drawTerrainAndBuildingsWindow, "Terrain & Buildings");
		SmartToolbarButton(new Rect(_toolbarPosition.x+500, _toolbarPosition.y, 96, 20), ref _drawUnitsWindow, "Units");
		SmartToolbarButton(new Rect(_toolbarPosition.x+600, _toolbarPosition.y, 96, 20), ref _drawSaveLoadWindow, "Save/Load");
	}
	void SmartToolbarButton(Rect pos, ref bool showWindow, string label)
	{
		const int toggleWidth = 20;
		pos.width -= toggleWidth;
		if(GUI.Button(pos, label, new GUIStyle("buttonleft")))
		{
			//bool windowWasOpen = showWindow;
			if (showWindow)
			{
				showWindow = false;
			}
			else
			{
				HideAllTools();
				showWindow = true;
			}
		}
		pos.x = pos.x + pos.width;
		pos.width = toggleWidth;
		showWindow = GUI.Toggle(pos, showWindow, "+", new GUIStyle("buttontoggleright"));
	}
	void HideAllTools()
	{
		_drawNewWindow = false;
		_drawChangeWindow = false;
		_drawFillWindow = false;
		_drawTerrainAndBuildingsWindow = false;
		_drawUnitsWindow = false;
		_drawSaveLoadWindow = false;
	}
	
	int ToMultipleOfFour(float nmbr)
	{
		nmbr += 2.0f;
		return (((int)nmbr)/4) *4;
	}
	
	void DrawNewGUI(int WindowID)
	{
		//Show size options
		GUI.Label(new Rect(10, 20, 200,20), "Choose Size");
		GUI.Label(new Rect(10, 40, 70,20), "Cols ("+_numColumns+"):");
		_numColumns = ToMultipleOfFour( GUI.HorizontalSlider(new Rect(80, 45, 125,20), _numColumns, _minSize, _maxSize) );
		GUI.Label(new Rect(10, 60, 70,20), "Rows ("+_numRows+"):");
		_numRows = ToMultipleOfFour( GUI.HorizontalSlider(new Rect(80, 65, 125,20), _numRows, _minSize, _maxSize) );
		
		//Show base type selection
		GUI.Label(new Rect(10, 90, 200,20), "Select Base Type");
		_newListEntry = GUI.SelectionGrid(new Rect(10, 110, 200, 80), _newListEntry, TileTypes.TerrainGUIContentList, 3);
		_newType = TileTypes.GetTerrainTileInfo(_newListEntry);
		
		//Show new button
		if(GUI.Button(new Rect(10, 210, 200,40), "New !"))
		{
			CreateNewBoard(_numColumns, _numRows, _newType);
		}
		
		DrawGUIWindowCloseButton(ref _drawNewWindow);
		GUI.DragWindow (new Rect (0,0,10000,20));
	}
	void DrawChangeGUI(int WindowID)
	{
		//Size
		GUI.Label(new Rect(10, 20, 200,20), "Choose New Size");
		
		string difCols = "";
		if(_changeNewNumCols>_boardScript.GetNumberOfColumns())
			difCols += "+";
		difCols += ( _changeNewNumCols-_boardScript.GetNumberOfColumns() );
		GUI.Label(new Rect(10, 40, 100,20), "Cols ("+difCols+"="+_changeNewNumCols+"):");
		_changeNewNumCols = ToMultipleOfFour( GUI.HorizontalSlider(new Rect(105, 45, 100,20), _changeNewNumCols, _minSize, _maxSize) );
		
		string difRows = "";
		if( _changeNewNumCols>_boardScript.GetNumberOfRows() )
			difRows += "+";
		difRows += _changeNewNumRows-_boardScript.GetNumberOfRows();
		GUI.Label(new Rect(10, 60, 100,20), "Rows ("+difRows+"="+_changeNewNumRows+"):");
		_changeNewNumRows = ToMultipleOfFour( GUI.HorizontalSlider(new Rect(105, 65, 100,20), _changeNewNumRows, _minSize, _maxSize) );
		
		//Show change type selection
		GUI.Label(new Rect(10, 90, 200,20), "Select Type");
		_changeTypeEntry = GUI.SelectionGrid(new Rect(10, 110, 200, 80), _changeTypeEntry, TileTypes.TerrainGUIContentList, 3);
		_changeType = TileTypes.GetTerrainTileInfo(_changeTypeEntry);
		
		//Show change position selection
		GUI.Label(new Rect(10, 200, 200,20), "Choose direction to expand to");
		_changePositionEntry = GUI.SelectionGrid(new Rect(10, 220, 100,80), _changePositionEntry, _changePositionGUIContent, 3);
		
		//Show change button
		if(GUI.Button(new Rect(10, 320, 200,40), "Change !"))
		{
			ChangeBoardSize(_changeNewNumCols, _changeNewNumRows, _changePositionEntry, _changeType);
		}

		DrawGUIWindowCloseButton(ref _drawChangeWindow);
		GUI.DragWindow (new Rect (0,0,10000,20));
	}
	void DrawFillGUI(int WindowID)
	{
		//Fill TYpe
		GUI.Label(new Rect(10, 20, 200, 20), "Select Fill Type");
		_fillListEntry = GUI.SelectionGrid(new Rect(10, 40, 200, 80), _fillListEntry, TileTypes.TerrainGUIContentList, 3);
		_fillType = TileTypes.GetTerrainTileInfo(_fillListEntry);
		
		//Show fill button
		if(GUI.Button(new Rect(10, 130, 200,40), "Fill !"))
		{
			FillBoardWithType(_fillType);
		}

		DrawGUIWindowCloseButton(ref _drawFillWindow);
		GUI.DragWindow (new Rect (0,0,10000,20));
		
	}
	void DrawTerrainAndBuildingsGUI(int WindowID)
	{
		//
		//Draw Buttons and catch the button on which is pressed in these new ints
		int terrainListEntry = -1;
		int buildingsListEntry = -1;
		//Terrain
		GUI.Label(new Rect(10, 20, 200,20), "Terrain");
		terrainListEntry = GUI.SelectionGrid(new Rect(10, 40, 200, 160), -1, TileTypes.TerrainGUIContentList, 3);
		//Buildings
		GUI.Label(new Rect(10, 210, 200,20), "Buildings");
		_selectedBuildingsColour = (TeamsInfo.Colour)GUI.SelectionGrid(new Rect(10,230, 200, 20), (int)_selectedBuildingsColour, TeamsInfo.ColourGUIContentList, 7);
		buildingsListEntry = GUI.SelectionGrid(new Rect(10, 260, 200, 80), -1, TileTypes.BuildingsGUIContentList, 3);
		//
		//See if any button was hit this time
		if (terrainListEntry!=-1 || buildingsListEntry!=-1)
		{
			//If left button was pressed
			if (Event.current.button == 0) //LEFT
			{
				//Convert terrain id to list id
				int id = TileTypes.TerrainIDToListID(terrainListEntry);
				//See if it is a proper ListID
				if(id >=0)
				{
					//Set the left type
					_leftType = TileTypes.GetTileInfo(id);
				}
				//If it is not a proper listID try the same for buildingsListEntry
				else
				{
					id  = TileTypes.BuildingsIDToListID(buildingsListEntry);
					if(id >=0)
						_leftType = TileTypes.GetTileInfo(id);
				}
				
				//Remember id to be able to draw the mouse icons.
				_leftListEntry = id;
			} 
			//And the same for the right button
			else if (Event.current.button == 1) //RIGHT
			{
				int id = TileTypes.TerrainIDToListID(terrainListEntry);
				if(id >=0)
				{
					_rightType = TileTypes.GetTileInfo(id);
				}
				else
				{
					id  = TileTypes.BuildingsIDToListID(buildingsListEntry);
					if(id >=0)
						_rightType = TileTypes.GetTileInfo(id);
				}
				
				//Remember id to be able to draw the mouse icons.
				_rightListEntry = id;
			}
		}
		
		//
		//Draw the mouse icons on the correct button
		//------------------------------------------------------------------------------------
		Rect leftPos = new Rect(0,0,11,14),
			rightPos = new Rect(0,0,11,14);
		//Calculate the x-positions
		int step = 68;
		int offset = -6;
		if(_leftListEntry < TileTypes.NumberOfTerrainTypes())
			leftPos.x = (_leftListEntry%3 +1) * step + offset;
		else
			leftPos.x = ( (_leftListEntry-TileTypes.NumberOfTerrainTypes())%3 +1 ) * step + offset;
		if(_rightListEntry < TileTypes.NumberOfTerrainTypes())
			rightPos.x = (_rightListEntry%3 +1) * step + offset;
		else
			rightPos.x = ( (_rightListEntry-TileTypes.NumberOfTerrainTypes())%3 +1 ) * step + offset;
		
		//Now calculate the y-positions
		int stepY = 41;
		int buildingsY = 220;
		int extraY = 3;
		int rightExtraY = 16;
		if(_leftListEntry < TileTypes.NumberOfTerrainTypes())
		{
			leftPos.y = (_leftListEntry/3 +1) * stepY + extraY;
		}
		else
		{
			leftPos.y = ( (_leftListEntry-TileTypes.NumberOfTerrainTypes())/3 +1 ) * stepY +buildingsY + extraY;
		}
		if(_rightListEntry < TileTypes.NumberOfTerrainTypes())
		{
			rightPos.y = (_rightListEntry/3 +1) * stepY + rightExtraY + extraY;
		}
		else
		{
			rightPos.y = ( (_rightListEntry-TileTypes.NumberOfTerrainTypes())/3 +1 ) * stepY +buildingsY + rightExtraY + extraY;
		}
		
		//Draw Left
		GUI.DrawTexture(leftPos, _mouseLeft);
		//Draw Right
		GUI.DrawTexture(rightPos, _mouseRight);
		
		DrawGUIWindowCloseButton(ref _drawTerrainAndBuildingsWindow);
		GUI.DragWindow (new Rect (0,0,10000,20));
		
	}
	void DrawUnitsGUI(int WindowID)
	{
		GUI.Label(new Rect(10,20,200,80), "Adding units to a map will be added later.");
		DrawGUIWindowCloseButton(ref _drawUnitsWindow);
		GUI.DragWindow (new Rect (0,0,10000,20));
		
	}
	void DrawSaveLoadGUI(int WindowID)
	{
		if( Application.platform == RuntimePlatform.OSXWebPlayer
			|| Application.platform == RuntimePlatform.WindowsWebPlayer
			|| Application.platform == RuntimePlatform.NaCl)
		{
			GUI.Label(new Rect(10,20,500,80), "Please download the desktop application to save and/or load. Save & load will, hopefully, be available later in the webplayer.");
		}
		else if( Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.OSXPlayer
			|| Application.platform == RuntimePlatform.OSXEditor
			|| Application.platform == RuntimePlatform.WindowsEditor)
		{
			if(_simpleSaveLoadMode)
			{
				if(_simpleSelectingFileToLoad == false)
				{
					_saveDirectoryPath = Directory.GetCurrentDirectory() + "\\Maps";
					
					//Name and description stuff
					GUI.Label(new Rect(10,15,350,20), "Name:");
					_saveName = GUI.TextArea(new Rect(10,35,350,20), _saveName, 50);
					GUI.Label(new Rect(10,55,350,20), "Description:");
					_saveDescription = GUI.TextArea(new Rect(10,75,350,60), _saveDescription, 150);
					
					if(GUI.Button(new Rect(10, 145, 170, 30), "Save!"))
					{
						SaveMap();
					}
					if(GUI.Button(new Rect(190, 145, 170, 30), "Load!"))
					{
						string mapFolderPath = Directory.GetCurrentDirectory() + "\\Maps";
						Directory.CreateDirectory(mapFolderPath);
						_simpleLoadFilesArray = Directory.GetFiles(mapFolderPath, "*."+LevelFileExtension);
						for(int i = 0, l = _simpleLoadFilesArray.Length; i < l; ++i)
						{
							int lastIndexOf = _simpleLoadFilesArray[i].LastIndexOf("\\");
							_simpleLoadFilesArray[i] = _simpleLoadFilesArray[i].Remove(0, lastIndexOf+1);
							lastIndexOf = _simpleLoadFilesArray[i].LastIndexOf(".");
							_simpleLoadFilesArray[i] = _simpleLoadFilesArray[i].Remove(lastIndexOf);
						}
						_simpleSelectingFileToLoad = true;
					}
				}
				else
				{
					GUI.Label(new Rect(10,15,350,20), "Select Map to Load:");
					if(_simpleLoadFilesArray.Length == 0)
					{
						GUI.Label(new Rect(10,35,350,20), "No Maps Found :(");
						if( GUI.Button(new Rect(10, 55, 350, 30), "Ok") )
						{
							_simpleSelectingFileToLoad = false;
						}
					}
					else
					{
						_simpleLoadScrollPosition = GUI.BeginScrollView(new Rect(10, 35, 350, 190), _simpleLoadScrollPosition, new Rect(0, 0, 320, _simpleLoadFilesArray.Length*20+40));
						_simpleLoadFileID = GUILayoutx.SelectionList(_simpleLoadFileID, _simpleLoadFilesArray, SimpleLoadMap);
						GUI.EndScrollView();
					}
				}
			}
			else
			{
				//Name and description stuff
				GUI.Label(new Rect(10,15,350,20), "Name:");
				_saveName = GUI.TextArea(new Rect(10,35,350,20), _saveName, 50);
				GUI.Label(new Rect(10,55,350,20), "Description:");
				_saveDescription = GUI.TextArea(new Rect(10,75,350,60), _saveDescription, 150);
				
				if (_saveDirectoryBrowser == null && _loadFileBrowser == null)
				{
					_saveLoadWindowRect.width = _nonBrowsingSize.width;
					_saveLoadWindowRect.height = _nonBrowsingSize.height;
					
					//SAVE
					if( GUI.Button(new Rect(10,145,290,30),
					               _saveDirectoryPath==null ? "Select Save Directory..." : _saveDirectoryPath
					               ) )
					{
						_saveDirectoryBrowser = new FileBrowser("Select Save Directory", SaveDirectorySelected);
						_saveDirectoryBrowser.BrowserType = FileBrowserType.Directory;
					}
					if( GUI.Button(new Rect(310, 145, 50, 30), "Save!") )
					{
						SaveMap();
					}
					
					//LOAD
					if( GUI.Button(new Rect(10, 185, 290, 30),
					               _loadFilePath==null ? "Select File to Load..." : _loadFilePath
					               ) )
					{
						_loadFileBrowser = new FileBrowser("Select File To Load", LoadFileSelected);
						_loadFileBrowser.BrowserType = FileBrowserType.File;
						_loadFileBrowser.SelectionPattern = "*."+LevelFileExtension;
					}
					if(GUI.Button(new Rect(310, 185, 50, 30), "Load!"))
					{
						LoadMap();
					}
				}
				else
				{
					_saveLoadWindowRect.width = _browsingSize.width;
					_saveLoadWindowRect.height = _browsingSize.height;
					
					if(_saveDirectoryBrowser!=null)
						DrawSaveGUI();
					else
						_loadFileBrowser.OnGUI();
				}
			}
		}
		else
		{
			GUI.Label(new Rect(10,20,200,50), "Please download the desktop application to save and/or load.");
		}
		
		DrawGUIWindowCloseButton(ref _drawSaveLoadWindow);
		GUI.DragWindow (new Rect (0,0,10000,20));
	}
	private void DrawSaveGUI()
	{
		_saveDirectoryBrowser.OnGUI();
	}
	private void SaveDirectorySelected(string path)
	{
		_saveDirectoryBrowser = null;
		_saveDirectoryPath = path;
	}
	private void LoadFileSelected(string path)
	{
		_loadFileBrowser = null;
		_loadFilePath = path;
	}
	
	void SaveMap()
	{
		_boardScript.SaveBoard(_saveName, _saveDescription, _saveDirectoryPath);
	}
	void LoadMap()
	{
		_boardScript.LoadBoard(_loadFilePath, out _saveName, out _saveDescription);
	}
	void SimpleLoadMap(int id)
	{
		_simpleSelectingFileToLoad = false;
		string fullFilePath = Directory.GetCurrentDirectory() + "\\Maps\\" + _simpleLoadFilesArray[id] + "." + LevelFileExtension;
		_boardScript.LoadBoard(fullFilePath, out _saveName, out _saveDescription);
	}

	void DrawGUIWindowCloseButton(ref bool drawWindowBool)
	{
		if(GUI.Button(new Rect(3,3,12,12), " ", new GUIStyle("textfield")))
		{
			drawWindowBool = false;
		}
	}
	
	void OnGUI()
	{
		//
		//Check if mouse is on GUI
		//------------------------------------------------
		Vector2 mousePos = Event.current.mousePosition;
		if(_newWindowRect.Contains(mousePos) && _drawNewWindow)
			_mouseIsOnGUI = true;
		else if(_changeWindowRect.Contains(mousePos) && _drawChangeWindow)
			_mouseIsOnGUI = true;
		else if(_fillWindowRect.Contains(mousePos) && _drawFillWindow)
			_mouseIsOnGUI = true;
		else if(_terrainAndBuildingsWindowRect.Contains(mousePos) && _drawTerrainAndBuildingsWindow)
			_mouseIsOnGUI = true;
		else if(_unitsWindowRect.Contains(mousePos) && _drawUnitsWindow)
			_mouseIsOnGUI = true;
		else if(_saveLoadWindowRect.Contains(mousePos) && _drawSaveLoadWindow)
			_mouseIsOnGUI = true;
		else
			_mouseIsOnGUI = false;
		
		GUI.skin = _GUISkin;
		
		//
		//Draw the toolbar
		//-----------------------------------
		DrawToolbar();
		
		//
		//Draw the rest of the GUI
		//-------------------------------------------------
		if(_drawNewWindow)
		{
			_newWindowRect = GUI.Window(newID, _newWindowRect, DrawNewGUI, "NEW");
		}
		if(_drawChangeWindow)
		{
			_changeWindowRect = GUI.Window(changeID, _changeWindowRect, DrawChangeGUI, "CHANGE");
		}
		if(_drawFillWindow)
		{
			_fillWindowRect = GUI.Window(fillID, _fillWindowRect, DrawFillGUI, "FILL");
		}
		if(_drawTerrainAndBuildingsWindow)
		{
			_terrainAndBuildingsWindowRect = GUI.Window(terrainAndBuildingsID, _terrainAndBuildingsWindowRect, DrawTerrainAndBuildingsGUI, "TERRAIN & BUILDINGS");
		}
		if(_drawUnitsWindow)
		{
			_unitsWindowRect = GUI.Window(unitsID, _unitsWindowRect, DrawUnitsGUI, "UNITS");
		}
		if(_drawSaveLoadWindow)
		{
			_saveLoadWindowRect = GUI.Window(saveLoadID, _saveLoadWindowRect, DrawSaveLoadGUI, "SAVE/LOAD");
		}
	}
	
	void FillBoardWithType(TileInfo tileInfo)
	{
		/*Object loadedTileObject = LoadTileOfType(tileInfo);
		for(int col = 0; col < _boardScript.GetNumberOfColumns(); ++col)
		{
			for(int row = 0; row < _boardScript.GetNumberOfRows(); ++row)
			{
				if(_boardScript.BoardTiles[col, row].tag != tileInfo.Tag ) //Only change if needed
				{
					Destroy(_boardScript.BoardTiles[col, row].gameObject);
					_boardScript.BoardTiles[col, row] = CreateNewTile(loadedTileObject, TeamsInfo.Colour.Neutral, col, row);
				}
			}
		}*/
		
		CreateNewBoard(_boardScript.GetNumberOfColumns(), _boardScript.GetNumberOfRows(), tileInfo);
			//Slower when only a few tiles need to be changed, but a lot faster when a lot have to be changed
	}
	
	void CreateNewBoard(int numColumns, int numRows, TileInfo tileInfo)
	{
		if(numColumns < 1)
			return;
		if(numRows < 1)
			return;

		DestroyCurrentBoard();
		_boardScript.BoardTiles = new BasicTile[numColumns, numRows];
		for(int col = 0; col < numColumns; ++col)
		{
			for(int row = 0; row < numRows; ++row)
			{
				_boardScript.BoardTiles[col, row] = CreateNewTileOfType(tileInfo, TeamsInfo.Colour.Neutral, col, row);
			}
		}
	}
	
	void ChangeBoardSize(int newNumCols, int newNumRows, int dir, TileInfo tileInfo)
	{
//		0=UL 1=UM 2=UR
//		3=L 4=M 5=R
//		6=LL 7=LM 8=LR
		//Check if there is a resizing
		if(newNumCols == _boardScript.GetNumberOfColumns() && newNumRows == _boardScript.GetNumberOfRows() )
			return;
		//Backup the old array
		BasicTile[,] backupArr = (BasicTile[,])_boardScript.BoardTiles.Clone();
		
		//Set begin position for copy depending on the direction
		
		// COLUMNS //
		int beginColNew;
		int beginColBackup;
		if ( newNumCols == _boardScript.GetNumberOfColumns() )
		{
			beginColNew = 0;
			beginColBackup = 0;
		}
		else if ( newNumCols > _boardScript.GetNumberOfColumns() )
		{
			if(dir == 0 || dir == 3 || dir == 6)
				beginColNew = 0;
			else if (dir == 1 || dir == 4 || dir == 7)
				beginColNew = ( newNumCols - _boardScript.GetNumberOfColumns() ) / 2;
			else
				beginColNew = newNumCols - _boardScript.GetNumberOfColumns();
			
			beginColBackup = 0;
		}
		else
		{
			if(dir == 0 || dir == 3 || dir == 6)
				beginColBackup = 0;
			else if (dir == 1 || dir == 4 || dir == 7)
				beginColBackup = ( _boardScript.GetNumberOfColumns() - newNumCols ) / 2;
			else
				beginColBackup = _boardScript.GetNumberOfColumns() - newNumCols;
			
			beginColNew = 0;
		}
		
		// ROWS //
		int beginRowNew;
		int beginRowBackup;
		if( newNumRows == _boardScript.GetNumberOfRows() )
		{
			beginRowNew = 0;
			beginRowBackup = 0;
		}
		else if ( newNumRows > _boardScript.GetNumberOfRows() )
		{
			if(dir == 6 || dir == 7 || dir == 8)// || newNumRows <= _boardScript.GetNumberOfRows())
				beginRowNew = 0;
			else if (dir == 3 || dir == 4 || dir == 5)
				beginRowNew = ( newNumRows - _boardScript.GetNumberOfRows() ) / 2;
			else
				beginRowNew = newNumRows - _boardScript.GetNumberOfRows();
			
			beginRowBackup = 0;
		}
		else
		{
			if(dir == 6 || dir == 7 || dir == 8)// || newNumRows <= _boardScript.GetNumberOfRows())
				beginRowBackup = 0;
			else if (dir == 3 || dir == 4 || dir == 5)
				beginRowBackup = ( _boardScript.GetNumberOfRows() - newNumRows ) / 2;
			else
				beginRowBackup = _boardScript.GetNumberOfRows() - newNumRows;
			
			beginRowNew = 0;
		}
		
		//Create the new board
		CreateNewBoard(newNumCols, newNumRows, tileInfo);
		
		//To copy (when reducing size)
		int colsToCopy = Mathf.Min(backupArr.GetLength(0), newNumCols);
		int rowsToCopy = Mathf.Min(backupArr.GetLength(1), newNumRows);;
		
		//Copy backup into new with proper begin position
		for(int col = 0; col < colsToCopy; ++col)
		{
			for(int row = 0; row < rowsToCopy; ++row)
			{
				TileInfo oldTileInfo = TileTypes.GetTileInfo(backupArr[beginColBackup + col,beginRowBackup + row].tag);
				TeamsInfo.Colour oldColour = backupArr[beginColBackup + col,beginRowBackup + row].Colour;
				ChangeTileToType(oldTileInfo, oldColour, beginColNew + col, beginRowNew +row);
			}
		}
	}
	
	private void DestroyCurrentBoard()
	{
		foreach(BasicTile tile in _boardScript.BoardTiles)
		{
			Destroy(tile.gameObject);
		}
	}
	
	private void ChangeTileToType(TileInfo newTileInfo, TeamsInfo.Colour newColour, int col, int row)
	{
		TileInfo oldTileInfo = TileTypes.GetTileInfo(_boardScript.BoardTiles[col, row].tag);
		TeamsInfo.Colour oldColour = _boardScript.BoardTiles[col, row].Colour;
		if(oldTileInfo.Number == newTileInfo.Number && oldColour == newColour)
		{
			return;
		}
		else if(oldTileInfo.Number == newTileInfo.Number && oldColour != newColour)
		{
			_boardScript.BoardTiles[col, row].Colour = newColour;
		}
		else
		{
			Destroy(_boardScript.BoardTiles[col, row].gameObject);
			_boardScript.BoardTiles[col, row] = CreateNewTileOfType(newTileInfo, newColour, col, row);
			//Play Sound
			SoundManager.PlayEditorTileChange();
		}
	}
	
	private BasicTile CreateNewTileOfType(TileInfo tileinfo, TeamsInfo.Colour colour, int col, int row)
	{
		Object tile = LoadTileOfType(tileinfo);
		return CreateNewTile(tile, colour, col, row);
	}
	
	private Object LoadTileOfType(TileInfo tileinfo)
	{
		return Resources.Load(tileinfo.ResourcePath, typeof(Object));
	}
	
	private BasicTile CreateNewTile(Object tile, TeamsInfo.Colour colour, int col, int row)
	{
		//Create the new tile, give it proper name and position.
    	GameObject newTileGO = Instantiate(tile) as GameObject;
		BasicTile newTile = newTileGO.GetComponent("BasicTile") as BasicTile;
    	newTile.transform.position = _boardScript.GetWorldPosition(col, row);
    	newTile.transform.parent = _boardScript.gameObject.transform;
    	newTile.gameObject.name = "Tile(" + col + "," + row +")" +newTile.tag;
		
		newTile.Collumn = col;
		newTile.Row = row;
		newTile.Colour = colour;
		
		return newTile;
	}
	
	private bool RayCastTile(out int col, out int row)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		LayerMask layer = 1 << LayerMask.NameToLayer("Tile");
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer) )
		{
			BasicTile hitTile = hit.transform.GetComponent("BasicTile") as BasicTile;
			col = hitTile.Collumn;
			row = hitTile.Row;
			return true;
		}
		else
		{
			col = -1;
			row = -1;
			return false;
		}
	}
}
