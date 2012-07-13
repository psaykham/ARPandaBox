using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class EnvironmentManager : Singleton<EnvironmentManager> 
{
	public void LoadEnvironment(string characterName)
	{
		// Load the character environment
		Character character = InteractionManager.Instance.CharacterList[characterName];
		if(character.m_environment != null) 
		{
			if(InteractionManager.Instance.EnvironmentListTransform.Find(characterName) == null)
			{
				// Instantiate the prefab
				GameObject characterEnvironment = (GameObject)Instantiate(character.m_environment);
				characterEnvironment.name = characterName;
				
				// Positionning the prefab	
				Vector3 environmentPosition = character.transform.position;
				environmentPosition.y  = -1.7f;
				characterEnvironment.transform.position = environmentPosition;
				characterEnvironment.transform.parent = InteractionManager.Instance.EnvironmentListTransform;
			}
		}
	}
	
	public void RemoveEnvironment(string characterName)
	{
		if(InteractionManager.Instance.CharacterList.ContainsKey(characterName))
		{
			Character character = InteractionManager.Instance.CharacterList[characterName];
			if(InteractionManager.Instance.EnvironmentListTransform != null)
			{
				Transform environmentTransform = InteractionManager.Instance.EnvironmentListTransform.Find(character.Name);
				if(environmentTransform != null)
				{
					Destroy(environmentTransform.gameObject);
				}
			}
		}
	}
}
