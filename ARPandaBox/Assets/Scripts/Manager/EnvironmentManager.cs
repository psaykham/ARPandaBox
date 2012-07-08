using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class EnvironmentManager : Singleton<EnvironmentManager> {
	
	private Dictionary<string, bool> m_characterEnvironmentIsLoaded = new Dictionary<string, bool>();
	
	void Start () {
	}
	
	void Awake () {
	}
	
	void Update () {
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			if(!m_characterEnvironmentIsLoaded.ContainsKey(kvp.Key)) {
				m_characterEnvironmentIsLoaded.Add(kvp.Key, true);			
				LoadEnvironment(kvp.Value);
			}
		}
	}
	
	private void LoadEnvironment(Character character)
	{
		GUIManager.Instance.LoadEnvironment(character);
	}
	
	public void SetActiveEnvironmentIfExists(Character character)
	{
		GUIManager.Instance.SetActiveEnvironmentIfExists(character);
	}
}
