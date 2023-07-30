using UnityEngine;
using UnityEngine.Events;

public class DeathRagdoll : MonoBehaviour
{
    public bool copyBone = true;
    public bool preCopyBones = true;

    public bool disableOriginal = true;
    public bool canActiveMultipleTimes = false;

    [SerializeField] GameObject original;
    public GameObject ragdoll;

    public UnityEvent onActiveRagdoll = new UnityEvent();
    public UnityEvent onActiveOriginal = new UnityEvent();
    public UnityEvent<string> onDeath = new UnityEvent<string>();

    [SerializeField] Transform[] originalBones;
    [SerializeField] Transform[] ragdollBones;
    private int activeCount = 0;

	private void Awake()
	{
		if (preCopyBones)
		{
            AVT.CopyBones.CreateBonesArray(original, ragdoll, out originalBones, out ragdollBones);
        }
	}

	/// <summary>
	/// Return false if active ragdoll more than once while can't active multiple times
	/// </summary>
	/// <returns></returns>
	public bool ActiveRagdoll(string deathReason = "unknown")
    {
        if (activeCount > 0 && canActiveMultipleTimes == false)
            return false;

        original.SetActive(!disableOriginal);

        if (activeCount == 0)
        {
            if (copyBone && !preCopyBones)
            {
                AVT.CopyBones.CreateBonesArray(original, ragdoll, out originalBones, out ragdollBones);
            }
        }

        ragdoll.transform.SetParent(original.transform.parent);
        if (copyBone)
            AVT.CopyBones.DoCopy(originalBones, ref ragdollBones);

        ragdoll.transform.SetParent(transform.parent, true);

        ragdoll.SetActive(true);

        onActiveRagdoll.Invoke();
        onDeath.Invoke(deathReason);
        
        activeCount += 1;

        return true;
    }
    [ButtonEditor] public bool ActiveRagdoll() => ActiveRagdoll("");


    [ButtonEditor]
    public bool ActiveOriginal()
    {
        ragdoll.SetActive(false);
        original.SetActive(true);

        onActiveOriginal.Invoke();

        return true;
    }

    private void OnDisable()
    {
        if (ragdoll != null)
            ragdoll.SetActive(false);
    }
}
