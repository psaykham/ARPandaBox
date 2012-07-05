using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;

[Serializable()]
public class TransformationKeyInfo : ISerializable
{	
	public bool Show = true;
	public bool IsRelative = true;
	public Vector3 From = Vector3.zero;
	public Vector3 To =  Vector3.zero;
	public Color FromColor = Color.white;
	public Color ToColor = Color.white;
	public float Duration = 1f;
	
	public TransformationKeyInfo()
	{
	}
	
	public TransformationKeyInfo(SerializationInfo info, StreamingContext ctxt)
	{
		Show = (bool)info.GetValue("Show", typeof(bool));
		IsRelative = (bool)info.GetValue("IsRelative", typeof(bool));
		From = (Vector3)info.GetValue("From", typeof(Vector3));
		To = (Vector3)info.GetValue("To", typeof(Vector3));
		FromColor = (Color)info.GetValue("FromColor", typeof(Color));
		ToColor = (Color)info.GetValue("ToColor", typeof(Color));
		Duration = (float)info.GetValue("Duration", typeof(float));
	}
		
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{	
		info.AddValue("Show", Show);
		info.AddValue("IsRelative", IsRelative);
		info.AddValue("From", From);
		info.AddValue("To", To);
		info.AddValue("FromColor", FromColor);
		info.AddValue("TColor", ToColor);
		info.AddValue("Duration", Duration);
	}
}
