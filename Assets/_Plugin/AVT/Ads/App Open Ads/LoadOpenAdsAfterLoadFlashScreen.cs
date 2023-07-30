using UnityEngine;

public class LoadOpenAdsAfterLoadFlashScreen : MonoBehaviour
{
	public SceneAutoLoad sceneAutoLoad;

	private void OnValidate()
	{
		if (sceneAutoLoad == false)
			sceneAutoLoad = GetComponent<SceneAutoLoad>();
	}

	private void Start()
	{
		EventManager.StartListening("App Open Ad Close", SkipFlashScreen);
	}

	public void SkipFlashScreen()
	{
		sceneAutoLoad.skipMinLoadingTime = true;
	}
}
