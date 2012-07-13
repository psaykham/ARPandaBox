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
	public GameObject m_foodPrefab;
	public GameObject m_ballPrefab;
	
	private Dictionary<string, Character> m_characterList = new Dictionary<string, Character>();
	public Dictionary<string, Character> CharacterList {get {return m_characterList;}}
	
	private Character m_mainCharacter = null;
	public Character MainCharacter {get{return m_mainCharacter;} set{m_mainCharacter=value;}}
	
	private int m_foodNumber = 0;
	public int FoodNumber {get{return m_foodNumber;} set{m_foodNumber=value;}}
	private int m_ballNumber = 0;
	public int BallNumber {get{return m_ballNumber;} set{m_ballNumber=value;}}
	
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

	public void AddCharacter(ref Character character)
	{		
		if(!character.IsNeedsInitialized)
			character.InitNeeds();
		
		if(m_mainCharacter == null)
		{
			m_mainCharacter = character;
			character.CreateStatusBar();
		}
		else
		{
			if(m_mainCharacter.Name == character.Name)
				character.CreateStatusBar();
		}
		
		print ("AddCharacter");
		if(!m_characterList.ContainsKey(character.Name))
		{
			m_characterList.Add(character.Name, character);
			
			// Only instantiate environment if worldmarker is tracked
			if(WorldMarker.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED)
				m_environmentManager.LoadEnvironment(character.Name);
		}
		
		if(m_characterList.Count > 1)
		{
			Dictionary<string, Character> m_characterListTmp = new Dictionary<string, Character>(m_characterList);
			foreach(KeyValuePair<string, Character> kvp in m_characterListTmp)
			{
				string otherName = "";
				foreach(KeyValuePair<string, Character> kvpCheck in m_characterList)
				{
					if(kvpCheck.Key != kvp.Key)
						otherName = kvpCheck.Key;
				}
				
				m_characterList[kvp.Key].Target = m_characterList[otherName].transform;
			}
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
	
	public void GiveFood()
	{
		if(m_foodNumber < 5)
		{
			if(WorldMarker.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED && m_mainCharacter != null)
			{
				Transform environment = EnvironmentListTransform.Find(m_mainCharacter.Name);
				if(environment != null)
				{
					GameObject food = (GameObject)Instantiate(m_foodPrefab);
					Vector3 direction = new Vector3(UnityEngine.Random.Range(-100, 100)/100f, 0f, UnityEngine.Random.Range(-100, 100)/100f);
					direction.Normalize();
					direction *= 3f;
					food.transform.position = m_mainCharacter.transform.position + new Vector3(0f, -1.7f, 0f) + direction;
					food.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
					food.transform.parent = environment;
					
					m_foodNumber++;
				}
			}
		}
	}
	
	public void GiveBall()
	{
		if(m_ballNumber < 10)
		{
			if(WorldMarker.GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED && m_mainCharacter != null)
			{
				Transform environment = EnvironmentListTransform.Find(m_mainCharacter.Name);
				if(environment != null)
				{
					GameObject ball = (GameObject)Instantiate(m_ballPrefab);
					Vector3 direction = new Vector3(UnityEngine.Random.Range(-100, 100)/100f, 0f, UnityEngine.Random.Range(-100, 100)/100f);
					direction.Normalize();
					direction *= 3f;
					ball.transform.position = m_mainCharacter.transform.position + new Vector3(0f, 2.5f, 0f) + direction;
					ball.transform.parent = environment;
					
					m_ballNumber++;
				}
			}	
		}
	}	
}
