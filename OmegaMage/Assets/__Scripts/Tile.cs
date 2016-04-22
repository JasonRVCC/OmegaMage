using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	//Fields

	public string		type;

	private string		_tex;
	private int			_height = 0;
	private Vector3		_pos;

	//Properties

	public int height
	{
		get{ return(_height);}
		set{
			_height = value;
			AdjustHeight();
		}
	}


	public string tex
	{
		get{ return(_tex);}
		set{
			_tex = value;
			name = "TilePrefab" + _tex;
			Texture2D t2D = LayoutTiles.S.GetTileTex(_tex);
			if(t2D == null)
			{
				Utils.tr("ERROR","Tile.type{set}=",value,
				         "No matching Texture2D in LayoutTiles.S.tileTextures!");
			}
			else
			{
				renderer.material.mainTexture = t2D;
			}
		}
	}


	new public Vector3 pos
	{
		get{ return(_pos);}
		set{
			_pos = value;
			AdjustHeight();
		}
	}


	//Methods

	public void AdjustHeight()
	{
		//Move block up or down
		Vector3 vertOffset = Vector3.back * (_height - 0.5f);
		transform.position = _pos + vertOffset;
	}
}
