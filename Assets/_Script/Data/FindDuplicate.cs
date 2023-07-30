using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindDuplicate : MonoBehaviour
{
	public GameDatabase gameDB;

	Dictionary<string, List<int>> dup = new Dictionary<string, List<int>>();

	[ButtonEditor]
	public void LogDuplicate()
	{
		int i = 0;
		foreach (var lv in gameDB.glds)
		{
			if (dup.ContainsKey(lv))
			{
				dup[lv].Add(i);
			} else
			{
				dup.Add(lv, new List<int>());
				dup[lv].Add(i);
			}

			i++;
		}

		foreach (var p in dup)
		{
			if (p.Value.Count > 1)
			{
				Debug.Log(p.Key);
				for (int k = 0; k < p.Value.Count; k++)
					Debug.Log(p.Value[k]);
			}
		}

		Debug.Log("Number of Unique Level " + dup.Count);
	}

	[ButtonEditor]
	public void EstimatePower()
	{
		int lvl = 1;
		foreach (var pos in gameDB.glds)
		{
			int totalBuyMelee = 0;
			int totalBuyRanger = 0;

			for (int i = 0; i + 2 < pos.Length; i += 3)
			{
				int champLvl = int.Parse(pos.Substring(i + 1, 2));
				int champOriginID = (int)(pos[i] - '0');
				
				switch (champOriginID)
				{
					case 0: // Melee
						totalBuyMelee += champLvl > 0 ? (int)Mathf.Pow(2, champLvl - 1) : 0;
						break;
					case 1: // Ranger
						totalBuyRanger += champLvl > 0 ? (int)Mathf.Pow(2, champLvl - 1) : 0;
						break;
				}
			}

			Debug.LogFormat("Level {0}: {1}\nMelee: {2} - Ranger: {3}", lvl, totalBuyMelee + totalBuyRanger, totalBuyMelee, totalBuyRanger);

			lvl++;
		}
	}
}
