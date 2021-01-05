using UnityEngine;

namespace FistVR
{
	public class FVRFusedThrowable : FVRPhysicalObject, IFVRDamageable
	{
		public FVRFuse Fuse;

		public override int GetTutorialState()
		{
			if (Fuse.IsIgnited())
			{
				return 1;
			}
			return 0;
		}

		public void Damage(Damage d)
		{
			if (base.QuickbeltSlot == null && (d.Dam_Thermal > 0f || d.Dam_TotalKinetic > 100f))
			{
				Fuse.Ignite(Random.Range(0.1f, 0.8f));
			}
		}
	}
}
