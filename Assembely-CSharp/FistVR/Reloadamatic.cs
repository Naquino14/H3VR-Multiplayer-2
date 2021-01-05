using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class Reloadamatic : MonoBehaviour
	{
		public Transform Accordian;

		private float m_accordianLerp;

		private bool m_isAnimating;

		public AnimationCurve AccordianCurve;

		public float AccordianSpeed = 1f;

		public Text DisplayText;

		public AudioEvent AudEvent_Accordian;

		public AudioEvent AudEvent_ReloadSuccess;

		public AudioEvent AudEvent_ReloadFailure;

		public bool SpawnsDefault = true;

		private Dictionary<FireArmRoundType, FireArmRoundClass> m_decidedTypes = new Dictionary<FireArmRoundType, FireArmRoundClass>();

		private bool m_hasDispensibleRound;

		private FireArmRoundType m_currentDispenseType;

		private AttachableFirearmPhysicalObject m_detectedAttachedFirearm;

		private FVRFireArmMagazine m_detectedMagazine;

		private FVRFireArmClip m_detectedClip;

		private Speedloader m_detectedSpeedloader;

		public Transform SpawnedRoundPoint;

		public LayerMask LM_MagDetectOverlay;

		public Transform MagDetectCenter;

		public Vector3 MagDetectExtends;

		private Collider[] colbuffer;

		private float m_scanTick = 1f;

		public void Start()
		{
			UpdateDisplay(shouldScan: true, FireArmRoundType.a9_18_Makarov, FireArmRoundClass.JHP);
			colbuffer = new Collider[50];
		}

		public void SetSpawnsDefault(bool b)
		{
			SpawnsDefault = b;
		}

		private FireArmRoundClass GetClassFromType(FireArmRoundType t)
		{
			if (!m_decidedTypes.ContainsKey(t))
			{
				if (SpawnsDefault)
				{
					FireArmRoundClass defaultRoundClass = AM.GetDefaultRoundClass(t);
					m_decidedTypes.Add(t, defaultRoundClass);
				}
				else
				{
					FireArmRoundClass randomNonDefaultRoundClass = AM.GetRandomNonDefaultRoundClass(t);
					m_decidedTypes.Add(t, randomNonDefaultRoundClass);
				}
			}
			return m_decidedTypes[t];
		}

		public void UpdateDisplay(bool shouldScan, FireArmRoundType RoundType, FireArmRoundClass RoundClass)
		{
			if (shouldScan)
			{
				DisplayText.text = "Place Gun, Mag, Clip\nor Speedloader On Bed";
			}
			else
			{
				DisplayText.text = AM.GetFullRoundName(RoundType, RoundClass);
			}
		}

		public void DispenseRound(int v)
		{
			if (m_hasDispensibleRound)
			{
				GameObject gameObject = AM.GetRoundSelfPrefab(m_currentDispenseType, GetClassFromType(m_currentDispenseType)).GetGameObject();
				Object.Instantiate(gameObject, SpawnedRoundPoint.position, SpawnedRoundPoint.rotation);
				m_accordianLerp = 0f;
				m_isAnimating = true;
				SM.PlayGenericSound(AudEvent_Accordian, base.transform.position);
			}
			else
			{
				SM.PlayGenericSound(AudEvent_ReloadFailure, base.transform.position);
			}
		}

		public void Reload(int v)
		{
			Scan();
			bool flag = false;
			if (m_detectedMagazine != null)
			{
				m_detectedMagazine.ReloadMagWithType(GetClassFromType(m_currentDispenseType));
				flag = true;
			}
			else if (m_detectedClip != null)
			{
				m_detectedClip.ReloadClipWithType(GetClassFromType(m_currentDispenseType));
				flag = true;
			}
			else if (m_detectedSpeedloader != null)
			{
				m_detectedSpeedloader.ReloadClipWithType(GetClassFromType(m_currentDispenseType));
				flag = true;
			}
			if (flag)
			{
				m_accordianLerp = 0f;
				m_isAnimating = true;
				SM.PlayGenericSound(AudEvent_Accordian, base.transform.position);
				SM.PlayGenericSound(AudEvent_ReloadSuccess, base.transform.position);
			}
			else
			{
				SM.PlayGenericSound(AudEvent_ReloadFailure, base.transform.position);
			}
		}

		private void Update()
		{
			Accordianing();
			m_scanTick -= Time.deltaTime;
			if (m_scanTick <= 0f)
			{
				m_scanTick = Random.Range(0.8f, 1f);
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				if (num < 12f)
				{
					Scan();
				}
			}
		}

		private void Accordianing()
		{
			if (m_isAnimating)
			{
				m_accordianLerp += Time.deltaTime;
				float y = AccordianCurve.Evaluate(m_accordianLerp);
				Accordian.localScale = new Vector3(1f, y, 1f);
				if (m_accordianLerp > 1f)
				{
					m_isAnimating = false;
				}
			}
		}

		private void ClearScanned()
		{
			m_hasDispensibleRound = false;
			m_detectedAttachedFirearm = null;
			m_detectedMagazine = null;
			m_detectedClip = null;
			m_detectedSpeedloader = null;
		}

		private void SetScanned(FVRFireArm f)
		{
			m_hasDispensibleRound = true;
			SetDispenseType(f.RoundType);
		}

		private void SetScanned(AttachableFirearmPhysicalObject af)
		{
			m_hasDispensibleRound = true;
			SetDispenseType(af.FA.RoundType);
			m_detectedAttachedFirearm = af;
		}

		private void SetScanned(FVRFireArmMagazine m)
		{
			m_hasDispensibleRound = true;
			SetDispenseType(m.RoundType);
			m_detectedMagazine = m;
		}

		private void SetScanned(FVRFireArmClip c)
		{
			m_hasDispensibleRound = true;
			SetDispenseType(c.RoundType);
			m_detectedClip = c;
		}

		private void SetScanned(Speedloader s)
		{
			m_hasDispensibleRound = true;
			SetDispenseType(s.Chambers[0].Type);
			m_detectedSpeedloader = s;
		}

		private void SetDispenseType(FireArmRoundType t)
		{
			m_currentDispenseType = t;
			UpdateDisplay(shouldScan: false, t, GetClassFromType(t));
		}

		private void Scan()
		{
			int num = Physics.OverlapBoxNonAlloc(MagDetectCenter.position, MagDetectExtends, colbuffer, MagDetectCenter.rotation, LM_MagDetectOverlay, QueryTriggerInteraction.Collide);
			ClearScanned();
			for (int i = 0; i < num; i++)
			{
				if (!(colbuffer[i].attachedRigidbody != null))
				{
					continue;
				}
				FVRFireArm component = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
				if (component != null)
				{
					if (component.Magazine == null)
					{
						SetScanned(component);
					}
					else
					{
						SetScanned(component.Magazine);
					}
					break;
				}
				AttachableFirearmPhysicalObject component2 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<AttachableFirearmPhysicalObject>();
				if (component2 != null)
				{
					SetScanned(component2);
					break;
				}
				FVRFireArmMagazine component3 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArmMagazine>();
				if (component3 != null && component3.FireArm == null)
				{
					SetScanned(component3);
					break;
				}
				FVRFireArmClip component4 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
				if (component4 != null && component4.FireArm == null)
				{
					SetScanned(component4);
					break;
				}
				Speedloader component5 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<Speedloader>();
				if (component5 != null)
				{
					SetScanned(component5);
					break;
				}
			}
		}
	}
}
