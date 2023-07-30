using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;




public class MapMakerTool : MonoBehaviour
{
	public bool selectLevelRealMode = false;
	public MergeChamp board;
	public UINumberSelect huggySelect;
	public UINumberSelect glamrockSelect;
	public UINumberSelect levelSelect;

	private int currentLevel = 0;

	public GameDatabase gameDB => GameDatabase.Instance;

	private void Start()
	{
		huggySelect.onSelectNumber.AddListener(SelectHuggy);
		glamrockSelect.onSelectNumber.AddListener(SelectGlamrock);
		if (selectLevelRealMode)
			levelSelect.onSelectNumber.AddListener(SelectLevelRealMode);
		else
			levelSelect.onSelectNumber.AddListener(SelectLevel);

		LoadLevelDatabase();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(1) && board.hoveringSlot != null)
			board.RemoveChampionAtSlot(board.hoveringSlot);

		if (Input.GetKeyDown(KeyCode.S))
		{
			SaveLevel();
			Debug.Log("Saved level " + (currentLevel+1));
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			AddNewLevel();
			Debug.Log("Add new level");
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			EstimatePowerLevel(currentLevel);
		}
	}

	public void LoadLevelDatabase()
	{
		levelSelect.maxIndex = gameDB.glds.Count;
	}

	public void AddNewLevel()
	{
		gameDB.glds.Add(string.Empty);
		LoadLevelDatabase();
	}

	public void SelectHuggy(int x)
	{
		board.AddChampionAtLastEmptySlot(gameDB.championOrigins[0].upgradeTree[x].prefab);
	}

	public void SelectGlamrock(int x)
	{
		board.AddChampionAtFirstEmptySlot(gameDB.championOrigins[1].upgradeTree[x].prefab);
	}

	public void SelectLevel(int x)
	{
		currentLevel = x;
		board.ClearAllChampion();
		board.DeserializePositioning(gameDB.glds[x]);
	}

	public void SelectLevelRealMode(int x)
	{
		GPExecutor.Instance.PlayLevel(x);
	}

	public void ClearAll()
	{
		board.ClearAllChampion();
	}

	public void SaveLevel()
	{
		gameDB.glds[currentLevel] = board.SerializePositioning();
#if UNITY_EDITOR
		EditorUtility.SetDirty(gameDB);
#endif
	}

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
			}
			else
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
	public void EstimatePowerAllLevel()
	{
		for (int lvl = 0; lvl < gameDB.glds.Count; lvl++)
			EstimatePowerLevel(lvl);
	}

	public void EstimatePowerLevel(int x)
	{
		string pos = gameDB.glds[x];
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

		Debug.LogFormat("Level {0}: {1}\nMelee: {2} - Ranger: {3}", x, totalBuyMelee + totalBuyRanger, totalBuyMelee, totalBuyRanger);
	}

	public int cloneNumber;

	[ButtonEditor]
	public void CloneLevel()
	{
		int startID = Mathf.Max(0, gameDB.glds.Count - cloneNumber);
		int endID = gameDB.glds.Count;

		for (int i = startID; i < endID; i++)
			gameDB.glds.Add(gameDB.glds[i]);

#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(gameDB);
#endif
	}
}