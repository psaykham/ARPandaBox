using UnityEngine;
using System.Collections;

public class CleanUp : MonoBehaviour {
	
	public int m_amount;
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.transform.parent != null) {
			Character character = collider.gameObject.transform.parent.GetComponent<Character>();
			if(character != null) {
				character.Clean(m_amount);
				character.RemoveStatusBar();
			}
		}
	}
}
