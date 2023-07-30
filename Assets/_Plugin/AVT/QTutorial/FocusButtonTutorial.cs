using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FocusButtonTutorial : MonoBehaviour
{
	public Button focusButton;
	private bool clicked = false;

	private void OnValidate()
	{
		if (focusButton == null)
			focusButton = GetComponent<Button>();
	}

	void Awake()
	{
		focusButton.onClick.AddListener(OnButtonClick);
	}

	public IEnumerator ShowAndWaitButtonClicked()
	{
		gameObject.SetActive(true);
		yield return WaitButtonClicked();
		gameObject.SetActive(false);
	}

	public IEnumerator WaitButtonClicked()
	{
		clicked = false;
		while (!clicked)
		{
			yield return 0;
		}
	}

	private void OnButtonClick()
	{
		clicked = true;
	}
}
