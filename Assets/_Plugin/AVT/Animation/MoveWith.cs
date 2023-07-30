using UnityEngine;

public class MoveWith : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3 offset;

    void Awake()
    {
        offset = transform.position - target.transform.position;
    }
    void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
