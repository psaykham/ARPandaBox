using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class EnvironmentManager : Singleton<EnvironmentManager> {
	
	private bool isEnvironmentLoaded;
	private Dictionary<string, bool> m_characterEnvironmentIsLoaded = new Dictionary<string, bool>();
	
	public GameObject m_treePrefab;
	public GameObject m_terrainPrefab;
	public GameObject m_waterPrefab;
	
	private XmlDocument m_document;
	
	void Start () {
		isEnvironmentLoaded = false;
		m_document = new XmlDocument();
		m_document.LoadXml((Resources.Load("Environment") as TextAsset).text);
	}
	
	void Awake () {
	}
	
	void Update () {
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			if(!m_characterEnvironmentIsLoaded.ContainsKey(kvp.Key)) {
				m_characterEnvironmentIsLoaded.Add(kvp.Key, true);
				LoadEnvironment(kvp.Key);
			}
		}
	}
	
	private void LoadEnvironment (string characterName) {
		XmlNodeList boxNodeList = m_document.GetElementsByTagName("Box");
		foreach(XmlNode boxNode in boxNodeList)
		{
			//Select the proper Box
			if(boxNode.Attributes["name"].Value == characterName)
			{
				foreach (XmlNode settingNode in boxNode.ChildNodes)
				{
					Vector3 offset = new Vector3(InteractionManager.Instance.CharacterList[characterName].transform.localPosition.x + float.Parse(settingNode.ChildNodes[0].InnerText), InteractionManager.Instance.CharacterList[characterName].transform.localPosition.y + float.Parse(settingNode.ChildNodes[1].InnerText), InteractionManager.Instance.CharacterList[characterName].transform.localPosition.z + float.Parse(settingNode.ChildNodes[2].InnerText));
					Debug.Log("y : " + offset.y);
					GUIManager.Instance.LoadEnvironment(characterName, GetCorrespondingGameObject(settingNode.Attributes["name"].Value), offset);
				}
			}
		}
		
		//GUIManager.Instance.LoadEnvironment(characterName);
	}
	
	private GameObject GetCorrespondingGameObject(string boxName)
	{
		GameObject objectToReturn = null;
		
		if(boxName == "TREE")
			return m_treePrefab;
		
		if(boxName == "WATER")
			return m_waterPrefab;
		
		if(boxName == "TERRAIN")
			return m_terrainPrefab;
		
		return objectToReturn;
	}
}
