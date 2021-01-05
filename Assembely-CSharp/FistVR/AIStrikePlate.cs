using UnityEngine;

namespace FistVR
{
	public class AIStrikePlate : MonoBehaviour, IFVRDamageable
	{
		public int NumStrikesLeft = 1;

		protected int m_originalNumStrikesLeft = 1;

		private AudioSource m_aud;

		public void Awake()
		{
			m_originalNumStrikesLeft = NumStrikesLeft;
			m_aud = GetComponent<AudioSource>();
		}

		public virtual void Damage(Damage dam)
		{
			if (dam.Class == FistVR.Damage.DamageClass.Projectile)
			{
				m_aud.PlayOneShot(m_aud.clip, 1.2f);
				if (NumStrikesLeft > 0)
				{
					NumStrikesLeft--;
				}
				if (NumStrikesLeft == 0)
				{
					PlateFelled();
				}
			}
		}

		public virtual void Reset()
		{
			NumStrikesLeft = m_originalNumStrikesLeft;
		}

		public virtual void PlateFelled()
		{
		}
	}
}
