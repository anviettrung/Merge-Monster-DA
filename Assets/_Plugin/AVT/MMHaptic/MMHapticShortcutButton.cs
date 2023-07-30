#if MOREMOUNTAINS_NICEVIBRATIONS

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MMHapticShortcutButton : MMHapticShortcut
{
	[Header("Ref")]
	public Button btn;

	private void OnValidate()
	{
		if (btn == null)
			btn = GetComponent<Button>();
	}

	private void Awake()
	{
		if (btn != null)
			btn.onClick.AddListener(PlayHaptic);
	}
}

#endif