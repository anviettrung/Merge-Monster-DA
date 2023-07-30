using UnityEngine;

namespace AVT
{
	[System.Serializable]
	public partial class GameSaveFile
	{
		[SerializeField] private int curGameLevel = 0;
		[SerializeField] protected long gold = 0;

		public int CurrentGameLevel { get => curGameLevel; set => curGameLevel = value; }
	}
}
