using UnityEngine;
using UnityEngine.UI;

public class SFXChild : MonoBehaviour
{
	public SFXGroup sfxGroup;
	public Button buttonTrigger;

	private void OnValidate()
	{
		if (buttonTrigger == null)
			buttonTrigger = GetComponent<Button>();
	}

	private void Awake()
	{
		if (buttonTrigger != null)
			buttonTrigger.onClick.AddListener(Play);
	}

	public void Play()
	{
		sfxGroup.Play();
	}

}
