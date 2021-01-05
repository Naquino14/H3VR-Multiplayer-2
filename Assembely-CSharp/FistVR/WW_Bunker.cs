using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class WW_Bunker : MonoBehaviour
	{
		public Transform TeleportPoint;

		public List<GameObject> TeleportButtons;

		public List<GameObject> TeleportTowerButtons;

		private WW_TeleportMaster m_master;

		public int BunkerIndex;

		public int TierNeeded;

		public GameObject TPPanel;

		public List<GameObject> Numbers_Outer;

		public List<GameObject> Numbers_Inner;

		public List<GameObject> TierLocks;

		public GameObject LockedUntil;

		public GameObject Door;

		public Transform Door_Up;

		public Transform Door_Down;

		private float m_doorTick;

		private bool m_isUnlocked;

		private bool m_isLockDown = true;

		public List<GameObject> TurnOffIfLockDown;

		public List<GameObject> TurnOnIfLockDown;

		public Transform UnlockCheckVolume;

		[Header("Probes")]
		public List<GameObject> ReflectionProbes;

		public WW_Bunker CopyFrom;

		public Transform ProbeCheckV;

		public Transform BunkerBounds;

		private bool m_isProbeEnabled;

		public TNH_WeaponCrate Crate;

		public GameObject Lever;

		public Transform Forcefield;

		public Transform FF_Up;

		public Transform FF_Down;

		public GameObject XmasFetti;

		public List<Transform> XmasFettiPoints;

		public AudioEvent AudEvent_Music;

		public AudioEvent AudEvent_DoorOpen;

		private float fieldTick;

		private bool m_isFieldOpened;

		private bool m_isFieldOpening;

		private float m_checkTick = 1f;

		private bool m_isDoorOpening;

		public bool IsUnlocked => m_isUnlocked;

		public bool IsLockDown => m_isLockDown;

		[ContextMenu("ProbeCopy")]
		public void ProbeCopy()
		{
			for (int i = 0; i < ReflectionProbes.Count; i++)
			{
				ReflectionProbe component = ReflectionProbes[i].GetComponent<ReflectionProbe>();
				ReflectionProbe component2 = CopyFrom.ReflectionProbes[i].GetComponent<ReflectionProbe>();
				component.size = component2.size;
				component.center = component2.center;
				component.customBakedTexture = component2.customBakedTexture;
			}
			CopyFrom = null;
		}

		public void ConfigInitBunker(int index, int curDay, int tierNeeded)
		{
			BunkerIndex = index;
			TierNeeded = tierNeeded;
			if (BunkerIndex <= curDay)
			{
				for (int i = 0; i < TierLocks.Count; i++)
				{
					if (i == TierNeeded)
					{
						TierLocks[i].SetActive(value: true);
					}
					else
					{
						TierLocks[i].SetActive(value: false);
					}
				}
				LockedUntil.SetActive(value: false);
				m_isLockDown = false;
			}
			else
			{
				for (int j = 0; j < TierLocks.Count; j++)
				{
					TierLocks[j].SetActive(value: false);
				}
				LockedUntil.SetActive(value: true);
				m_isLockDown = true;
				for (int k = 0; k < TurnOffIfLockDown.Count; k++)
				{
					TurnOffIfLockDown[k].SetActive(value: false);
				}
				for (int l = 0; l < TurnOnIfLockDown.Count; l++)
				{
					TurnOnIfLockDown[l].SetActive(value: true);
				}
			}
			UpdateTPButtons();
			if (GM.Options.XmasFlags.BunkersOpened[BunkerIndex])
			{
				Door.SetActive(value: false);
				m_isUnlocked = true;
			}
			for (int m = 0; m < Numbers_Outer.Count; m++)
			{
				if (m == BunkerIndex)
				{
					Numbers_Outer[m].SetActive(value: true);
					Numbers_Inner[m].SetActive(value: true);
				}
				else
				{
					Numbers_Outer[m].SetActive(value: false);
					Numbers_Inner[m].SetActive(value: false);
				}
			}
			Forcefield.SetParent(null);
			if (GM.Options.XmasFlags.FieldsOpened[BunkerIndex])
			{
				Forcefield.gameObject.SetActive(value: false);
				m_isFieldOpened = true;
				Lever.SetActive(value: false);
			}
			Door.transform.SetParent(null);
		}

		public void SetMaster(WW_TeleportMaster m)
		{
			m_master = m;
		}

		private void Start()
		{
		}

		public void TeleportTo(int i)
		{
			m_master.TeleportTo(i);
		}

		public void TeleportToTowerPad(int i)
		{
			m_master.TeleportToTowerPad(i);
		}

		private void Update()
		{
			KeyCheck();
			if (m_isFieldOpening)
			{
				fieldTick += Time.deltaTime * 0.1f;
				if (fieldTick >= 1f)
				{
					m_isFieldOpening = false;
				}
				Forcefield.position = Vector3.Lerp(FF_Up.position, FF_Down.position, fieldTick);
			}
			if (m_isDoorOpening)
			{
				m_doorTick += Time.deltaTime * 1f;
				if (m_doorTick >= 1f)
				{
					m_isDoorOpening = false;
				}
				Door.transform.position = Vector3.Lerp(Door_Up.position, Door_Down.position, m_doorTick);
			}
		}

		public void UpdateTPButtons()
		{
			for (int i = 0; i < TeleportButtons.Count; i++)
			{
				TeleportButtons[i].SetActive(GM.Options.XmasFlags.BunkersOpened[i]);
			}
			for (int j = 0; j < TeleportTowerButtons.Count; j++)
			{
				TeleportTowerButtons[j].SetActive(GM.Options.XmasFlags.TowersActive[j + 1]);
			}
		}

		private void Unlock()
		{
			GM.Options.XmasFlags.BunkersOpened[BunkerIndex] = true;
			GM.Options.SaveToFile();
			m_master.BunkerUnlockedUpdate();
		}

		public void OpenField()
		{
			if (!m_isFieldOpened)
			{
				m_isFieldOpened = true;
				GM.Options.XmasFlags.FieldsOpened[BunkerIndex] = true;
				GM.Options.SaveToFile();
				for (int i = 0; i < XmasFettiPoints.Count; i++)
				{
					Object.Instantiate(XmasFetti, XmasFettiPoints[i].position, XmasFettiPoints[i].rotation);
				}
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Music, Forcefield.position);
				m_isFieldOpening = true;
			}
		}

		public void Entered()
		{
			m_master.EnteredBunker(BunkerIndex);
		}

		public void KeyCheck()
		{
			if (m_isLockDown)
			{
				return;
			}
			m_checkTick -= Time.deltaTime;
			if (!(m_checkTick < 0f))
			{
				return;
			}
			m_checkTick = Random.Range(0.3f, 0.5f);
			if (TestVolumeBool(ProbeCheckV, GM.CurrentPlayerBody.Head.position))
			{
				if (!m_isProbeEnabled)
				{
					m_isProbeEnabled = true;
					for (int i = 0; i < ReflectionProbes.Count; i++)
					{
						ReflectionProbes[i].SetActive(value: true);
						TPPanel.SetActive(value: true);
						UpdateTPButtons();
					}
				}
			}
			else if (m_isProbeEnabled)
			{
				m_isProbeEnabled = false;
				for (int j = 0; j < ReflectionProbes.Count; j++)
				{
					ReflectionProbes[j].SetActive(value: false);
					TPPanel.SetActive(value: false);
				}
			}
			if (m_isUnlocked || !TestVolumeBool(UnlockCheckVolume, GM.CurrentPlayerBody.Head.position))
			{
				return;
			}
			for (int k = 0; k < GM.CurrentMovementManager.Hands.Length; k++)
			{
				if (GM.CurrentMovementManager.Hands[k].CurrentInteractable != null && GM.CurrentMovementManager.Hands[k].CurrentInteractable is WW_Keycard)
				{
					WW_Keycard wW_Keycard = GM.CurrentMovementManager.Hands[k].CurrentInteractable as WW_Keycard;
					if (wW_Keycard.TierType == TierNeeded)
					{
						UnlockDoor();
					}
				}
			}
		}

		private void UnlockDoor()
		{
			if (!m_isUnlocked)
			{
				m_isUnlocked = true;
				GM.Options.XmasFlags.BunkersOpened[BunkerIndex] = true;
				GM.Options.SaveToFile();
				m_master.BunkerUnlockedUpdate();
				m_isDoorOpening = true;
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_DoorOpen, Door.transform.position);
			}
		}

		public bool TestVolumeBool(Transform t, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}
	}
}
