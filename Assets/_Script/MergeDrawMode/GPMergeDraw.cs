using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using AVT;

public class GPMergeDraw : GPBase
{
	public bool isTest = false;
	public bool overrideByTool = false;
	public int toolLevel = 0;

	[Header("Data")]
	public List<MergeDraw> playerPositioning;
	public List<MergeDraw> enemyPositioning;

	[Header("Main")]
	public GameTutorial tutorial;
	public MergeChamp playerBoard;
	public MergeChamp enemyBoard;
	[HideInInspector] public Transform allProjectileHolder;

	public List<Champion> playerChamps = new List<Champion>();
	public List<Champion> enemyChamps = new List<Champion>();
	public List<Champion> allChamps = new List<Champion>();
	public List<Projectile> allProjectiles = new List<Projectile>();

	[Header("Effect")]
	public ParticleSystem confetti;
	public AudioSource oneshotAudio;

	public GameDatabase gameDB => GameDatabase.Instance;

	private bool updateAllUnit;

	[SerializeField] long totalEnemyHP = 0;

	#region Core loop  

	public override void OnInitializationGame()
	{
		base.OnInitializationGame();

		bool doTutorial = tutorial.CheckDrawModeTutorial();

		// Board
		
		//int highestMeleeLevel = doTutorial ? 2 : Mathf.Clamp(PlayerSave.GetHighestMeleeLevel(), 2, 9);
		bool isExistLevel = gameDB.bonusLevels.Exists(e => e.gameLevel == SaveLoad.SaveFile.CurrentGameLevel + 1);
		var playerPos = !doTutorial && isExistLevel ? gameDB.bonusLevels.Find(e => e.gameLevel == SaveLoad.SaveFile.CurrentGameLevel + 1).prefab : gameDB.bonusLevels[0].prefab;

		if (isTest)
			playerPos = playerPositioning[SaveLoad.SaveFile.CurrentGameLevel % playerPositioning.Count];
		else if (overrideByTool)
			playerPos = playerPositioning[toolLevel % playerPositioning.Count];

		playerBoard = Instantiate(playerPos).GetComponent<MergeChamp>();
		foreach (var slot in playerBoard.slots)
		{
			if (slot.gameObject.activeInHierarchy)
				playerBoard.AddChampionAtSlot(gameDB.championOrigins[0].upgradeTree[0].prefab, slot);
		}

		enemyBoard = Instantiate(enemyPositioning[0]).GetComponent<MergeChamp>();
		var enemyChampToAdd = playerPos.mergeMaxUnit - 2;
		foreach (var slot in enemyBoard.slots)
		{
			if (slot.gameObject.activeInHierarchy)
			{
				enemyBoard.AddChampionAtSlot(gameDB.championOrigins[0].upgradeTree[enemyChampToAdd].prefab, slot);
			}
		}
		//-------------------------------------------------------------------------------------------------------------------

		playerBoard.GetComponent<MergeDraw>().onMergeDone.AddListener((t) => OnStartGame());

		// enviroment
		int evID = (AVT.SaveLoad.SaveFile.CurrentGameLevel / 10) % gameDB.environments.Count;
		Instantiate(gameDB.environments[evID].prefab);
		UIManager.Instance.SetCurrentEnvironmentImage(gameDB.environments[evID].icon);
		UIManager.Instance.SetNextEnvironmentImage(gameDB.environments[(evID + 1) % gameDB.environments.Count].icon);

		UIManager.Instance.SetLevelTitle("BONUS LEVEL");

		PlayerSave.InitGold();
	}

	public override void OnStartGame()
	{
		base.OnStartGame();

		UIManager.Instance.DeactiveAllMainMenuElement();

		playerBoard.SetActiveGround(false);
		enemyBoard.SetActiveGround(false);

		playerBoard.enabled = false;

		for (int i = 0; i < playerBoard.slots.Count; i++)
		{
			if (playerBoard.slots[i].HasUnit)
			{
				AddChampToList(playerBoard.slots[i].Unit, playerChamps, 0);
			}
		}

		for (int i = 0; i < enemyBoard.slots.Count; i++)
		{
			if (enemyBoard.slots[i].HasUnit)
			{
				AddChampToList(enemyBoard.slots[i].Unit, enemyChamps, 1);
			}
		}


		// Setup 1 hit
		//Champion boss = enemyChamps[0];
		//boss.doFacingTarget = false;
		//boss.onDoAttack.AddListener((c) => 
		//{
		//	while (playerChamps.Count > 0)
		//		playerChamps[0].Die();
		//});

		totalEnemyHP = GetTotalHP(enemyChamps);

		updateAllUnit = true;
	}

	public override void CheckGameLogic()
	{
		base.CheckGameLogic();

		// Update Champion
		Champion p; // ref
		for (int pN = 0; pN < allChamps.Count; pN++)
		{
			p = allChamps[pN];

			if (updateAllUnit)
				p.state = Champion.State.IDLE; // forces the assignment of a target in the switch below

			switch (p.state)
			{
				case Champion.State.DISABLE:
					break;
				case Champion.State.IDLE:
					if (p.target == null)
						p.FindTargetInList(p.teamID == 0 ? enemyChamps : playerChamps);

					if (p.target == null)
						Debug.LogWarning("Can't find target!");
					else
					{
						if (p.IsTargetInRange())
							p.StartAttack();
						else
							p.Seek();
					}
					break;
				case Champion.State.SEEKING:
					p.FindTargetInList(p.teamID == 0 ? enemyChamps : playerChamps);

					if (p.IsTargetInRange())
					{
						p.StartAttack();
					}
					else
					{
						p.DoMove();
						p.DoFacing();
					}
					break;
				case Champion.State.ATTACKING:
					if (p.IsTargetInRange())
					{
						if (Time.time >= p.lastBlowTime + p.soData.atkCooldown)
						{
							//Animation will produce the damage, calling animation events OnDealDamage and OnProjectileFired. See Champion
							p.DealBlow();
						}
						p.DoFacing();
					}
					else
					{
						p.Seek();
					}

					break;
				case Champion.State.DEAD:
					Debug.LogErrorFormat("A dead Champion shouldn't be in this loop {0}", p.name);
					break;
			}
		}

		if (playerChamps.Count <= 0 || enemyChamps.Count <= 0)
			OnEndGame();

		updateAllUnit = false;
	}

	private void Update()
	{
		// Update Projectile
		Projectile currProjectile;
		float progressToTarget;
		for (int prjN = 0; prjN < allProjectiles.Count; prjN++)
		{
			currProjectile = allProjectiles[prjN];
			progressToTarget = currProjectile.Move();
			if (progressToTarget >= 1f)
			{
				currProjectile.DoDamage();

				currProjectile.Dispose();
				allProjectiles.RemoveAt(prjN);
			}
		}
	}


	public override void OnEndGame()
	{
		base.OnEndGame();

		for (int i = 0; i < allChamps.Count; i++)
		{
			allChamps[i].DanceVictory();
			allChamps[i].SetActiveHPBar(false);
		}

		bool isWin = enemyChamps.Count == 0;

		if (isWin)
		{
			confetti.Play();
		}

		oneshotAudio.clip = isWin ? gameDB.winSound : gameDB.loseSound;
		oneshotAudio.Play();

		DOVirtual.DelayedCall(4, () =>
		{
			if (isWin)
			{
				// WIN
				UIManager.Instance.winResult.UpdateResult(1, 1, GameFormula.HasBonus(AVT.SaveLoad.SaveFile.CurrentGameLevel), AVT.SaveLoad.SaveFile.CurrentGameLevel);
				UIManager.Instance.winResult.gameObject.SetActive(true);

				AnalyticsManager.LogEventLevelPassed(AVT.SaveLoad.SaveFile.CurrentGameLevel);
				AnalyticsManager.LogEventLevelEngage(AVT.SaveLoad.SaveFile.CurrentGameLevel, 1);
			}
			else
			{
				// LOSE
				UIManager.Instance.loseResult.UpdateResult(totalEnemyHP - GetTotalHP(enemyChamps), totalEnemyHP, false, AVT.SaveLoad.SaveFile.CurrentGameLevel);
				UIManager.Instance.loseResult.gameObject.SetActive(true);


				AnalyticsManager.LogEventLevelFailed(AVT.SaveLoad.SaveFile.CurrentGameLevel);
				AnalyticsManager.LogEventLevelEngage(AVT.SaveLoad.SaveFile.CurrentGameLevel, 0);
			}
		});
	}

	#endregion

	#region Visual

	public void CheckBoardFull()
	{
		UIManager.Instance.SetActiveBuyUnitGroup(playerBoard.GetFirstEmptySlot() != null);
	}

	#endregion

	#region Unit Manager

	public long GetTotalHP(List<Champion> champList)
	{
		long res = 0;

		for (int i = 0; i < champList.Count; i++)
			res += champList[i].currHP;

		return res;
	}

	public void AddChampToList(Champion champ, List<Champion> champList, int teamID)
	{
		champ.teamID = teamID;
		champ.name = string.Format("T{0}-{1}", teamID, allChamps.Count);

		champ.Activate();
		champ.onDoAttack.AddListener(OnChampionDoAttack);
		champ.onFireProjectile.AddListener(OnFireProjectile);
		champ.onDead.AddListener(OnChampionDead);

		champList.Add(champ);
		allChamps.Add(champ);
	}

	public void RemoveChampFromList(Champion champ)
	{
		allChamps.Remove(champ);

		if (champ.teamID == 0)
			playerChamps.Remove(champ);
		else
			enemyChamps.Remove(champ);
	}

	private void OnFireProjectile(Champion p)
	{
		if (!p.IsTargetInRange())
			return;

		Vector3 adjTargetPos = p.target.transform.position;
		adjTargetPos.y = 1.5f;
		Quaternion rot = Quaternion.LookRotation(adjTargetPos - p.projectileSpawnPoint.position);

		Projectile prj = p.GetProjectileFromPool();

		if (prj != null)
		{
			prj.transform.SetPositionAndRotation(p.projectileSpawnPoint.position, rot);
			prj.Init(p.target, p.soData);

			allProjectiles.Add(prj);
		}
	}

	private void OnChampionDoAttack(Champion p)
	{
		IDealDamage dealDamage = p.GetComponent<IDealDamage>();
		if (dealDamage != null && p.IsTargetInRange())
			dealDamage.DealDamage(p.target, p.soData.damage);
	}

	private void OnChampionDead(Champion p)
	{
		p.onDead.RemoveListener(OnChampionDead); //remove the listener

		RemoveChampFromList(p);
		p.onDoAttack.RemoveListener(OnChampionDoAttack);
		p.onFireProjectile.RemoveListener(OnFireProjectile);

		StartCoroutine(Dispose(p));

		updateAllUnit = true;
	}

	private IEnumerator Dispose(Champion p)
	{
		p.SetActiveHPBar(false);

		yield return new WaitForSeconds(10f);

		p.gameObject.SetActive(false);
	}

	#endregion
}
