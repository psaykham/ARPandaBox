using UnityEngine;
using System.Collections;

/// <summary>
/// Single allow to have a unique instance for a class
/// This is important for Managers
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Create the instance if not done yet
	private static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
				m_instance = (T)GameObject.FindObjectOfType(typeof(T));
            }
            return m_instance;
        }
    }
}
