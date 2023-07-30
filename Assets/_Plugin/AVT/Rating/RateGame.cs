using UnityEngine;

public class RateGame : MonoBehaviour
{
	public string storeLink;

	public GameObject ratePopup;

	public void RateUs()
	{
		Application.OpenURL(storeLink);
		ratePopup.SetActive(false);
	}

	public void Later()
	{
		ratePopup.SetActive(false);
	}
}
