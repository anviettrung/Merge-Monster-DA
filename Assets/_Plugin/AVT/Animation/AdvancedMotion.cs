using UnityEngine;

namespace AVT
{
	using DG.Tweening;

	public static class AdvancedMotion
	{
		public static Tweener DoParabolaMotion(this Transform trans, Vector3 destination, float height, float duration)
		{
			float higherPoint = trans.position.y > destination.y ? trans.position.y : destination.y;

			trans.DOMoveY(higherPoint + height, duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
				trans.DOMoveY(destination.y, duration * 0.5f).SetEase(Ease.InQuad));

			trans.DOMoveX(destination.x, duration).SetEase(Ease.Linear);
			return trans.DOMoveZ(destination.z, duration).SetEase(Ease.Linear);
		}

		public static Tweener DoParabolaYMotion(this Transform trans, float height, float duration)
		{
			var ground = trans.position.y;
			return trans.DOMoveY(ground + height, duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
				trans.DOMoveY(ground, duration * 0.5f).SetEase(Ease.InQuad));
		}

		public static Tweener DoArcMotion(this Transform trans, Vector3 destination, float duration)
		{
			trans.DOMoveX(destination.x, duration).SetEase(Ease.OutQuad);
			trans.DOMoveZ(destination.z, duration).SetEase(Ease.OutQuad);
			return trans.DOMoveY(destination.y, duration).SetEase(Ease.InQuad);
		}

		public static Tweener DoBreath(this Transform trans, Vector3 targetValue, float duration)
		{
			Vector3 startValue = trans.localScale;
			return trans.DOScale(targetValue, duration * 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
			{
				trans.DOScale(startValue, duration * 0.5f).SetEase(Ease.InQuad);
			});
		}
	}
}