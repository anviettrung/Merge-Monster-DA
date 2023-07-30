using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AVT // Stand for An Viet Trung
{
	public static class Utility
	{
		public static bool IsPointerOverUIObject()
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			return results.Count > 0;
		}


		public static void Shuffle<T>(List<T> list)
		{
			if (list.Count < 1) return;

			//To shuffle an array a of n elements(indices 0..n - 1):
			//for i from n - 1 downto 1 do
			//j = random integer with 0 <= j <= i

			//exchange a[j] and a[i]
			int j = 0;
			T t;
			for (int i = list.Count - 1; i > 0; i--) {
				j = Random.Range(0, i);
				//swap
				t = list[i];
				list[i] = list[j];
				list[j] = t;
			}
		}
		public static void Shuffle<T>(T[] list)
		{
			if (list.Length < 1) return;

			//To shuffle an array a of n elements(indices 0..n - 1):
			//for i from n - 1 downto 1 do
			//j = random integer with 0 <= j <= i

			//exchange a[j] and a[i]
			int j = 0;
			T t;
			for (int i = list.Length - 1; i > 0; i--)
			{
				j = Random.Range(0, i);
				//swap
				t = list[i];
				list[i] = list[j];
				list[j] = t;
			}
		}

		public static bool RandWithRate(float rate) => Random.Range(0.0f, 1.0f) < rate;
		public static int RandInd<T>(List<T> list) => Random.Range(0, list.Count);
		public static T RandItem<T>(List<T> list) => list[RandInd(list)];

		public enum TimeFormat
		{
			SECOND = 1, // 0001
			MINUTE = 2, // 0010
			HOUR = 4, // 0100
			DAY = 8, // 1000
		}

		public static string GetTimef(float time, TimeFormat timeFormat)
		{
			string res = "";
			Vector4 t = Vector4.zero;
			bool doOther = false;
			if (timeFormat == TimeFormat.DAY) {
				t.x = Mathf.Floor(time / 86400);
				time -= t.x * 86400;

				if (t.x < 10) res += "0";
				res += t.x.ToString() + " : ";

				doOther = true;
			}

			if (timeFormat == TimeFormat.HOUR || doOther) {
				t.y = Mathf.Floor(time / 3600);
				time -= t.y * 3600;

				if (t.y < 10) res += "0";
				res += t.y.ToString() + " : ";

				doOther = true;
			}

			if (timeFormat == TimeFormat.MINUTE || doOther) {
				t.z = Mathf.Floor(time / 60);
				time -= t.z * 60;

				if (t.z < 10) res += "0";
				res += t.z.ToString() + " : ";

			}

			if (time < 9) res += "0";
			res += Mathf.Ceil(time).ToString();

			return res;
		}

		public static int GetUTCTime()
		{
			System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			return (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
		}

		public static void LookAtDirection(this Transform trans, Vector3 direction, float rotateSpeed = 50f)
		{
			if (direction == Vector3.zero)
				return;

			trans.rotation = Quaternion.Lerp(
				trans.rotation,
				Quaternion.LookRotation(direction),
				Time.deltaTime * rotateSpeed);
		}
	}

	public static class ExtendMath
	{
	
	}

}
// Define Events
[System.Serializable] public class EventGameObject : UnityEvent<GameObject> { }

[System.Serializable] public class EventFloat : UnityEvent<float> { }
[System.Serializable] public class EventInt : UnityEvent<int> { }
[System.Serializable] public class EventLong : UnityEvent<long> { }
[System.Serializable] public class EventBool : UnityEvent<bool> { }
[System.Serializable] public class EventString : UnityEvent<string> { }
[System.Serializable] public class EventVec2 : UnityEvent<Vector2> { }

