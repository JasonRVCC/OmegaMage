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

//Element Types for spells
public enum ElementType
{
	earth,
	water,
	air,
	fire,
	aether,
	none
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

	public float 		mTapTime = 0.1f; 
	public GameObject 	tapIndicatorPrefab;
	public float 		mDragDist = 5; 

	public float 		activeScreenWidth = 1; 

	public float 		speed = 2;

	public GameObject[] elementPrefabs; 
	public float 		elementRotDist = 0.5f; 
	public float 		elementRotSpeed = 0.5f; 
	public int 			maxNumSelectedElements = 1;
	
	public bool ________________;

	public MPhase 			mPhase = MPhase.idle;
	public List<MouseInfo> 	mouseInfos = new List<MouseInfo>();

	public bool 			walking = false; 
	public Vector3 			walkTarget; 
	public Transform 		characterTrans; 

	public List<Element> 	selectedElements = new List<Element>(); 


	void Awake()
	{ 
		S = this;
		mPhase = MPhase.idle;

		characterTrans = transform.Find("CharacterTrans");
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

				if (selectedElements.Count == 0)
				{ 
					mPhase = MPhase.drag; //**
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

		OrbitSelectedElements();
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
		WalkTo(lastMouseInfo.loc);
		ShowTap(lastMouseInfo.loc);
	}
		
	void MouseDrag()
	{
		if (DEBUG) print("Mage.MouseDrag()");

		WalkTo(mouseInfos[mouseInfos.Count - 1].loc);
	}

	void MouseDragUp()
	{
		if (DEBUG) print("Mage.MouseDragUp()");

		StopWalking();
	}


	public void WalkTo(Vector3 xTarget)
	{
		walkTarget = xTarget; 
		walkTarget.z = 0; 
		walking = true; 
		Face(walkTarget); 
	}
	
	public void Face(Vector3 poi)
	{ 
		Vector3 delta = poi - pos;

		float rZ = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);

		characterTrans.rotation = Quaternion.Euler(0, 0, rZ);
	}
	
	public void StopWalking()
	{ // Stops the _Mage from walking
		walking = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}


	void FixedUpdate()
	{ 
		if (walking)
		{ 
			if ((walkTarget - pos).magnitude < speed * Time.fixedDeltaTime)
			{
				pos = walkTarget;
				StopWalking();
			}
			else
			{
				rigidbody.velocity = (walkTarget - pos).normalized * speed;
			}
		}
		else
		{
			rigidbody.velocity = Vector3.zero;
		}
	}


	void OnCollisionEnter(Collision coll)
	{
		GameObject otherGO = coll.gameObject;

		Tile ti = otherGO.GetComponent<Tile>();
		if (ti != null)
		{
			if (ti.height > 0)
			{ 
				StopWalking();
			}
		}
	}

	public void ShowTap(Vector3 loc)
	{
		GameObject go = Instantiate(tapIndicatorPrefab) as GameObject;
		go.transform.position = loc;
	}


	public void SelectElement(ElementType elType)
	{
		if (elType == ElementType.none)
		{ 
			ClearElements(); 
			return; 
		}

		if (maxNumSelectedElements == 1)
		{
			ClearElements(); 
		}

		if (selectedElements.Count >= maxNumSelectedElements) 
		{ return; }

		GameObject go = Instantiate(elementPrefabs[(int)elType]) as GameObject;

		Element el = go.GetComponent<Element>();
		el.transform.parent = this.transform;

		selectedElements.Add(el);
	}
	
	public void ClearElements()
	{
		foreach (Element el in selectedElements)
		{
			Destroy(el.gameObject);
		}
		selectedElements.Clear(); // and clear the list
	}
	
	void OrbitSelectedElements()
	{
		if (selectedElements.Count == 0) 
		{ return; }

		Element el;
		Vector3 vec;
		float theta0, theta;
		float tau = Mathf.PI * 2; //tau is 360 degress in radians (6.283...)

		float rotPerElement = tau / selectedElements.Count;

		theta0 = elementRotSpeed * Time.time * tau;

		for (int i = 0; i < selectedElements.Count; i++)
		{
			//Determine the rotation angle
			theta = theta0 + i * rotPerElement;
			el = selectedElements[i];
			vec = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
			vec *= elementRotDist;
			vec.z = -0.5f;
			el.lPos = vec; 
		}
	}
}
