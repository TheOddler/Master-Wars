using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RoadCreationScript : MonoBehaviour
{
	public Board_Script _boardScript;
	private MeshFilter _meshFilter;
	private const float _roadWidth = .45f;
	private const float _roadHeight = 1.02f;
	private const float _roadStartEndOffset = 0.0f;
	private const int _numRoadMidPoints = 12;
	
	private List<Vector3> _points;
	private List<Vector3> _newVertices;
	private List<Vector3> _newNormals;
	private List<Vector2> _newUVs;
	private List<int> _newTriangles;
	
	// Use this for initialization
	void Start ()
	{
		_meshFilter = GetComponent<MeshFilter>();
		
		_points = new List<Vector3>();
		_newVertices = new List<Vector3>();
		_newNormals = new List<Vector3>();
		_newUVs = new List<Vector2>();
		_newTriangles = new List<int>();
	}
	
	// Update is called once per frame
	void Update()
	{
		CreateRoads();
	}
	
	void OnGUI()
	{
//		if( GUI.Button(new Rect(10,500,80,20), "Calc Roads") )
//			CreateRoads();
	}

	public void CreateRoads()
	{
		//
		//Clear old road and resize for new ones !
		//-----------------------------------
		_points.Clear();
		_newVertices.Clear();
		_newNormals.Clear();
		_newUVs.Clear();
		_newTriangles.Clear();
		
		//
		//Find all road-& bridge tiles
		//--------------------------------------------------------
		foreach(BasicTile tile in _boardScript.BoardTiles)
		{
			if(tile.tag == "Road"
			   || tile.tag == "Bridge"
			   || tile.tag == "Airport"
			   || tile.tag == "City"
			   || tile.tag == "Factory"
			   || tile.tag == "HQ"
			   || tile.tag == "Port")
			{
				_points.Add(new Vector3(tile.transform.position.x, _roadHeight ,tile.transform.position.z));
			}
		}
		
		//
		//Calculate connected roads
		//---------------------------------------------------
		//Check for each point if any of the other points is close enough,
		//if so make a road part, only check the next parts
		for(int first = 0; first < _points.Count; ++first)
		{
			for(int second = first+1; second < _points.Count; ++second)
			{
				float dist = Vector3.Distance(_points[first], _points[second]);
				if(dist > 0.1f && dist < 2.0f) //ligt er dus vlak naast
				{
					AddRoadPart(_points[first], _points[second]);
				}
			}
		}
		
		//
		//Add Mids
		//----------------------------------------
		for(int i = 0; i < _points.Count; ++i)
		{
			AddRoadMid(_points[i]);
		}
		
		//
		// Add Normals
		//------------------------------------------------
		_newNormals.Capacity = _newVertices.Count;
		for(int i = 0; i < _newVertices.Count; ++i)
		{
			_newNormals.Add(Vector3.up);
		}
		
		//
		//Now set the new Mesh stuff in the renderer
		//-----------------------------------------------------------------------
		Mesh newMesh = new Mesh();
		_meshFilter.mesh = newMesh;
		newMesh.vertices = _newVertices.ToArray();
		newMesh.uv = _newUVs.ToArray();
		newMesh.triangles = _newTriangles.ToArray();
		newMesh.normals = _newNormals.ToArray();
	}
	
	private void AddRoadPart(Vector3 start, Vector3 end)
	{
		//Info
		Vector3 dir = end - start;
			dir.Normalize();
		Vector3 lr = Vector3.Cross(dir, Vector3.up);
			lr.Normalize();
		//First make indices
		_newTriangles.Add(_newVertices.Count+0);
		_newTriangles.Add(_newVertices.Count+2);
		_newTriangles.Add(_newVertices.Count+1);
		
		_newTriangles.Add(_newVertices.Count+2);
		_newTriangles.Add(_newVertices.Count+3);
		_newTriangles.Add(_newVertices.Count+1);
		//Points
		//Extra y depending on angle
		float extra = Mathf.Abs( Vector3.Dot(new Vector3( 0.707106781f, 0, -0.707106781f ), dir) * 0.01f );
		_newVertices.Add( start + dir*_roadStartEndOffset + lr*_roadWidth + new Vector3(0,extra,0) );
		_newVertices.Add( start + dir*_roadStartEndOffset - lr*_roadWidth + new Vector3(0,extra,0) );
		_newVertices.Add( end - dir*_roadStartEndOffset + lr*_roadWidth + new Vector3(0,extra,0) );
		_newVertices.Add( end - dir*_roadStartEndOffset - lr*_roadWidth + new Vector3(0,extra,0) );
		//UV's
		_newUVs.Add( new Vector2(0,0) );
		_newUVs.Add( new Vector2(0,1) );
		_newUVs.Add( new Vector2(2,0) );
		_newUVs.Add( new Vector2(2,1) );
		
	}
	private void AddRoadMid(Vector3 pos)
	{
		for(int i = 0; i < _numRoadMidPoints - 2; ++i)
		{
			_newTriangles.Add(_newVertices.Count + i + 2);
			_newTriangles.Add(_newVertices.Count + i + 1);
			_newTriangles.Add(_newVertices.Count + 0);
		}
		
		//Add points
		for(int i = 0; i < _numRoadMidPoints; ++i)
		{
			Vector3 newVert = pos + new Vector3(Mathf.Cos(i * Mathf.PI*2 / _numRoadMidPoints) * _roadWidth,
			                                    -0.01f, 
			                                    Mathf.Sin(i * Mathf.PI*2 / _numRoadMidPoints) * _roadWidth
			                                    );
			_newVertices.Add(newVert);
			_newUVs.Add(new Vector2(0,0));
		}
	}
}
