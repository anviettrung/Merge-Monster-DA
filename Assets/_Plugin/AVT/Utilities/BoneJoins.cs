using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneJoins : MonoBehaviour
{
    public GameObject objPlayer;

	public List<GameObject> AddLimb(GameObject BoneObj) => AddLimb(BoneObj, objPlayer);

    public List<GameObject> AddLimb(GameObject BoneObj, GameObject RootObj)
    {
        List<GameObject> newBonedObjs = new List<GameObject>();

        var BonedObjects = BoneObj.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach ( SkinnedMeshRenderer SkinnedRenderer in BonedObjects )
            newBonedObjs.Add(ProcessBonedObject(SkinnedRenderer, RootObj));

        return newBonedObjs;
    }

    private GameObject ProcessBonedObject(SkinnedMeshRenderer ThisRenderer, GameObject RootObj)
    {
        // Create the SubObject
        var NewObj = new GameObject(ThisRenderer.gameObject.name);
        NewObj.transform.parent = RootObj.transform;

        // Add the renderer
        var NewRenderer = NewObj.AddComponent<SkinnedMeshRenderer>();

        /*      Assemble Bone Structure     */
        var MyBones = new Transform[ThisRenderer.bones.Length];
        for ( var i = 0; i < ThisRenderer.bones.Length; i++ )
            MyBones[i] = FindChildByName(ThisRenderer.bones[i].name, RootObj.transform);
        /*      Assemble Renderer       */
        NewRenderer.bones = MyBones;
        NewRenderer.sharedMesh = ThisRenderer.sharedMesh;
        NewRenderer.materials = ThisRenderer.sharedMaterials;

        return NewObj;
    }

    private Transform FindChildByName(string ThisName, Transform ThisGObj)
    {
        Transform ReturnObj;
        if ( ThisGObj.name == ThisName )
            return ThisGObj.transform;
        foreach ( Transform child in ThisGObj )
        {
            ReturnObj = FindChildByName(ThisName, child);
            if ( ReturnObj )
                return ReturnObj;
        }
        return null;
    }
}