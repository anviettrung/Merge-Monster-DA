using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICardSlot : MonoBehaviour
{
	public TextMeshProUGUI champNameLabel;
	public Image champIconImg;
	public TextMeshProUGUI hpTxt;
	public TextMeshProUGUI dmgTxt;
	public GameObject lockFrame;

	public void Display(ChampionData champData)
	{
		champNameLabel.text = champData.display_name;
		champIconImg.sprite = champData.icon;
		champIconImg.color = Color.white;

		if (hpTxt != null)
			hpTxt.text = UIManager.AbbreviationIntString(champData.hp);
		if (dmgTxt != null)
			dmgTxt.text = UIManager.AbbreviationIntString(champData.damage);
	}

	public void ActiveLockFrame(bool value)
	{
		if (value)
			champIconImg.color = Color.black;
		lockFrame?.SetActive(value);
	}
}
