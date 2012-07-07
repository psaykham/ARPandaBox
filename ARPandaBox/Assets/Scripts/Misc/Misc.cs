using UnityEngine;
using System.Collections;

public class Misc
{
	// Instantiate a prefab as child
	public static T InstantiateAsChild<T>(T prefab, Component parent) where T : Component
	{
		T prefabInstance = (T)GameObject.Instantiate(prefab);
		prefabInstance.transform.parent = parent.transform;
		prefabInstance.name = prefab.name;
		
		return prefabInstance;
	}
	
	public static void SetRendererActive(Transform tr, bool isActive)
	{
		tr.renderer.enabled = isActive;
		foreach(Transform child in tr)
			SetRendererActive(child, isActive);
	}
	
	// If the sprite are hidden at start we need to manipulate them for alpha
	public static void SetVisibleSpriteHiddenAtStart(Transform tr)
	{	
		// Basic Sprite
		SpriteRoot sprite = tr.GetComponent<SpriteRoot>();
		if(sprite != null && tr.renderer != null)
		{
			if(sprite.hideAtStart && !sprite.renderer.enabled)
			{	
				sprite.Hide(false);	
			}
		}
		// SpriteText
		else
		{
			SpriteText spriteText = tr.GetComponent<SpriteText>();
			if(spriteText != null && tr.renderer != null)
			{
				if(spriteText.hideAtStart && !spriteText.renderer.enabled)
				{
					spriteText.Hide(false);	
				}
			}
		}
		
		// Child
		foreach(Transform child in tr)
		{
			SetVisibleSpriteHiddenAtStart(child);	
		}
	}
}
