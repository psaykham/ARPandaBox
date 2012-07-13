using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour, ITrackableEventHandler
{
	public enum EmotionType
	{
		GREAT,
		FRIENDLY,
		NORMAL,
		BAD,
		WICKED,
	}
	
 	enum StatusBarType
	{
		HUNGER,
		HYGIENE,
		FUN,
		SOCIAL,
		ENERGY,
		BLADDER,
	};
	
	public string Name;
	public EmotionType CurrentEmotion;
	public Transform Shape;
	public float EyesRadius = 20f;
	public GameObject Wireframe;
	public GameObject m_environment;
	
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
	
	// Status Bar
	private GameObject[] m_listStatusBar = new GameObject[4];
	public GameObject[] StatusBarList {get{return m_listStatusBar;}}
	private Coroutine statusBarCoRoutine = null;
	
	void Awake()
	{	
		
		CurrentEmotion = (EmotionType)UnityEngine.Random.Range(0,3);
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
	public void InitNeeds()
	{
		StartCoroutine(UpdateHunger());
		StartCoroutine(UpdateHygiene());
		StartCoroutine(UpdateFun());
		StartCoroutine(UpdateSocial());
	}
	
	private IEnumerator UpdateHunger()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(200, 1000) / 100f);
		Eat(-1);
		yield return StartCoroutine(UpdateHunger());
	}
	
	private IEnumerator UpdateHygiene()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(1000, 2000) / 100f);
		Clean(-1);
		yield return StartCoroutine(UpdateHygiene());
	}
	
	private IEnumerator UpdateFun()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(500, 600) / 100f);
		Fun(-1);
		yield return StartCoroutine(UpdateFun());
	}
	
	private IEnumerator UpdateSocial()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(800, 1200) / 100f);
		Social(-1);
		yield return StartCoroutine(UpdateSocial());
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
	
	public void Fun(int amount)
	{
		m_fun = Mathf.Clamp(m_fun + (amount / 100f), 0f, 1f);
	}
	
	public void Social(int amount)
	{
		m_social = Mathf.Clamp(m_social + (amount / 100f), 0f, 1f);
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
		UpdatePrimitive();
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
				eyePosition *= EyesRadius;
				eyePosition.z = -2f;
				m_eyeList[i].transform.localPosition = eyePosition;	
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
	
	private void UpdatePrimitive()
	{
		float humor = m_hungrer + m_hygiene + m_fun + m_social;
		if(humor >= 3.5f)
		{
			CurrentEmotion = EmotionType.GREAT;
		}
		else if(humor > 2.5f && humor < 3.5f)
		{
			CurrentEmotion = EmotionType.FRIENDLY;	
		}
		else if(humor >= 1.5f && humor <= 2.5f)
		{
			CurrentEmotion = EmotionType.NORMAL;	
		}
		else if(humor > 0.5f && humor < 1.5f)
		{
			CurrentEmotion = EmotionType.BAD;	
		}
		else
		{
			CurrentEmotion = EmotionType.WICKED;		
		}
	}
	
	private IEnumerator EyeAlive()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(100, 500) / 100f);
		if(m_isVisible)
		{
			print("EyeAlive");
			SetVisibleEyes(false);
			yield return new WaitForSeconds(0.25f);
			SetVisibleEyes(true);
		}
		yield return StartCoroutine(EyeAlive());
		
	}
	
	// Animation on eyes
	private void SetVisibleEyes(bool isVisible)
	{
		for(int i=0; i<m_eyeList.Count; i++)
			Misc.SetRendererActive(m_eyeList[i].transform, isVisible);
	}
	
	// Add the character for the interaction
    private void OnTrackingFound()
    {
		StartCoroutine(OnTrackingFoundProcess());
    }
	
	private IEnumerator OnTrackingFoundProcess()
	{
		yield return StartCoroutine(CheckInteractionManagerPresence());
		m_isVisible = true;
		Character character = this;
		InteractionManager.Instance.AddCharacter(ref character); 
	}
	
	// Remove the character from the scene
    private void OnTrackingLost()
    {
		RemoveStatusBar();
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
	public void UpdateStatusBar()
	{
		for(int i=0; i<m_listStatusBar.Length; i++) 
		{
			if(m_listStatusBar[i] != null) 
			{
				Transform statusBarFull = m_listStatusBar[i].transform.GetChild(1);
				statusBarFull.localScale = 
					GetCurrentStatusBarScale(GetCorrespondingAttributes(i), 
						m_listStatusBar[i].transform.GetChild(2));
			}
		}	
	}
	
	// Create all status bar
	public void CreateStatusBar()
	{
		for(int i=0; i<m_listStatusBar.Length; i++) 
		{
			m_listStatusBar[i] = (GameObject)Instantiate(GUIManager.Instance.m_statusBarPrefab);
			m_listStatusBar[i].transform.parent = GUIManager.Instance.StatusBarTransform;
			m_listStatusBar[i].GetComponentInChildren<SpriteText>().Text = GetCorrespondingAttributesName(i);
			switch(i)
			{
				case 0:
				m_listStatusBar[i].transform.localPosition = new Vector3(10, 0, 0);
				break;
				
				case 1:
				m_listStatusBar[i].transform.localPosition = new Vector3(10, m_listStatusBar[i-1].transform.localPosition.y - 30, 0);
				break;
				
				case 2:
				m_listStatusBar[i].transform.localPosition = new Vector3(250, 0, 0);
				break;
				
				case 3:
				m_listStatusBar[i].transform.localPosition = new Vector3(250, m_listStatusBar[i-1].transform.localPosition.y - 30, 0);
				break;
			}
		}
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
