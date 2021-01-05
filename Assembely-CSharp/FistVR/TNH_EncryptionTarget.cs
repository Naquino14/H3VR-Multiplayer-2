using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_EncryptionTarget : MonoBehaviour, IFVRDamageable
	{
		public enum ForceDir
		{
			Outward,
			Random,
			Forward
		}

		[Header("Core")]
		public TNH_EncryptionType Type;

		private TNH_HoldPoint m_point;

		public int NumHitsTilDestroyed = 1;

		private int m_maxHits = 1;

		private int m_numHitsLeft = 1;

		public bool UsesDamagePerHit;

		public float DamagePerHit = 500f;

		private float m_damLeftForAHit = 500f;

		public bool StartsRandomRot;

		public float RandomRotMag = 1f;

		[Header("SubParts")]
		public List<GameObject> SpawnOnDestruction;

		public List<Transform> SpawnPoints;

		public bool AreSomeSubpartsTargets;

		public bool UseSubpartForce;

		public Vector2 SubpartVelRange;

		public ForceDir SubpartDir;

		[Header("Display")]
		public bool UsesMultipleDisplay;

		public List<GameObject> DisplayList;

		[Header("Flash")]
		public bool FlashOnDestroy;

		public Color FlashColor;

		public float FlashIntensity;

		public float FlashRange;

		[Header("Required SubTargets")]
		public bool UsesSubTargs;

		public List<GameObject> SubTargs;

		private int m_numSubTargsLeft;

		[Header("Recursive SubTargets")]
		public bool UsesRecursiveSubTarg;

		public int StartingSubTargs = 3;

		[Header("AgileMovement")]
		public bool UsesAgileMovement;

		public Rigidbody RB;

		public LayerMask LM_AgileMove;

		public Transform AgilePointer;

		public float CastRadius = 0.4f;

		public float DistFromSpawnMax = 10f;

		private Vector3 agileStartPos;

		private float m_nextDist;

		private List<Vector3> m_validAgilePos;

		private RaycastHit m_hit;

		private Quaternion m_fromRot;

		private float m_timeTilWarp;

		private float m_warpSpeed = 4f;

		private float m_warpSpeedFactor = 1f;

		private Vector3 nextWarpPos;

		private Quaternion nextWarpRot;

		private bool m_hasNextWarp;

		public AudioEvent AudEvent_WarpFrom;

		public AudioEvent AudEvent_WarpTo;

		[Header("RegenerativeSubTargets")]
		public bool UsesRegenerativeSubTarg;

		public int StartingRegenSubTarg = 5;

		public float MaxGrowthDistance = 16f;

		private float m_regenRotPieceRot;

		public LayerMask LM_RegenPlacement;

		public List<GameObject> Tendrils;

		public List<float> TendrilFloats;

		public List<Vector3> GrowthPoints;

		public LayerMask LM_Regen;

		public Transform Core2;

		private float m_regenGrowthSpeed = 1f;

		[Header("Sound")]
		public bool SoundOnDamage;

		public AudioEvent AudEvent_SoundOnDamage;

		private bool m_isDestroyed;

		private float damRefireLimited;

		public void SetHoldPoint(TNH_HoldPoint p)
		{
			m_point = p;
		}

		public void Start()
		{
			m_numHitsLeft = NumHitsTilDestroyed;
			m_maxHits = NumHitsTilDestroyed;
			m_damLeftForAHit = DamagePerHit;
			agileStartPos = base.transform.position;
			m_fromRot = base.transform.rotation;
			m_timeTilWarp = 0f;
			m_warpSpeed = Random.Range(4f, 5f);
			if (UsesAgileMovement)
			{
				m_validAgilePos = new List<Vector3>();
			}
			if (UsesRegenerativeSubTarg)
			{
				for (int i = 0; i < Tendrils.Count; i++)
				{
					Tendrils[i].transform.SetParent(null);
					SubTargs[i].transform.SetParent(null);
				}
				PopulateInitialRegen();
			}
			if (UsesRecursiveSubTarg)
			{
				for (int j = 0; j < StartingSubTargs; j++)
				{
					int index = Random.Range(0, SubTargs.Count);
					if (!SubTargs[index].activeSelf)
					{
						SubTargs[index].SetActive(value: true);
						m_numSubTargsLeft++;
					}
				}
			}
			if (UsesSubTargs && !UsesRecursiveSubTarg && !UsesRegenerativeSubTarg)
			{
				m_numSubTargsLeft = SubTargs.Count;
			}
			if (StartsRandomRot)
			{
				base.transform.rotation = Random.rotation;
				GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * RandomRotMag;
			}
		}

		private void PopulateInitialRegen()
		{
			for (int i = 0; i < StartingRegenSubTarg; i++)
			{
				Vector3 position = base.transform.position;
				Vector3 onUnitSphere = Random.onUnitSphere;
				float num = MaxGrowthDistance;
				if (Physics.Raycast(position, onUnitSphere, out m_hit, num, LM_Regen))
				{
					num = m_hit.distance;
				}
				SpawnGrowth(i, position + onUnitSphere * num);
			}
			m_numSubTargsLeft = StartingRegenSubTarg;
		}

		private void ResetGrowth(int index, Vector3 point)
		{
			GrowthPoints[index] = point;
			TendrilFloats[index] = 0f;
			Vector3 forward = point - Tendrils[index].transform.position;
			Tendrils[index].transform.rotation = Quaternion.LookRotation(forward);
			Tendrils[index].transform.localScale = new Vector3(0.2f, 0.2f, forward.magnitude);
		}

		private void SpawnGrowth(int index, Vector3 point)
		{
			if (!SubTargs[index].activeSelf)
			{
				Tendrils[index].SetActive(value: true);
				GrowthPoints[index] = point;
				SubTargs[index].transform.position = point;
				SubTargs[index].SetActive(value: true);
				TendrilFloats[index] = 1f;
				Vector3 forward = point - Tendrils[index].transform.position;
				Tendrils[index].transform.rotation = Quaternion.LookRotation(forward);
				Tendrils[index].transform.localScale = new Vector3(0.2f, 0.2f, forward.magnitude);
				SubTargs[index].transform.rotation = Random.rotation;
				m_numSubTargsLeft++;
			}
		}

		public void DisableSubtarg(int i)
		{
			if (!SubTargs[i].activeSelf)
			{
				return;
			}
			if (UsesRegenerativeSubTarg)
			{
				Vector3 position = base.transform.position;
				Vector3 onUnitSphere = Random.onUnitSphere;
				float num = MaxGrowthDistance;
				if (Physics.Raycast(position, onUnitSphere, out m_hit, num, LM_Regen))
				{
					num = m_hit.distance;
				}
				ResetGrowth(i, position + onUnitSphere * num);
			}
			m_numSubTargsLeft--;
			if (m_numSubTargsLeft <= 0)
			{
				Destroy();
				return;
			}
			if (UsesRegenerativeSubTarg)
			{
				Tendrils[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			}
			SubTargs[i].SetActive(value: false);
			if (SoundOnDamage)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_SoundOnDamage, base.transform.position);
			}
		}

		private void RespawnRandomSubTarg()
		{
			int index = Random.Range(0, SubTargs.Count);
			if (!SubTargs[index].activeSelf)
			{
				SubTargs[index].SetActive(value: true);
				m_numSubTargsLeft++;
			}
		}

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Explosive)
			{
				return;
			}
			damRefireLimited = 0.05f;
			if (UsesRecursiveSubTarg)
			{
				RespawnRandomSubTarg();
			}
			if (UsesRegenerativeSubTarg)
			{
				m_regenGrowthSpeed = 0f;
			}
			if (UsesSubTargs)
			{
				return;
			}
			if (!UsesDamagePerHit)
			{
				m_numHitsLeft--;
			}
			else
			{
				float dam_TotalKinetic = d.Dam_TotalKinetic;
				if (dam_TotalKinetic <= m_damLeftForAHit)
				{
					m_damLeftForAHit -= dam_TotalKinetic;
					if (m_damLeftForAHit <= 0f)
					{
						m_damLeftForAHit = DamagePerHit;
						m_numHitsLeft--;
					}
				}
				else
				{
					m_numHitsLeft--;
					dam_TotalKinetic -= m_damLeftForAHit;
					int num = Mathf.FloorToInt(dam_TotalKinetic / DamagePerHit);
					if (num > 1)
					{
						num--;
					}
					m_numHitsLeft -= num;
					m_damLeftForAHit = dam_TotalKinetic % DamagePerHit;
				}
			}
			if (m_numHitsLeft <= 0)
			{
				Destroy();
				return;
			}
			if (UsesMultipleDisplay)
			{
				UpdateDisplay();
			}
			if (damRefireLimited <= 0f && SoundOnDamage)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AudEvent_SoundOnDamage, base.transform.position);
			}
		}

		private void UpdateDisplay()
		{
			int numHitsLeft = m_numHitsLeft;
			for (int i = 0; i < DisplayList.Count; i++)
			{
				if (i == numHitsLeft)
				{
					DisplayList[i].SetActive(value: true);
				}
				else
				{
					DisplayList[i].SetActive(value: false);
				}
			}
		}

		public void Update()
		{
			if (damRefireLimited > 0f)
			{
				damRefireLimited -= Time.deltaTime;
			}
			if (UsesAgileMovement && (float)m_validAgilePos.Count < 20f)
			{
				Vector3 vector = agileStartPos;
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y *= 0.2f;
				onUnitSphere.Normalize();
				Vector3 item = vector + onUnitSphere * Random.Range(DistFromSpawnMax * 0.2f, DistFromSpawnMax * 0.8f);
				if (Physics.SphereCast(vector, CastRadius, onUnitSphere, out m_hit, DistFromSpawnMax, LM_AgileMove))
				{
					item = vector + onUnitSphere * Random.Range(m_hit.distance * 0.5f, m_hit.distance * 0.8f);
				}
				m_validAgilePos.Add(item);
			}
			if (!UsesRegenerativeSubTarg)
			{
				return;
			}
			m_regenRotPieceRot = Mathf.Repeat(m_regenRotPieceRot + m_regenGrowthSpeed * m_regenGrowthSpeed * m_regenGrowthSpeed * 360f * Time.deltaTime, 360f);
			Core2.localEulerAngles = new Vector3(m_regenRotPieceRot, m_regenRotPieceRot, m_regenRotPieceRot);
			m_regenGrowthSpeed = Mathf.MoveTowards(m_regenGrowthSpeed, 1f, Time.deltaTime * 0.2f);
			for (int i = 0; i < SubTargs.Count; i++)
			{
				if (!SubTargs[i].activeSelf)
				{
					TendrilFloats[i] += Time.deltaTime * (m_regenGrowthSpeed * m_regenGrowthSpeed);
					if (TendrilFloats[i] >= 1f)
					{
						SpawnGrowth(i, GrowthPoints[i]);
						continue;
					}
					Vector3 vector2 = GrowthPoints[i] - base.transform.position;
					Tendrils[i].transform.localScale = new Vector3(0.2f, 0.2f, vector2.magnitude * TendrilFloats[i]);
				}
			}
		}

		private void FixedUpdate()
		{
			if (!UsesAgileMovement || m_validAgilePos.Count <= 0)
			{
				return;
			}
			if (!m_hasNextWarp)
			{
				m_fromRot = base.transform.rotation;
				int index = Random.Range(0, m_validAgilePos.Count);
				nextWarpPos = m_validAgilePos[index];
				m_validAgilePos.RemoveAt(index);
				nextWarpRot = Quaternion.LookRotation(nextWarpPos - base.transform.position);
				m_hasNextWarp = true;
				m_timeTilWarp = 0f;
				m_nextDist = Vector3.Distance(base.transform.position, nextWarpPos);
				return;
			}
			m_timeTilWarp += Time.deltaTime * m_warpSpeed;
			if (m_timeTilWarp > 1f)
			{
				m_warpSpeed = Random.Range(2f, 4f) * m_warpSpeedFactor;
				m_warpSpeedFactor *= 0.92f;
				m_hasNextWarp = false;
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_WarpFrom, base.transform.position);
				RB.position = nextWarpPos;
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_WarpTo, nextWarpPos, 0.2f);
				AgilePointer.localScale = new Vector3(0.24f, 0.24f, 0.24f);
			}
			else
			{
				RB.rotation = Quaternion.Slerp(m_fromRot, nextWarpRot, m_timeTilWarp * 2f);
				if (m_timeTilWarp > 0.5f)
				{
					AgilePointer.localScale = new Vector3(0.1f, 0.1f, (m_timeTilWarp - 0.5f) * 2f * m_nextDist * 1.35137212f);
				}
			}
		}

		private void Destroy()
		{
			if (m_isDestroyed)
			{
				return;
			}
			m_isDestroyed = true;
			if (UsesRegenerativeSubTarg)
			{
				for (int i = 0; i < Tendrils.Count; i++)
				{
					Object.Destroy(Tendrils[i]);
					Object.Destroy(SubTargs[i]);
				}
			}
			if (FlashOnDestroy)
			{
				FXM.InitiateMuzzleFlash(base.transform.position, Vector3.up, FlashIntensity, FlashColor, FlashRange);
			}
			for (int j = 0; j < SpawnOnDestruction.Count; j++)
			{
				GameObject gameObject = Object.Instantiate(SpawnOnDestruction[j], SpawnPoints[j].position, SpawnPoints[j].rotation);
				if (AreSomeSubpartsTargets)
				{
					TNH_EncryptionTarget component = gameObject.GetComponent<TNH_EncryptionTarget>();
					if (component != null)
					{
						component.SetHoldPoint(m_point);
					}
					m_point.RegisterNewTarget(this);
				}
				if (!UseSubpartForce)
				{
					continue;
				}
				Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
				if (component2 != null)
				{
					switch (SubpartDir)
					{
					case ForceDir.Outward:
					{
						Vector3 vector2 = (component2.velocity = (gameObject.transform.position - base.transform.position).normalized * Random.Range(SubpartVelRange.x, SubpartVelRange.y));
						component2.angularVelocity = Random.onUnitSphere;
						break;
					}
					case ForceDir.Random:
						component2.velocity = Random.onUnitSphere * Random.Range(SubpartVelRange.x, SubpartVelRange.y);
						component2.angularVelocity = Random.onUnitSphere;
						break;
					case ForceDir.Forward:
						component2.velocity = SpawnPoints[j].forward * Random.Range(SubpartVelRange.x, SubpartVelRange.y);
						break;
					}
				}
			}
			if (m_point != null)
			{
				m_point.TargetDestroyed(this);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
