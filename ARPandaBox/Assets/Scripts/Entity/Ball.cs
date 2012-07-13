using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour 
{
	void Update () 
	{
		if(transform.position.y <= -50f)
			Destroy(gameObject);
	}
}
