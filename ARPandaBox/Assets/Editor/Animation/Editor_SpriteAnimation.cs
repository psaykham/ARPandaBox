using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SpriteAnimation))]
public class Editor_SpriteAnimation : Editor 
{
	private SpriteAnimation m_spriteAnimation;
	private SpriteAnimation.TransformationType m_transformationSelected;
	
	public override void OnInspectorGUI()
	{
		// Draw Base Inspector
		this.DrawDefaultInspector();
		m_spriteAnimation = (SpriteAnimation)target;
		
		// Transformation List Header
		EditorGUILayout.BeginHorizontal();	
		m_spriteAnimation.m_showTransformationList = EditorGUILayout.Foldout(m_spriteAnimation.m_showTransformationList, "Transformation List");
		m_transformationSelected = (SpriteAnimation.TransformationType)EditorGUILayout.EnumPopup("Transformation Type", m_transformationSelected);
		if(GUILayout.Button("+", GUILayout.Width(30)))
		{
			m_spriteAnimation.m_transformationList.Add(new TransformationInfo(m_transformationSelected));
		}
		EditorGUILayout.EndHorizontal();
		
		// Transformation List Content
		if(m_spriteAnimation.m_showTransformationList)
		{
			for(int i=0; i<m_spriteAnimation.m_transformationList.Count; i++)
			{
				
				// Transformation Content
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(15);
				m_spriteAnimation.m_transformationList[i].Show = EditorGUILayout.Foldout(m_spriteAnimation.m_transformationList[i].Show, "Animation "+m_spriteAnimation.m_transformationList[i].Transformation+" "+(i+1));
				if(GUILayout.Button("-", GUILayout.Width(30)))
				{
					m_spriteAnimation.m_transformationList.RemoveAt(i);
				}
				EditorGUILayout.EndHorizontal();
				
				if(m_spriteAnimation.m_transformationList.Count > i)
				{
				
					// Transformation Content
					if(m_spriteAnimation.m_transformationList[i].Show)
					{					
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(30);
						m_spriteAnimation.m_transformationList[i].NormalEasing = (EZAnimation.EASING_TYPE)EditorGUILayout.EnumPopup("Normal Easing", m_spriteAnimation.m_transformationList[i].NormalEasing);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(30);
						m_spriteAnimation.m_transformationList[i].ReverseEasing = (EZAnimation.EASING_TYPE)EditorGUILayout.EnumPopup("Reverse Easing", m_spriteAnimation.m_transformationList[i].ReverseEasing);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(30);
						if(GUILayout.Button("Add Key", GUILayout.Width(100)))
						{
							m_spriteAnimation.m_transformationList[i].TransformationKeyList.Add(new TransformationKeyInfo());
						}
						EditorGUILayout.EndHorizontal();
						
						// Transformation Key List Content
						if(m_spriteAnimation.m_transformationList[i].TransformationKeyList != null)
						{
							for(int j=0; j<m_spriteAnimation.m_transformationList[i].TransformationKeyList.Count; j++)
							{
								
								// Transformation Key Detail Header
								EditorGUILayout.BeginHorizontal();
								GUILayout.Space(30);
								m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].Show = EditorGUILayout.Foldout(m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].Show, "Key "+(j+1));
								if(GUILayout.Button("-", GUILayout.Width(30)))
								{
									m_spriteAnimation.m_transformationList[i].TransformationKeyList.RemoveAt(j);
								}
								EditorGUILayout.EndHorizontal();	
								
								// Transformation Key Detail Content
								if(m_spriteAnimation.m_transformationList[i].TransformationKeyList.Count > j)
								{
									if(m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].Show)
									{
										EditorGUILayout.BeginHorizontal();
										GUILayout.Space(45);
										m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].IsRelative = EditorGUILayout.Toggle("Is Relative", m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].IsRelative);	
										EditorGUILayout.EndHorizontal();
										
										// relative
										if(!m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].IsRelative)
										{
											EditorGUILayout.BeginHorizontal();
											GUILayout.Space(45);
											switch(m_spriteAnimation.m_transformationList[i].Transformation)
											{
												case SpriteAnimation.TransformationType.COLOR:
												case SpriteAnimation.TransformationType.ALPHA:
												m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].FromColor = EditorGUILayout.ColorField("From", m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].FromColor);
												break;
												
												default:
												m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].From = EditorGUILayout.Vector3Field("From", m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].From);
												break;
											}		
											EditorGUILayout.EndHorizontal();
										}
										
										EditorGUILayout.BeginHorizontal();
										GUILayout.Space(45);
										switch(m_spriteAnimation.m_transformationList[i].Transformation)
										{
											case SpriteAnimation.TransformationType.COLOR:
											case SpriteAnimation.TransformationType.ALPHA:
											m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].ToColor = EditorGUILayout.ColorField("To", m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].ToColor);
											break;
											
											default:
											m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].To = EditorGUILayout.Vector3Field("To", m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].To);
											break;
										}
										EditorGUILayout.EndHorizontal();
											
										EditorGUILayout.BeginHorizontal();
										GUILayout.Space(45);
										m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].Duration = EditorGUILayout.FloatField("Duration", m_spriteAnimation.m_transformationList[i].TransformationKeyList[j].Duration);
										EditorGUILayout.EndHorizontal();
									}
								}
							}
						}
					}
				}
			}
		}
		
		// If there is any changes, we force unity to save
		if(GUI.changed)
			EditorUtility.SetDirty(m_spriteAnimation.gameObject);
	}	
}
