using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SFXGroupObjectShortcutCreate
{
    // Load prefab at (Assets/AVT/SFXGroup/Resources/SFX Group)
    const string prefabPath = "SFX Group";
    const string objName = "SFX Group";

    [MenuItem("GameObject/AVT/SFX Group", false, 10)]
    static void Create(MenuCommand menuCommand)
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
