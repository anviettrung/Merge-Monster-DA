using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
	#region Properties
	GameDatabase gameDB => GameDatabase.Instance;

	public bool ignoreDisplayNewCard;

	[Header("Main Menu")]
	[SerializeField] Button settingsBtn;
	[SerializeField] Button cardInvenBtn;
	[SerializeField] Button noAdsBtn;

	[SerializeField] TextMeshProUGUI levelTitleTxt;
	[SerializeField] TextMeshProUGUI goldTxt;
	[SerializeField] GameObject goldPanel;
	[SerializeField] UIBuyUnit[] buyUnitBtn;

	[SerializeField] Button startBtn;
	[SerializeField] GameObject buyUnitBtnGroup;

	[Header("Enviroment")]
	[SerializeField] GameObject environmentPanel;
	[SerializeField] Image currEnvironmentImg;
	[SerializeField] Image nextEnvironmentImg;
	[SerializeField] Image[] environmentBarSteps;
	[SerializeField] Sprite[] environmentBarStepFillSprites;
	[Space]
	[SerializeField] GameObject environmentPopup;
	[SerializeField] Image enviPopupIcon;
	[SerializeField] TextMeshProUGUI enviPopupNameText;

	[Header("Merge")]
	public GameObject mergePanel;
	[SerializeField] UICardSlot[] cardMerging;
	[SerializeField] UICardSlot cardMerged;

	[Header("Pop up")]
	public GameObject bonusLevelPopup;
	public GameObject bonusLevelPlayTutBtn;
	public GameObject bonusLevelGetBtn;
	[SerializeField] CanvasGroup adNotAvailablePopup;
	private Sequence adNotAvailablePopupSeq;

	[SerializeField] List<int> rateAtLevel;
	[SerializeField] GameObject ratePopup;

	[Header("Result")]
	public UIResult winResult, loseResult;

#endregion

	#region Unity Callback
	public void Init()
	{
		GPExecutor.Instance.Gameplay.playerBoard.onMergeDone.AddListener(OnPlayerMergeChamp);

		PlayerSave.onGoldChange.AddListener(SetGold);
		PlayerSave.onGoldChange.AddListener(buyUnitBtn[0].OnGoldChange);
		PlayerSave.onGoldChange.AddListener(buyUnitBtn[1].OnGoldChange);

		buyUnitBtn[0].onBuySuccess.AddListener(() => AVT.SaveLoad.SaveFile.BoughtHuggy++);
		buyUnitBtn[1].onBuySuccess.AddListener(() => AVT.SaveLoad.SaveFile.BoughtGlamrock++);

		buyUnitBtn[0].UpdatePrice(AVT.SaveLoad.SaveFile.BoughtHuggy, AVT.SaveLoad.SaveFile.CurrentGameLevel);
		buyUnitBtn[1].UpdatePrice(AVT.SaveLoad.SaveFile.BoughtGlamrock, AVT.SaveLoad.SaveFile.CurrentGameLevel);

		FillEnvironmentBar(AVT.SaveLoad.SaveFile.CurrentGameLevel);


		int gameLevel = AVT.SaveLoad.SaveFile.CurrentGameLevel;
		if (rateAtLevel.Contains(gameLevel+1) && PlayerPrefs.GetInt("last_rate_level", -1) != gameLevel+1)
		{
			ratePopup.SetActive(true);
			PlayerPrefs.SetInt("last_rate_level", gameLevel + 1);
		}
	}

	private void OnDestroy()
	{
		PlayerSave.onGoldChange.RemoveListener(SetGold);
		PlayerSave.onGoldChange.RemoveListener(buyUnitBtn[0].OnGoldChange);
		PlayerSave.onGoldChange.RemoveListener(buyUnitBtn[1].OnGoldChange);
	}

	#endregion

	#region Main Menu

	public const string abbreviationSuffix = "\0KMBTQSX-";
	public static string AbbreviationLongString(long num)
	{
		int divTimes = 0;
		long lastDigit = 000;
		while (num > 999)
		{
			lastDigit = num % 1000 / 100;
			num /= 1000;
			divTimes++;
		}

		if (divTimes > 0)
			return string.Format("{0}.{1}{2}", num, lastDigit, abbreviationSuffix[divTimes].ToString());
		else
			return string.Format("{0}", num);
	}

	public static string AbbreviationIntString(int num)
	{
		int divTimes = 0;
		int lastDigit = 000;
		while (num > 999)
		{
			lastDigit = num % 1000 / 100;
			num /= 1000;
			divTimes++;
		}

		if (divTimes > 0)
			return string.Format("{0}.{1}{2}", num, lastDigit, abbreviationSuffix[divTimes].ToString());
		else
			return string.Format("{0}", num);
	}

	public void SetLevelTitle(int level)
	{
		levelTitleTxt.text = string.Format("Level {0}", level);
	}
	
	public void SetLevelTitle(string str)
	{
		levelTitleTxt.text = str;
	}
	
	public void SetGold(long gold)
	{
		goldTxt.text = AbbreviationLongString(gold);
	}

	public void SetActiveBuyUnitGroup(bool status) => buyUnitBtnGroup.SetActive(status);

	public void DeactiveAllMainMenuElement()
	{
		settingsBtn.gameObject.SetActive(false);
		cardInvenBtn.gameObject.SetActive(false);
		noAdsBtn.gameObject.SetActive(false);
		environmentPanel.gameObject.SetActive(false);
		levelTitleTxt.gameObject.SetActive(false);
		goldPanel.gameObject.SetActive(false);

		startBtn.gameObject.SetActive(false);
		buyUnitBtnGroup.gameObject.SetActive(false);
	}

	#endregion

	#region Popup
	[ButtonEditor]
	public void ShowAdNotAvailablePopup()
	{
		if (adNotAvailablePopupSeq != null)
			adNotAvailablePopupSeq.Kill();

		adNotAvailablePopupSeq = DOTween.Sequence();

		adNotAvailablePopupSeq.AppendCallback(() =>
		{
			adNotAvailablePopup.alpha = 1;
			adNotAvailablePopup.gameObject.SetActive(true);
		});

		adNotAvailablePopupSeq.Append(adNotAvailablePopup.transform.DOScale(1.05f, 0.2f).SetEase(Ease.OutBack));
		adNotAvailablePopupSeq.Append(adNotAvailablePopup.transform.DOScale(1, 0.3f).SetEase(Ease.Linear));
		adNotAvailablePopupSeq.Append(adNotAvailablePopup.DOFade(0, 0.5f).SetEase(Ease.Linear).SetDelay(2));
	}

	public void PopupBonusLevel()
	{
		if (GameTutorial.DoDrawModeTutorial())
		{
			// set active unskipable button
			bonusLevelPlayTutBtn.SetActive(true);
			bonusLevelGetBtn.gameObject.SetActive(false);
		}
		bonusLevelPopup.gameObject.SetActive(true);
	}

	public void BonusLevelPopupPlayButton()
	{
		var adAvailable = MAXManager.Instance.ShowRewardedAd(() =>
		{
			GPExecutor.Instance.PlayBonusLevel();
		});

		if (!adAvailable)
		{
			ShowAdNotAvailablePopup();
		}
	}

	public void BonusLevelPopupPlayButtonWithoutAds()
	{
		GPExecutor.Instance.PlayBonusLevel();
	}

	public void BonusLevelPopupSkipButton()
	{
		bonusLevelPopup.gameObject.SetActive(false);
		winResult.gameObject.SetActive(true);
	}

	#endregion

	#region Environment
	public void SetCurrentEnvironmentImage(Sprite sprite)
	{
		currEnvironmentImg.sprite = sprite;
	}

	public void SetNextEnvironmentImage(Sprite sprite)
	{
		nextEnvironmentImg.sprite = sprite;
	}


	public void FillEnvironmentBar(int n)
	{
		n %= environmentBarSteps.Length;
		for (int i = 0; i < environmentBarSteps.Length; i++)
			environmentBarSteps[i].sprite = environmentBarStepFillSprites[i < n ? 0 : (i == n ? 1 : 2)];
	}
	public void NewEnvironmentPopup(EnvironmentData enviData)
	{
		enviPopupIcon.sprite = enviData.icon;
		enviPopupNameText.text = enviData.environmentName;

		environmentPopup.SetActive(true);
	}

	#endregion

	#region Merge
	public void OnPlayerMergeChamp(Champion mergedChamp)
	{
		if (PlayerSave.HasMergedChamp(mergedChamp.soData) == false)
		{
			PlayerSave.SetHasMergedChamp(mergedChamp.soData, 1);

			if (ignoreDisplayNewCard == false)
			{
				cardMerging[0].Display(mergedChamp.soData.prevUpgrade);
				cardMerging[1].Display(mergedChamp.soData.prevUpgrade);

				cardMerged.Display(mergedChamp.soData);

				PlayMergePanel();
			}
		}
	}

	public void PlayMergePanel()
	{
		mergePanel.SetActive(true);
	}
	#endregion
}
