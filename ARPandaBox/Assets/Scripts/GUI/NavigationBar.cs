using UnityEngine;
using System.Collections;

public class NavigationBar : MonoBehaviour 
{
	void Awake()
	{
		// Hide Active state
		foreach(Transform child in transform)
		{
			child.Find("Active").gameObject.active = false;
			UIButton buttonControl = child.Find("Background").GetComponent<UIButton>();
			buttonControl.AddInputDelegate(NavigationBarDelegate);
		}	
	}
	
	private void NavigationBarDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{	
			GameObject buttonActive = ptr.targetObj.transform.parent.Find("Active").gameObject;
			if(!buttonActive.active)
			{
				foreach(Transform child in transform)
					child.Find("Active").gameObject.active = false;
				
				buttonActive.active = true;	
				ButtonAction(ptr.targetObj.transform.parent.name);
			}
			else
			{
				buttonActive.active = false;	
			}
		}
	}
	
	private void ButtonAction(string buttonName)
	{
		print(buttonName);
		switch(buttonName)
		{
			case "Profile":
			print("You are "+InteractionManager.Instance.MainCharacter.Name);
			break;
			
			case "Talk":
			iPhoneKeyboard.Open("", iPhoneKeyboardType.Default, false, false, true, true);
			break;
		}
	}
}
