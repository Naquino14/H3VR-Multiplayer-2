using UnityEngine;

namespace FistVR
{
	public class FVRFuseContact : MonoBehaviour, IFVRDamageable
	{
		public FVRFuse Fuse;

		public float StartF;

		public Collider IgnoreThis;

		private void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.activeSelf && col != IgnoreThis)
			{
				Fuse.Ignite(StartF);
			}
		}

		public void Ignite()
		{
			Fuse.Ignite(StartF);
		}

		public void Damage(Damage d)
		{
			Ignite();
		}
	}
}
