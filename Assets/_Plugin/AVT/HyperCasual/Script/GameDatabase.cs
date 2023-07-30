using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameDatabase : SingletonScriptableObject<GameDatabase>
{
	[Header("Game Level Data")]
	public List<string> glds;
	public List<BonusLevelData> bonusLevels;

	[Header("Champions")]
	public ChampionOrigin[] championOrigins;

	[Header("UI")]
	public Color playerHPColor;
	public Color enemyHPColor;


	[Header("Sound")]
	public AudioClip winSound;
	public AudioClip loseSound;

	[Header("Environment")]
	public List<EnvironmentData> environments;

	//    [Header("Other")]

}

[System.Serializable]
public struct BonusLevelData
{
	public int gameLevel;
	public MergeDraw prefab;
}