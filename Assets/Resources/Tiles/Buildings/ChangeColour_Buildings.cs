using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuildingColour
{
	[SerializeField] private Material _material;
	public Material Mat{get{return _material;} set{_material = value;}}
	[SerializeField] private Mesh _mesh;
	public Mesh BuildingMesh{get{return _mesh;} set{_mesh = value;}}
}

public class ChangeColour_Buildings : MonoBehaviour
{
	BasicTile _basicTile;
	Renderer[] _neutralRenderers;
	Renderer[] _blackRenderers;
	Renderer[] _blueRenderers;
	Renderer[] _greenRenderers;
	Renderer[] _redRenderers;
	Renderer[] _whiteRenderers;
	Renderer[] _yellowRenderers;
	
	// Use this for initialization
	void Start ()
	{
		_basicTile = GetComponent(typeof(BasicTile)) as BasicTile;
		
		Transform tempChild = transform.Find("Neutral");
		_neutralRenderers = tempChild.GetComponentsInChildren<Renderer>();
		
			tempChild = transform.Find("Black");
		_blackRenderers = tempChild.GetComponentsInChildren<Renderer>();
			tempChild = transform.Find("Blue");
		_blueRenderers = tempChild.GetComponentsInChildren<Renderer>();
			tempChild = transform.Find("Green");
		_greenRenderers = tempChild.GetComponentsInChildren<Renderer>();
			tempChild = transform.Find("Red");
		_redRenderers = tempChild.GetComponentsInChildren<Renderer>();
			tempChild = transform.Find("White");
		_whiteRenderers = tempChild.GetComponentsInChildren<Renderer>();
			tempChild = transform.Find("Yellow");
		_yellowRenderers = tempChild.GetComponentsInChildren<Renderer>();

		SetRenderersFor(_basicTile.Colour);
	}
	
	// Update is called once per frame
	void Update ()
	{
		SetRenderersFor(_basicTile.Colour);
	}
	
	private void SetRenderersFor(TeamsInfo.Colour colour)
	{
		switch (colour)
		{
		case TeamsInfo.Colour.Neutral:
			EnableRenderers(_neutralRenderers);
			DisableRenderers(_blackRenderers);
			DisableRenderers(_blueRenderers);
			DisableRenderers(_greenRenderers);
			DisableRenderers(_redRenderers);
			DisableRenderers(_whiteRenderers);
			DisableRenderers(_yellowRenderers);
			break;
		case TeamsInfo.Colour.Black:
			DisableRenderers(_neutralRenderers);
			EnableRenderers(_blackRenderers);
			DisableRenderers(_blueRenderers);
			DisableRenderers(_greenRenderers);
			DisableRenderers(_redRenderers);
			DisableRenderers(_whiteRenderers);
			DisableRenderers(_yellowRenderers);
			break;
		case TeamsInfo.Colour.Blue:
			DisableRenderers(_neutralRenderers);
			DisableRenderers(_blackRenderers);
			EnableRenderers(_blueRenderers);
			DisableRenderers(_greenRenderers);
			DisableRenderers(_redRenderers);
			DisableRenderers(_whiteRenderers);
			DisableRenderers(_yellowRenderers);
			break;
		case TeamsInfo.Colour.Green:
			DisableRenderers(_neutralRenderers);
			DisableRenderers(_blackRenderers);
			DisableRenderers(_blueRenderers);
			EnableRenderers(_greenRenderers);
			DisableRenderers(_redRenderers);
			DisableRenderers(_whiteRenderers);
			DisableRenderers(_yellowRenderers);
			break;
		case TeamsInfo.Colour.Red:
			DisableRenderers(_neutralRenderers);
			DisableRenderers(_blackRenderers);
			DisableRenderers(_blueRenderers);
			DisableRenderers(_greenRenderers);
			EnableRenderers(_redRenderers);
			DisableRenderers(_whiteRenderers);
			DisableRenderers(_yellowRenderers);
			break;
		case TeamsInfo.Colour.White:
			DisableRenderers(_neutralRenderers);
			DisableRenderers(_blackRenderers);
			DisableRenderers(_blueRenderers);
			DisableRenderers(_greenRenderers);
			DisableRenderers(_redRenderers);
			EnableRenderers(_whiteRenderers);
			DisableRenderers(_yellowRenderers);
			break;
		case TeamsInfo.Colour.Yellow:
			DisableRenderers(_neutralRenderers);
			DisableRenderers(_blackRenderers);
			DisableRenderers(_blueRenderers);
			DisableRenderers(_greenRenderers);
			DisableRenderers(_redRenderers);
			DisableRenderers(_whiteRenderers);
			EnableRenderers(_yellowRenderers);
			break;
		}
	}
	
	private void DisableRenderers(Renderer[] components)
		
	{
		foreach(Renderer component in components)
		{
			component.enabled = false;
		}
	}
	private void EnableRenderers(Renderer[] components)
	{
		foreach(Renderer component in components)
		{
			component.enabled = true;
		}
	}
	
}
