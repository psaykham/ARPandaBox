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
			GameObject characterEnvironment = (GameObject)Instantiate(character.Environment);
			characterEnvironment.name = "Environment";
			//Vector3 EnvronmentCenter = characterEnvironment.transform.Find("Terrain").transform.localPosition;
			characterEnvironment.transform.parent = character.transform;
			characterEnvironment.transform.localPosition = new Vector3(-750f, 750f, -170f);
			characterEnvironment.transform.localRotation = Quaternion.identity;
		}
	}
	
	public void RemoveEnvironment(string characterName)
	{
		Character character = InteractionManager.Instance.CharacterList[characterName];
		Transform environmentTransform = character.transform.Find("Environment");
		if(environmentTransform != null)
		{
			Destroy(environmentTransform.gameObject);
		}
	}
}
