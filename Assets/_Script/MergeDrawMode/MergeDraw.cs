using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class MergeDraw : MergeBase<Champion>
{
    public UnityEvent onStartDrawing = new UnityEvent();

    public int mergeMaxUnit = 5;
    public Champion defaultChamp;
    public bool isDrawLine = false;
    public LineRenderer drawLine;
    public float neighborDistance;
    protected LinkedList<MergeChampSlot> selectingSlots = new LinkedList<MergeChampSlot>();

    bool merging = false;

	public override bool CanMerge(Champion fromUnit, Champion toUnit)
	{
		return true;
	}

    public void DoMerge()
	{
        merging = true;

        int totalLevel = 0;
        Vector3 mergePos = Vector3.zero;

        for (var p = selectingSlots.First; p != null; p = p.Next)
		{
            mergePos += p.Value.transform.position;
            totalLevel += p.Value.Unit.Level;
		}
        mergePos /= selectingSlots.Count;

        for (var p = selectingSlots.First; p != null; p = p.Next)
        {
            p.Value.Unit.transform.DOMove(mergePos, 1).SetEase(Ease.InOutSine);
        }

        Champion upgradeUnit = null;

        DOVirtual.DelayedCall(1, () => 
        {
            Champion upgradeUnitPrefab = selectingSlots.First.Value.Unit.soData.origin.upgradeTree[Mathf.Clamp(totalLevel-1, 0, 10)].prefab;
            upgradeUnit = Instantiate(upgradeUnitPrefab, transform);
            MergeChampSlot firstSlot = selectingSlots.First.Value;

            for (var p = selectingSlots.First; p != null; p = p.Next)
			{
                Destroy(p.Value.Unit.gameObject);
                p.Value.Unit = null;
			}

            RemoveAllFromList();

            firstSlot.Unit = upgradeUnit;
            upgradeUnit.transform.position = mergePos;
            upgradeUnit.transform.localScale = Vector3.one * 0.25f;
            upgradeUnit.SetActiveLevelLabel(true);
            

            upgradeUnit.transform.DOScale(1, 0.5f).SetEase(Ease.OutSine);
        });

        DOVirtual.DelayedCall(1.6f, () =>
        {
            onMergeDone.Invoke(upgradeUnit);
            upgradeUnit.SetActiveHPBar(true);

            //merging = false;
        });
    }

	protected override void Update()
	{
        if (merging)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            MergeChampSlot hovering = GetHoveringSlot() as MergeChampSlot;
            if (hovering != null)
			{
                onStartDrawing.Invoke();
                AddSlotToList(hovering);
            } 
        }
        else if (Input.GetMouseButtonUp(0) && selectingSlots.Count > 0)
        {
            if (selectingSlots.Count > 1)
                DoMerge();
            else
			{
                selectingSlots.First.Value.Unit.SetActiveOutline(false);
                selectingSlots.Clear();
			}
            drawLine.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0) && selectingSlots.Count > 0)
        {
            MergeChampSlot hovering = GetHoveringSlot() as MergeChampSlot;
            if (hovering != null)
            {
                UpdateSlotInList(hovering);        
            }
        }
    }

    void UpdateSlotInList(MergeChampSlot slot)
	{
        if (slot == selectingSlots.First.Value)
		{

		}
        else  if (selectingSlots.Contains(slot))
		{
            while (selectingSlots.First.Value != slot)
                RemoveSlotFromList();
		}
		else if (IsNeighborSlot(selectingSlots.First.Value, slot))
		{
            AddSlotToList(slot);
		}

        drawLine.gameObject.SetActive(selectingSlots.Count > 1 && isDrawLine);
	}

    void AddSlotToList(MergeChampSlot slot)
	{
        slot.Unit.SetActiveOutline(true);
        drawLine.positionCount = selectingSlots.Count + 1;
        drawLine.SetPosition(selectingSlots.Count, slot.transform.position + draggingOffset);
        selectingSlots.AddFirst(slot);
    }

    void RemoveSlotFromList()
	{
        selectingSlots.First.Value.Unit?.SetActiveOutline(false);
        selectingSlots.RemoveFirst();

        drawLine.positionCount = selectingSlots.Count;
    }

    void RemoveAllFromList()
	{
        while (selectingSlots.Count > 0)
            RemoveSlotFromList();
	}

    bool IsNeighborSlot<T>(MergeSlot<T> a, MergeSlot<T> b) where T : MonoBehaviour
	{
        return (a.transform.position - b.transform.position).sqrMagnitude <= neighborDistance * neighborDistance;
	}

	private void OnDrawGizmos()
	{
        for (int i = 0; i < slots.Count - 1; i++)
            for (int j = i+1; j < slots.Count; j++)
			{
                if (!slots[i].gameObject.activeInHierarchy || !slots[j].gameObject.activeInHierarchy)
                    continue;

                Gizmos.color = Color.red;
                if (IsNeighborSlot(slots[i], slots[j]))
                    Gizmos.DrawLine(slots[i].transform.position, slots[j].transform.position);
            }

    }
}
