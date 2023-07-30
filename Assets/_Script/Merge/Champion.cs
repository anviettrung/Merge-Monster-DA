using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using AVT;
using TMPro;

[SelectionBase]
public class Champion : MonoBehaviour
{
	public enum State
	{
		DISABLE, // do nothing
		IDLE,
		SEEKING, // going for the target
		ATTACKING, // attack cycle animation, not moving
		DEAD // Dead animation, before removal from play field
	}

	const int anim_attack_count = 3;
	const string anim_attack = "attack";
	const string anim_attack_style = "attackStyle";
	const string anim_is_moving = "isMoving";
	const string anim_victory = "victory";
	const string anim_dead = "dead";
	const string anim_drag = "drag";
	const string anim_land = "land";

	public ChampionData soData;

	[Header("Main Stat")]
	public bool logAttack = false;
	public int currHP;
	public int teamID;

	[Header("Visual")]
	[SerializeField] Animator animator;
	[SerializeField] ChampionAnimEvent animEvent;
	[SerializeField] ToonOutlineControl outline;
	[SerializeField] Slider hpBar;
	[SerializeField] Image hpBarFill;
	[SerializeField] TextMeshProUGUI levelLabel;
	[SerializeField] GameObject weaponOnBack;
	[SerializeField] GameObject weaponOnHand;

	[Header("Combat")]
	public bool doFacingTarget = true;
	public bool enableColliderBodyOnActive = true;
	public CharacterController controller; 
	public Champion target = null;
	public State state = State.IDLE;
	public float lastBlowTime;
	public Projectile projectilePrefab;
	public Transform projectileSpawnPoint;
	public List<Projectile> projectilePool = new List<Projectile>();

	public UnityEvent<Champion> onDoAttack = new UnityEvent<Champion>();
	public UnityEvent<Champion> onFireProjectile = new UnityEvent<Champion>();
	public UnityEvent<Champion, int> onSufferDamage = new UnityEvent<Champion, int>();
	[HideInInspector] public UnityEvent<Champion> onActivate = new UnityEvent<Champion>();

	private int attackStyle = 0;

	[Header("Event")]
	public UnityEvent<Champion> onDead = new UnityEvent<Champion>();

	public int Level 
	{
		get
		{
			int res = 1;
			ChampionData pChamp = soData.origin.init_champion;
			while (pChamp != soData && pChamp.nextUpgrade != null)
			{
				pChamp = pChamp.nextUpgrade;
				res++;
			}

			return res;
		}
	}
	public bool CanUpgrade() => soData.nextUpgrade == null;

	public float HPRatio => (float)currHP / soData.hp;

	private void OnValidate()
	{
		if (animator == null)
			animator = GetComponentInChildren<Animator>();

		if (animator != null && animEvent == null)
			animEvent = animator.GetComponent<ChampionAnimEvent>();
	}

	private void Start()
	{
		// Init
		currHP = soData.hp;
		SetActiveHPBar(false);
		SetLevelLabel(Level);

		animEvent.onDoAttack.AddListener(DoAttack);
		animEvent.onFireProjectile.AddListener(FireProjectile);

		// pre instantiate project
		if (projectilePrefab != null)
		{
			int preCount = Mathf.CeilToInt((projectilePrefab.diposingTime + 1.1f) / soData.atkCooldown);
			for (int i = 0; i < preCount; i++)
			{
				var clone = Instantiate(projectilePrefab, this.transform);
				clone.gameObject.SetActive(false);
				projectilePool.Add(clone);
			}
		}

		SetWeaponOnBack();
	}

	public void Activate()
	{
		state = State.IDLE;

		SetHPBarColor(teamID);

		controller.enabled = enableColliderBodyOnActive;

		SetWeaponOnHand();

		onActivate.Invoke(this);
	}

	#region Combat

	public void Stop()
	{
		state = State.IDLE;

		animator.SetBool(anim_is_moving, false);
	}

	public void SetTarget(Champion t)
	{
		target = t;
		t.onDead.AddListener(TargetIsDead);
	}

	protected void TargetIsDead(Champion p)
	{
		state = State.IDLE;

		if (target != null)
			target.onDead.RemoveListener(TargetIsDead);
		target = null;

		//timeToActNext = lastBlowTime + attackRatio;
	}

	public void Seek()
	{
		state = State.SEEKING;

		animator.SetBool(anim_is_moving, true);
	}

	public void StartAttack()
	{
		state = State.ATTACKING;

		animator.SetBool(anim_is_moving, false);
	}

	public void DoMove()
	{
		if (target != null)
		{ 
			controller.Move((target.transform.position - transform.position).normalized * Time.deltaTime * soData.moveSpeed);
		}
		//	transform.position += (target.transform.position - transform.position).normalized * Time.deltaTime * soData.moveSpeed;
	}

	public void DoFacing()
	{
		if (target != null && doFacingTarget)
			transform.LookAtDirection(target.transform.position - transform.position, 5);
	}

	public void DealBlow()
	{
		lastBlowTime = Time.time;

		animator.SetFloat(anim_attack_style, attackStyle++ % anim_attack_count);
		animator.SetTrigger(anim_attack);
	}

	//Animation event hooks
	public void DoAttack()
	{
		if (onDoAttack != null)
		{
			if (projectilePrefab != null && IsTargetInRange())
			{
				var clone = GetProjectileFromPool();
				clone.transform.position = target.transform.position;
				clone.Dispose();
			}

			onDoAttack.Invoke(this);
		}
	}
	public void FireProjectile()
	{
		if (onFireProjectile != null)
			onFireProjectile.Invoke(this);
	}

	public void DanceVictory()
	{
		if (state != State.DEAD)
		{
			transform.eulerAngles = Vector3.up * 180;
			animator.SetBool(anim_victory, true);
			SetWeaponOnBack();
		}
	}

	public void Die()
	{
		state = State.DEAD;

		controller.enabled = false;
		animator.SetTrigger(anim_dead);

		SetActiveHPBar(false);
		SetActiveLevelLabel(false);

		onDead.Invoke(this);
	}

	public float SufferDamage(int amount)
	{
		currHP -= amount;

		if (state != State.DEAD && currHP <= 0f)
		{
			Die();
		}

		// UI
		if (hpBar != null)
			hpBar.value = HPRatio;

		onSufferDamage.Invoke(this, amount);

		return currHP;
	}

	public void FindTargetInList(List<Champion> opponents)
	{
		float minDist = Mathf.Infinity;
		target = null;

		for (int i = 0; i < opponents.Count; i++)
		{
			if (opponents[i].state == State.DEAD)
				continue;

			float dist = (transform.position - opponents[i].transform.position).sqrMagnitude;
			if (dist < minDist)
			{
				minDist = dist;
				SetTarget(opponents[i]);
			}
		}
	}

	public bool IsTargetInRange()
	{
		if (target != null)
			return (transform.position - target.transform.position).sqrMagnitude <= soData.atkRange * soData.atkRange;
		else 
			return false;
	}

	public Projectile GetProjectileFromPool()
	{
		for (int i = 0; i < projectilePool.Count; i++)
		{
			if (projectilePool[i].gameObject.activeInHierarchy == false)
			{
				projectilePool[i].gameObject.SetActive(true);
				return projectilePool[i];
			}
		}
		return null;
	}

	#endregion

	#region Visual

	public void SetWeaponOnBack()
	{
		if (weaponOnBack != null)
		{
			weaponOnBack.SetActive(true);

			if (weaponOnHand != null)
				weaponOnHand.SetActive(false);
		}	
	}

	public void SetWeaponOnHand()
	{
		if (weaponOnHand != null)
		{
			weaponOnHand.SetActive(true);

			if (weaponOnBack != null)
				weaponOnBack.SetActive(false);
		}
	}

	public void DoDrag(bool status) => animator.SetBool(anim_drag, status);
	public void DoLand() => animator.SetTrigger(anim_land);

	public void SetActiveOutline(bool status) => outline.OverrideOutline(status);

	public void SetHPBarColor(int teamID)
	{
		hpBarFill.color = teamID == 0 ? GameDatabase.Instance.playerHPColor : GameDatabase.Instance.enemyHPColor;
	}

	public void SetActiveHPBar(bool status) => hpBar.gameObject.SetActive(status);

	public void SetActiveLevelLabel(bool status) => levelLabel.gameObject.SetActive(status);
	public void SetLevelLabel(int x) => levelLabel.text = x.ToString();

	#endregion

	#region Overloading Equal
	public static bool IsSameOriginAndLevel(Champion a, Champion b) => a.soData == b.soData;
	#endregion

	#region Editor
#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		if (soData == null)
			return;

		if (soData.atkRange < 30)
		{
			Color atkRangeSphereColor = IsTargetInRange() ? Color.green : Color.yellow;
			atkRangeSphereColor.a = 0.5f;

			Gizmos.color = atkRangeSphereColor;
			Gizmos.DrawSphere(transform.position, soData.atkRange);
		}
	}
#endif
	#endregion
}

#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(Champion))]
public class ChampionEditor : GenericEditor<Champion>
{
	private void OnSceneGUI()
	{
		if (mTarget.target != null)
		{
			Handles.color = Color.red;
			Handles.DrawLine(mTarget.transform.position, mTarget.target.transform.position, 10);
		}

	}
}

#endif
#endregion
