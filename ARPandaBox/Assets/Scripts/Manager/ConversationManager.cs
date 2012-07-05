using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ConversationManager : Singleton<ConversationManager> 
{
	public enum ConversationStep
	{
		NONE,
		ALONE,
		GREETING,
		SPEAK,
		QUESTION,
		RESPONSE,
		GOODBYE,
	}
	
	public List<ConversationStep> m_conversationSpeakAvaible;
	public List<string> m_variableNameList;
	public float m_timeInteraction = 3.5f;
	
	private XmlDocument m_document;
	private int m_cursorConversation = 0;
	private Dictionary<string, bool> m_characterIsSpeaking = new Dictionary<string, bool>();
	private Dictionary<string, bool> m_characterHadSpoken = new Dictionary<string, bool>();
	private string m_currentCategory = "";
	
	void Awake()
	{
		print("ConversationManager");
		m_document = new XmlDocument();
		m_document.LoadXml((Resources.Load("Conversation") as TextAsset).text);
	}
	
	void Update()
	{
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			// Add the character to the list
			if(!m_characterIsSpeaking.ContainsKey(kvp.Key))
			{
				m_characterIsSpeaking.Add(kvp.Key, false);
				m_characterHadSpoken.Add(kvp.Key, false);
			}
			
			CheckCharacterAlone(kvp.Key);
			StartCoroutine(Speak(kvp.Key));
		}
	}
	
	private void CheckCharacterAlone(string characterName)
	{
		// Alone
		if(InteractionManager.Instance.CharacterList.Count < 2)
		{
			switch(InteractionManager.Instance.CharacterList[characterName].CurrentConversationStep)
			{
				case ConversationStep.GREETING:
				SetConversationStep(characterName, ConversationStep.ALONE);
				break;
				
				case ConversationStep.ALONE:
				SetConversationStep(characterName, ConversationStep.NONE);
				break;
			}
		}
	}
	
	private void CheckCharacterMute(string characterName)
	{
		// The character doesn't speak anymore
		if(InteractionManager.Instance.CharacterList[characterName].CurrentConversationStep == ConversationStep.NONE)
		{	
			int indexCharacterRandom = UnityEngine.Random.Range(0, InteractionManager.Instance.CharacterList.Count);
			int indexStepRandom = UnityEngine.Random.Range(0, m_conversationSpeakAvaible.Count);
			Transform characterTransform = InteractionManager.Instance.CharacterListTransfrom.GetChild(indexCharacterRandom);
			string nameCharacterRandom = characterTransform.GetComponent<Character>().Name;
			SetConversationStep(nameCharacterRandom, m_conversationSpeakAvaible[indexStepRandom]);
			
			foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
			{
				if(kvp.Key != characterName)
					UpdateCharacterStep(kvp.Key);	
			}
		}	
	}
	
	// Update step for the conversation
	private void UpdateCharacterStep(string characterName)
	{	
		// Continue the conversation
		switch(InteractionManager.Instance.CharacterList[characterName].CurrentConversationStep)
		{
			case ConversationStep.NONE:
			if(IsSomebodyQuestion())
				SetConversationStep(characterName, ConversationStep.RESPONSE);
			break;
			
			case ConversationStep.ALONE:
			SetConversationStep(characterName, ConversationStep.GREETING);
			break;
			
			case ConversationStep.GREETING:
			if(!IsSomebodyQuestion())
				SetConversationStep(characterName, ConversationStep.QUESTION);
			else
				SetConversationStep(characterName, ConversationStep.RESPONSE);
			break;
			
			case ConversationStep.SPEAK:			
			case ConversationStep.QUESTION:
			case ConversationStep.RESPONSE:
			SetConversationStep(characterName, ConversationStep.NONE);
			break;
		}
	}
	
	private void SetConversationStep(string characterName, ConversationStep conversationStep)
	{
		InteractionManager.Instance.CharacterList[characterName].CurrentConversationStep = conversationStep;
	}
	
	// Check if something is already speaking
	private bool IsSomebodySpeaking()
	{
		foreach(KeyValuePair<string, bool> kvp in m_characterIsSpeaking)
		{
			if(kvp.Value)
				return true;
		}
		
		return false;		
	}
	
	// Check if something is already speaking
	private bool IsSomebodyQuestion()
	{
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			if(kvp.Value.CurrentConversationStep == ConversationStep.QUESTION)
				return true;
		}
		
		return false;
	}
	
	private string WhoIsQuestioning()
	{
		foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
		{
			if(kvp.Value.CurrentConversationStep == ConversationStep.QUESTION)
				return kvp.Key;
		}
		
		return "";
	}
	
	private bool CanChangeCategory()
	{
		foreach(KeyValuePair<string, bool> kvp in m_characterHadSpoken)
		{
			if(kvp.Value)
				return false;
		}
		
		return true;	
	}
	
	// A character is speaking
	private IEnumerator Speak(string characterName)
	{	
		//print(characterName+": " + InteractionManager.Instance.CharacterList[characterName].CurrentConversationStep);
		if(!IsSomebodySpeaking() && !m_characterHadSpoken[characterName])
		{
			m_cursorConversation++;
			CheckCharacterMute(characterName);
			m_characterIsSpeaking[characterName] = true;
			yield return new WaitForSeconds(m_timeInteraction);
			if(InteractionManager.Instance.CharacterList.ContainsKey(characterName))
			{	
				DisplaySentence(characterName);
				m_characterIsSpeaking[characterName] = false;
				AllowOtherCharacterToSpeak(characterName);
				UpdateCharacterStep(characterName);
			}
		}
	}
	
	// Allow the other characters to speak
	private void AllowOtherCharacterToSpeak(string characterName)
	{
		Dictionary<string, bool> characterHasSpokenTmp = new Dictionary<string, bool>(m_characterHadSpoken);
		foreach(KeyValuePair<string, bool> kvp in characterHasSpokenTmp)
		{
			if(kvp.Key != characterName)
				m_characterHadSpoken[kvp.Key] = false;
			else
				m_characterHadSpoken[kvp.Key] = true;
		}
	}
	
	// Display the character sentence
	private void DisplaySentence(string characterName)
	{	
		// Emotion list
		XmlNodeList emotionNodeList = m_document.GetElementsByTagName("Emotion");
		foreach(XmlNode emotionNode in emotionNodeList)
		{
			// Select the proper emotion
			if(emotionNode.Attributes["name"].Value == InteractionManager.Instance.CharacterList[characterName].CurrentEmotion.ToString())
			{
				// Conversation step list
				foreach (XmlNode stepNode in emotionNode.ChildNodes)
				{
					// Select the proper step (Greeting, Goodbye, ...)
					if(stepNode.Attributes["name"].Value == InteractionManager.Instance.CharacterList[characterName].CurrentConversationStep.ToString())
					{
						// Random on the sentence avaible on a specific category
						int indexSentence = UnityEngine.Random.Range(0, stepNode.ChildNodes.Count);
						if(m_currentCategory == "" || m_cursorConversation >= InteractionManager.Instance.CharacterList.Count)
						{
							m_cursorConversation = 0;
							m_currentCategory = stepNode.ChildNodes[indexSentence].Attributes["Category"].Value;
						}
						else
						{
							while(stepNode.ChildNodes[indexSentence].Attributes["Category"].Value != m_currentCategory)
							{
								indexSentence = UnityEngine.Random.Range(0, stepNode.ChildNodes.Count);	
							}
						}
						
						string message = FinalizeSetence(characterName, stepNode.ChildNodes[indexSentence].InnerText); 
						GUIManager.Instance.DisplayMessage(characterName, message);
					}
				}
			}	
		}
	}
	
	// Replace the variable in the setence with proper values
	private string FinalizeSetence(string characterName, string sentence)
	{
		foreach(string variableName in m_variableNameList)
		{
			if(sentence.Contains(variableName))
			{
				sentence = sentence.Replace(variableName, GetVariable(characterName, variableName));	
			}
		}
		
		return sentence;
	}
	
	// Variable from XML
	private string GetVariable(string characterName, string variableName)
	{
		string variableValue = "";
		switch(variableName)
		{
			case "$[NAME]":
			foreach(KeyValuePair<string, Character> kvp in InteractionManager.Instance.CharacterList)
			{
				if(kvp.Key != characterName)
					variableValue += kvp.Key+",";
			}
			if(variableValue.Length > 0)
				variableValue = variableValue.Substring(0, variableValue.Length-1);
			break;
		}
		
		return variableValue;
	}
	
	// Remove a character from the conversation
	public void Remove(string characterName)
	{
		m_characterIsSpeaking.Remove(characterName);
		m_characterHadSpoken.Remove(characterName);
	}
}
