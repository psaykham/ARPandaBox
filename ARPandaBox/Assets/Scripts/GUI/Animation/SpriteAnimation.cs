using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable()]
public class SpriteAnimation : MonoBehaviour, ISerializable
{
	public enum TransformationType
	{
		SCALE,
		SCREEN_POSITION,
		ROTATION,
		COLOR,
		ALPHA,
	}
	
	public float m_durationInSeconds = 1f;
	public bool m_launchAtStart = false;
	public bool m_isTwoWayAnimation = false;
	public bool m_isLooping = false;
	public bool m_isReversed = false;
	
	private MonoBehaviour m_manager;
	
	// Animation attributes
	private UIStateToggleBtn m_spriteList;
	private int m_nbSprite = 0;
	private float m_durationBySprite = 0;
	private float m_timeline = 0;
	
	// Sprite States
	private bool m_isPlaying = false;
	private bool m_isMoving = false;
	public bool IsMoving {get{return m_isMoving;} set{m_isMoving = value;}}
	private bool m_isVisible = true;
	public bool isVisible {get{return m_isVisible;} set{m_isVisible = value;}}
	
	// Editor attributes
	[HideInInspector]
	public bool m_showTransformationList = true;
	[HideInInspector]
	public List<TransformationInfo> m_transformationList = new List<TransformationInfo>();
	
	public SpriteAnimation(SerializationInfo info, StreamingContext ctxt)
	{
		m_transformationList = (List<TransformationInfo>)info.GetValue("m_transformationList", typeof(List<TransformationInfo>));
	}
		
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{		
		info.AddValue("m_transformationList", m_transformationList);
	}
	
	void Awake()
	{
		m_manager = this;
		
		// RollOver is hidden
		SpriteRoot spriteRoot = transform.GetComponent<SpriteRoot>();
		if(spriteRoot != null)
		{
			if(spriteRoot.hideAtStart)
				m_isVisible = false;
		}
		
		// Animation list (sprite)
		m_spriteList = transform.GetComponent<UIStateToggleBtn>();
		if(m_spriteList != null)
		{
			// Don't take the disabled state
			m_nbSprite = m_spriteList.States.Length-1;
			m_durationBySprite = m_durationInSeconds/ m_nbSprite;
		}
		
		if(m_isTwoWayAnimation)
			m_durationInSeconds *= 2f;
	}
	
	void Start()
	{
		if(m_launchAtStart)
		{
			Play();			
		}
	}
	
	public void Play()
	{
		m_isPlaying = true;
		m_isReversed = false;	
		
		if(m_spriteList != null)
		{
			
			StartCoroutine(PlayInternal(m_spriteList.StateNum));
		}
		
		DoAnimation();
	}
	
	public void PlayReverse()
	{
		m_isPlaying = true;
		m_isReversed = true;	
		
		DoAnimation();
	}
	
	private IEnumerator PlayInternal(int index)
	{				
		// Display the next sprite
		m_spriteList.SetState(index);
		
		// Wait a moment
		yield return new WaitForSeconds(m_durationBySprite);
		m_timeline += m_durationBySprite;
		
		// If this is not the end, continue
		if(m_timeline < m_durationInSeconds && m_isPlaying)
		{
			if(m_isTwoWayAnimation && m_timeline >= m_durationInSeconds/2f)
				index--;
			else
				index++;
			
			index = Mathf.Clamp(index,0, m_spriteList.States.Length-1);
			
			if(index >= 0)
				yield return StartCoroutine(PlayInternal(index));
		}
		
		// If this is an animaiton loop recall play
		if(m_isLooping && m_isPlaying)
		{
			m_timeline = 0f;
			yield return StartCoroutine(PlayInternal(0));
		}
	}
	
	public void Pause()
	{
		m_isPlaying = false;	
	}
	
	public void Resume()
	{
		m_isPlaying = true;	
	}
	
	public void Stop()
	{
		m_isPlaying = false;
		Reset();
	}
	
	private void Reset()
	{
		m_timeline = 0f;
		if(m_spriteList != null)
			m_spriteList.SetState(0);	
	}
	
	private void DoAnimation()
	{				
		foreach(TransformationInfo transformationInfo in m_transformationList)
		{			
			switch(transformationInfo.Transformation)
			{
				case TransformationType.SCALE:
				m_manager.StartCoroutine(DoScaleAnimation(gameObject, transformationInfo, m_isLooping));
				break;
				
				case TransformationType.ROTATION:
				break;
				
				case TransformationType.SCREEN_POSITION:
				m_manager.StartCoroutine(DoScreenPositionAnimation(gameObject, transformationInfo, m_isLooping, m_isReversed));
				break;
				
				case TransformationType.COLOR:
				m_manager.StartCoroutine(DoColorAnimation(gameObject, transformationInfo, m_isLooping, m_isReversed));
				break;
				
				case TransformationType.ALPHA:
				m_manager.StartCoroutine(DoAlphaAnimation(gameObject, transformationInfo, m_isLooping, m_isReversed));
				break;
			}
		}
	}
	
	private float GetDuration(float duration)
	{			
		return duration;
	}
	
	// Scale
	public IEnumerator DoScaleAnimation(GameObject target, TransformationInfo transformationInfo, bool isLooping)
	{	
		if(target != null)
		{
			Vector3 fromValue = Vector3.zero;
			Vector3 toValue = Vector3.zero;
			
			// All animation
			foreach(TransformationKeyInfo transformationKeyInfo in transformationInfo.TransformationKeyList)
			{
				if(target != null)
				{
					if(transformationKeyInfo.IsRelative)
					{
						fromValue = target.transform.localScale;
						toValue = transformationKeyInfo.To;
					}
					else
					{
						fromValue = transformationKeyInfo.From;
						toValue = transformationKeyInfo.To;
					}	
						
					float duration = GetDuration(transformationKeyInfo.Duration);
					AnimateScale.Do(target,EZAnimation.ANIM_MODE.FromTo, fromValue, toValue, EZAnimation.GetInterpolator(transformationInfo.NormalEasing), duration,0f, null, null);
					yield return new WaitForSeconds(duration);
				}
			}
		
			// Re call if loop
			if(isLooping && target != null)
				m_manager.StartCoroutine(DoScaleAnimation(target, transformationInfo, isLooping));
		}
	}
	
	public float GetTransformationDuration(SpriteAnimation.TransformationType type)
	{
		float duration = 0f;
		foreach(TransformationInfo transformationInfo in m_transformationList)
		{			
			if(transformationInfo.Transformation == type)
			{
				foreach(TransformationKeyInfo transformationKeyInfo in transformationInfo.TransformationKeyList)
				{
					duration += GetDuration(transformationKeyInfo.Duration);
				}	
			}
		}
		
		return duration;
	}	
	
	// Screen Position
	public IEnumerator DoScreenPositionAnimation(GameObject target, TransformationInfo transformationInfo, bool isLooping, bool isReversed)
	{	
		if(target != null)
		{
			m_isMoving = true;
			Vector3 fromValue = Vector3.zero;
			Vector3 toValue = Vector3.zero;
			
			EZAnimation.EASING_TYPE currentEasing = transformationInfo.NormalEasing;
			if(isReversed)
				currentEasing = transformationInfo.ReverseEasing;

			// All animation
			foreach(TransformationKeyInfo transformationKeyInfo in transformationInfo.TransformationKeyList)
			{
				if(target != null)
				{
					if(transformationKeyInfo.IsRelative)
					{	
						fromValue = target.GetComponent<EZScreenPlacement>().screenPos;
						
						// Reverse
						if(isReversed)
							toValue = fromValue - transformationKeyInfo.To;
						else
							toValue = fromValue + transformationKeyInfo.To;
					}
					else
					{
						if(isReversed)
						{
							fromValue = transformationKeyInfo.To;
							toValue = transformationKeyInfo.From;
						}
						else
						{
							fromValue = transformationKeyInfo.From;
							toValue = transformationKeyInfo.To;
						}
					}
					
					float duration = GetDuration(transformationKeyInfo.Duration);
					AnimateScreenPosition.Do(target,EZAnimation.ANIM_MODE.FromTo, fromValue, toValue, EZAnimation.GetInterpolator(currentEasing), duration,0f, null, null);	
					yield return new WaitForSeconds(duration);
				}
			}
		
			// Re call if loop
			if(isLooping && target != null)
				m_manager.StartCoroutine(DoScreenPositionAnimation(target, transformationInfo, isLooping, isReversed));
			else
				m_isMoving = false;
		}
	}
	
	// Color animation
	public IEnumerator DoColorAnimation(GameObject target, TransformationInfo transformationInfo, bool isLooping, bool isReversed)
	{			
		if(target != null)
		{
			yield return 1;
			
			Color fromValue = Color.white;
			Color toValue = Color.white;
			
			EZAnimation.EASING_TYPE currentEasing = transformationInfo.NormalEasing;
			if(isReversed)
				currentEasing = transformationInfo.ReverseEasing;

			// All animation
			foreach(TransformationKeyInfo transformationKeyInfo in transformationInfo.TransformationKeyList)
			{
				if(target != null)
				{
					// Relative animation
					/*if(transformationKeyInfo.IsRelative)
					{	
						fromValue = target.renderer.material.color;
						
						// Reverse
						Vector3 newValue = Vector3.zero;
						if(isReversed)
						{
							newValue = new Vector3(fromValue.r, fromValue.g, fromValue.b) - transformationKeyInfo.To;
							toValue = new Color(newValue.x, newValue.y, newValue.z);
						}
						else
						{
							newValue =  new Vector3(fromValue.r, fromValue.g, fromValue.b) + transformationKeyInfo.To;
							toValue = new Color(newValue.x, newValue.y, newValue.z);
						}
					}
					else
					{*/
						fromValue = transformationKeyInfo.FromColor;
						toValue = transformationKeyInfo.ToColor;
					//}
					 
					float duration = GetDuration(transformationKeyInfo.Duration);
					test(target.transform, fromValue, toValue, currentEasing, duration);
					yield return new WaitForSeconds(duration);
				}
			}
		
			// Re call if loop
			if(isLooping && target != null)
				m_manager.StartCoroutine(DoColorAnimation(target, transformationInfo, isLooping, isReversed));
		}
	}
	
	private void test(Transform target, Color fromValue, Color toValue, EZAnimation.EASING_TYPE currentEasing, float duration)
	{
		SpriteText targetText = target.GetComponent<SpriteText>();
		if(targetText != null)
		{
			//Fade
			FadeText.Do(targetText,EZAnimation.ANIM_MODE.FromTo, fromValue, toValue, EZAnimation.GetInterpolator(currentEasing), duration, 0.1f, null, null);	
		}
		else
		{
			SpriteRoot targetSprite = (SpriteRoot)target.GetComponent<SpriteRoot>();
			if(targetSprite != null)
				FadeSprite.Do(targetSprite,EZAnimation.ANIM_MODE.FromTo, fromValue, toValue, EZAnimation.GetInterpolator(currentEasing), duration, 0.1f, null, null);	
		}
		
		foreach(Transform child in target)
		{
			test(child, fromValue, toValue, currentEasing, duration);
		}
	}
	
	// Color animation
	public IEnumerator DoAlphaAnimation(GameObject target, TransformationInfo transformationInfo, bool isLooping, bool isReversed)
	{			
		if(target != null)
		{
			yield return 1;
			
			Color fromValue = Color.white;
			Color toValue = Color.white;
			
			EZAnimation.EASING_TYPE currentEasing = transformationInfo.NormalEasing;
			if(isReversed)
				currentEasing = transformationInfo.ReverseEasing;

			// All animation
			foreach(TransformationKeyInfo transformationKeyInfo in transformationInfo.TransformationKeyList)
			{
				if(target != null)
				{
					// Relative animation
					if(transformationKeyInfo.IsRelative)
					{							
						// Reverse
						if(isReversed)
						{
							toValue = transformationKeyInfo.FromColor;
						}
						else
						{
							toValue = transformationKeyInfo.ToColor;
						}
					}
					else
					{
						// Reverse
						if(isReversed)
						{
							fromValue = transformationKeyInfo.ToColor;
							toValue = transformationKeyInfo.FromColor;
						}
						else
						{
							fromValue = transformationKeyInfo.FromColor;
							toValue = transformationKeyInfo.ToColor;
						}
					}
					 
					// Repeat on all child
					float duration = GetDuration(transformationKeyInfo.Duration);
					DoAlphaAnimationRecursive(target.transform, fromValue, toValue, currentEasing, duration);
					
					// Wait the animation starting to display sprite hidden at sprite
					yield return new WaitForSeconds(0.1f);
					if(target != null)
						Misc.SetVisibleSpriteHiddenAtStart(transform);
					
					// Wait the animation end
					yield return new WaitForSeconds(Mathf.Clamp01(duration-0.1f));
				}
			}
		
			// Re call if loop
			if(isLooping && target != null && m_isPlaying)
				m_manager.StartCoroutine(DoAlphaAnimation(target, transformationInfo, isLooping, isReversed));
			else
			{
				// Check if the rollover is visible
				if(toValue.a > 0)
					m_isVisible = true;	
				else
					m_isVisible = false;
			}
		}
	}
	
	private void DoAlphaAnimationRecursive(Transform target, Color fromValue, Color toValue, EZAnimation.EASING_TYPE currentEasing, float duration)
	{
		SpriteText targetText = target.GetComponent<SpriteText>();
		if(targetText != null)
		{
			//Fade			
			fromValue = new Color(targetText.color.r, targetText.color.g, targetText.color.b, fromValue.a);
			toValue = new Color(targetText.color.r, targetText.color.g, targetText.color.b, toValue.a);
			FadeTextAlpha.Do(targetText,EZAnimation.ANIM_MODE.FromTo, fromValue, toValue, EZAnimation.GetInterpolator(currentEasing), duration, 0.1f, null, null);	
		}
		else
		{
			SpriteRoot targetSprite = (SpriteRoot)target.GetComponent<SpriteRoot>();
			if(targetSprite != null)
				FadeSpriteAlpha.Do(targetSprite,EZAnimation.ANIM_MODE.FromTo, fromValue, toValue, EZAnimation.GetInterpolator(currentEasing), duration, 0.1f, null, null);	
		}
		
		foreach(Transform child in target)
		{
			DoAlphaAnimationRecursive(child, fromValue, toValue, currentEasing, duration);
		}
	}
	
	// Increment a SpriteText
	public static void IncrementSpriteText(SpriteText sprite, int fromValue, int toValue, float duration)
	{
//		print("IncrementSpriteText on " + sprite.gameObject.name);
		InteractionManager.Instance.StartCoroutine(IncrementSpriteTextAnimation(sprite, fromValue, toValue, duration));
	}
	
	// Increment a SpriteText Animation
	private static IEnumerator IncrementSpriteTextAnimation(SpriteText sprite, int fromValue, int toValue, float duration)
	{
		// Attributes needed
		float durationInternal = 0;
		int differenceValue = toValue-fromValue;
		int fromValueInternal = 0;
			
		while(durationInternal < duration)
		{		
			// Check sprite
			if(sprite == null)
			{
				break;
			}
			else
			{
				// Display Value
				sprite.Text = (fromValue + fromValueInternal).ToString();
				
				// Calculating new value
				float ratio = durationInternal/duration;
				fromValueInternal = (int)(differenceValue*ratio);
				
				// Wait
				yield return new WaitForSeconds(Time.deltaTime);
				durationInternal += Time.deltaTime;
			}
		}

		// To be sur the player see the exact value		
		if(sprite != null)
			sprite.Text = toValue.ToString();	
	}
	
	// Interpolation Animation
	public static void SphericalInterpolation(GameObject sprite, Transform fromObject, Transform toObject)
	{
		InteractionManager.Instance.StartCoroutine(SphericalInterpolationAnimation(sprite, 3, fromObject, toObject, 0.25f, 35f));
	}
	
	public static void SphericalInterpolation(GameObject sprite, int ghost, Transform fromObject, Transform toObject)
	{
		InteractionManager.Instance.StartCoroutine(SphericalInterpolationAnimation(sprite, ghost, fromObject, toObject, 0.25f, 35f));
	}
	
	public static void SphericalInterpolation(GameObject sprite, Transform fromObject, Transform toObject, float duration)
	{
		InteractionManager.Instance.StartCoroutine(SphericalInterpolationAnimation(sprite, 3, fromObject, toObject, duration, 35f));
	}
	
	public static void SphericalInterpolation(GameObject sprite, int ghost, Transform fromObject, Transform toObject, float duration)
	{
		InteractionManager.Instance.StartCoroutine(SphericalInterpolationAnimation(sprite, ghost, fromObject, toObject, duration, 35f));
	}
	
	public static void SphericalInterpolation(GameObject sprite, int ghost, Transform fromObject, Transform toObject, float duration, float strengh)
	{
		InteractionManager.Instance.StartCoroutine(SphericalInterpolationAnimation(sprite, ghost, fromObject, toObject, duration, strengh));
	}
	
	private static IEnumerator SphericalInterpolationAnimation(GameObject sprite, int ghost, Transform fromObject, Transform toObject, float duration, float strengh)
	{		
		// Make Instances
		GameObject[] spriteInstanceList = new GameObject[ghost];
		Color spriteColor = Color.white;
		for(int i=0; i<ghost; i++)
		{
			spriteInstanceList[i] = (GameObject)Instantiate(sprite, fromObject.position, fromObject.rotation);	
			
			// Increase alpha
			SpriteRoot spriteRoot = spriteInstanceList[i].GetComponent<SpriteRoot>();
			if(spriteRoot != null)
			{
				spriteColor.a = 1f - Mathf.Clamp01((float)i/ghost);
				FadeSpriteAlpha.Do(spriteRoot,EZAnimation.ANIM_MODE.To, spriteColor, EZAnimation.GetInterpolator(EZAnimation.EASING_TYPE.Linear), 0f, 0f, null, null);	
			}
		}
		
		// Wait a frame
		yield return 1;

		// Move it to the destination
		float durationInternal = 0;
		while(durationInternal <= duration)
		{
			// Wait a frame
			yield return 1;
			
			if(fromObject == null)
			{
				break;
			}
			else
			{
				// Move all instances
				for(int i=0; i<ghost; i++)
				{
					Vector3 spritePosition = Vector3.Slerp(fromObject.position, toObject.position, durationInternal/duration - i/strengh);
					spritePosition.z = fromObject.position.z - ghost + i;
					spriteInstanceList[i].transform.position = spritePosition;
				}
				
				// Wait
				yield return new WaitForSeconds(Time.deltaTime);
				durationInternal += Time.deltaTime;	
			}
		}
		
		// Destory instances
		for(int i=0; i<ghost; i++)
		{
			Destroy(spriteInstanceList[i]);
		}
	}
}