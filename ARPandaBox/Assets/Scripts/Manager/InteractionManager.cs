using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionManager : Singleton<InteractionManager>
{
	// Manager
	public Transform WorldMarker;
	public Transform CharacterListTransfrom;
	public Transform EnvironmentListTransform;
	public GUIManager m_guiManager;
	public ConversationManager m_conversationManager;
	public EnvironmentManager m_environmentManager;
	
	private Dictionary<string, Character> m_characterList = new Dictionary<string, Character>();
	public Dictionary<string, Character> CharacterList {get {return m_characterList;}}
	
	private Character m_mainCharacter = null;
	public Character MainCharacter {get{return m_mainCharacter;} set{m_mainCharacter=value;}}
	
	void Awake()
	{
		Misc.InstantiateAsChild(m_guiManager, this);
		Misc.InstantiateAsChild(m_conversationManager, this);
		Misc.InstantiateAsChild(m_environmentManager, this);
	}	
	
	void Start () 
	{
	}
	
	void Update () 
	{
	
	}

	public void AddCharacter(Character character)
	{
		if(m_mainCharacter == null)
			m_mainCharacter = character;
		
		print ("AddCharacter");
		if(!m_characterList.ContainsKey(character.Name))
		{
			m_characterList.Add(character.Name, character);
			
			// Only instantiate environment if worldmarker is tracked
			if(WorldMarker.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED)
				m_environmentManager.LoadEnvironment(character.Name);
		}
	}
	
	public void RemoveCharacter(string characterName)
	{
		if(ConversationManager.Instance != null)
			ConversationManager.Instance.Remove(characterName);
		
		if(EnvironmentManager.Instance != null)
			EnvironmentManager.Instance.RemoveEnvironment(characterName);
		
		m_characterList.Remove(characterName);
	}
}
