using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Explosion : MonoBehaviour
	{
		public int MaxChecksPerFrame = 10;

		public float ExplosionRadius;

		public float ExplosiveForce;

		public bool UsesExplosiveForceOverride;

		public Transform DirOverride;

		public LayerMask Mask_Explosion;

		public LayerMask Mask_Blockers;

		public bool UsesBlockers = true;

		private Collider[] m_colliders;

		private HashSet<Rigidbody> hitRBs = new HashSet<Rigidbody>();

		private List<Rigidbody> hitRBsList = new List<Rigidbody>();

		private int currentIndex;

		private bool m_hasExploded;

		public bool HasSecondaries;

		public GameObject SecondaryPrefab;

		public int NumSecondaries;

		public float SecondaryVelocity;

		public float LegacyUpForce = 0.2f;

		public GameObject ShrapnelPrefab;

		public int ShrapnelAmount;

		public int ShrapnelPerFrame;

		private int ShrapnelSoFar;

		public Vector2 ShrapnelVelocityRange;

		public bool DoesRadiusDamage;

		public AnimationCurve DamageCurve;

		public float PointsDamageMax;

		public AnimationCurve DamageCurve_Stun;

		public float StunDamageMax = 0.5f;

		public AnimationCurve DamageCurve_Blind;

		public float BlindDamageMax;

		public AnimationCurve DamageCurve_EMP;

		public float EMPDamageMax;

		public bool DoesIgnite;

		public AnimationCurve IgnitionChanceCurve;

		public int IFF;

		public bool CanGenerateRocketJump;

		public Vector2 MinMaxRocketJumpRange = new Vector2(1.5f, 2.5f);

		public float RocketJumpVelocity = 20f;

		public bool UsesNormalizedForce;

		private void Awake()
		{
			Invoke("Explode", 0.05f);
		}

		private void OnDestroy()
		{
			hitRBs.Clear();
			hitRBsList.Clear();
		}

		private void Explode()
		{
			if (m_hasExploded)
			{
				return;
			}
			m_hasExploded = true;
			GM.CurrentSceneSettings.PingReceivers(base.transform.position);
			if (ExplosionRadius > 0.01f)
			{
				m_colliders = Physics.OverlapSphere(base.transform.position, ExplosionRadius, Mask_Explosion);
				if (ExplosiveForce > 0f)
				{
					for (int i = 0; i < m_colliders.Length; i++)
					{
						if (m_colliders[i].attachedRigidbody != null && hitRBs.Add(m_colliders[i].attachedRigidbody))
						{
							hitRBsList.Add(m_colliders[i].attachedRigidbody);
						}
					}
				}
			}
			if (DoesRadiusDamage)
			{
				float num = 0.5f;
				switch (SM.GetSoundEnvironment(base.transform.position))
				{
				case FVRSoundEnvironment.InsideSmall:
					num = 1.5f;
					break;
				case FVRSoundEnvironment.InsideNarrowSmall:
				case FVRSoundEnvironment.InsideMedium:
					num = 1.2f;
					break;
				case FVRSoundEnvironment.InsideNarrow:
				case FVRSoundEnvironment.InsideLarge:
					num = 1f;
					break;
				}
				Vector3 position = base.transform.position;
				if (UsesExplosiveForceOverride)
				{
					position = DirOverride.position;
				}
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				for (int j = 0; j < m_colliders.Length; j++)
				{
					if (!(m_colliders[j] != null))
					{
						continue;
					}
					IFVRDamageable component = m_colliders[j].gameObject.GetComponent<IFVRDamageable>();
					if (component == null && m_colliders[j].attachedRigidbody != null)
					{
						component = m_colliders[j].attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
					}
					if (!((MonoBehaviour)component != null))
					{
						continue;
					}
					Vector3 vector = m_colliders[j].transform.position - position;
					Vector3 normalized = vector.normalized;
					bool flag = false;
					if (UsesBlockers && Physics.Raycast(position, normalized, out var hitInfo, vector.magnitude, Mask_Blockers) && !hitInfo.collider.gameObject.CompareTag("noOcclude"))
					{
						flag = true;
					}
					float magnitude = vector.magnitude;
					if (DoesIgnite)
					{
						FVRIgnitable component2 = m_colliders[j].gameObject.GetComponent<FVRIgnitable>();
						if (component2 != null && Random.Range(0f, 1f) > IgnitionChanceCurve.Evaluate(magnitude))
						{
							FXM.Ignite(component2, 1f);
						}
					}
					num2 = DamageCurve.Evaluate(magnitude) * num;
					if (StunDamageMax > 0f)
					{
						num3 = DamageCurve_Stun.Evaluate(magnitude) * num;
					}
					if (BlindDamageMax > 0f)
					{
						num4 = DamageCurve_Blind.Evaluate(magnitude) * num;
					}
					if (EMPDamageMax > 0f)
					{
						num5 = DamageCurve_EMP.Evaluate(magnitude) * num;
					}
					if (flag)
					{
						num2 *= 0.01f;
						num3 *= 0.2f;
						num4 *= 0.01f;
						num5 *= 0.3f;
					}
					Damage damage = new Damage();
					damage.Dam_Piercing = PointsDamageMax * num2 * 0.1f;
					damage.Dam_Blunt = PointsDamageMax * num2 * 0.9f;
					damage.Dam_TotalKinetic = PointsDamageMax * num2;
					damage.Dam_Blinding = BlindDamageMax * num4;
					damage.Dam_Stunning = StunDamageMax * num3;
					damage.Dam_EMP = EMPDamageMax * num5;
					damage.hitNormal = -vector;
					damage.point = m_colliders[j].transform.position;
					damage.strikeDir = normalized;
					damage.Class = Damage.DamageClass.Explosive;
					damage.damageSize = 1f;
					damage.Source_IFF = IFF;
					if (!CanGenerateRocketJump)
					{
						component.Damage(damage);
					}
					else if (m_colliders[j].gameObject.GetComponent<FVRPlayerHitbox>() != null)
					{
						if (GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage == MF_BlastJumpingSelfDamage.Arcade)
						{
							damage.Dam_Piercing *= 0.04f;
							damage.Dam_Blunt *= 0.04f;
							damage.Dam_TotalKinetic *= 0.04f;
							damage.Dam_Blinding *= 0.04f;
							damage.Dam_Stunning *= 0.04f;
						}
						else if (GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage == MF_BlastJumpingSelfDamage.Realistic)
						{
							component.Damage(damage);
						}
					}
					else
					{
						component.Damage(damage);
					}
				}
			}
			if (HasSecondaries)
			{
				for (int k = 0; k < NumSecondaries; k++)
				{
					Vector3 onUnitSphere = Random.onUnitSphere;
					GameObject gameObject = Object.Instantiate(SecondaryPrefab, base.transform.position + onUnitSphere * 0.025f, Quaternion.identity);
					Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
					component3.velocity = onUnitSphere * SecondaryVelocity * Random.Range(0.5f, 1f);
				}
			}
			if (CanGenerateRocketJump && GM.MFFlags.PlayerSetting_BlastJumping == MF_BlastJumping.On)
			{
				GM.CurrentMovementManager.RocketJump(base.transform.position, MinMaxRocketJumpRange, RocketJumpVelocity);
			}
		}

		private void Update()
		{
			if (m_hasExploded && ShrapnelPrefab != null && ShrapnelSoFar < ShrapnelAmount)
			{
				for (int i = 0; i < ShrapnelPerFrame; i++)
				{
					if (ShrapnelSoFar < ShrapnelAmount)
					{
						GameObject gameObject = Object.Instantiate(ShrapnelPrefab, base.transform.position, Random.rotation);
						BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
						if (component != null)
						{
							component.Fire(Random.Range(ShrapnelVelocityRange.x, ShrapnelVelocityRange.y), gameObject.transform.forward, null);
							component.Source_IFF = IFF;
							ShrapnelSoFar++;
						}
						ShrapnelSoFar++;
					}
				}
			}
			int num = currentIndex;
			Vector3 position = base.transform.position;
			float num2 = ExplosionRadius;
			if (UsesExplosiveForceOverride)
			{
				position = DirOverride.position;
				num2 *= 3f;
			}
			if (hitRBsList.Count <= 0 || currentIndex >= hitRBsList.Count)
			{
				return;
			}
			for (int j = num; j < Mathf.Min(num + MaxChecksPerFrame, hitRBsList.Count); j++)
			{
				if (hitRBsList[j] != null)
				{
					Vector3 vector = hitRBsList[j].position - position;
					Vector3 normalized = vector.normalized;
					if (!Physics.Raycast(position, normalized, vector.magnitude, Mask_Blockers) && !hitRBsList[j].isKinematic)
					{
						if (UsesNormalizedForce)
						{
							hitRBsList[j].AddExplosionForce(ExplosiveForce, position, num2, LegacyUpForce, ForceMode.VelocityChange);
						}
						else
						{
							hitRBsList[j].AddExplosionForce(ExplosiveForce, position, num2, LegacyUpForce, ForceMode.Impulse);
						}
					}
				}
				currentIndex++;
			}
		}
	}
}
