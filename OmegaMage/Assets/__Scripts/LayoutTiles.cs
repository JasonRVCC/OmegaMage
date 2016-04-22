using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TileTex{
	public string		str;
	public Texture2D	tex;
}

public class LayoutTiles : MonoBehaviour {
	static public LayoutTiles S;

	//Fields

	public TextAsset	roomsText;
	public string		roomNumber = "0";
	public GameObject	tilePrefab;
	public TileTex[]	tileTextures;

	public bool ______________________________;

	public PT_XMLReader		roomsXMLR;
	public PT_XMLHashList	roomsXML;
	public Tile[,]			tiles;
	public Transform		tileAnchor;

	//Methods

	void Awake()
	{
		S = this;

		//Make tile anchor
		GameObject tAnc = new GameObject ("TileAnchor");
		tileAnchor = tAnc.transform;

		//Read room xml
		roomsXMLR = new PT_XMLReader ();
		roomsXMLR.Parse(roomsText.text);
		roomsXML = roomsXMLR.xml ["xml"] [0] ["room"];

		//Build 0th room
		BuildRoom(roomNumber);
	}


	public Texture2D GetTileTex(string tStr)
	{
		foreach (TileTex tTex in tileTextures) {
			if(tTex.str == tStr){
				return(tTex.tex);
			}
		}
		return(null);
	}


	public void BuildRoom(PT_XMLHashtable room)
	{
		//get the texture names for the room
		string floorTexStr = room.att ("floor");
		string wallTexStr = room.att ("wall");

		//split the room into rows
		string[] roomRows = room.text.Split ('\n');
		for (int i=0; i < roomRows.Length; i++) {
			roomRows[i] = roomRows[i].Trim('\t');
		}
		//clear tiles array
		tiles = new Tile[100, 100];

		//fields for later
		Tile ti;
		string type, rawType, tileTexStr;
		GameObject go;
		int height;
		float maxY = roomRows.Length - 1;

		//scan through each tile
		for (int y=0; y < roomRows.Length; y++) {
			for(int x=0; x < roomRows[y].Length; x++){
				height = 0;
				tileTexStr = floorTexStr;

				//Get tile character
				type = rawType = roomRows[y][x].ToString();
				switch(rawType){
				case "":  //Empty
				case "_": //Empty
					continue;
				
				case ".": //default floor
					break;

				case "|": //default wall
					height = 1;
					break;

				default:
					type=".";
					break;
				}

				//Set floor/wall texture
				if(type == "."){
					tileTexStr = floorTexStr;
				}
				else if(type == "|"){
					tileTexStr = wallTexStr;
				}

				//instantiate a new tile
				go = Instantiate(tilePrefab) as GameObject;
				ti = go.GetComponent<Tile>();
				ti.transform.parent = tileAnchor;
				ti.pos= new Vector3(x,maxY-y,0);
				tiles[x,y] = ti;

				//Set tile fields
				ti.type = type;
				ti.height = height;
				ti.tex = tileTexStr;

				//if type is still rawtype, continue
				if(rawType == type) continue;

				//Check for specific entities
				switch(rawType){
				case "X":
					Mage.S.pos = ti.pos;
					break;
				}

				//<=TO BE CONTINUED===
			}
		}
	}

	public void BuildRoom(string rNumStr) 
	{
		PT_XMLHashtable roomHT = null;
		for (int i=0; i<roomsXML.Count; i++) {
			PT_XMLHashtable ht = roomsXML[i];

			if (ht.att("num") == rNumStr) {
				roomHT = ht;
				break;
			}
		}
		if (roomHT == null) {
			Utils.tr("ERROR","LayoutTiles.BuildRoom()",
			         "Room not found: "+rNumStr);
			return;
		}
		BuildRoom(roomHT);
	}
}
