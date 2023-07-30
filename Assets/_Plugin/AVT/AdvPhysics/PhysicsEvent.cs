using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class EventCollider : UnityEvent<Collider> { }
[System.Serializable] public class EventCollision : UnityEvent<Collision> { }

public class PhysicsEvent : MonoBehaviour
{
	public EventCollider onTriggerEnter = new EventCollider();
	public EventCollider onTriggerExit = new EventCollider();

	public EventCollision onCollisionEnter = new EventCollision();
	public EventCollision onCollisionExit = new EventCollision();


	private void OnTriggerEnter(Collider other)
	{
		onTriggerEnter.Invoke(other);
	}

	private void OnTriggerExit(Collider other)
	{
		onTriggerExit.Invoke(other);
	}

	private void OnCollisionEnter(Collision collision)
	{
		onCollisionEnter.Invoke(collision);
	}

	private void OnCollisionExit(Collision collision)
	{
		onCollisionExit.Invoke(collision);
	}
}
