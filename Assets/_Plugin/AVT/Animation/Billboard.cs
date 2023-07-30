using UnityEngine;

public class Billboard : MonoBehaviour
{
	public bool useMainCameraOnInit = true;
	public Transform cam;

	private void Start()
	{
		if (useMainCameraOnInit)
		{
			cam = Camera.main.transform;
		}
	}

	private void LateUpdate()
	{
		transform.LookAt(transform.position + cam.forward);
	}
}
