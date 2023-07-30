using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class PerformanceStatisticObjShortcutCreate
{
    // Load prefab at (Assets/AVT/PerformanceStatistics/Resources/AVT Performance Statistics)
    const string prefabPath = "AVT Performance Statistics";
    const string objName = "Performance Statistics";

    [MenuItem("GameObject/AVT/Performance Statistics", false, 10)]
    static void CreatePerformanceStatistics(MenuCommand menuCommand)
    {
        var prefab = Resources.Load<GameObject>(prefabPath);
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        go.name = objName;

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
#endif
