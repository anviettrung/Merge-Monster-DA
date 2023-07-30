#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


public class GenericEditor<T> : Editor where T : Object
{
	protected T mTarget;

	protected virtual void Awake()
	{
		mTarget = (T)target;
	}
}

#endif
