using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJoin : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<BoneJoins>().AddLimb(gameObject);
		Destroy(gameObject);
	}
}