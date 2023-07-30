using System.Collections.Generic;
using UnityEngine;

public class ToonOutlineControl : MonoBehaviour
{
	public List<Renderer> rends;

	public Color outlineColorOverride;
	public float outlineWidthOverride;

	Color baseOutlineColor;
	float baseOutlineWidth;

	private void Awake()
	{
		baseOutlineColor = rends[0].sharedMaterial.GetColor("_OtlColor");
		baseOutlineWidth = rends[0].sharedMaterial.GetFloat("_OtlWidth");
	}

	[ButtonEditor]
	public void OverrideOutline() => OverrideOutline(true);
	[ButtonEditor]
	public void StopOverrideOutline() => OverrideOutline(false);

	public void OverrideOutline(bool status)
	{
		if (rends == null)
			return;

		for (int i = 0; i < rends.Count; i++)
		{
			if (rends[i].gameObject.activeInHierarchy)
			{
				rends[i].material.SetColor("_OtlColor", status ? outlineColorOverride : baseOutlineColor);
				rends[i].material.SetFloat("_OtlWidth", status ? outlineWidthOverride : baseOutlineWidth);
			}
		}
	}


}
