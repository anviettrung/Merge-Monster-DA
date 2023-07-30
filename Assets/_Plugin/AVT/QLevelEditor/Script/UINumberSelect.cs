using UnityEngine;
using TMPro;

public class UINumberSelect : MonoBehaviour
{
	public int maxIndex;
	public TextMeshProUGUI numberText;
	public EventInt onSelectNumber = new EventInt();

	private int current = 0;

	private void Start()
	{
		RefreshNumberText();
	}

	public void OnSelect()
	{
		onSelectNumber.Invoke(current);
	}
	
	public void RefreshNumberText()
	{
		numberText.text = (current + 1).ToString();
	}

	public void ChangeNumber(int x)
	{
		current = x % maxIndex;
		RefreshNumberText();
	}

	public void IncNumber()
	{
		current = (current + 1) % maxIndex;
		RefreshNumberText();
	}

	public void DecNumber()
	{
		current = (current - 1 + maxIndex) % maxIndex;
		RefreshNumberText();
	}
}
