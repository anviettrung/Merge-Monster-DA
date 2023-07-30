using UnityEngine;

public class DealDamageSingleTarget : MonoBehaviour, IDealDamage
{
	public void DealDamage(Champion target, int damage)
	{
		if (target.state != Champion.State.DEAD)
		{
			target.SufferDamage(damage);
		}
	}
}
