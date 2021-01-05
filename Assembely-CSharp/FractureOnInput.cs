using DinoFracture;
using UnityEngine;

[RequireComponent(typeof(FractureGeometry))]
public class FractureOnInput : MonoBehaviour
{
	public KeyCode Key;

	private void Update()
	{
		if (Input.GetKeyDown(Key))
		{
			GetComponent<FractureGeometry>().Fracture();
		}
	}
}
