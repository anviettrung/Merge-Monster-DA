using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    [SerializeField]
    private int fps = 60;
    void Awake()
    {
        Application.targetFrameRate = fps;
    }
}
