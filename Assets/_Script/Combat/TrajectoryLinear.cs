using UnityEngine;

public class TrajectoryLinear : ITrajectory
{
	public override void DoMove(Vector3 startPos, Vector3 endPos, float t)
	{
		base.DoMove(startPos, endPos, t);
		transform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, t), Quaternion.LookRotation(endPos - startPos));
	}
}
