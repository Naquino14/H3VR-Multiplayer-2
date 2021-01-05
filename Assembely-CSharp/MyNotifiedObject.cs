using UnityEngine;

public class MyNotifiedObject : MonoBehaviour, IFastPoolItem
{
	private void Start()
	{
		base.name = "I'm Instantiated!";
	}

	private void Update()
	{
	}

	public void OnFastInstantiate()
	{
		base.name = "I'm spawned!";
	}

	public void OnFastDestroy()
	{
		base.name = "I'm cached...";
	}
}
