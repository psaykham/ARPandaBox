using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraSpace : MonoBehaviour 
{
	private Quaternion m_rotationBase;
	private Quaternion m_rotationRation;
	private float m_moveSpeed = 50000f;
	private Vector3 m_currentAcceleration;
	private Vector3 m_oldAcceleration;
	private Vector3 m_oldVelocity;
	private float m_threshold = 0.3f;
	private Rect m_guiArea;
	
	void Start () 
	{
		// Parent for the camera
		Transform originalParent = transform.parent;
		GameObject camParent = new GameObject ("ARCameraParent");
		camParent.transform.position = transform.position;
		transform.parent = camParent.transform;
		camParent.transform.parent = originalParent;
		
		// Is Gyroscope avaible ?
		if (SystemInfo.supportsGyroscope) 
		{
			Input.gyro.enabled = true;
			
			// Orientation base
			if (Screen.orientation == ScreenOrientation.LandscapeLeft) 
			{
				camParent.transform.eulerAngles = new Vector3(90,90,0);
			}
			else if (Screen.orientation == ScreenOrientation.Portrait) 
			{
				camParent.transform.eulerAngles = new Vector3(90,180,0);
			}
			
			// Rotation Ratio
			if (Screen.orientation == ScreenOrientation.LandscapeLeft) 
			{
				m_rotationRation = new Quaternion(0,0,0.7071f,0.7071f);
			}
			else if (Screen.orientation == ScreenOrientation.Portrait) 
			{
				m_rotationRation = new Quaternion(0,0,1,0);
			}
		} 
	}
	
	// Reset the variables
	private void Reset()
	{
		transform.localPosition = Vector3.zero;	
		m_currentAcceleration = Vector3.zero;	
		m_oldAcceleration = Vector3.zero;
		m_oldVelocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(InteractionManager.Instance != null)
		{
			if(InteractionManager.Instance.CharacterList.Count == 0)
			{
				// Check if the device support Gyroscope
				if (SystemInfo.supportsGyroscope) 
				{
					// Camera Rotation
					transform.localRotation = Input.gyro.attitude * m_rotationRation;
					
					// Threshold to avoid jerking - (User Acceleration is Acceleration - Gravity)
					if(Input.gyro.userAcceleration.magnitude > m_threshold)
					{		
						// Compute 	distance from acceleration
						m_currentAcceleration += (Input.gyro.userAcceleration + m_oldAcceleration) / 2f * Time.deltaTime;
						Vector3 distance = (m_currentAcceleration + m_oldVelocity) / 2f * m_moveSpeed * Time.deltaTime ;
						
						// Compute position with camera rotation
						Vector3 position = Vector3.zero;
						position += transform.right * distance.x;
						position += transform.up * distance.z;
						position += transform.forward * distance.y;
						transform.localPosition += position;
								
						// Save acceleration end velocity
						m_oldAcceleration = Input.gyro.userAcceleration;
						m_oldVelocity = m_currentAcceleration;
					}
					else
					{
						// Reset Value
						m_currentAcceleration = Vector3.zero;	
						m_oldAcceleration = Vector3.zero;
						m_oldVelocity = Vector3.zero;
					}
				}
			}
		}
	}
	
	void OnGUI()
	{	
		if(InteractionManager.Instance != null)
		{
			if(InteractionManager.Instance.CharacterList.Count == 0)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Acceleration = " +Input.gyro.userAcceleration);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Acceleration Magnitude = " +Input.gyro.userAcceleration.magnitude);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Velocity Acceleration = " + m_currentAcceleration);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Position = " + transform.localPosition);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Threshold = " + m_threshold);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Speed = " + m_moveSpeed);
				GUILayout.EndHorizontal();
			
				// Setup style for buttons.
				computePosition();
		        GUIStyle buttonGroupStyle = new GUIStyle(GUI.skin.button);
		        buttonGroupStyle.stretchWidth = true;
		        buttonGroupStyle.stretchHeight = true;
				GUILayout.BeginArea(m_guiArea);
		        GUILayout.BeginHorizontal(buttonGroupStyle);
		        if (GUILayout.Button("Reset", buttonGroupStyle))
		        {
		      		Reset();
		        }
				if (GUILayout.Button("- threshold", buttonGroupStyle))
		        {
		      		m_threshold -= 0.01f;
		        }
				if (GUILayout.Button("+ threshold", buttonGroupStyle))
		        {
		      		m_threshold += 0.01f;
		        }
				if (GUILayout.Button("- speed", buttonGroupStyle))
		        {
		      		m_moveSpeed -= 1000f;
		        }
				if (GUILayout.Button("+ speed", buttonGroupStyle))
		        {
		      		m_moveSpeed += 1000f;
		        }	
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
	}
	
	/// Compute the coordinates of the menu depending on the current orientation.
    private void computePosition()
    {
        int areaWidth = Screen.width;
        int areaHeight = (Screen.height / 5);
        int areaLeft = 0;
        int areaTop = Screen.height - areaHeight;
        m_guiArea = new Rect(areaLeft, areaTop, areaWidth, areaHeight);
    }
}
