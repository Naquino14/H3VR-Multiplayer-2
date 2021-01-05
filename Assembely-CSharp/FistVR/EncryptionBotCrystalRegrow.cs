using UnityEngine;

namespace FistVR
{
	public class EncryptionBotCrystalRegrow : MonoBehaviour, IFVRDamageable
	{
		public EncryptionBotCrystal Crystal;

		public void Damage(Damage d)
		{
			if (d.Class != FistVR.Damage.DamageClass.Explosive)
			{
				Crystal.Regrow();
			}
		}
	}
}
