using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SosigLink : MonoBehaviour, IFVRDamageable, IVaporizable
	{
		public enum SosigBodyPart
		{
			Head,
			Torso,
			UpperLink,
			LowerLink
		}

		[Header("Connections")]
		public Sosig S;

		public FVRPhysicalObject O;

		public Collider C;

		public Rigidbody R;

		public CharacterJoint J;

		public SosigBodyPart BodyPart;

		[Header("Damage Stuff")]
		public float StaggerMagnitude = 1f;

		public float DamMult = 1f;

		public float CollisionBluntDamageMultiplier = 1f;

		private float m_fullintegrity = 100f;

		private float m_integrity = 100f;

		private bool m_isExploded;

		private bool m_isJointBroken;

		private bool m_isJointSevered;

		[Header("Audio Stuff")]
		public AudioEvent AudEvent_JointBreak;

		public AudioEvent AudEvent_JointSever;

		private List<SosigWearable> m_wearables = new List<SosigWearable>();

		private List<Collider> m_wearableColliders = new List<Collider>();

		[Header("Spawn Stuff")]
		public bool HasSpawnOnDestroy;

		public FVRObject SpawnOnDestroy;

		[Header("Meshes")]
		public Mesh[] Meshes_Whole;

		public Mesh[] Meshes_Severed_Top;

		public Mesh[] Meshes_Severed_Bottom;

		public Mesh[] Meshes_Severed_Both;

		private RaycastHit m_hit;

		private bool m_hasJustBeenSevered;

		private float timeSinceCollision = 0.1f;

		private float m_timeSeperate;

		public bool IsExploded => m_isExploded;

		public void SetColDamMult(float f)
		{
			CollisionBluntDamageMultiplier = Mathf.Min(CollisionBluntDamageMultiplier, f);
		}

		public float GetIntegrityRatio()
		{
			return Mathf.Clamp(m_integrity / m_fullintegrity, 0f, 1f);
		}

		public void SetIntegrity(float f)
		{
			m_integrity = f;
		}

		public void HealIntegrity(float percentage)
		{
			float integrity = m_integrity;
			float num = m_fullintegrity * percentage;
			m_integrity += num;
			m_integrity = Mathf.Clamp(m_integrity, m_integrity, m_fullintegrity);
			if (m_integrity > integrity)
			{
				S.UpdateRendererOnLink((int)BodyPart);
			}
		}

		public void RemoveIntegrity(float percentage, Damage.DamageClass c)
		{
			float integrity = m_integrity;
			float num = m_fullintegrity * percentage;
			m_integrity -= num;
			if (m_integrity < integrity)
			{
				if (m_integrity <= 0f)
				{
					LinkExplodes(c);
				}
				else
				{
					S.UpdateRendererOnLink((int)BodyPart);
				}
			}
		}

		public void Vaporize(int IFF)
		{
			S.Vaporize(S.DamageFX_Vaporize, IFF);
		}

		public void RegisterWearable(SosigWearable w)
		{
			if (!m_wearables.Contains(w))
			{
				m_wearables.Add(w);
			}
			for (int i = 0; i < w.Cols.Count; i++)
			{
				m_wearableColliders.Add(w.Cols[i]);
			}
		}

		public void DeRegisterWearable(SosigWearable w)
		{
			if (m_wearables.Contains(w))
			{
				m_wearables.Remove(w);
			}
			for (int i = 0; i < w.Cols.Count; i++)
			{
				m_wearableColliders.Remove(w.Cols[i]);
			}
		}

		public void DisableWearables()
		{
			for (int i = 0; i < m_wearables.Count; i++)
			{
				m_wearables[i].Hide();
			}
		}

		public void EnableWearables()
		{
			for (int i = 0; i < m_wearables.Count; i++)
			{
				m_wearables[i].Show();
			}
		}

		public void RegisterSpawnOnDestroy(FVRObject o)
		{
			SpawnOnDestroy = o;
			HasSpawnOnDestroy = true;
		}

		public void BreakJoint(bool isStart, Damage.DamageClass damClass)
		{
			if (!m_isJointBroken && J != null)
			{
				m_isJointBroken = true;
				if (!isStart)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_JointBreak, base.transform.position);
				}
				S.BreakJoint(this, isStart, damClass);
			}
		}

		private void SeverJoint(Damage.DamageClass damClass, bool isPullApart)
		{
			if (!m_isJointSevered && J != null)
			{
				m_isJointSevered = true;
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_JointSever, base.transform.position);
				S.SeverJoint(this, isJointSlice: false, damClass, isPullApart);
			}
		}

		public int GetDamageStateIndex()
		{
			if (m_integrity > 70f)
			{
				return 0;
			}
			if (m_integrity > 40f)
			{
				return 1;
			}
			return 2;
		}

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				float num = Vector3.Angle(d.strikeDir, -d.hitNormal);
				float num2 = Mathf.Clamp((1f - num / 90f) * 1.5f, 0.4f, 1.5f);
				d.Dam_Blunt *= num2;
				d.Dam_Cutting *= num2;
				d.Dam_Piercing *= num2;
				d.Dam_TotalKinetic *= num2;
			}
			if (S.IsFrozen)
			{
				d.Dam_Blunt *= 3f;
				d.Dam_Cutting *= 0.2f;
				d.Dam_Piercing *= 0.02f;
			}
			if (S != null)
			{
				if (S.IsInvuln)
				{
					return;
				}
				float num3 = 1f;
				if (S.IsDamResist || S.IsDamMult)
				{
					num3 = S.BuffIntensity_DamResistHarm;
				}
				if (S.IsFragile)
				{
					num3 *= 100f;
				}
				d.Dam_Blunt *= num3;
				d.Dam_Cutting *= num3;
				d.Dam_Piercing *= num3;
				d.Dam_TotalKinetic *= num3;
				d.Dam_Thermal *= num3;
				d.Dam_Chilling *= num3;
				d.Dam_EMP *= num3;
				if (d.Class == FistVR.Damage.DamageClass.Projectile)
				{
					d.Dam_Blunt *= S.DamMult_Projectile;
					d.Dam_Cutting *= S.DamMult_Projectile;
					d.Dam_Piercing *= S.DamMult_Projectile;
					d.Dam_TotalKinetic *= S.DamMult_Projectile;
				}
				else if (d.Class == FistVR.Damage.DamageClass.Melee)
				{
					d.Dam_Blunt *= S.DamMult_Melee;
					d.Dam_Cutting *= S.DamMult_Melee;
					d.Dam_Piercing *= S.DamMult_Melee;
					d.Dam_TotalKinetic *= S.DamMult_Melee;
				}
				else if (d.Class == FistVR.Damage.DamageClass.Explosive)
				{
					d.Dam_Blunt *= S.DamMult_Explosive;
					d.Dam_Cutting *= S.DamMult_Explosive;
					d.Dam_Piercing *= S.DamMult_Explosive;
					d.Dam_TotalKinetic *= S.DamMult_Explosive;
				}
			}
			if (d.Class == FistVR.Damage.DamageClass.Melee && m_wearableColliders.Count > 0)
			{
				SosigWearable outwear = null;
				if (HitsWearable(d.point + d.hitNormal, -d.hitNormal, 1.5f, out outwear))
				{
					d.Dam_Blunt *= outwear.MeleeDamMult_Blunt;
					d.Dam_Cutting *= outwear.MeleeDamMult_Cutting;
					d.Dam_Piercing *= outwear.MeleeDamMult_Piercing;
					d.Dam_TotalKinetic = d.Dam_Blunt + d.Dam_Cutting + d.Dam_Piercing;
				}
			}
			d.Dam_Thermal *= S.DamMult_Thermal;
			d.Dam_Chilling *= S.DamMult_Chilling;
			d.Dam_EMP *= S.DamMult_EMP;
			S.ProcessDamage(d, this);
			bool flag = false;
			if (m_integrity > 0f)
			{
				flag = DamageIntegrity(d);
			}
			if (d.Dam_TotalKinetic > 80f)
			{
				if (d.Class == FistVR.Damage.DamageClass.Projectile)
				{
					S.RequestHitDecal(d.point, d.hitNormal, d.damageSize * 2f, this);
				}
				else if (d.Class == FistVR.Damage.DamageClass.Melee && !flag)
				{
					if (d.Dam_Blunt > d.Dam_Cutting && d.Dam_Blunt > d.Dam_Piercing && d.Dam_Blunt > 100f)
					{
						S.RequestHitDecal(d.point, d.hitNormal, d.damageSize * 2f, this);
					}
					else if (d.Dam_Cutting > d.Dam_Piercing && d.Dam_Cutting > 100f && d.edgeNormal.magnitude > 0.1f)
					{
						S.RequestHitDecal(d.point, d.hitNormal, d.edgeNormal, d.damageSize * 2f, this);
					}
					else if (d.Dam_Piercing > 100f)
					{
						S.RequestHitDecal(d.point, d.hitNormal, d.damageSize * 2f, this);
					}
				}
			}
			if (S.Mustard > 0f && !flag)
			{
				float num4 = (d.Dam_Piercing + d.Dam_Cutting - 50f) * 0.05f;
				num4 = Mathf.Clamp(num4 * DamMult, 0f, 100f);
				S.SetLastIFFDamageSource(d.Source_IFF);
				S.AccurueBleedingHit(this, d.point, d.strikeDir, num4);
			}
		}

		public bool HitsWearable(Vector3 startPoint, Vector3 direction, float distance, out SosigWearable outwear)
		{
			bool flag = false;
			outwear = null;
			if (C.Raycast(new Ray(startPoint, direction), out m_hit, distance))
			{
				float distance2 = m_hit.distance;
				flag = false;
				SosigWearable sosigWearable = null;
				for (int i = 0; i < m_wearables.Count; i++)
				{
					SosigWearable sosigWearable2 = m_wearables[i];
					if (sosigWearable2.Cols.Count <= 0)
					{
						continue;
					}
					for (int j = 0; j < sosigWearable2.Cols.Count; j++)
					{
						Collider collider = sosigWearable2.Cols[j];
						if (collider.Raycast(new Ray(startPoint, direction), out m_hit, distance))
						{
							float distance3 = m_hit.distance;
							if (distance3 < distance2)
							{
								flag = true;
								sosigWearable = sosigWearable2;
								outwear = sosigWearable2;
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		public bool GetHasJustBeenSevered()
		{
			return m_hasJustBeenSevered;
		}

		public bool DamageIntegrity(Damage d)
		{
			return DamageIntegrity(d.Dam_Blunt, d.Dam_Piercing, d.Dam_Cutting, d.Dam_Thermal, d.strikeDir, d.point, d.Class, d.Source_IFF);
		}

		public bool DamageIntegrity(float b, float p, float c, float t, Vector3 dir, Vector3 pos, Damage.DamageClass damClass, int iff)
		{
			if (S.IsInvuln)
			{
				return false;
			}
			S.SetLastIFFDamageSource(iff);
			if (S.AppliesDamageResistToIntegrityLoss)
			{
				p *= S.DamMult_Piercing;
				c *= S.DamMult_Cutting;
				b *= S.DamMult_Blunt;
				t *= S.DamMult_Thermal;
			}
			float num = b * 0.032f;
			num += Mathf.Clamp((p - 500f) * 0.008f, 0f, 100f);
			num += Mathf.Clamp((c - 500f) * 0.008f, 0f, 100f);
			num += Mathf.Clamp(t * 0.01f, 0f, 100f);
			num = Mathf.Clamp(num * DamMult, 0f, 100f);
			if (S.CanBeSevered && c > 350f && c > b && J != null && Vector3.Distance(pos, base.transform.position) > 0.12f && Vector3.Angle(dir, base.transform.up) > 65f && Vector3.Angle(dir, base.transform.up) < 115f)
			{
				m_hasJustBeenSevered = true;
				SeverJoint(damClass, isPullApart: false);
				return true;
			}
			m_integrity -= num;
			if (m_integrity <= 0f)
			{
				LinkExplodes(damClass);
				return true;
			}
			S.UpdateRendererOnLink((int)BodyPart);
			return false;
		}

		public void LinkExplodes(Damage.DamageClass damClass)
		{
			if (!m_isExploded)
			{
				m_isExploded = true;
				S.DestroyLink(this, damClass);
			}
		}

		public void OnCollisionEnter(Collision col)
		{
			ProcessCollision(col);
		}

		private void ProcessCollision(Collision col)
		{
			if (!(timeSinceCollision > 0f))
			{
				S.ProcessCollision(this, col);
				timeSinceCollision = 0.1f;
			}
		}

		public void Update()
		{
			if (timeSinceCollision >= 0f)
			{
				timeSinceCollision -= Time.deltaTime;
			}
			bool flag = S.CanCurrentlyBeHeld();
			if (O.IsHeld)
			{
				if (!flag)
				{
					O.ForceBreakInteraction();
				}
				if (S.CoreRB != null)
				{
					float num = S.DistanceFromCoreTarget();
					if (num > 1f)
					{
						S.Stagger(1f);
					}
				}
			}
			if (!(J != null) || !(J.connectedBody != null))
			{
				return;
			}
			SosigLink component = J.connectedBody.gameObject.GetComponent<SosigLink>();
			if (!(component != null))
			{
				return;
			}
			if (O.IsHeld && component.O.IsHeld && (Vector3.Angle(base.transform.up, component.transform.up) > 135f || Vector3.Angle(base.transform.forward, component.transform.forward) > 75f))
			{
				S.SetLastIFFDamageSource(GM.CurrentPlayerBody.GetPlayerIFF());
				BreakJoint(isStart: false, FistVR.Damage.DamageClass.Melee);
			}
			float num2 = Vector3.Distance(base.transform.position, component.transform.position);
			if (num2 > 0.5f)
			{
				m_timeSeperate += Time.deltaTime;
				if (m_timeSeperate > 0.1f)
				{
					if (O.IsHeld || component.O.IsHeld)
					{
						S.SetLastIFFDamageSource(GM.CurrentPlayerBody.GetPlayerIFF());
					}
					SeverJoint(FistVR.Damage.DamageClass.Melee, isPullApart: true);
				}
			}
			else
			{
				m_timeSeperate = 0f;
			}
		}

		public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;
			float num = Vector3.Dot(lineVec1, lineVec1);
			float num2 = Vector3.Dot(lineVec1, lineVec2);
			float num3 = Vector3.Dot(lineVec2, lineVec2);
			float num4 = num * num3 - num2 * num2;
			if (num4 != 0f)
			{
				Vector3 rhs = linePoint1 - linePoint2;
				float num5 = Vector3.Dot(lineVec1, rhs);
				float num6 = Vector3.Dot(lineVec2, rhs);
				float num7 = (num2 * num6 - num5 * num3) / num4;
				float num8 = (num * num6 - num5 * num2) / num4;
				closestPointLine1 = linePoint1 + lineVec1 * num7;
				closestPointLine2 = linePoint2 + lineVec2 * num8;
				return true;
			}
			return false;
		}
	}
}
