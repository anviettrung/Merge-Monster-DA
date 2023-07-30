using UnityEngine;
using TMPro;

public class TestCube : MonoBehaviour
{
	public TextMeshPro textMesh;
	public int level = 0;

	private void Start()
	{
		RefreshVisual();
	}

	public void RefreshVisual()
	{
		textMesh.text = level.ToString();
	}

	public void Upgrade()
	{
		level += 1;
		RefreshVisual();
	}
}
