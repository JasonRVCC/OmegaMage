using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//MPhase tracks phases of mouse iteraction
public enum MPhase
{
	idle,
	down,
	drag
}

//MouseInfo stores mouse information
[System.Serializable]
public class MouseInfo
{
	public Vector3 loc; 
	public Vector3 screenLoc; 
	public Ray ray; 
	public float time; 
	public RaycastHit hitInfo; 
	public bool hit; 


	public RaycastHit Raycast()
	{
		hit = Physics.Raycast(ray, out hitInfo);
		return (hitInfo);
	}

	public RaycastHit Raycast(int mask)
	{
		hit = Physics.Raycast(ray, out hitInfo, mask);
		return (hitInfo);
	}
}

public class Mage : PT_MonoBehaviour {
	static public Mage S;
	static public bool DEBUG = true;

	public float 	mTapTime = 0.1f; 
	public float 	mDragDist = 5; 

	public float 	activeScreenWidth = 1; 

	public bool ________________;

	public MPhase 			mPhase = MPhase.idle;
	public List<MouseInfo> 	mouseInfos = new List<MouseInfo>();


	void Awake()
	{ 
		S = this;
		mPhase = MPhase.idle;
	}


	void Update()
	{
		//Was mouse button 0 pressed or released this frame?
		bool b0Down = Input.GetMouseButtonDown(0);
		bool b0Up = Input.GetMouseButtonUp(0);
		
		bool inActiveArea = (float)Input.mousePosition.x / Screen.width < activeScreenWidth;
		
		if (mPhase == MPhase.idle)
		{ 
			if (b0Down && inActiveArea)
			{
				mouseInfos.Clear(); 
				AddMouseInfo(); 

				if (mouseInfos[0].hit)
				{ 
					MouseDown(); 
					mPhase = MPhase.down; 
				}
			}
		}
		if (mPhase == MPhase.down)
		{ 
			AddMouseInfo(); 
			if (b0Up)
			{ 
				MouseTap(); 
				mPhase = MPhase.idle;
			}
			else if (Time.time - mouseInfos[0].time > mTapTime)
			{
				//Check to see if this was a drag
				float dragDist = (lastMouseInfo.screenLoc -
				                  mouseInfos[0].screenLoc).magnitude;
				if (dragDist >= mDragDist)
				{
					mPhase = MPhase.drag;
				}
			}
		}
		if (mPhase == MPhase.drag)
		{ 
			AddMouseInfo();
			if (b0Up)
			{
				MouseDragUp();
				mPhase = MPhase.idle;
			}
			else
			{
				MouseDrag(); 
			}
		}
	}

	//Get's info from the mouse, adds it to mouseInfos, and returns it
	MouseInfo AddMouseInfo()
	{
		MouseInfo mInfo = new MouseInfo();
		mInfo.screenLoc = Input.mousePosition;
		mInfo.loc = Utils.mouseLoc; 
		mInfo.ray = Utils.mouseRay; 
		mInfo.time = Time.time;
		mInfo.Raycast();

		if (mouseInfos.Count == 0)
		{
			mouseInfos.Add(mInfo); 
		}
		else
		{
			float lastTime = mouseInfos[mouseInfos.Count - 1].time;
			if (mInfo.time != lastTime)
			{					
				mouseInfos.Add(mInfo); 
			}
		}
		return (mInfo);
	}

	public MouseInfo lastMouseInfo
	{			
		get
		{
			if (mouseInfos.Count == 0) return (null);
			return (mouseInfos[mouseInfos.Count - 1]);
		}
	}

	void MouseDown()
	{
		if (DEBUG) print("Mage.MouseDown()");
	}

	void MouseTap()
	{
		if (DEBUG) print("Mage.MouseTap()");
	}
		
	void MouseDrag()
	{
		if (DEBUG) print("Mage.MouseDrag()");
	}

	void MouseDragUp()
	{
		if (DEBUG) print("Mage.MouseDragUp()");
	}
}
