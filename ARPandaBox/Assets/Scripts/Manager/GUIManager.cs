using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : Singleton<GUIManager>
{
	public Transform ScreenPosition;
	public Transform NavigationBarTransform;
	public GameObject m_navigationBarPrefab;
	public Transform StatusBarTransform;
	public GameObject m_statusBarPrefab;
	public GameObject m_dialogBoxPrefab;
	
	public Vector3 m_dialogBoxOffet;
	
	private List<GameObject> m_messageInstanceList = new List<GameObject>();
	private Rect m_menuArea;
	
	void Start()
	{
		GameObject navigationBar = (GameObject)Instantiate(m_navigationBarPrefab, NavigationBarTransform.position, NavigationBarTransform.rotation);
		navigationBar.transform.parent = NavigationBarTransform;
	}
	
	// Display a message to the screen
	public void DisplayMessage(string characterName, string message)
	{	
		GameObject messageObject = (GameObject)Instantiate(m_dialogBoxPrefab);
		messageObject.transform.parent = InteractionManager.Instance.CharacterList[characterName].Wireframe.transform;
		messageObject.transform.localPosition = m_dialogBoxOffet;
		messageObject.GetComponentInChildren<SpriteText>().Text = message;
		m_messageInstanceList.Add(messageObject);
		StartCoroutine(RemoveMessage(messageObject));
	}
	
	// Simple  GUI with default system
	void OnGUI()
	{
		// Camera position
		/*GUILayout.BeginHorizontal();
		GUILayout.Label("Camera Position:" +GameObject.FindGameObjectWithTag("MainCamera").transform.position.ToString());
		GUILayout.EndHorizontal();
		
		// Character Position
		if(InteractionManager.Instance.CharacterList.Count > 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Character Position: " + InteractionManager.Instance.CharacterList["Yoann"].transform.position.ToString());
			GUILayout.EndHorizontal();
		}
		
		ComputePosition();
			
		// Setup style for buttons.
        GUIStyle buttonGroupStyle = new GUIStyle(GUI.skin.button);
        buttonGroupStyle.stretchWidth = true;
        buttonGroupStyle.stretchHeight = true;
		
		// Activate / Desactivate Character present in the scene
        GUILayout.BeginArea(m_menuArea);
        GUILayout.BeginHorizontal(buttonGroupStyle);
		if(InteractionManager.Instance != null)
		{
			foreach(Transform character in InteractionManager.Instance.CharacterListTransfrom)
			{
				if (GUILayout.Button("Toggle "+character.GetComponent<Character>().Name, buttonGroupStyle))
	        	{
					character.gameObject.SetActiveRecursively(!character.gameObject.active);
				}
			}
		}
        GUILayout.EndHorizontal();
        GUILayout.EndArea();*/
	}
	
	/// Compute the coordinates of the menu depending on the current orientation.
    private void ComputePosition()
    {
        int areaWidth = Screen.width;
        int areaHeight = Screen.height / 5;
        int areaLeft = 0;
        int areaTop = Screen.height - areaHeight;
        m_menuArea = new Rect(areaLeft, areaTop, areaWidth, areaHeight);
    }
	
	// Delete the message
	private IEnumerator RemoveMessage(GameObject messageObject)
	{
		yield return new WaitForSeconds(ConversationManager.Instance.m_timeInteraction - 0.1f);
		m_messageInstanceList.Remove(messageObject);
		Destroy(messageObject);
	}
}
