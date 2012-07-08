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
			// Instantiate the prefab
			GameObject characterEnvironment = (GameObject)Instantiate(character.Environment);
			Vector3 terrainSize = characterEnvironment.GetComponent<Terrain>().terrainData.size;
			characterEnvironment.name = characterName;
			
			// Positionning the prefab
			characterEnvironment.transform.parent = InteractionManager.Instance.EnvironmentListTransform;
			characterEnvironment.transform.localPosition = new Vector3(-terrainSize.x/2, -character.Wireframe.transform.localScale.y, -terrainSize.y/2);
		}
	}
	
	public void RemoveEnvironment(string characterName)
	{
		Character character = InteractionManager.Instance.CharacterList[characterName];
		Transform environmentTransform = InteractionManager.Instance.EnvironmentListTransform.Find(character.Name);
		if(environmentTransform != null)
		{
			Destroy(environmentTransform.gameObject);
		}
	}
}
