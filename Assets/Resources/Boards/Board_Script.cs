using UnityEngine;
using System.Collections;
using System.IO;

public class Board_Script : MonoBehaviour
{
	
	private BasicTile[,] _boardTiles;
	public BasicTile[,] BoardTiles
	{
		get{return _boardTiles;}
		set{_boardTiles = value;}
	}
	
	public BasicTile GetTileOnPos(int cols, int rows)
	{
		return BoardTiles[cols, rows];
	}

	// Use this for initialization
	void Start ()
	{
		BoardTiles = new BasicTile[0,0];
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public int GetNumberOfColumns()
	{
		return BoardTiles.GetLength(0);
	}
	
	public int GetNumberOfRows()
	{
		return BoardTiles.GetLength(1);
	}
	
	public Vector3 GetWorldPosition(int col, int row)
	{
		return new Vector3((float)(col*2+(row%2) ) * Mathf.Cos(Mathf.PI/6.0f),
		                   0.0f,
		                   (float)row * (1.0f+ Mathf.Sin(Mathf.PI/6.0f))
		                   );
	}
	
	public bool SaveBoard(string name, string description, string folderPath)
	{
		try
		{
			//Make filepath
			string fullPath = folderPath;
			if( !folderPath.EndsWith("/") )
			{
				fullPath += "/";
			}
			fullPath += name + ".mwm";
			
			//Save the actual file
			Directory.CreateDirectory(folderPath);
			using (FileStream stream = new FileStream(fullPath, FileMode.Create))
			{
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					//4bytes; File Version
					writer.Write((uint)1);
					//string; Level Name
					writer.Write(name);
					//string; Description
					writer.Write(description);
					//byte; cols
					writer.Write((byte)GetNumberOfColumns());
					//byte; rows
					writer.Write((byte)GetNumberOfRows());
					//4 bytes per tile
					for(int c = 0, cols = GetNumberOfColumns(); c < cols; ++c)
					{
						for(int r = 0, rows = GetNumberOfRows(); r < rows; ++r)
						{
							BasicTile tile = GetTileOnPos(c, r);
							ushort type = TileTypes.GetTileInfo(tile.tag).Number;
							byte col = (byte)tile.Colour;
							byte other = 100;
							//4bytes -> 2byte type; 1bytes color; 1byte other
							writer.Write(type);
							writer.Write(col);
							writer.Write(other);
						}
					}
					//Close the write (automaticly also closes the stream)
					writer.Close();
				}
			}
			
			UnityGrowl.Show("Saved !\nYour level has succesfully been saved.");
			return true;
		}
		catch
		{
			UnityGrowl.Show("Save error\nAn error has occured during saving.\nMake sure the map/file is not read-only.", 5);
			return false;
		}
	}
	
	public bool LoadBoard(string path, out string name, out string description)
	{
		try
		{
			//Load the file
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					//4bytes; File Version
					uint version = reader.ReadUInt32();
					
					switch(version)
					{
					case 1:
							LoadVersionOne(reader, out name, out description);
							break;
					default:
							name = "No Map";
							description = "The selected file is corrupt";
							return false;
					}
					
					reader.Close();
				}
			}
			
			UnityGrowl.Show("Loaded !\nYour level has succesfully been loaded.");
			return true;
		}
		catch
		{
			UnityGrowl.Show("Load error\nAn error has occured during loading.", 4);
			name = "Error";
			description = "An error has occured, please try again.";
			DestroyCurrentBoard();
			return false;
		}
	}
	
	private bool LoadVersionOne(BinaryReader reader, out string name, out string description)
	{
		//string; Level Name
		name = reader.ReadString();
		//string; Description
		description = reader.ReadString();
		//byte; cols
		byte cols = reader.ReadByte();
		//byte; rows
		byte rows = reader.ReadByte();
		
		//Create new board with just-read dimentions
		DestroyCurrentBoard();
		BoardTiles = new BasicTile[cols,rows];
		
		//4 bytes per tile
		for(int c = 0; c < cols; ++c)
		{
			for(int r = 0; r < rows; ++r)
			{
				//4bytes -> 2byte type; 1bytes color; 1byte other
				ushort type = reader.ReadUInt16();
				byte color = reader.ReadByte();
				byte other = reader.ReadByte();
				
				BoardTiles[c, r] = CreateNewTileOfType(TileTypes.GetTileInfo(type), (TeamsInfo.Colour) color, c, r);
			}
		}
		
		return true;
	}
	
	private void DestroyCurrentBoard()
	{
		foreach(BasicTile tile in BoardTiles)
		{
			Destroy(tile.gameObject);
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
    	newTile.transform.position = GetWorldPosition(col, row);
    	newTile.transform.parent = gameObject.transform;
    	newTile.gameObject.name = "Tile(" + col + "," + row +")" +newTile.tag;
		
		newTile.Collumn = col;
		newTile.Row = row;
		newTile.Colour = colour;
		
		return newTile;
	}
}
