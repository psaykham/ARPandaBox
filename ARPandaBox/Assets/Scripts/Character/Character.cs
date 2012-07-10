using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour, ITrackableEventHandler
{
	public enum EmotionType
	{
		FRIENDLY,
		WICKED,
	}
	
 	enum StatusBarType
	{
		HUNGER,
		HYGIENE,
		ENERGY,
		BLADDER,
		FUN,
		SOCIAL,
	};
	
	public string Name;
	public EmotionType CurrentEmotion;
	public Transform Shape;
	public float EyesRadius = 20f;
	public GameObject Wireframe;
	
	// Commmunication
	private ConversationManager.ConversationStep m_currentConversationStep;
	public ConversationManager.ConversationStep CurrentConversationStep {get {return m_currentConversationStep;} set{m_currentConversationStep=value;}}
    private TrackableBehaviour m_trackableBehaviour;
    
	// Atributs
	private int m_level = 1;
	private int m_experience = 0;
	
	private float m_energy = 1f;
	private float m_hungrer = 1f;
	private float m_fun = 1f;
	private float m_social = 1f;
	private float m_bladder = 1f;
	private float m_hygiene = 1f;
	
	// Shape
	private bool m_isVisible = false;
	private List<GameObject> m_eyeList = new List<GameObject>();
	private UIStateToggleBtn m_mouth;
	private Transform m_target;
	public Transform Target { get {return m_target;} set{m_target = value;}}
	
	// Environnement correspondant
	public GameObject m_environment;
	public GameObject Environment {get{return m_environment;} set{m_environment = value;}}
	
	// Status Bar
	public GameObject m_statusBarPrefab;
	private GameObject[] m_listStatusBar = new GameObject[2];
	private Coroutine statusBarCoRoutine = null;
	
	void Awake()
	{	
		// Pupil
		m_currentConversationStep = ConversationManager.ConversationStep.GREETING;
		
		if(Shape != null)
		{	
			// Eyes
			GameObject leftEye = Shape.Find("Head/Eyes/LeftCornea/LeftPupil").gameObject;
			GameObject rightEye = Shape.Find("Head/Eyes/RightCornea/RightPupil").gameObject;
			m_eyeList.Add(leftEye);
			m_eyeList.Add(rightEye);
			
			// Mouth
			m_mouth = Shape.Find("Head/Mouth").GetComponent<UIStateToggleBtn>();
		}
		
		InitNeeds();
	}
	
    void Start()
    {
		// Init Trackable Behaviour
        m_trackableBehaviour = GetComponent<TrackableBehaviour>();
        if (m_trackableBehaviour)
        {
            m_trackableBehaviour.RegisterTrackableEventHandler(this);
        }
		
		StartCoroutine(EyeAlive());
    }
	
	// Init Needs
	private void InitNeeds()
	{
		StartCoroutine(UpdateHunger());
		StartCoroutine(UpdateHygiene());
	}
	
	private IEnumerator UpdateHunger()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(100, 300) / 100f);
		Eat(-1);
		yield return StartCoroutine(UpdateHunger());
	}
	
	private IEnumerator UpdateHygiene()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(100, 300) / 100f);
		Clean(-1);
		yield return StartCoroutine(UpdateHygiene());
	}
	
	// Event trackable
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }
	
	void Update()
	{
		UpdateEyes();
		UpdateMouth();
	}
	
	// Update pupil position
	private void UpdateEyes()
	{
		if(m_target != null)
		{
			for(int i=0; i<m_eyeList.Count; i++)
			{
				Vector3 eyePosition = m_target.position - m_eyeList[i].transform.position;
				eyePosition.Normalize();
				m_eyeList[i].transform.localPosition = eyePosition * EyesRadius;	
			}
		}
	}
	
	private void UpdateMouth()
	{
		if(m_mouth != null)
		{
			m_mouth.SetToggleState(CurrentEmotion.ToString());
		}
	}
	
	private IEnumerator EyeAlive()
	{
		if(m_isVisible)
		{
			print("EyeAlive");
			yield return new WaitForSeconds(UnityEngine.Random.Range(100, 500) / 100f);
			SetVisibleEyes(false);
			yield return new WaitForSeconds(0.25f);
			SetVisibleEyes(true);
			yield return StartCoroutine(EyeAlive());
		}
		
	}
	
	// Animation on eyes
	private void SetVisibleEyes(bool isVisible)
	{
		for(int i=0; i<m_eyeList.Count; i++)
			Misc.SetRendererActive(m_eyeList[i].transform, isVisible);
	}
	
	// Eat some food
	public void Eat(int amount)
	{
		m_hungrer = Mathf.Clamp(m_hungrer + (amount / 100f), 0f, 1f);
	}
	
	// It cleans itself
	public void Clean(int amount)
	{
		m_hygiene = Mathf.Clamp(m_hygiene + (amount / 100f), 0f, 1f);
	}
	
	// Add the character for the interaction
    private void OnTrackingFound()
    {
		StartCoroutine(OnTrackingFoundProcess());
		CreateStatusBar();
		//statusBarCoRoutine = StartCoroutine("UpdateStatusBar", UpdateStatusBar());
    }
	
	private IEnumerator OnTrackingFoundProcess()
	{
		yield return StartCoroutine(CheckInteractionManagerPresence());
		m_isVisible = true;
		InteractionManager.Instance.AddCharacter(this); 
	}
	
	// Remove the character from the scene
    private void OnTrackingLost()
    {
		if(InteractionManager.Instance != null)
		{
			m_isVisible = false;
			InteractionManager.Instance.RemoveCharacter(Name); 
		}
    }
	
	// Wait that the InteractionManager is created
	private IEnumerator CheckInteractionManagerPresence()
	{
		while(InteractionManager.Instance == null)
			yield return 100;
	}
	
	//Update the StatusBar
	public IEnumerator UpdateStatusBar()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(100, 300) / 100f);
		
		if(m_statusBarPrefab != null) {
			for(int i=0; i<m_listStatusBar.Length; i++) {
				Transform statusBarFull = m_listStatusBar[i].transform.GetChild(1);
				statusBarFull.localScale = 
					GetCurrentStatusBarScale(GetCorrespondingAttributes(i), 
						m_listStatusBar[i].transform.GetChild(2));
			}
		}
		
		yield return StartCoroutine("UpdateStatusBar", UpdateStatusBar());
	}
	
	// Create all status bar
	public void CreateStatusBar()
	{
		if(m_statusBarPrefab != null) {
			for(int i=0; i<m_listStatusBar.Length; i++) {
				m_listStatusBar[i] = (GameObject)Instantiate(m_statusBarPrefab);
				m_listStatusBar[i].transform.parent = GUIManager.Instance.ScreenPosition.transform;
				m_listStatusBar[i].GetComponentInChildren<SpriteText>().Text = GetCorrespondingAttributesName(i);
				if(i == 0) {
					m_listStatusBar[i].transform.localPosition = new Vector3(10, 0, 0);
				} else {
					m_listStatusBar[i].transform.localPosition = new Vector3(10, m_listStatusBar[i-1].transform.localPosition.y - 30, 0);
				}
				
				m_listStatusBar[i].transform.GetChild(1).localScale = 
					GetCurrentStatusBarScale(GetCorrespondingAttributes(i), 
						m_listStatusBar[i].transform.GetChild(2));
			}
		}
		statusBarCoRoutine = StartCoroutine("UpdateStatusBar", UpdateStatusBar());
	}
	
	// Returns the current status bar scale
	private Vector3 GetCurrentStatusBarScale(float attributeValue, Transform statusBarFull)
	{
		float currentBar = (attributeValue * statusBarFull.localScale.x) / 1F;
		
		if(currentBar < 0f)
			currentBar = 0f;
		if(currentBar > 1F)
			currentBar = 1F;
		
		Vector3 scale = statusBarFull.localScale;
		scale.x = currentBar;
		
		return scale;
	}
	
	// Remove the status bar from the screen
	public void RemoveStatusBar()
	{
		if(statusBarCoRoutine != null) {
			StopCoroutine("UpdateStatusBar");
		}
		
		for(int i=0; i<m_listStatusBar.Length; i++) {
			if(m_listStatusBar[i] != null) {
				Destroy(m_listStatusBar[i]);
			}
		}
		
	}
	
	// Returns the corresponding attribute
	private float GetCorrespondingAttributes(int statusBarType)
	{
		switch(statusBarType) {
		case (int)StatusBarType.ENERGY:
			return m_energy;
			
		case (int)StatusBarType.HUNGER:
			return m_hungrer;
			
		case (int)StatusBarType.FUN:
			return m_fun;
			
		case (int)StatusBarType.SOCIAL:
			return m_social;
			
		case (int)StatusBarType.BLADDER:
			return m_bladder;
			
		case (int)StatusBarType.HYGIENE:
			return m_hygiene;
			
		default:
			return -1F;
		}
	}
	
	// Returns the corresponding attribute's name
	private string GetCorrespondingAttributesName(int statusBarType)
	{
		switch(statusBarType) {
		case (int)StatusBarType.ENERGY:
			return "ENERGY";
			
		case (int)StatusBarType.HUNGER:
			return "HUNGER";
			
		case (int)StatusBarType.FUN:
			return "FUN";
			
		case (int)StatusBarType.SOCIAL:
			return "SOCIAL";
			
		case (int)StatusBarType.BLADDER:
			return "BLADDER";
			
		case (int)StatusBarType.HYGIENE:
			return "HYGIENE";
			
		default:
			return "";
		}
	}
}
