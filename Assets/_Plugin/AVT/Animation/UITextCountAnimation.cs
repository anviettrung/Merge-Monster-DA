using UnityEngine;
using TMPro;
using DG.Tweening;

public class UITextCountAnimation : MonoBehaviour
{
	public bool useSeperator = false;
	[SerializeField] TextMeshProUGUI tmgui;

	private int currentValue;
	private int targetValue;

	private Tween tween;

    private void OnValidate()
    {
		if (tmgui == null)
			tmgui = GetComponent<TextMeshProUGUI>();
	}

	public void SetTargetValue(int newTarget, bool force = false)
	{
		targetValue = newTarget;

		tween?.Kill();
		if (force)
		{
			currentValue = targetValue;
			SetText(targetValue);
		}
		else
		{
			tween = DOVirtual.Float(currentValue, targetValue, 1, (t) =>
			{
				SetText(Mathf.RoundToInt(t));
			}).OnComplete(() => SetText(targetValue));
		}
	}

	public void SetText(int number)
	{
		currentValue = number;

		if (useSeperator)
			tmgui.text = string.Format("{0:#,###0}", number).Replace(".", ",");
		else
			tmgui.text = number.ToString();
	}

	private void OnDisable()
	{
		if (currentValue != targetValue)
			SetText(targetValue);
	}
}
