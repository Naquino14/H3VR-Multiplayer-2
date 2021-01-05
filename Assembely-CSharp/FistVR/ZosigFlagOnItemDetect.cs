using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigFlagOnItemDetect : MonoBehaviour
	{
		[Header("Flag")]
		public string Flag;

		public int SetsToValue;

		[Header("Blocking Flag")]
		public bool HasBlockingFlag;

		public string BlockingFlag;

		public int BlockingFlagValue;

		[Header("Required Flag Flag")]
		public bool RequiresFlagForDetect;

		public string RequiredFlag;

		public int RequiredFlagValueOrAbove;

		private bool m_hasSetFlag;

		[Header("Other Stuff")]
		public List<FVRObject> ObjectsToBeDetected = new List<FVRObject>();

		private HashSet<FVRPhysicalObject> m_objsDetected = new HashSet<FVRPhysicalObject>();

		private List<string> m_IDsToBeDetected = new List<string>();

		public bool DestroysOnDetect;

		public bool PingSpawners;

		public List<ZosigSpawnFromTable> TablesToPing;

		public bool KeepsDetecting;

		public bool InstaSpawnsSomething;

		public GameObject SpawnThing;

		private void Start()
		{
			if (GM.ZMaster.FlagM.GetFlagValue(Flag) >= SetsToValue)
			{
				base.gameObject.SetActive(value: false);
			}
			for (int i = 0; i < ObjectsToBeDetected.Count; i++)
			{
				m_IDsToBeDetected.Add(ObjectsToBeDetected[i].ItemID);
			}
		}

		public void RefreshFlagCache()
		{
			m_IDsToBeDetected.Clear();
			m_objsDetected.Clear();
			for (int i = 0; i < ObjectsToBeDetected.Count; i++)
			{
				m_IDsToBeDetected.Add(ObjectsToBeDetected[i].ItemID);
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			ProcessTrigger(col);
		}

		private void ProcessTrigger(Collider col)
		{
			if ((m_hasSetFlag && !KeepsDetecting) || col.attachedRigidbody == null || (RequiresFlagForDetect && GM.ZMaster.FlagM.GetFlagValue(RequiredFlag) < RequiredFlagValueOrAbove) || (HasBlockingFlag && GM.ZMaster.FlagM.GetFlagValue(BlockingFlag) > BlockingFlagValue))
			{
				return;
			}
			FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
			if (component == null || component.QuickbeltSlot != null || component.ObjectWrapper == null || m_objsDetected.Contains(component) || !m_IDsToBeDetected.Contains(component.ObjectWrapper.ItemID))
			{
				return;
			}
			m_objsDetected.Add(component);
			if (InstaSpawnsSomething)
			{
				Object.Instantiate(SpawnThing, base.transform.position, base.transform.rotation);
			}
			if (!m_hasSetFlag)
			{
				m_hasSetFlag = true;
				GM.ZMaster.FlagM.SetFlagMaxBlend(Flag, SetsToValue);
				if (!KeepsDetecting)
				{
					base.gameObject.SetActive(value: false);
				}
			}
			if (PingSpawners)
			{
				for (int i = 0; i < TablesToPing.Count; i++)
				{
					TablesToPing[i].SpawnKernel();
				}
			}
			if (DestroysOnDetect)
			{
				Object.Destroy(component.gameObject);
			}
		}
	}
}
