using UnityEngine;

// Gameplay Base
public abstract class GPBase : MonoBehaviour
{
	protected GLDBase gld;
	protected bool isGameStarted = false;
	public bool IsGameStared => isGameStarted;

	public void LoadGPData(GLDBase dat) => gld = dat;
	public virtual void OnInitializationGame() { }
	public virtual void OnStartGame() { isGameStarted = true; }
	public virtual void CheckGameLogic() { }
	public virtual void OnPauseGame() { Time.timeScale = 0; }
	public virtual void OnEndGame() { isGameStarted = false; }
}
