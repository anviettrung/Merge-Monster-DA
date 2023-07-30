using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class ChampionOrigin : ScriptableObject
{
	public string uid;
	public string unique_name;
	public string display_name;

	[Space]

	public ChampionData init_champion;
	public List<ChampionData> upgradeTree;

#if UNITY_EDITOR

	[MenuItem("CONTEXT/ChampionOrigin/Build Upgrade Tree")]
	public static void BuildChampUpgradeTreeBaseOnInitChampion(MenuCommand cmd)
	{
		ChampionOrigin mTarget = (ChampionOrigin)cmd.context;

		if (mTarget.upgradeTree == null)
			mTarget.upgradeTree = new List<ChampionData>();

		mTarget.upgradeTree?.Clear();
		ChampionData pChamp = mTarget.init_champion;
		do
		{
			mTarget.upgradeTree.Add(pChamp);
			pChamp = pChamp.nextUpgrade;
		} while (pChamp != null);

		EditorUtility.SetDirty(mTarget);

		Debug.Log("Build tree success");
	}

	[MenuItem("CONTEXT/ChampionOrigin/Reverse Build Upgrade Tree")]
	public static void ReverseBuildChampUpgradeTreeBaseOnInitChampion(MenuCommand cmd)
	{
		ChampionOrigin mTarget = (ChampionOrigin)cmd.context;

		if (mTarget.upgradeTree == null)
			return;

		for (int i = 0; i < mTarget.upgradeTree.Count; i++)
		{
			mTarget.upgradeTree[i].prevUpgrade = i == 0 ? null : mTarget.upgradeTree[i - 1];
			mTarget.upgradeTree[i].nextUpgrade = i == mTarget.upgradeTree.Count - 1 ? null : mTarget.upgradeTree[i + 1];
		}

		EditorUtility.SetDirty(mTarget);

		Debug.Log("Reverse Build tree success");
	}

#endif
}
