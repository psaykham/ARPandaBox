using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationBar : MonoBehaviour 
{
	void Awake()
	{
		// Hide Active state
		foreach(Transform child in transform)
		{
			if(child.name != "Profile")
				child.Find("Active").gameObject.active = false;
			UIButton buttonControl = child.Find("Background").GetComponent<UIButton>();
			buttonControl.AddInputDelegate(NavigationBarDelegate);
		}	
	}
	
	private void NavigationBarDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			Transform button = ptr.targetObj.transform.parent;
			GameObject buttonActive = button.Find("Active").gameObject;
			print(button.name);
			switch(button.name)
			{
				case "Profile":
				if(InteractionManager.Instance != null)
				{
					buttonActive.active = !buttonActive.active;
					Misc.SetRendererActive(GUIManager.Instance.StatusBarTransform, buttonActive.active);
				}
				break;
				
				case "Talk":
				StartCoroutine(KeyboardActivate());
				break;
				
				case "Food":
				InteractionManager.Instance.GiveFood();
				break;
				
				case "Ball":
				InteractionManager.Instance.GiveBall();
				break;
				
				case "Settings":
				buttonActive.active = !buttonActive.active;
				Dictionary<string, Character> characterListTmp = new Dictionary<string, Character>(InteractionManager.Instance.CharacterList);
				foreach(KeyValuePair<string, Character> kvp in characterListTmp)
				{
					Misc.SetRendererActive(InteractionManager.Instance.CharacterList[kvp.Key].Wireframe.transform, buttonActive.active);
				}
				break;
			}
		}
	}
	
	private IEnumerator KeyboardActivate()
	{
		if(InteractionManager.Instance.MainCharacter != null)
		{
			iPhoneKeyboard keyboard = iPhoneKeyboard.Open("", iPhoneKeyboardType.Default, true, true, false);
			while(keyboard.active)
				yield return 100;
			
			if(keyboard.text.Length > 0)
				GUIManager.Instance.DisplayMessage(InteractionManager.Instance.MainCharacter.Name, keyboard.text);
		}
	}
}
