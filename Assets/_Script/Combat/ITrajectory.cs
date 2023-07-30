using UnityEngine;

public class ITrajectory : MonoBehaviour
{
	protected Projectile proj;

    public virtual void Init(Projectile p)
	{
		proj = p;
	}

    public virtual void DoMove(Vector3 startPos, Vector3 endPos, float t)
	{

	}
}