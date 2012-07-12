using UnityEngine;
using System.Collections;


public class Cube : MonoBehaviour 
{
	private Quaternion m_rotationBase;
	private Quaternion m_rotationRation;
	private float m_moveRatio = 1f;
	private Vector3 m_velocityAcceleration;
	private Vector3 m_positionAcceleration;
	private Vector3 m_velocityGyroAcceleration;
	private Vector3 m_positionGyroAcceleration;
	private Vector3 m_velocityOld;
	private float m_threshold = 0.03f;
	private Rect mAreaRect;
	
	void Start () 
	{
		Transform originalParent = transform.parent; // check if this transform has a parent
		GameObject camParent = new GameObject ("camParent"); // make a new parent
		camParent.transform.position = transform.position; // move the new parent to this transform position
		transform.parent = camParent.transform; // make this transform a child of the new parent
		camParent.transform.parent = originalParent; // make the new parent a child of the original parent*/
		
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
		
		Input.location.Start();
	}
	
	private void Reset()
	{
		transform.position = Vector3.zero;	
		m_velocityAcceleration = Vector3.zero;
		m_positionAcceleration = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Input.gyro.
		///float iphoney=-Input.acceleration.y;
		//transform.Rotate(Vector3.up*iphoney*5f); 
		
		
		if (SystemInfo.supportsGyroscope) 
		{
			//if(Input.gyro.userAcceleration.magnitude > 0.03f)
			//{
			/*Vector3 filteredAcceleration = Input.gyro.userAcceleration - Input.gyro.gravity;
			m_velocity += filteredAcceleration * Time.deltaTime;
			m_position += filteredAcceleration.normalized * Time.deltaTime;		
			print(m_position);*/
			//}
			//Isolating gravity vector
/*gravity.x = currentAcceleration.x * kFileringFactor + gravity.x * (1.0 - kFileringFactor);
gravity.y = currentAcceleration.y * kFileringFactor + gravity.y * (1.0 - kFileringFactor);
gravity.z = currentAcceleration.z * kFileringFactor + gravity.z * (1.0 - kFileringFactor);
float gravityNorm = sqrt(gravity.x * gravity.x + gravity.y * gravity.y + gravity.z * gravity.z);

//Removing gravity vector from initial acceleration
filteredAcceleration.x = acceleration.x - gravity.x / gravityNorm;
filteredAcceleration.y = acceleration.y - gravity.y / gravityNorm;
filteredAcceleration.z = acceleration.z - gravity.z / gravityNorm;

//Calculating velocity related to time interval
velocity.x = velocity.x + filteredAcceleration.x * interval;
velocity.y = velocity.y + filteredAcceleration.y * interval;
velocity.z = velocity.z + filteredAcceleration.z * interval;

//Finding position
position.x = position.x + velocity.x * interval * 160;
position.y = position.y + velocity.y * interval * 230;*/
			
			
			
			transform.localRotation = Input.gyro.attitude * m_rotationRation;
			if(Input.gyro.userAcceleration.magnitude > m_threshold)
			{
				m_velocityAcceleration += (Input.gyro.userAcceleration - m_velocityOld) / 2f * m_moveRatio * Time.deltaTime;
				//m_velocityAcceleration += (Input.acceleration - Input.gyro.gravity) * Time.deltaTime;	
				//m_positionAcceleration += (Input.acceleration - Input.gyro.gravity).normalized * Time.deltaTime;
				m_positionAcceleration +=  m_velocityAcceleration * Time.deltaTime;
				
				transform.localPosition = m_positionAcceleration;
				m_velocityOld = Input.gyro.userAcceleration;
			}
			//Input.location.
			//print (Input.gyro.attitude);
			
			//print(Input.location.lastData.altitude+"/"+ Input.location.lastData.longitude + "/"+Input.acceleration.normalized+ "/" + Input.gyro.userAcceleration.normalized);
			/*if(Input.gyro.userAcceleration.magnitude > 0.03f)
			{
				LocationInfo info = Input.location.lastData;
				//print(Input.location.status+"="+info.altitude+"/"+ info.latitude + "/" + info.horizontalAccuracy + "/" + info.verticalAccuracy);
				print (Input.gyro.userAcceleration);
				//Input.gyro.
				//Vector3 acceleration = Input.gyro.userAcceleration.normalized;
				Vector3 direction = Input.gyro.userAcceleration * m_moveRatio * Time.deltaTime;
				//Vector3 acc = Input.gyro.userAcceleration;
				//acc.z *= -1;
				//print(Input.gyro.attitude+"/"+Input.gyro.userAcceleration.magnitude +"="+ acc);
				transform.localPosition += direction;
			}*/
		}
		/*else
		{
			transform.position += transform.forward * Time.deltaTime;
		}*/
	}
	
	void OnGUI()
	{	
		GUILayout.BeginHorizontal();
		GUILayout.Label("Acceleration = " +Input.acceleration);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Gyro Acceleration = " +Input.gyro.userAcceleration);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Acceleration Magnitude = " +Input.acceleration.magnitude);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Acceleration SQRMagnitude = " +Input.acceleration.sqrMagnitude);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Gravity = " +Input.gyro.gravity);
		GUILayout.EndHorizontal();
		
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Filtered Acceleration = " +(Input.acceleration - Input.gyro.gravity));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Velocity Acceleration = " +m_velocityAcceleration);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Position Acceleration = " + m_positionAcceleration);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Threshold = " + m_threshold);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Speed = " + m_moveRatio);
		GUILayout.EndHorizontal();
		
		// Setup style for buttons.
		computePosition();
        GUIStyle buttonGroupStyle = new GUIStyle(GUI.skin.button);
        buttonGroupStyle.stretchWidth = true;
        buttonGroupStyle.stretchHeight = true;
		GUILayout.BeginArea(mAreaRect);
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
      		m_moveRatio -= 10f;
        }
		if (GUILayout.Button("+ speed", buttonGroupStyle))
        {
      		m_moveRatio += 10f;
        }
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	/// Compute the coordinates of the menu depending on the current orientation.
    private void computePosition()
    {
        int areaWidth = Screen.width;
        int areaHeight = (Screen.height / 5);
        int areaLeft = 0;
        int areaTop = Screen.height - areaHeight;
        mAreaRect = new Rect(areaLeft, areaTop, areaWidth, areaHeight);
    }
}
