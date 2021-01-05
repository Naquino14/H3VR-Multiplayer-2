using UnityEngine;

namespace FistVR
{
	public class FVRAmbienceZone : MonoBehaviour
	{
		public int ZoneIndex;

		public Transform t;

		public void Awake()
		{
			t = base.transform;
		}
	}
}
