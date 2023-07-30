using UnityEngine;

public class MergeCube : MergeBase<TestCube>
{
	public override bool CanMerge(TestCube fromUnit, TestCube toUnit)
	{
		return fromUnit.level == toUnit.level;
	}

	public override void DoMerge(MergeSlot<TestCube> fromSlot, MergeSlot<TestCube> toSlot)
	{
		toSlot.Unit.Upgrade();

		Destroy(fromSlot.Unit.gameObject);

		base.DoMerge(fromSlot, toSlot);
	}

	// Extend Function for demo
	[Header("Demo")]
	public TestCube cubePrefab;

	public void AddCube()
	{
		MergeSlot<TestCube> availableSlot = GetFirstEmptySlot();
		if (availableSlot != null)
		{
			TestCube clone = Instantiate(cubePrefab, transform);
			availableSlot.Unit = clone;
		}
	}
}
