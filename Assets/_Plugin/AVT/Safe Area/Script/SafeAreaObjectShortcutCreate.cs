using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SafeAreaObjectShortcutCreate
{
    // Load prefab at (Assets/AVT/Safe Area/Resources/Responsive Canvas)
    const string prefabPath = "Responsive Canvas";
    const string objName = "[Canvas] Safe Area";

    [MenuItem("GameObject/AVT/Safe Area Canvas", false, 10)]
    static void CreateSafeAreaCanvas(MenuCommand menuCommand)
    {
        var prefab = Resources.Load<GameObject>(prefabPath);
        GameObject go = MonoBehaviour.Instantiate(prefab);
        go.name = objName;

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
#endif