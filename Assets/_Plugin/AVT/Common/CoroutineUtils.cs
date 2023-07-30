using UnityEngine;
using System;
using System.Collections;

namespace AVT
{
	public static class CoroutineUtils
	{
		/* ---------------------------------------------------------
		 * Usage: StartCoroutine(CoroutineUtils.Chain(...))
		 * For example:
		 *     StartCoroutine(CoroutineUtils.Chain(
		 *         CoroutineUtils.Do(() => Debug.Log("A")),
		 *         CoroutineUtils.WaitForSeconds(2),
		 *         CoroutineUtils.Do(() => Debug.Log("B"))));
		 * --------------------------------------------------------- */
		/// <summary>
		/// Call a chain of actions in sequence
		/// </summary>
		/// <param name="actions">List of coroutines.</param>
		public static IEnumerator Chain(params IEnumerator[] actions)
		{
			foreach (IEnumerator action in actions) {
				yield return VoidObject.Instance.StartCoroutine(action);
			}
		}

		/* ---------------------------------------------------------
		 * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
		 * For example:
		 *     StartCoroutine(CoroutineUtils.DelaySeconds(
		 *         () => DebugUtils.Log("2 seconds past"),
		 *         2);
		 * --------------------------------------------------------- */
		/// <summary>
		/// Excute action after few seconds
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="delay">Delay time (in second).</param>
		public static IEnumerator DelaySeconds(float delay, Action action)
		{
			yield return new WaitForSeconds(delay);
			action();
		}

		/// <summary>
		/// Waits for seconds.
		/// </summary>
		/// <param name="time">Time.</param>
		public static IEnumerator WaitForSeconds(float time)
		{
			yield return new WaitForSeconds(time);
		}

		/// <summary>
		/// Waits for seconds in realtime.
		/// </summary>
		/// <param name="time">Time.</param>
		public static IEnumerator WaitForSecondsRealtime(float time)
		{
			yield return new WaitForSecondsRealtime(time);
		}

		/// <summary>
		/// Do the specified action.
		/// </summary>
		/// <param name="action">Action.</param>
		public static IEnumerator Do(Action action)
		{
			action();
			yield return 0;
		}

		/// <summary>
		/// Do the specified action in next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		public static IEnumerator DoNextFrame(Action action)
		{
			yield return new WaitForEndOfFrame();
			action();
		}

		/* ---------------------------------------------------------
		 * Usage: StartCoroutine(CoroutineUtils.LinearAction(time, action))
		 * For example:
		 *     StartCoroutine(CoroutineUtils.LinearAction(3, (weight) => {
		 * 			position = lerp (start, end, weight); 
		 *        });
		 * --------------------------------------------------------- */
		/// <summary>
		/// Excute action in linear time
		/// </summary>
		/// <param name="time">Duration.</param>
		/// <param name="callback">Action.</param>
		public static IEnumerator LinearAction(float time, Action<float> callback)
		{
			float elapsed = 0;
			while (elapsed < time) {

				callback(elapsed / time);

				yield return new WaitForEndOfFrame();
				elapsed += Time.deltaTime;
			}

			callback(1);
		}

		/* ---------------------------------------------------------
		 * Usage: StartCoroutine(CoroutineUtils.IntervalAction(interval, duration, action))
		 * For example:
		 *     StartCoroutine(CoroutineUtils.IntervalAction(0.2f, 3f, (n) => {
		 * 			Debug.Log("Hooray " + n + " times");
		 *        });
		 * --------------------------------------------------------- */
		/// <summary>
		/// Excute action interval in given time
		/// </summary>
		/// <returns>The action.</returns>
		/// <param name="interval">Interval time.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="callback">Action.</param>
		public static IEnumerator IntervalAction(float interval, float duration, Action<int> callback)
		{
			float elapsed = 0;
			int count = -1;
			while (elapsed < duration) {

				if (Mathf.FloorToInt(elapsed / interval) > count)
					callback(++count);

				yield return new WaitForEndOfFrame();
				elapsed += Time.deltaTime;
			}
		}
	}
}
