using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour 
{
	public enum PickupType
	{
		FOOD,	
	}
	public PickupType m_type;
	public int m_amount = 1;
	
	void OnCollisionEnter(Collision collision) 
	{
		if(collision.transform.parent != null)
		{
			Character character = collision.transform.parent.GetComponent<Character>();
			if(character != null)
			{
				switch(m_type)
				{
					case PickupType.FOOD:
					character.Eat(m_amount);
					break;
				}
				
				Destroy(gameObject);
			}
		}

    }
}
