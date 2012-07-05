using UnityEngine;
using System.Collections;


public class Cube : MonoBehaviour 
{
	private Quaternion m_rotationBase;
	private Quaternion m_rotationRation;
	private float m_moveRatio = 0.1f;
	
	void Start () 
	{
		Transform originalParent = transform.parent; // check if this transform has a parent
		GameObject camParent = new GameObject ("camParent"); // make a new parent
		camParent.transform.position = transform.position; // move the new parent to this transform position
		transform.parent = camParent.transform; // make this transform a child of the new parent
		camParent.transform.parent = originalParent; // make the new parent a child of the original parent*/
		
		// Is Gyroscope avaible ?
		if (Input.isGyroAvailable) 
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
	
	// Update is called once per frame
	void Update () 
	{
		//Input.gyro.
		///float iphoney=-Input.acceleration.y;
		//transform.Rotate(Vector3.up*iphoney*5f); 
		
		if (Input.isGyroAvailable) 
		{
			//print (Input.gyro.attitude);
			transform.localRotation = Input.gyro.attitude * m_rotationRation;
			//print(Input.gyro.userAcceleration.magnitude +"="+ Input.gyro.userAcceleration);
			if(Input.gyro.userAcceleration.magnitude > 0.03f)
			{
				print (Input.gyro.userAcceleration.magnitude +"/"+Input.gyro.userAcceleration.normalized * m_moveRatio);
				//Input.gyro.
				Vector3 acc = Input.gyro.userAcceleration.normalized * m_moveRatio;
				//Vector3 acc = Input.gyro.userAcceleration;
				//acc.z *= -1;
				//print(Input.gyro.attitude+"/"+Input.gyro.userAcceleration.magnitude +"="+ acc);
				transform.position += acc;
			}
		}
	}
}
