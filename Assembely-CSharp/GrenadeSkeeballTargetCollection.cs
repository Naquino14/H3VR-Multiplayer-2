using UnityEngine;

public class GrenadeSkeeballTargetCollection : MonoBehaviour
{
	public GrenadeSkeeballTarget[] Targets;

	public void SetGameReference(GrenadeSkeeballGame game)
	{
		for (int i = 0; i < Targets.Length; i++)
		{
			Targets[i].SetGame(game);
		}
	}
}
