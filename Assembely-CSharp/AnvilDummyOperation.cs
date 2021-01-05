using UnityEngine;

public class AnvilDummyOperation : AsyncOperation
{
	public readonly Object Result;

	public AnvilDummyOperation(Object syncResult)
	{
		Result = syncResult;
	}
}
