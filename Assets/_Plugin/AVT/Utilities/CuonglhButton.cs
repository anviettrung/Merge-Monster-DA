using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class CuonglhButton : MonoBehaviour
{
    [ButtonEditor]
    public void TestButton()
    {
        Debug.Log("Execute");
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class ButtonEditor : Attribute { }

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class CuonglhButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var mTarget = (MonoBehaviour) target;
        var methodInfors = mTarget.GetType().GetMethods();
        foreach (var methodInfor in methodInfors)
        {
            var buttonEditor = methodInfor.GetCustomAttribute(typeof(ButtonEditor), true);
            if (buttonEditor != null)
            {
                if (GUILayout.Button(methodInfor.Name))
                {
                    methodInfor.Invoke((object)mTarget, null);
                }
            }
        }
    }
}

#endif
