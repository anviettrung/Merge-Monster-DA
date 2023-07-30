using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class MergeChamp : MergeBase<Champion>
{
	[Header("Visual")]
	public bool showLevelLabel = false;
	public bool showHPBar = true;
	public bool highlightOnHover = false;
	public bool playAppearVFX = true;
	public MMHapticShortcut hoveringHaptic;
	[HideInInspector] public MergeChampSlot hoveringSlot;

	#region Main Merge
	public override bool CanMerge(Champion fromUnit, Champion toUnit)
	{
		return Champion.IsSameOriginAndLevel(fromUnit, toUnit) && fromUnit.CanUpgrade() == false;
	}

	public override void DoMerge(MergeSlot<Champion> fromSlot, MergeSlot<Champion> toSlot)
	{
		Champion upgradeUnit = Instantiate(fromSlot.Unit.soData.nextUpgrade.prefab, transform);

		Destroy(fromSlot.Unit?.gameObject);
		Destroy(toSlot.Unit?.gameObject);

		toSlot.Unit = upgradeUnit;

		(toSlot as MergeChampSlot).PlayMergeVFX();
		MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
		toSlot.Unit.DoLand();
		toSlot.Unit.onActivate.AddListener((c) => c.SetActiveHPBar(showHPBar));

		base.DoMerge(fromSlot, toSlot);
	}

	protected override void OnStartDragging(Champion target)
	{
		base.OnStartDragging(target);

		target.DoDrag(true);
		target.SetActiveOutline(true);
		MMVibrationManager.Haptic(HapticTypes.Warning);
	}

	private MergeChampSlot lastHighlightSlot = null;
	protected override void OnDragging(Champion target)
	{
		base.OnDragging(target);

		MergeChampSlot hoveringSlot = GetNearestSlot() as MergeChampSlot;

		hoveringSlot.SetActiveHighlight(true);

		if (lastHighlightSlot != null && lastHighlightSlot != hoveringSlot)
		{
			lastHighlightSlot.SetActiveHighlight(false);

			// on hover different slot last frame
			if (hoveringSlot.Unit != null)
			{
				if (CanMerge(hoveringSlot.Unit, draggingUnit) && draggingUnitFromSlot != hoveringSlot)
					MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
				else
					hoveringHaptic?.PlayHaptic();
			}
			else
			{
				hoveringHaptic?.PlayHaptic();
			}
		}

		lastHighlightSlot = hoveringSlot;
	}

	protected override void OnEndDragging(MergeSlot<Champion> targetSlot)
	{
		base.OnEndDragging(targetSlot);

		// Animation & Visual
		lastHighlightSlot?.SetActiveHighlight(false);
		var targetUnit = targetSlot.Unit;
		if (targetUnit != null)
		{
			Vector3 originalPos = targetUnit.transform.localPosition;
			targetUnit.transform.position += draggingOffset;
			targetUnit.transform.DOLocalMove(originalPos, 0.15f).SetEase(Ease.InCubic);

			(targetSlot as MergeChampSlot).PlayPlacedSmokeVFX();
			(targetSlot as MergeChampSlot).PlayPlacedSFX();

			targetUnit.SetActiveOutline(false);
			targetUnit.DoDrag(false);
		}
		MMVibrationManager.Haptic(HapticTypes.Warning);
	}

	protected override void Update()
	{
		base.Update();

		if (highlightOnHover && draggingUnit == null)
		{
			hoveringSlot = GetHoveringSlot() as MergeChampSlot;
			
			if (hoveringSlot != null)
			{
				hoveringSlot.SetActiveHighlight(true);

				if (lastHighlightSlot != null && lastHighlightSlot != hoveringSlot)
				{
					lastHighlightSlot.SetActiveHighlight(false);
				}

				lastHighlightSlot = hoveringSlot;
			} else
			{
				lastHighlightSlot?.SetActiveHighlight(false);
			}
		}
	}

	#endregion

	#region Add/Remove Champ

	public void AddChampionAtSlot(Champion champ, MergeSlot<Champion> slot)
	{
		if (slot != null)
		{
			Champion clone = Instantiate(champ, transform);
			slot.Unit = clone;

			if (playAppearVFX)
				(slot as MergeChampSlot).PlayAppearVFX();

			slot.Unit.SetActiveLevelLabel(showLevelLabel);
			slot.Unit.onActivate.AddListener((c) => c.SetActiveHPBar(showHPBar));

			onBoardChange.Invoke();
		}
	}

	public void AddChampionAtSlot(Champion champ, int slotID) => AddChampionAtSlot(champ, slots[slotID]);
	public void AddChampionAtFirstEmptySlot(Champion champ) => AddChampionAtSlot(champ, GetFirstEmptySlot());
	public void AddChampionAtFirstEmptySlot(int champID) => AddChampionAtSlot(GameDatabase.Instance.championOrigins[champID].init_champion.prefab, GetFirstEmptySlot());
	public void AddChampionAtLastEmptySlot(Champion champ) => AddChampionAtSlot(champ, GetLastEmptySlot());
	public void AddChampionAtLastEmptySlot(int champID) => AddChampionAtSlot(GameDatabase.Instance.championOrigins[champID].init_champion.prefab, GetLastEmptySlot());

	[ButtonEditor]
	public void AddHuggy() => AddChampionAtLastEmptySlot(0);

	[ButtonEditor]
	public void AddGlamrock() => AddChampionAtFirstEmptySlot(1);

	// -------------------------------------------------------------------------------------------------------------

	public void RemoveChampionAtSlot(MergeSlot<Champion> slot)
	{
		if (slot != null)
		{
			Destroy(slot.Unit?.gameObject);
			slot.Unit = null;

			onBoardChange.Invoke();
		}
	}

	public void RemoveChampionAtSlot(int slotID) => RemoveChampionAtSlot(slots[slotID]);

	[ButtonEditor]
	public void RemoveChampionFirstSlot() => RemoveChampionAtSlot(0);

	[ButtonEditor]
	public void ClearAllChampion()
	{
		for (int i = 0; i < slots.Count; i++)
			RemoveChampionAtSlot(slots[i]);
	}

	#endregion

	#region Serialize/Deserialize

	[ButtonEditor]
	public void SerializePositioningWithLog() => Debug.Log(SerializePositioning());

	public string SerializePositioning()
	{
		var res = "";

		foreach (var slot in slots)
		{
			if (slot.Unit == null)
				res += "000";
			else
				res += $"{slot.Unit.soData.origin.uid}{slot.Unit.Level:D2}";
		}

		return res;
	}

	public void DeserializePositioning(string pos)
	{
		ClearAllChampion();

		var curStatus = playAppearVFX;
		playAppearVFX = false;
		for (var i = 0; i + 2 < pos.Length; i += 3)
		{
			var champLvl = int.Parse(pos.Substring(i + 1, 2));
			if (champLvl <= 0) continue;
			
			var champOriginID = pos[i] - '0';
			AddChampionAtSlot(GameDatabase.Instance.championOrigins[champOriginID].upgradeTree[champLvl - 1].prefab, i / 3);
		}
		playAppearVFX = curStatus;
	}

	#endregion

	#region Visual

	public void SetActiveGround(bool status)
	{
		foreach (var slot in slots)
			((MergeChampSlot) slot).SetActiveGround(status);
	}

	#endregion
}
