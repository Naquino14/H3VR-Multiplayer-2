using UnityEngine;

namespace FistVR
{
	public class EncryptionBotCrystalWeakPoint : MonoBehaviour, IFVRDamageable
	{
		public EncryptionBotCrystal Crystal;

		private EncryptionBotCrystal.Crystal m_c;

		public int WhichCrystal;

		public void SetMC(EncryptionBotCrystal.Crystal c)
		{
			m_c = c;
		}

		public void Damage(Damage d)
		{
			float num = d.Dam_TotalKinetic;
			if (d.Class == FistVR.Damage.DamageClass.Explosive)
			{
				num *= 0.4f;
			}
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				num *= 1.5f;
			}
			Crystal.CrystalHit(m_c, num);
		}
	}
}
