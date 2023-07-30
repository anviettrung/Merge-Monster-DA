public class GameFormula
{
	public static long GetUnitPrice(int boughtUnit, int gameLevel) => GetBaseUnitPrice(boughtUnit) * GetMultiplerUnitPrice(gameLevel);

	public static long GetBaseUnitPrice(int n)
	{
		if (n == 0)
			return 2;
		else
			return 32 + 128 * n;
	}

	public static long GetMultiplerUnitPrice(int gameLevel)
	{
		gameLevel += 1;

		if (gameLevel < 6)
			return 1;

		long n = 0;
		if (6 <= gameLevel && gameLevel <= 7)
			n = 3;
		else if (8 <= gameLevel && gameLevel <= 10)
			n = 4;
		else
			n = gameLevel - 6;

		// pow 4
		n = n * n;
		n = n * n;

		return n / 16;
	}

	public static long Earning(int gameLevel, bool x10 = false)
	{
		gameLevel += 1;

		long n = 0;

		if (1 <= gameLevel && gameLevel <= 2)
			n = 1;
		else if (3 <= gameLevel && gameLevel <= 5)
			n = 2;
		else if (6 <= gameLevel && gameLevel <= 7)
			n = 3;
		else if (8 <= gameLevel && gameLevel <= 10)
			n = 4;
		else
			n = gameLevel - 6;

		// pow 4
		n = n * n;
		n = n * n;

		return 40 * n * (x10 ? 10 : 1);
	}

	public static int[] noBonusLevel = { 1, 3, 4, 6, 8, 9 };
	public static bool HasBonus(int gameLevel)
	{
		gameLevel += 1;

		for (int i = 0; i < noBonusLevel.Length; i++)
			if (gameLevel == noBonusLevel[i])
				return false;

		return true;
	}

}
