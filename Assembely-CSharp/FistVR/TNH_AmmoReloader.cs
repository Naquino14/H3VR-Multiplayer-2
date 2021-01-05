using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_AmmoReloader : MonoBehaviour
	{
		public TNH_Manager M;

		public Transform Spawnpoint_Round;

		public Transform ScanningVolume;

		public LayerMask ScanningLM;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_Spawn;

		public AudioEvent AudEvent_Reload;

		private List<FVRFireArmMagazine> m_detectedMags = new List<FVRFireArmMagazine>();

		private List<FVRFireArmClip> m_detectedClips = new List<FVRFireArmClip>();

		private List<Speedloader> m_detectedSLs = new List<Speedloader>();

		private List<FireArmRoundType> m_roundTypes = new List<FireArmRoundType>();

		private Collider[] colbuffer;

		private Dictionary<FireArmRoundType, FireArmRoundClass> m_decidedTypes = new Dictionary<FireArmRoundType, FireArmRoundClass>();

		private List<FVRObject.OTagEra> m_validEras = new List<FVRObject.OTagEra>();

		private List<FVRObject.OTagSet> m_validSets = new List<FVRObject.OTagSet>();

		private float m_scanTick = 1f;

		private void Start()
		{
			colbuffer = new Collider[50];
		}

		public void SetValidErasSets(List<FVRObject.OTagEra> eras, List<FVRObject.OTagSet> sets)
		{
			for (int i = 0; i < eras.Count; i++)
			{
				m_validEras.Add(eras[i]);
			}
			for (int j = 0; j < sets.Count; j++)
			{
				m_validSets.Add(sets[j]);
			}
		}

		private FireArmRoundClass GetClassFromType(FireArmRoundType t)
		{
			if (!m_decidedTypes.ContainsKey(t))
			{
				List<FireArmRoundClass> list = new List<FireArmRoundClass>();
				for (int i = 0; i < AM.SRoundDisplayDataDic[t].Classes.Length; i++)
				{
					FVRObject objectID = AM.SRoundDisplayDataDic[t].Classes[i].ObjectID;
					if (m_validEras.Contains(objectID.TagEra) && m_validSets.Contains(objectID.TagSet))
					{
						list.Add(AM.SRoundDisplayDataDic[t].Classes[i].Class);
					}
				}
				if (list.Count > 0)
				{
					m_decidedTypes.Add(t, list[Random.Range(0, list.Count)]);
				}
				else
				{
					m_decidedTypes.Add(t, AM.GetRandomValidRoundClass(t));
				}
			}
			return m_decidedTypes[t];
		}

		public void Button_SpawnRound()
		{
			if (m_roundTypes.Count < 1)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
				return;
			}
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Spawn, base.transform.position);
			for (int i = 0; i < m_roundTypes.Count; i++)
			{
				FireArmRoundType fireArmRoundType = m_roundTypes[i];
				FireArmRoundClass classFromType = GetClassFromType(fireArmRoundType);
				FVRObject roundSelfPrefab = AM.GetRoundSelfPrefab(fireArmRoundType, classFromType);
				Object.Instantiate(roundSelfPrefab.GetGameObject(), Spawnpoint_Round.position + Vector3.up * i * 0.1f, Spawnpoint_Round.rotation);
			}
		}

		public void Button_ReloadGuns()
		{
			if (m_detectedMags.Count < 1 && m_detectedClips.Count < 1 && m_detectedSLs.Count < 1)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
				return;
			}
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Reload, base.transform.position);
			for (int i = 0; i < m_detectedMags.Count; i++)
			{
				FireArmRoundType roundType = m_detectedMags[i].RoundType;
				FireArmRoundClass classFromType = GetClassFromType(roundType);
				m_detectedMags[i].ReloadMagWithType(classFromType);
			}
			for (int j = 0; j < m_detectedClips.Count; j++)
			{
				FireArmRoundType roundType2 = m_detectedClips[j].RoundType;
				FireArmRoundClass classFromType2 = GetClassFromType(roundType2);
				m_detectedClips[j].ReloadClipWithType(classFromType2);
			}
			for (int k = 0; k < m_detectedSLs.Count; k++)
			{
				FireArmRoundType type = m_detectedSLs[k].Chambers[0].Type;
				FireArmRoundClass classFromType3 = GetClassFromType(type);
				m_detectedSLs[k].ReloadClipWithType(classFromType3);
			}
		}

		private void Update()
		{
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

		private void Scan()
		{
			int num = Physics.OverlapBoxNonAlloc(ScanningVolume.position, ScanningVolume.localScale * 0.5f, colbuffer, ScanningVolume.rotation, ScanningLM, QueryTriggerInteraction.Collide);
			m_roundTypes.Clear();
			m_detectedMags.Clear();
			m_detectedClips.Clear();
			m_detectedSLs.Clear();
			for (int i = 0; i < num; i++)
			{
				if (!(colbuffer[i].attachedRigidbody != null))
				{
					continue;
				}
				FVRFireArm component = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
				if (component != null)
				{
					if (!m_roundTypes.Contains(component.RoundType))
					{
						m_roundTypes.Add(component.RoundType);
					}
					if (component.Magazine != null && !m_detectedMags.Contains(component.Magazine))
					{
						m_detectedMags.Add(component.Magazine);
					}
					if (component.Attachments.Count > 0)
					{
						for (int j = 0; j < component.Attachments.Count; j++)
						{
							if (component.Attachments[j] is AttachableFirearmPhysicalObject && !m_roundTypes.Contains((component.Attachments[j] as AttachableFirearmPhysicalObject).FA.RoundType))
							{
								m_roundTypes.Add((component.Attachments[j] as AttachableFirearmPhysicalObject).FA.RoundType);
							}
						}
					}
					if (component.GetIntegratedAttachableFirearm() != null && !m_roundTypes.Contains(component.GetIntegratedAttachableFirearm().RoundType))
					{
						m_roundTypes.Add(component.GetIntegratedAttachableFirearm().RoundType);
					}
				}
				AttachableFirearmPhysicalObject component2 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<AttachableFirearmPhysicalObject>();
				if (component2 != null && !m_roundTypes.Contains(component2.FA.RoundType))
				{
					m_roundTypes.Add(component2.FA.RoundType);
				}
				FVRFireArmMagazine component3 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArmMagazine>();
				if (component3 != null && component3.FireArm == null && !m_detectedMags.Contains(component3))
				{
					m_detectedMags.Add(component3);
				}
				FVRFireArmClip component4 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
				if (component4 != null && component4.FireArm == null && !m_detectedClips.Contains(component4))
				{
					m_detectedClips.Add(component4);
				}
				Speedloader component5 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<Speedloader>();
				if (component5 != null && !m_detectedSLs.Contains(component5))
				{
					m_detectedSLs.Add(component5);
				}
			}
		}
	}
}
