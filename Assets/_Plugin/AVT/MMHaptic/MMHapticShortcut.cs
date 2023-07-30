#if MOREMOUNTAINS_NICEVIBRATIONS

/*
	THIS SCRIPT ONLY COMPATIBLE WITH NICEVIBRATIONS V3.9

	For Android to support vibrations you’ll need to have the following line in your Android manifest :

		<uses-permission android:name="android.permission.VIBRATE" />

	To do so, you can either add a Handheld.Vibrate() somewhere in your code (it 
	doesn’t have to be triggered for Unity to change the manifest), or you can copy
	the one provided with the asset (it’s in the Common/Plugins/Android/ folder) to
	Assets/Plugins/Android/AndroidManifest.xml.
 */

using UnityEngine;
using MoreMountains.NiceVibrations;

public class MMHapticShortcut : MonoBehaviour
{
	[Header("Haptic")]
	public HapticTypes hapticType;
	public float hapticCooldown = 0;

	private float hapticReadyTime = 0;

	public void PlayVibrate() => MMVibrationManager.Vibrate();
	public void PlayHaptic()
	{
		if (Time.time >= hapticReadyTime)
		{
			MMVibrationManager.Haptic(hapticType);
			hapticReadyTime = Time.time + hapticCooldown;
		}
	}
	public void StopAllHaptics(bool alsoRumble) => MMVibrationManager.StopAllHaptics(alsoRumble);
	public void StopContinuousHaptic(bool alsoRumble) => MMVibrationManager.StopContinuousHaptic(alsoRumble);
	public void SetHapticsActive(bool status) => MMVibrationManager.SetHapticsActive(status);
}

#endif