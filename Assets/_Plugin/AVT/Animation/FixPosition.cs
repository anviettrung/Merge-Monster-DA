using UnityEngine;

public class FixPosition : MonoBehaviour
{
	private Vector3 position;

	private void Awake()
	{
		position = transform.position;
	}

	void LateUpdate()
	{
		transform.position = position;
	}

	public void ChangeFixedPosition(Vector3 newPosition)
	{
		position = newPosition;
		transform.position = position;
	}
}
