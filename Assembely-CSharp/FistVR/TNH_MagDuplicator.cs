using UnityEngine;

namespace FistVR
{
	public class TNH_MagDuplicator : MonoBehaviour
	{
		public TNH_Manager M;

		public Transform Spawnpoint_Mag;

		public TNH_ObjectConstructorIcon OCIcon;

		public Transform ScanningVolume;

		public LayerMask ScanningLM;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_Spawn;

		private FVRFireArmMagazine m_detectedMag;

		private FVRFireArmClip m_detectedClip;

		private Speedloader m_detectedSL;

		private Collider[] colbuffer;

		private int m_storedCost;

		private float m_scanTick = 1f;

		private void Start()
		{
			colbuffer = new Collider[50];
		}

		public void Button_Duplicate()
		{
			if (m_detectedMag == null && m_detectedClip == null && m_detectedSL == null)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
			}
			else if (M.GetNumTokens() >= m_storedCost)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Spawn, base.transform.position);
				M.SubtractTokens(m_storedCost);
				M.Increment(10, statOnly: false);
				FVRObject fVRObject = null;
				if (m_detectedMag != null)
				{
					fVRObject = m_detectedMag.ObjectWrapper;
					GameObject gameObject = Object.Instantiate(fVRObject.GetGameObject(), Spawnpoint_Mag.position, Spawnpoint_Mag.rotation);
					FVRFireArmMagazine component = gameObject.GetComponent<FVRFireArmMagazine>();
					for (int i = 0; i < Mathf.Min(m_detectedMag.LoadedRounds.Length, component.LoadedRounds.Length); i++)
					{
						if (m_detectedMag.LoadedRounds[i] != null && m_detectedMag.LoadedRounds[i].LR_Mesh != null)
						{
							component.LoadedRounds[i].LR_Class = m_detectedMag.LoadedRounds[i].LR_Class;
							component.LoadedRounds[i].LR_Mesh = m_detectedMag.LoadedRounds[i].LR_Mesh;
							component.LoadedRounds[i].LR_Material = m_detectedMag.LoadedRounds[i].LR_Material;
							component.LoadedRounds[i].LR_ObjectWrapper = m_detectedMag.LoadedRounds[i].LR_ObjectWrapper;
						}
					}
					component.m_numRounds = m_detectedMag.m_numRounds;
					component.UpdateBulletDisplay();
				}
				else if (m_detectedClip != null)
				{
					fVRObject = m_detectedClip.ObjectWrapper;
					GameObject gameObject2 = Object.Instantiate(fVRObject.GetGameObject(), Spawnpoint_Mag.position, Spawnpoint_Mag.rotation);
					FVRFireArmClip component2 = gameObject2.GetComponent<FVRFireArmClip>();
					for (int j = 0; j < Mathf.Min(m_detectedClip.LoadedRounds.Length, component2.LoadedRounds.Length); j++)
					{
						if (m_detectedClip.LoadedRounds[j] != null && m_detectedClip.LoadedRounds[j].LR_Mesh != null)
						{
							component2.LoadedRounds[j].LR_Class = m_detectedClip.LoadedRounds[j].LR_Class;
							component2.LoadedRounds[j].LR_Mesh = m_detectedClip.LoadedRounds[j].LR_Mesh;
							component2.LoadedRounds[j].LR_Material = m_detectedClip.LoadedRounds[j].LR_Material;
							component2.LoadedRounds[j].LR_ObjectWrapper = m_detectedClip.LoadedRounds[j].LR_ObjectWrapper;
						}
					}
					component2.m_numRounds = m_detectedClip.m_numRounds;
					component2.UpdateBulletDisplay();
				}
				else
				{
					if (!(m_detectedSL != null))
					{
						return;
					}
					fVRObject = m_detectedSL.ObjectWrapper;
					GameObject gameObject3 = Object.Instantiate(fVRObject.GetGameObject(), Spawnpoint_Mag.position, Spawnpoint_Mag.rotation);
					Speedloader component3 = gameObject3.GetComponent<Speedloader>();
					for (int k = 0; k < m_detectedSL.Chambers.Count; k++)
					{
						if (m_detectedSL.Chambers[k].IsLoaded)
						{
							component3.Chambers[k].Load(m_detectedSL.Chambers[k].LoadedClass);
						}
						else
						{
							component3.Chambers[k].Unload();
						}
					}
				}
			}
			else
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
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
			m_detectedMag = null;
			m_detectedClip = null;
			m_detectedSL = null;
			for (int i = 0; i < num; i++)
			{
				if (colbuffer[i].attachedRigidbody != null)
				{
					FVRFireArmMagazine component = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArmMagazine>();
					if (component != null && component.FireArm == null && !component.IsHeld && component.QuickbeltSlot == null)
					{
						m_detectedMag = component;
						break;
					}
					FVRFireArmClip component2 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
					if (component2 != null && component2.FireArm == null && !component2.IsHeld && component2.QuickbeltSlot == null)
					{
						m_detectedClip = component2;
						break;
					}
					Speedloader component3 = colbuffer[i].attachedRigidbody.gameObject.GetComponent<Speedloader>();
					if (component3 != null && !component3.IsHeld && component3.QuickbeltSlot == null)
					{
						m_detectedSL = component3;
						break;
					}
				}
			}
			SetCostBasedOnMag();
		}

		private void SetCostBasedOnMag()
		{
			if (m_detectedMag == null && m_detectedClip == null && m_detectedSL == null)
			{
				OCIcon.SetOption(TNH_ObjectConstructorIcon.IconState.Cancel, OCIcon.Sprite_Cancel, 0);
				m_storedCost = 0;
				return;
			}
			int num = 1;
			if (m_detectedMag != null)
			{
				if (m_detectedMag.m_capacity > 100)
				{
					num = 8;
				}
				else if (m_detectedMag.m_capacity > 50)
				{
					num = 5;
				}
				else if (m_detectedMag.m_capacity > 30)
				{
					num = 3;
				}
				else if (m_detectedMag.m_capacity > 15)
				{
					num = 2;
				}
				switch (AM.GetRoundPower(m_detectedMag.RoundType))
				{
				case FVRObject.OTagFirearmRoundPower.Shotgun:
					num++;
					break;
				case FVRObject.OTagFirearmRoundPower.Intermediate:
					num++;
					break;
				case FVRObject.OTagFirearmRoundPower.FullPower:
					num += 2;
					break;
				case FVRObject.OTagFirearmRoundPower.Exotic:
					num += 3;
					break;
				case FVRObject.OTagFirearmRoundPower.AntiMaterial:
					num += 3;
					break;
				case FVRObject.OTagFirearmRoundPower.Ordnance:
					num += 4;
					break;
				}
			}
			else if (m_detectedClip != null)
			{
				switch (AM.GetRoundPower(m_detectedClip.RoundType))
				{
				case FVRObject.OTagFirearmRoundPower.Shotgun:
					num++;
					break;
				case FVRObject.OTagFirearmRoundPower.Intermediate:
					num++;
					break;
				case FVRObject.OTagFirearmRoundPower.FullPower:
					num += 2;
					break;
				case FVRObject.OTagFirearmRoundPower.Exotic:
					num += 3;
					break;
				case FVRObject.OTagFirearmRoundPower.AntiMaterial:
					num += 3;
					break;
				case FVRObject.OTagFirearmRoundPower.Ordnance:
					num += 2;
					break;
				}
			}
			else if (m_detectedSL != null)
			{
				switch (AM.GetRoundPower(m_detectedSL.Chambers[0].Type))
				{
				case FVRObject.OTagFirearmRoundPower.Shotgun:
					num++;
					break;
				case FVRObject.OTagFirearmRoundPower.Intermediate:
					num++;
					break;
				case FVRObject.OTagFirearmRoundPower.FullPower:
					num += 2;
					break;
				case FVRObject.OTagFirearmRoundPower.Exotic:
					num += 3;
					break;
				case FVRObject.OTagFirearmRoundPower.AntiMaterial:
					num += 3;
					break;
				case FVRObject.OTagFirearmRoundPower.Ordnance:
					num += 2;
					break;
				}
			}
			OCIcon.SetOption(TNH_ObjectConstructorIcon.IconState.Item, OCIcon.Sprite_Accept, num);
			m_storedCost = num;
		}
	}
}
