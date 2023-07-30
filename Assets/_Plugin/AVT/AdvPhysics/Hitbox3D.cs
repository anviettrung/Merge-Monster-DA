using UnityEngine;
using UnityEngine.Events;

namespace AVT
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox3D : MonoBehaviour
    {
        [SerializeField] Collider coll;

        public UnityEvent<Collider> onTriggerEnter = new UnityEvent<Collider>();
        public UnityEvent<Collider> onTriggerExit = new UnityEvent<Collider>();

        public void SetColliderActive(bool value) => coll.enabled = value;

        void OnValidate()
        {
            if (coll == null)
                coll = GetComponent<Collider>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != coll.attachedRigidbody)
            {
                onTriggerEnter.Invoke(other);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody != coll.attachedRigidbody)
            {
                onTriggerExit.Invoke(other);
            }
        }
    }
}
