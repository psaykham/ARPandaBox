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
    
	// Attributs
	private List<GameObject> m_eyeList = new List<GameObject>();
	private UIStateToggleBtn m_mouth;
	private Transform m_target;
	public Transform Target { get {return m_target;} set{m_target = value;}}
	
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
	}
	
    void Start()
    {
        m_trackableBehaviour = GetComponent<TrackableBehaviour>();
        if (m_trackableBehaviour)
        {
            m_trackableBehaviour.RegisterTrackableEventHandler(this);
        }
		
		StartCoroutine(EyeAlive());
    }
	
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
		print("EyeAlive");
		yield return new WaitForSeconds(UnityEngine.Random.Range(100, 500) / 100f);
		SetVisibleEyes(false);
		yield return new WaitForSeconds(0.25f);
		SetVisibleEyes(true);
		yield return StartCoroutine(EyeAlive());
		
	}
	
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
		InteractionManager.Instance.AddCharacter(this); 
	}
	
	// Remove the character from the scene
    private void OnTrackingLost()
    {
		if(InteractionManager.Instance != null) {
			InteractionManager.Instance.RemoveCharacter(Name); 
			this.gameObject.SetActiveRecursively(!this.gameObject.active);
		}
    }
	
	// Wait that the InteractionManager is created
	private IEnumerator CheckInteractionManagerPresence()
	{
		while(InteractionManager.Instance == null)
			yield return 100;
	}
}
