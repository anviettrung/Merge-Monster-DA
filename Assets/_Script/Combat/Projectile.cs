using UnityEngine;

public class Projectile : MonoBehaviour
{

	public bool fixedFlyTime = false;
	public float flyTime = 1;

	[HideInInspector] public Champion target;
	[HideInInspector] public int damage;
	[HideInInspector] public float speed = 3f;

	public GameObject model;
	public ParticleSystem trailVFX;
	public ParticleSystem impactVFX;

	public Vector3 offset = new Vector3(0f, 1.2f, 0f);
	public float diposingTime = 0.5f;

	private IDealDamage dealDamage;
	private ITrajectory trajectory;
	private float progress = 0f;

	[HideInInspector] public Vector3 destination;
	private bool diposing = false;
	[HideInInspector] public Vector3 startPos;
	private float Dist => (startPos - destination).magnitude;

	private void Awake()
	{
		if (dealDamage == null)
			dealDamage = GetComponent<IDealDamage>();
		if (trajectory == null)
		{
			trajectory = GetComponent<ITrajectory>();
		}	
	}

	void OnEnable()
	{
		if (trailVFX != null)
			trailVFX.Play();

		if (model != null)
			model.SetActive(true);
	}

	public void Start()
	{
		transform.SetParent(null);
	}

	public void Init(Champion target, ChampionData cData)
	{
		damage = cData.damage;
		speed = cData.projectileMoveSpeed;
		this.target = target;
		destination = target.transform.position + offset;
		startPos = transform.position;
		progress = 0;

		trajectory?.Init(this);
	}

	public void DoDamage()
	{
		if (dealDamage != null)
			dealDamage.DealDamage(target, damage);
	}

	public void Dispose()
	{
		diposing = true;

		if (trailVFX != null)
			trailVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);

		if (impactVFX != null)
			impactVFX.Play();

		if (model != null)
			model.SetActive(false);

		DG.Tweening.DOVirtual.DelayedCall(diposingTime, () =>
		{
			gameObject.SetActive(false);
			diposing = false;
		});
	}

	public float Move()
	{
		if (diposing)
			return 0;

		if (target.state != Champion.State.DEAD)
			destination = target.transform.position + offset;

		progress = fixedFlyTime ? progress + Time.deltaTime / flyTime : Mathf.Min(progress + speed / Dist * Time.deltaTime, 1);

		if (trajectory != null)
			trajectory.DoMove(startPos, destination, progress);

		return progress;
	}
}