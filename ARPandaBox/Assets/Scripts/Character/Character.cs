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
	public GameObject Wireframe;
		
	private ConversationManager.ConversationStep m_currentConversationStep;
	public ConversationManager.ConversationStep CurrentConversationStep {get {return m_currentConversationStep;} set{m_currentConversationStep=value;}}
    private TrackableBehaviour m_trackableBehaviour;
    
	void Awake()
	{	
		m_currentConversationStep = ConversationManager.ConversationStep.GREETING;
	}
	
    void Start()
    {
        m_trackableBehaviour = GetComponent<TrackableBehaviour>();
        if (m_trackableBehaviour)
        {
            m_trackableBehaviour.RegisterTrackableEventHandler(this);
        }
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
		//print(Camera.mainCamera.projectionMatrix);	
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
