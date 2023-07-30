using UnityEngine;
using TMPro;

public class PerformanceStatistics : MonoBehaviour
{
	public enum DisplayMode { FPS, MS }

	[Header("Settings")]
	public bool showFPS = true;

	[SerializeField] DisplayMode displayMode = DisplayMode.FPS;
	[SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;

	[Header("Ref")]
	[SerializeField] TextMeshProUGUI fpsDisplay;

	int frames;
	float duration;

	float bestDuration = float.MaxValue;
	float worstDuration = 0;

	private void Update()
	{
		if (showFPS)
			UpdateFPS();
	}

	private void UpdateFPS()
	{
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;

		if (frameDuration < bestDuration)
		{
			bestDuration = frameDuration;
		}
		if (frameDuration > worstDuration)
		{
			worstDuration = frameDuration;
		}

		if (duration > sampleDuration)
		{
			if (displayMode == DisplayMode.FPS)
			{
				fpsDisplay.SetText("FPS\nA: {0:0}\nB: {1:0}\nW: {2:0}",
					frames / duration,
					1f / bestDuration,
					1f / worstDuration);
			}
			else
			{
				fpsDisplay.SetText("FPS\nA: {0:1}\nB: {1:1}\nW: {2:1}",
					1000f * frames / duration,
					1000f * bestDuration,
					1000f * worstDuration);
			}

			frames = 0;
			duration = 0;

			bestDuration = float.MaxValue;
			worstDuration = 0f;
		}

	}
}
