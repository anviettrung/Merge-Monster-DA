using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageAreaOfEffect : MonoBehaviour, IDealDamage
{
	//public ThinkingPlaceable.AttackType attackType = ThinkingPlaceable.AttackType.Ground;
	public float radius;

	public void DealDamage(Champion target, int damage)
	{
	//	var targetList = GameManager.Instance.GetAttackListForAttackType(
	//		attackType, 
	//		GameManager.Instance.GetAttackListSameTeamAs(target.faction, Placeable.PlaceableTarget.Both));


	//	float sqrRadius = radius * radius;

	//	for (int tN = 0; tN < targetList.Count; tN++)
	//	{
	//		ThinkingPlaceable t = targetList[tN];

	//		if (t.state != ThinkingPlaceable.States.Dead) 
	//		{
	//			if ((target.transform.position - t.transform.position).sqrMagnitude < sqrRadius)
	//			{
	//				t.SufferDamage(damage);
	//			}
	//		}
	//	}
	}
}
