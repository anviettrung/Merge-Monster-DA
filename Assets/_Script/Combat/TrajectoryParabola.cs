using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AVT;
using DG.Tweening;

public class TrajectoryParabola : ITrajectory
{
	public float height = 2;
	private float peak = 0;
	private const float standardDist = 3;

	private Vector3 lastPos;

	public override void DoMove(Vector3 startPos, Vector3 endPos, float t)
	{
		base.DoMove(startPos, endPos, t);

		lastPos = proj.transform.position;

		var dist = (endPos - startPos).magnitude;
		peak = Mathf.Max(startPos.y, endPos.y) + Mathf.Floor(Mathf.Min(dist / standardDist, 1)) * height;
		var newPos = new Vector3(
			DOVirtual.EasedValue(startPos.x, endPos.x, t, Ease.Linear),
			t < 0.5f ? DOVirtual.EasedValue(startPos.y, peak, t * 2, Ease.OutSine) : DOVirtual.EasedValue(peak, endPos.y, (t - 0.5f) * 2, Ease.InSine),
			DOVirtual.EasedValue(startPos.z, endPos.z, t, Ease.Linear));

		var newRot = Quaternion.LookRotation(newPos - lastPos);

		proj.transform.SetPositionAndRotation(newPos, newRot);
	}
}
