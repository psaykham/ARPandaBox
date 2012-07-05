using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionManager : Singleton<InteractionManager>
{
	// Manager
	public Transform CharacterListTransfrom;
	public GUIManager m_guiManager;
	public ConversationManager m_conversationManager;
	
	private Dictionary<string, Character> m_characterList = new Dictionary<string, Character>();
	public Dictionary<string, Character> CharacterList {get {return m_characterList;}}
	
	void Awake()
	{
		Misc.InstantiateAsChild(m_guiManager, this);
		Misc.InstantiateAsChild(m_conversationManager, this);
	}	
	
	void Start () 
	{
	}
	
	void Update () 
	{
	
	}

	public void AddCharacter(Character character)
	{
		print ("AddCharacter");
		if(!m_characterList.ContainsKey(character.Name))
			m_characterList.Add(character.Name, character);
	}
	
	public void RemoveCharacter(string characterName)
	{
		m_characterList.Remove(characterName);
		if(ConversationManager.Instance != null)
			ConversationManager.Instance.Remove(characterName);
	}
}
