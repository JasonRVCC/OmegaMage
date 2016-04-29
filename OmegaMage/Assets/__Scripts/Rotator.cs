using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{
	public float rotation = 5;

	void Update () 
	{
		transform.Rotate(new Vector3(0, 0, rotation) * Time.deltaTime);
	}
}
