using UnityEngine;
using System.Collections;

public class CameraFollow : PT_MonoBehaviour {
	static public CameraFollow S;

	//Fields

	public Transform	targetTransform;
	public float		camEasing = 0.1f;
	public Vector3		followOffset = new Vector3(0,0,-2);

	//Methods

	void Awake()
	{
		S = this;
	}

	void FixedUpdate()
	{
		Vector3 posl = targetTransform.position + followOffset;
		pos = Vector3.Lerp (pos, posl, camEasing);
	}
}
