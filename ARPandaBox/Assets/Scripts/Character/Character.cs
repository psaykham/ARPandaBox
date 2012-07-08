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
	}
	
	private IEnumerator UpdateHunger()
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(100, 300) / 100f);
		Eat(-1);
		yield return StartCoroutine(UpdateHunger());
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
	
	// Add the character for the interaction
    private void OnTrackingFound()
    {
		StartCoroutine(OnTrackingFoundProcess());
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
}
