using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class MergeChampSlot : MergeSlot<Champion>
{
	[Header("VFX")]
	public bool showGround = true;
	[SerializeField] GameObject ground;
	[SerializeField] GameObject highlight;
	[SerializeField] ParticleSystem placedSmokeVFX;
	[SerializeField] ParticleSystem appearVFX;
	[SerializeField] ParticleSystem mergeVFX;

	[Header("SFX")]
	[SerializeField] AudioSource placedSFX;

	public override void OnUnitChange()
	{
		base.OnUnitChange();
	}

	public override void Init()
	{
		base.Init();

		ground.SetActive(showGround);
	}

	#region Visual & SFX

	public void SetActiveGround(bool value) => ground.SetActive(value);
	public void SetActiveHighlight(bool value) => highlight.SetActive(value);
	public void PlayPlacedSmokeVFX() => placedSmokeVFX.Play();
	public void PlayAppearVFX() => appearVFX.Play();
	public void PlayMergeVFX() => mergeVFX.Play();
	public void PlayPlacedSFX() => placedSFX.Play();

	#endregion
}
