using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour 
{
	public float m_minSpeed = 1f;
	public float m_maxSpeed = 5f;
	
	private float m_speed;
	
	void Start () 
	{
		m_speed = UnityEngine.Random.Range(m_minSpeed, m_maxSpeed);
	}
	
	void Update () 
	{
		transform.position += Vector3.right * Time.deltaTime * m_speed;
	}
}
