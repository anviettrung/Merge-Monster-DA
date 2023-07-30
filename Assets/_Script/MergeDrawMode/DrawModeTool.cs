using UnityEngine;
using UnityEngine.UI;

public class DrawModeTool : MonoBehaviour
{
	static int selectingLevel = 0;
	static bool isDrawLine = true;

	public GPExecutor exe;
	public GPMergeDraw gp;
	public UINumberSelect levelSelect;
	public Toggle drawLineToggle;

	public GameDatabase gameDB => GameDatabase.Instance;

	private void Awake()
	{
		gp.overrideByTool = true;
		exe.gameplaySceneIndex = exe.bonusLevelSceneIndex = 0;

		gp.toolLevel = selectingLevel;

		levelSelect.ChangeNumber(selectingLevel);
		levelSelect.onSelectNumber.AddListener(SelectLevel);

		drawLineToggle.isOn = isDrawLine;
		drawLineToggle.onValueChanged.AddListener(OnDrawLineChange);
	}

	private void Start()
	{
		LoadLevelDatabase();
		StartCoroutine(AVT.CoroutineUtils.DoNextFrame(() => OnDrawLineChange(isDrawLine)));
	}

	public void OnDrawLineChange(bool status)
	{
		isDrawLine = status;
		gp.playerBoard.GetComponent<MergeDraw>().isDrawLine = isDrawLine;
	}

	public void LoadLevelDatabase()
	{
		levelSelect.maxIndex = gp.playerPositioning.Count;
	}

	public void SelectLevel(int x)
	{
		selectingLevel = x;
		GPExecutor.Instance.Replay();
	}
}
