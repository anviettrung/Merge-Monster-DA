using UnityEngine;

public abstract class MergeSlot<T> : MonoBehaviour where T : MonoBehaviour
{
	[SerializeField] private T unit;
	[SerializeField] protected Collider coll;

	public bool HasUnit => unit != null;

	public T Unit
	{
		get => unit;
		set
		{
			unit = value;
			OnUnitChange();
		}
	}

	public void OnValidate()
	{
		if (coll == null)
			coll = GetComponent<Collider>();
	}

	public void SwapUnitWith(MergeSlot<T> otherSlot)
	{
		T temp = Unit;
		Unit = otherSlot.Unit;
		otherSlot.Unit = temp;
	}

	public void Start()
	{
		Init();
		OnUnitChange();
	}

	public virtual void OnUnitChange()
	{
		if (unit != null)
		{
			unit.transform.position = transform.position;
		}
		coll.enabled = HasUnit;
	}

	public virtual void Init()
    {

    }
}
