using UnityEngine;
using UnityEngine.Events;
using AVT;

public class PlayerSave
{
	public const string has_merged_champ = "has_merged_{0}";
	public const string init_board = "000000001000000000000000000000000000000000000"; // has one huggy lvl 1

	public static UnityEvent<long> onGoldChange = new UnityEvent<long>();

	public static bool HasMergedChamp(ChampionData _data) => PlayerPrefs.GetInt(string.Format(has_merged_champ, _data.name), 0) == 1;
	public static void SetHasMergedChamp(ChampionData _data, int value) => PlayerPrefs.SetInt(string.Format(has_merged_champ, _data.name), value);

	public static void SavePlayerBoard(string s) => SaveLoad.SaveFile.PlayerBoard = s;
	public static string GetPlayerBoard() => SaveLoad.SaveFile.PlayerBoard;

	public static int GetHighestMeleeLevel()
	{
		int max = 1;

		string pos = GameDatabase.Instance.glds[SaveLoad.SaveFile.CurrentGameLevel];
		Debug.Log(pos);
		for (int i = 0; i + 2 < pos.Length; i += 3)
		{
			var champLvl = int.Parse(pos.Substring(i + 1, 2));
			if (champLvl > max && pos[i] == '0') // if type == melee and level > max
			{
				max = champLvl;
			}
		}

		return max;
	}

	public static void InitGold() => onGoldChange.Invoke(SaveLoad.SaveFile.Gold);
}


namespace AVT
{
	public partial class GameSaveFile
	{
		[SerializeField] private int boughtHuggy = 0;
		[SerializeField] private int boughtGlamrock = 0;
		[SerializeField] private string player_board;

		public int BoughtHuggy { get => boughtHuggy; set => boughtHuggy = value; }
		public int BoughtGlamrock { get => boughtGlamrock; set => boughtGlamrock = value; }
		public string PlayerBoard { get => player_board; set => player_board = value; }

		public long Gold { 
			get => gold; 
			set
			{
				gold = value;
				PlayerSave.onGoldChange.Invoke(gold);
			}
		}

		// Set default value for all properties in player data
		void InitBeforeLoad()
		{
			CurrentGameLevel = 0;
			Gold = 0;

			BoughtHuggy = 1;
			BoughtGlamrock = 1;

			PlayerBoard = PlayerSave.init_board;

			for (int i = 0; i < GameDatabase.Instance.championOrigins.Length; i++)
				PlayerSave.SetHasMergedChamp(GameDatabase.Instance.championOrigins[i].init_champion, 1);
		}

		// Fix & add data in player data
		// Eg: ensure list of skins always have default skin id=0
		void InitAfterLoad()
		{
		}
	}
}
