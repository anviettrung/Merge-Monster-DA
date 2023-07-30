#if MOREMOUNTAINS_NICEVIBRATIONS

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MMHapticShortcutToggle : MMHapticShortcut
{
	[Header("Ref")]
	public Toggle toggle;

	private void OnValidate()
	{
		if (toggle == null)
			toggle = GetComponent<Toggle>();
	}

	private void Awake()
	{
		if (toggle != null)
			toggle.onValueChanged.AddListener((value) => PlayHaptic());
	}
}

#endif
