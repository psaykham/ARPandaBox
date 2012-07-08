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
		if(character.Environment != null) 
		{
			GameObject characterEnvironment = (GameObject)Instantiate(character.Environment, character.transform.position, Quaternion.identity);
			characterEnvironment.name = "Environment";
			characterEnvironment.transform.parent = character.transform;
		}
	}
	
	public void RemoveEnvironment(string characterName)
	{
		Character character = InteractionManager.Instance.CharacterList[characterName];
		GameObject characterEnvironnement = character.transform.Find("Environment").gameObject;
		Destroy(characterEnvironnement);
	}
}
