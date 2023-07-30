using UnityEngine;
using UnityEngine.Events;

public class ChampionAnimEvent : MonoBehaviour
{
	public UnityEvent onDoAttack = new UnityEvent();
	public UnityEvent onFireProjectile = new UnityEvent();
	[Space]
	public UnityEvent onStartAttackAnim = new UnityEvent();
	public UnityEvent onEndAttackAnim = new UnityEvent();

	public void DoAttack()
	{
		onDoAttack.Invoke();
	}

	public void FireProjectile()
	{
		onFireProjectile.Invoke();
	}

	public void OnStartAttackAnimation() => onStartAttackAnim.Invoke();
	public void OnEndAttackAnimation() => onEndAttackAnim.Invoke();
}
