using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMarker : MonoBehaviour, ITrackableEventHandler
{
	
	private TrackableBehaviour m_trackableBehaviour;
	
	void Start()
    {
		// Init Trackable Behaviour
        m_trackableBehaviour = GetComponent<TrackableBehaviour>();
        if (m_trackableBehaviour)
        {
            m_trackableBehaviour.RegisterTrackableEventHandler(this);
        }
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
	
	// Load environment with worldMarker
    private void OnTrackingFound()
    {
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			if(InteractionManager.Instance.EnvironmentListTransform.Find(kvp.Key) == null)
				EnvironmentManager.Instance.LoadEnvironment(kvp.Key);
		}
    }
	
	// Rmove environment
    private void OnTrackingLost()
    {	
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			if(InteractionManager.Instance.EnvironmentListTransform.Find(kvp.Key) != null)
				EnvironmentManager.Instance.RemoveEnvironment(kvp.Key);
		}
    }
}
