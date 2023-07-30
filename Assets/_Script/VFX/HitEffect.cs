using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
	public GameObject sufferDamageObjectEvent;
	public float duration = 0.5f;
	[Range(0, 1)]
	public float intensity = 0.8f;
	public List<Renderer> rends;

	public bool playAudioOnHit = true;
	[SerializeField] AudioSource audioSrc;

	Color baseEmission;
	float baseLumin;

	private void Awake()
	{
		if (sufferDamageObjectEvent != null)
		{
			var t = sufferDamageObjectEvent.GetComponent<Champion>();
			if (t != null)
			{
				t.onSufferDamage.AddListener((a, b) => Play());
			}
		}
		//baseEmission = rends[0].sharedMaterial.GetColor("_EmissionColor");
		baseLumin = rends[0].sharedMaterial.GetFloat("_Lumin");
	}

	[ButtonEditor]
	public void Play()
	{
		for (int i = 0; i < rends.Count; i++)
		{
			if (rends[i].gameObject.activeInHierarchy)
			{
				rends[i].material.DOKill();
				rends[i].material.SetFloat("_Lumin", intensity);
				rends[i].material.DOFloat(baseLumin, "_Lumin", duration).SetEase(Ease.OutQuad);
			}
		}

		if (playAudioOnHit && audioSrc != null)
			audioSrc.Play();
	}


}
