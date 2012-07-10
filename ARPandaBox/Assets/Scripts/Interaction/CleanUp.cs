using UnityEngine;
using System.Collections;

public class CleanUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if(collision.transform.parent != null)
		{
			Character character = collision.transform.parent.GetComponent<Character>();
			/*if(character != null)
			{
				switch(m_type)
				{
					case PickupType.FOOD:
					character.Eat(m_amount);
					break;
				}
				
				Destroy(gameObject);
			}*/
			
			character.RemoveStatusBar();
		}

    }
}
