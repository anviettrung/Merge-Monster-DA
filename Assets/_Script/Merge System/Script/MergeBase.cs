using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class MergeBase<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected LayerMask mergeSlotLayer;
    public Vector3 draggingOffset = Vector3.zero;

    [Header("Ref")]
    [SerializeField] protected  Collider plane;
    public List<MergeSlot<T>> slots;

    protected MergeSlot<T> draggingUnitFromSlot = null;
    protected T draggingUnit = null;

    [Header("Event")]
    public UnityEvent<T> onMergeDone = new UnityEvent<T>();
    public UnityEvent onBoardChange = new UnityEvent();

    #region Actions

    public abstract bool CanMerge(T fromUnit, T toUnit);
    public virtual void DoMerge(MergeSlot<T> fromSlot, MergeSlot<T> toSlot)
	{
        // Do After Merge
        if (fromSlot.Unit != null)
        {
            fromSlot.Unit = null;
        }

        onMergeDone.Invoke(toSlot.Unit);
	}

    #endregion

    #region Virtual Event

    protected virtual void OnStartDragging(T target) { }
    protected virtual void OnDragging(T target) { }
    protected virtual void OnEndDragging(MergeSlot<T> targetSlot) { onBoardChange.Invoke(); }

	#endregion

	#region Unity Callback

	protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            draggingUnit = GetHoveringUnit();
            if (draggingUnit != null)
                OnStartDragging(draggingUnit);
        } 
        else if (Input.GetMouseButtonUp(0) && draggingUnitFromSlot != null)
        {
            MergeSlot<T> targetSlot = GetNearestSlot();

            if (targetSlot.Unit != null && targetSlot != draggingUnitFromSlot)
            {
                if (CanMerge(draggingUnit, targetSlot.Unit))
                {
                    DoMerge(draggingUnitFromSlot, targetSlot);
                }
                else
				{
                    draggingUnitFromSlot.SwapUnitWith(targetSlot);
                }
            }
            else
            {
                draggingUnitFromSlot.SwapUnitWith(targetSlot);
            }

            draggingUnitFromSlot = null;
            draggingUnit = null;

            OnEndDragging(targetSlot);
        }

        if (Input.GetMouseButton(0) && draggingUnit != null)
        {
            DragObject(draggingUnit.transform);
            OnDragging(draggingUnit);
        }
    }

    #endregion

    #region Control Methods
    protected Ray RayFromCamera => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    public static readonly int[] slotOrderUp = { 2, 1, 3, 0, 4, 7, 6, 8, 5, 9, 12, 11, 13, 10, 14 };
    public static readonly int[] slotOrderDown = { 12, 11, 13, 10, 14, 7, 6, 8, 5, 9, 2, 1, 3, 0, 4 };


    public MergeSlot<T> GetFirstEmptySlot()
	{
        for (int i = 0; i < slotOrderUp.Length; i++)
		{
            if (slots[slotOrderUp[i]].HasUnit == false)
			{
                return slots[slotOrderUp[i]];
			}
		}

        return null;
	}

    public MergeSlot<T> GetLastEmptySlot()
	{
        for (int i = 0; i < slotOrderDown.Length; i++)
        {
            if (slots[slotOrderDown[i]].HasUnit == false)
            {
                return slots[slotOrderDown[i]];
            }
        }

        return null;
    }

    public MergeSlot<T> GetNearestSlot()
	{
        if (draggingUnit != null)
		{
            MergeSlot<T> res = slots[0];
            float minDist = (draggingUnit.transform.position - slots[0].transform.position).sqrMagnitude;

            for (int i = 1; i < slots.Count; i++)
			{
                float dist = (draggingUnit.transform.position - slots[i].transform.position).sqrMagnitude;
                if (dist < minDist)
				{
                    minDist = dist;
                    res = slots[i];
				}
            }

            return res;
		}

        return null;
	}

    protected T GetHoveringUnit()
	{
        if (IsPointerOverUIObject())
            return null;

        draggingUnitFromSlot = GetHoveringSlot();

        if (draggingUnitFromSlot != null) 
			return draggingUnitFromSlot.Unit;

		return null;
	}

    public MergeSlot<T> GetHoveringSlot()
	{
        if (IsPointerOverUIObject())
            return null;

        MergeSlot<T> res = null;
        if (Physics.Raycast(RayFromCamera, out RaycastHit hit, Mathf.Infinity, mergeSlotLayer))
        {
            res = hit.transform.GetComponent<MergeSlot<T>>();
            if (slots.Contains(res) == false)
                res = null;
        }

        return res;
    }

    protected void DragObject(Transform target)
	{
        //if (IsPointerOverUIObject())
        //    return;

        if (plane.Raycast(RayFromCamera, out RaycastHit hit, 100))
		{
			target.transform.position = hit.point + draggingOffset;
		}
	}

    public int GetEmptySlotCount()
	{
        int count = 0;

        for (int i = 0; i < slots.Count; i++)
            if (slots[i].HasUnit == false)
                count++;

        return count;
	}

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    #endregion
}
