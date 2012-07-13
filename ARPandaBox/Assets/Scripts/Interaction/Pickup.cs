using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour 
{
	public enum PickupType
	{
		FOOD,
		FUN,
	}
	public PickupType m_type;
	public int m_amount;
	
	void Update () 
	{
		if(transform.position.y <= -50f)
		{
			switch(m_type)
			{
				case PickupType.FOOD:
				InteractionManager.Instance.FoodNumber--;
				break;
				
				case PickupType.FUN:
				InteractionManager.Instance.BallNumber--;
				break;
			}
			Destroy(gameObject);
		}
	}
	
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
					Destroy(gameObject);
					break;
					
					case PickupType.FUN:
					character.Fun(m_amount);
					break;
				}
			}
		}

    }
}
