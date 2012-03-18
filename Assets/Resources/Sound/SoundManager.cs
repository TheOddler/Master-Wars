using UnityEngine;
using System.Collections;

static public class SoundManager
{
	//Editor Sounds
	static private AudioClip _editorTileChange;
	
	static SoundManager()
	{
		_editorTileChange = Resources.Load("Sound/PlopHoog", typeof(AudioClip)) as AudioClip;
	}
	
	static public void PlayEditorTileChange()
	{
		AudioSource.PlayClipAtPoint(_editorTileChange, Vector3.zero);
	}
}
