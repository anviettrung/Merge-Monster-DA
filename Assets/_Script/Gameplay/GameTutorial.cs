using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AVT;

public class GameTutorial : MonoBehaviour
{
	#region General
	[Header("General")]
	public bool skipTutorial = false;

	[SerializeField] MergeChamp playerBoard;
	[SerializeField] MergeChamp enemyBoard;
	[SerializeField] MergeDraw playerDrawBoard => GPExecutor.Instance.GetComponent<GPMergeDraw>().playerBoard.GetComponent<MergeDraw>();
	[SerializeField] GameObject realBuyUnitButtonList;
	[SerializeField] GameObject tutoBuyUnitButtonList;
	[Space]
	[SerializeField] GameObject blackPanel;
	[SerializeField] Button startFightBtn;
	[SerializeField] FocusButtonTutorial buyHuggyBtn;
	[SerializeField] FocusButtonTutorial buyGlamrockBtn;
	[SerializeField] GameObject handClick;
	[Space]
	[SerializeField] ChampionData weakHuggy;
	[SerializeField] ChampionData weakGlamrock;
	[Space]
	[SerializeField] GameObject tutorialDrag;
	[SerializeField] GameObject tutorialMerge;
	[SerializeField] GameObject tutorialDraw;
	[Space]
	[SerializeField] Button[] replayBtn;
	

	public bool CheckTutorial()
	{
		if (skipTutorial)
			return false;

		// Init
		realBuyUnitButtonList.SetActive(false);
		tutoBuyUnitButtonList.SetActive(false);
		startFightBtn.gameObject.SetActive(false);
		buyHuggyBtn.gameObject.SetActive(false);
		buyGlamrockBtn.gameObject.SetActive(false);

		if (SaveLoad.SaveFile.CurrentGameLevel == 0)
		{
			
			for (int i = 0; i < enemyBoard.slots.Count; i++)
			{
				Champion champ = enemyBoard.slots[i].Unit;
				if (champ == null)
					continue;

				if (champ.soData.origin == weakHuggy.origin)
					champ.soData = weakHuggy;

				if (champ.soData.origin == weakGlamrock.origin)
					champ.soData = weakGlamrock;
			}
		}


		if (PlayerPrefs.GetInt("tut_first_fight", 0) == 0)
		{
			startFightBtn.gameObject.SetActive(true);
			for (int i = 0; i < replayBtn.Length; i++)
				replayBtn[i].onClick.AddListener(() => PlayerPrefs.SetInt("tut_first_fight", 1));

			return true;
		}

		if (PlayerPrefs.GetInt("tut_first_buy_unit", 0) == 0)
		{
			tutoBuyUnitButtonList.SetActive(true);
			blackPanel.SetActive(true);
			handClick.SetActive(true);

			playerBoard.DeserializePositioning("000000001000000000000000000000000000000000000");
			buyGlamrockBtn.GetComponent<UIBuyUnit>().onBuySuccess.AddListener(() => playerBoard.AddChampionAtSlot(GameDatabase.Instance.championOrigins[1].init_champion.prefab, 1));

			StartCoroutine(CoroutineUtils.Chain(
				buyGlamrockBtn.ShowAndWaitButtonClicked(),
				CoroutineUtils.Do(() =>
				{
					handClick.SetActive(false);
					tutorialDrag.SetActive(true);
					startFightBtn.onClick.AddListener(() => tutorialDrag.SetActive(false));

					playerBoard.onBoardChange.AddListener(() => tutorialDrag.SetActive(false));
					blackPanel.SetActive(false);
					startFightBtn.gameObject.SetActive(true);
					PlayerPrefs.SetInt("tut_first_buy_unit", 1);
				})
			));
			return true;
		}

		if (AVT.SaveLoad.SaveFile.CurrentGameLevel == 0)
		{
			startFightBtn.gameObject.SetActive(true);
			return true;
		}

		if (PlayerPrefs.GetInt("tut_second_buy_unit", 0) == 0)
		{
			tutoBuyUnitButtonList.SetActive(true);
			blackPanel.SetActive(true);
			handClick.SetActive(true);

			playerBoard.DeserializePositioning("000101000000000000000000000000000000001000000");
			buyHuggyBtn.GetComponent<UIBuyUnit>().onBuySuccess.AddListener(() => playerBoard.AddChampionAtSlot(GameDatabase.Instance.championOrigins[0].init_champion.prefab, 13));

			StartCoroutine(CoroutineUtils.Chain(
				buyHuggyBtn.ShowAndWaitButtonClicked(),
				CoroutineUtils.Do(() =>
				{
					handClick.SetActive(false);
					blackPanel.SetActive(false);
					PlayerPrefs.SetInt("tut_second_buy_unit", 1);
					DoMergeTutorial();
				})
			));
			return true;
		}

		if (PlayerPrefs.GetInt("tut_first_merge", 0) == 0)
		{
			DoMergeTutorial();
			return true;
		}

		// Revert to normal
		realBuyUnitButtonList.SetActive(true);
		startFightBtn.gameObject.SetActive(true);

		return false;
	}

	public static bool DoDrawModeTutorial() => PlayerPrefs.GetInt("play_draw_mode_first_time", 1) == 1;

	public bool CheckDrawModeTutorial()
	{
		if (skipTutorial)
			return false;

		if (PlayerPrefs.GetInt("play_draw_mode_first_time", 1) == 1)
		{
			UIManager.Instance.winResult.onContinueAction.AddListener(() =>
			{
				PlayerPrefs.SetInt("play_draw_mode_first_time", 0);
			});

			return true;
		}

		return false;
	}

	public void DoMergeTutorial()
	{
		// show arrow
		tutorialMerge.SetActive(true);

		playerBoard.onMergeDone.AddListener((t) => 
		{
			// deactive arrow
			startFightBtn.gameObject.SetActive(true);
			tutorialMerge.SetActive(false);
			PlayerPrefs.SetInt("tut_first_merge", 1);
		});
	}

	#endregion


}
