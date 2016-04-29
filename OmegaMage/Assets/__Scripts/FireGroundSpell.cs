using UnityEngine;
using System.Collections;

public class FireGroundSpell : PT_MonoBehaviour
{
	public float duration = 4; 
	public float durationVariance = 0.5f;
	public float fadeTime = 1f; 
	public float timeStart; 


	void Start()
	{
		timeStart = Time.time;
		duration = Random.Range(duration - durationVariance,
		                        duration + durationVariance);
	}


	void Update()
	{
		//what percent of the duration has passed
		float u = (Time.time - timeStart) / duration;

		//at what percent should the spell start fading
		float fadePercent = 1 - (fadeTime / duration);
		if (u > fadePercent)
		{ 
			float u2 = (u - fadePercent) / (1 - fadePercent);
			Vector3 loc = pos;
			loc.z = u2 * 2;
			pos = loc;
		}

		//If the duration has passed, end the spell
		if (u > 1)
		{ 
			Destroy(gameObject); 
		}
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject go = Utils.FindTaggedParent(other.gameObject);
		if (go == null)
		{
			go = other.gameObject;
		}
		Utils.tr("Flame hit", go.name);
	}

	//TODO: inflict damage
}
