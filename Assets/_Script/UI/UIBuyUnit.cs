using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UIBuyUnit : MonoBehaviour
{
	[SerializeField] Image icon;
	[Space]
	[SerializeField] GameObject goldGrp;
	[SerializeField] GameObject adsGrp;
	[Space]
	[SerializeField] TextMeshProUGUI priceTxt;
	[Space]
	[SerializeField] Image bgImg;
	[SerializeField] Sprite[] bgFrame;

	public UnityEvent onBuySuccess = new UnityEvent();
	public UnityEvent adsNotAvailable = new UnityEvent();
	public UnityEvent slotNotAvailable = new UnityEvent();

	[SerializeField] Button btn;
	private BuyMode currBuyMode = BuyMode.ByGold;

	int m_bought = 0;
	int m_gameLevel = 0;
	[SerializeField] long price = 0;


	private void OnValidate()
	{
		if (btn == null)
			btn = GetComponent<Button>();
	}

	private void Awake()
	{
		// add listener on gold change then reset visual
		btn.onClick.AddListener(OnClickBuyBtn);
	}

	// Call when
	// - play new level
	// - after buy new unit
	public void UpdatePrice(int bought, int gameLevel)
	{
		m_bought = bought;
		m_gameLevel = gameLevel;
		price = GameFormula.GetUnitPrice(bought, gameLevel);
		SetPrice(price);
	}

	public void OnGoldChange(long gold)
	{
		SetVisual(gold >= price ? BuyMode.ByGold : BuyMode.ByAds);
	}

	public void OnClickBuyBtn()
	{
		// Check if slot available
		// Show Warning if slot not available and return

		switch (currBuyMode)
		{
			case BuyMode.ByGold:
				BuyUnitByGold();
				break;
			case BuyMode.ByAds:
				BuyUnitByAds();
				break;
		}
	}

	public void BuyUnitByGold()
	{
		long curPrice = price;
		UpdatePrice(m_bought + 1, m_gameLevel);

		// Check if enough gold then trigger on buy success
		AVT.SaveLoad.SaveFile.Gold -= curPrice;

		// Test
		onBuySuccess.Invoke();
	}

	public void BuyUnitByAds()
	{
		var adAvailable = MAXManager.Instance.ShowRewardedAd(() =>
		{
			var availableSlot = GPExecutor.Instance.Gameplay.playerBoard.GetEmptySlotCount();
			availableSlot = Mathf.Clamp(availableSlot, 0, 2);

			for (int i = 0; i < availableSlot; i++)
			{
				UpdatePrice(m_bought + 1, m_gameLevel);
				onBuySuccess.Invoke();
			}
		});

		if (!adAvailable)
		{
			UIManager.Instance.ShowAdNotAvailablePopup();
		}
	}

	public void SetIcon(Sprite sprite)
	{
		icon.sprite = sprite;

	}

	public void SetPrice(long price)
	{
		priceTxt.text = UIManager.AbbreviationLongString(price);
	}

	[ButtonEditor]
	public void SetVisualBuyByGold() => SetVisual(BuyMode.ByGold);
	[ButtonEditor]
	public void SetVisualBuyByAds() => SetVisual(BuyMode.ByAds);

	public void SetVisual(BuyMode mode)
	{
		currBuyMode = mode;

		goldGrp.SetActive(mode == BuyMode.ByGold);
		adsGrp.SetActive(mode == BuyMode.ByAds);

		bgImg.sprite = bgFrame[(int)mode];
	}

	public enum BuyMode
	{
		ByGold = 0,
		ByAds = 1
	}
}
