using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable()]
public class TransformationInfo : ISerializable
{
	public bool Show = true;
	public SpriteAnimation.TransformationType Transformation;
	public EZAnimation.EASING_TYPE NormalEasing;
	public EZAnimation.EASING_TYPE ReverseEasing;
	public List<TransformationKeyInfo> TransformationKeyList = null;
	
	public TransformationInfo(SpriteAnimation.TransformationType transformationType)
	{
		Transformation = transformationType;
		NormalEasing = EZAnimation.EASING_TYPE.Linear;
		ReverseEasing = EZAnimation.EASING_TYPE.Linear;
		TransformationKeyList = new List<TransformationKeyInfo>();
	}
	
	public TransformationInfo(SerializationInfo info, StreamingContext ctxt)
	{
		Show = (bool)info.GetValue("Show", typeof(bool));
		Transformation = (SpriteAnimation.TransformationType)info.GetValue("Transformation", typeof(SpriteAnimation.TransformationType));
		NormalEasing = (EZAnimation.EASING_TYPE)info.GetValue("NormalEasing", typeof(EZAnimation.EASING_TYPE));
		ReverseEasing = (EZAnimation.EASING_TYPE)info.GetValue("ReverseEasing", typeof(EZAnimation.EASING_TYPE));
		TransformationKeyList = (List<TransformationKeyInfo>)info.GetValue("AnimationKeyList", typeof(List<TransformationKeyInfo>));
	}
		
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{	
		info.AddValue("Show", Show);
		info.AddValue("Transformation", Transformation);
		info.AddValue("NormalEasing", NormalEasing);
		info.AddValue("ReverseEasing", ReverseEasing);
		info.AddValue("AnimationKeyList", TransformationKeyList);
	}
}
