using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SosigWearable : MonoBehaviour, IFVRDamageable
	{
		public enum SosigWearableSlot
		{
			Helmet = 0,
			HeadTop = 1,
			Face = 2,
			HeadWrap = 3,
			Torso = 10,
			UpperLink = 11,
			LowerLink = 12,
			Backpack = 20
		}

		public Sosig S;

		public SosigLink L;

		public List<Collider> Cols;

		public float BluntDamageTransmission = 1f;

		public float MeleeDamMult_Blunt = 1f;

		public float MeleeDamMult_Cutting = 1f;

		public float MeleeDamMult_Piercing = 1f;

		public float ColBluntDamageMult = 1f;

		public bool IsLodgeable;

		public bool IsStabbable;

		private Renderer m_rendMain;

		private bool hasRendMain;

		public Renderer OverrideRendMain;

		public List<SosigWearableSlot> SlotsTaken;

		public virtual void Start()
		{
			if (L != null)
			{
				L.SetColDamMult(ColBluntDamageMult);
			}
			m_rendMain = GetComponent<Renderer>();
			if (m_rendMain != null)
			{
				hasRendMain = true;
			}
			else if (OverrideRendMain != null)
			{
				m_rendMain = OverrideRendMain;
				hasRendMain = true;
			}
		}

		public void RegisterWearable(SosigLink l)
		{
			L = l;
			S = l.S;
			L.RegisterWearable(this);
		}

		public void Show()
		{
			if (hasRendMain)
			{
				m_rendMain.enabled = true;
			}
		}

		public void Hide()
		{
			if (hasRendMain)
			{
				m_rendMain.enabled = false;
			}
		}

		public virtual void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				if (S != null && BluntDamageTransmission > 0.01f)
				{
					float num = 1f;
					if (S.IsDamResist || S.IsDamMult)
					{
						num = S.BuffIntensity_DamResistHarm;
					}
					if (S.IsFragile)
					{
						num *= 100f;
					}
					S.ProcessDamage(0f, 0f, d.Dam_Blunt * BluntDamageTransmission * S.DamMult_Blunt * num, 0f, d.point, L);
				}
			}
			else if (d.Class == FistVR.Damage.DamageClass.Melee && L != null)
			{
				L.Damage(d);
			}
		}
	}
}
