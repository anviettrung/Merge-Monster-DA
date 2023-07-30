using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrajectoryArc : ITrajectory
{
	public float outerRange = 3;

	private Vector3 lastPos;

	Vector3 controlPoint;

	public override void Init(Projectile p)
	{
		base.Init(p);

		var originDirect = (p.destination - p.startPos);
		p.transform.forward = originDirect;
		controlPoint = Quaternion.AngleAxis(Random.Range(-180, 180), originDirect) * p.transform.right * outerRange;

		controlPoint = Random.Range(0.0f, 0.3f) * originDirect + Random.Range(0.0f, outerRange) * Vector3.up + Random.Range(-outerRange, outerRange) * Vector3.right;
	}

	public override void DoMove(Vector3 startPos, Vector3 endPos, float t)
	{
		base.DoMove(startPos, endPos, t);

		lastPos = proj.transform.position;
		var newPos = DOCurve.CubicBezier.GetPointOnSegment(startPos, controlPoint, endPos, controlPoint, t);

		var newRot = Quaternion.LookRotation(newPos - lastPos);

		proj.transform.SetPositionAndRotation(newPos, newRot);
	}
}
