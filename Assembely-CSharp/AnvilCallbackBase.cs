using UnityEngine;

public abstract class AnvilCallbackBase : CustomYieldInstruction
{
	public bool IsCompleted
	{
		get;
		protected set;
	}

	public abstract float Progress
	{
		get;
	}

	public abstract bool Pump();

	public abstract void CompleteNow();
}
