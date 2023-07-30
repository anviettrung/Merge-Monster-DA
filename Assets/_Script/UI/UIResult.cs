using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIResult : MonoBehaviour
{
	public static int skipRewardAdsStreak = 0;

	public enum ResultType
	{
		LOSE,
		WIN
	}

	[Header("Settings")]
	public ResultType result;
	public float delayContinueAction = 2;

	[Header("Main Ref")]
	public Animator anim;
	public TextMeshProUGUI earnedTxt;
	public TextMeshProUGUI goldClaimTxt;
	public Button claimBtn;
	public Button getBtn;

	[Header("Wheel")]
	public List<long> multipler;
	public Transform arrow;
	public bool spinning;
	public float spinSpeed = 180;
	public float segmentArc;
	public float halfSegmentArc;

	[Header("Win Result")]
	public GameObject levelCompleteBonusTxt;

	[Header("Sound")]
	public AudioSource wheelSFX;

	[Header("Event")]
	public UnityEvent onContinueAction = new UnityEvent();

	private long earnBase;
	private bool isEarned = false;

	private void Awake()
	{
		segmentArc = 360 / multipler.Count;
		halfSegmentArc = segmentArc / 2;

		claimBtn.onClick.AddListener(Claim);
		getBtn.onClick.AddListener(Get);
	}

	void OnEnable()
	{
		spinning = true;
	}

	public void UpdateResult(long damageDealt, long damageRequire, bool bonus, int gameLevel)
	{
		earnBase = GameFormula.Earning(gameLevel, bonus) * damageDealt / damageRequire;
		SetEarnedGold(earnBase);

		if (levelCompleteBonusTxt != null)
			levelCompleteBonusTxt.SetActive(bonus);
	}

	void Update()
	{
		if (spinning)
		{
			arrow.Rotate(0, 0, spinSpeed * Time.deltaTime);
			SetClaimGold(earnBase * GetCurrentMultipler());
		}
	}

	public long GetCurrentMultipler()
	{
		float curRotation = (arrow.eulerAngles.z + halfSegmentArc + 360) % 360;
		return multipler[Mathf.FloorToInt(curRotation / segmentArc)];
	}

	[ButtonEditor]
	public void StopSpin()
	{
		spinning = false;
		wheelSFX.Stop();
	}

	[ButtonEditor]
	public void DoSpin()
	{
		spinning = true;
	}

	public void SetEarnedGold(long gold)
	{
		earnedTxt.text = UIManager.AbbreviationLongString(gold);
	}

	public void SetClaimGold(long gold)
	{
		goldClaimTxt.text = UIManager.AbbreviationLongString(gold);
	}

	public void Claim()
	{
		if (isEarned)
			return;

		StopSpin();
		var adAvailable = MAXManager.Instance.ShowRewardedAd(() => 
		{
			long newEarn = earnBase * GetCurrentMultipler();
			AVT.SaveLoad.SaveFile.Gold += newEarn;
			SetEarnedGold(newEarn);
			anim.SetTrigger("Earn");

			isEarned = true;
			DG.Tweening.DOVirtual.DelayedCall(delayContinueAction, ContinueAction);

			skipRewardAdsStreak = 0;
		});

		if (!adAvailable)
		{
			UIManager.Instance.ShowAdNotAvailablePopup();
		}
	}

	public void Get()
	{
		if (isEarned)
			return;

		// skipRewardAdsStreak++;
		// if (skipRewardAdsStreak >= 2)
		// {
		// 	MAXManager.Instance.ShowInterstitialAd();
		// 	skipRewardAdsStreak = 0;
		// }

		if (AVT.SaveLoad.SaveFile.CurrentGameLevel > 1)
			MAXManager.Instance.ShowInterstitialAd();

		StopSpin();

		// Claim gold
		AVT.SaveLoad.SaveFile.Gold += earnBase;
		anim.SetTrigger("Earn");

		isEarned = true;
		DG.Tweening.DOVirtual.DelayedCall(delayContinueAction, ContinueAction);
	}

	public void ContinueAction()
	{
		onContinueAction.Invoke();
		switch (result)
		{
			case ResultType.LOSE:
				GPExecutor.Instance.Replay();
				break;
			case ResultType.WIN:
				GPExecutor.Instance.NextLevel();
				break;
		}
	}
}
