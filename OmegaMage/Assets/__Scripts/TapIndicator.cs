using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TapIndicator : PT_Mover
{
	public float lifeTime = 0.4f; 
	public float[] scales; 
	public Color[] colors; 


	void Awake()
	{
		scale = Vector3.zero;
	}


	void Start()
	{
		PT_Loc pLoc;
		List<PT_Loc> locs = new List<PT_Loc>();

		Vector3 tPos = pos;
		tPos.z = -0.1f;

		//The number of scales and colors in the Inspector must be equal
		for (int i = 0; i < scales.Length; i++)
		{
			pLoc = new PT_Loc();
			pLoc.scale = Vector3.one * scales[i]; 
			pLoc.pos = tPos;
			pLoc.color = colors[i]; 
			locs.Add(pLoc); 
		}

		callback = CallbackMethod; 

		PT_StartMove(locs, lifeTime);
	}


	void CallbackMethod()
	{
		Destroy(gameObject);
	}
}