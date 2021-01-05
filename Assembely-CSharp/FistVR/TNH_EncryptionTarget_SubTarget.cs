using UnityEngine;

namespace FistVR
{
	public class TNH_EncryptionTarget_SubTarget : MonoBehaviour, IFVRDamageable
	{
		public TNH_EncryptionTarget Target;

		public int Index;

		public void Damage(Damage d)
		{
			if (d.Class != FistVR.Damage.DamageClass.Explosive)
			{
				Target.DisableSubtarg(Index);
			}
		}
	}
}
