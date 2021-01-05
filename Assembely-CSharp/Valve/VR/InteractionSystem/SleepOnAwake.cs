using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SleepOnAwake : MonoBehaviour
	{
		private void Awake()
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.Sleep();
			}
		}
	}
}
