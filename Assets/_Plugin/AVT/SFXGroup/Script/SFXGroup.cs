using UnityEngine;

public class SFXGroup : MonoBehaviour
{
	[SerializeField] AudioSource source;

	public void Play()
	{
		source.Play();
	}
}
