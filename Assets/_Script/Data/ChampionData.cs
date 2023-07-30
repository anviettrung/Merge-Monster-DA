using UnityEngine;

[CreateAssetMenu]
public class ChampionData : ScriptableObject
{
	public string display_name;
	public Sprite icon;
	public int stars;
	public Champion prefab;

	[Space]

	public int damage = 10;
	public int hp = 100;
	public float atkRange = 1;
	public float atkSpeed = 100;
	public float atkCooldown => 100.0f / atkSpeed;
	public float moveSpeed = 2;
	public float projectileMoveSpeed = 1.5f;

	[Space]

	public ChampionOrigin origin;
	public ChampionData prevUpgrade;
	public ChampionData nextUpgrade;
}
