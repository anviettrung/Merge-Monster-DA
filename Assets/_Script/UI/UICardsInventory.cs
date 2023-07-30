using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UICardsInventory : MonoBehaviour
{
	public List<UICardSlot> cards;
	public List<Toggle> tabToggles;
	public List<TextMeshProUGUI> tabTxt;
	public Color tabTxtColorEnable;
	public Color tabTxtColorDisable;

	public RectTransform focusTabBar;

	private int lastestTab = 0;

	private void Awake()
	{
		for (int i = 0; i < tabToggles.Count; i++)
		{
			var t = i;
			tabToggles[i].onValueChanged.AddListener((s) => OnTabValueChange(t, s));
		}
	}

	public void OnEnable()
	{
		Refresh(lastestTab);
	}

	public void Refresh(int tabID)
	{
		ChampionData pChamp = GameDatabase.Instance.championOrigins[tabID].init_champion;
		for (int i = 0; i < cards.Count; i++)
		{
			cards[i].gameObject.SetActive(pChamp != null);
			if (pChamp != null)
			{
				cards[i].Display(pChamp);
				cards[i].ActiveLockFrame(!PlayerSave.HasMergedChamp(pChamp));
				pChamp = pChamp.nextUpgrade;
			}
		}

		focusTabBar.transform.DOMoveX(tabToggles[tabID].transform.position.x, 0.25f);
		lastestTab = tabID;
	}

	public void OnTabValueChange(int tabID, bool status)
	{
		tabTxt[tabID].color = status ? tabTxtColorEnable : tabTxtColorDisable;
		// move focus

		if (status)
		{
			Refresh(tabID);
		}
	}
}
