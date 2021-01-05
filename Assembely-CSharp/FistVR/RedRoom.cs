using UnityEngine;

namespace FistVR
{
	public class RedRoom : MonoBehaviour
	{
		public enum RedRoomType
		{
			Starting,
			Trap,
			Meat,
			MonsterCloset,
			Items
		}

		public enum Quadrant
		{
			None,
			Office,
			Boiler,
			ColdStorage,
			Restaurant
		}

		public bool HasBeenConfigured;

		public RedRoomType RoomType;

		public int RoomSize;

		public GameObject Triggerable;

		public bool DoDoorsShutWhenTriggered;

		public GameObject EntryTrigger;

		public AudioEvent AudEvent_DoorOpen;

		public AudioEvent AudEvent_DoorClose;

		public Quadrant MyQuadrant;

		private bool m_hasTriggered;

		public Transform[] Doors;

		private void Awake()
		{
			if (RoomType != 0)
			{
				OpenDoors(playSound: false);
			}
		}

		public void SetRoomType(RedRoomType t)
		{
			RoomType = t;
			switch (t)
			{
			case RedRoomType.Starting:
				DoDoorsShutWhenTriggered = false;
				break;
			case RedRoomType.Trap:
				DoDoorsShutWhenTriggered = true;
				break;
			case RedRoomType.MonsterCloset:
				break;
			case RedRoomType.Meat:
				DoDoorsShutWhenTriggered = false;
				break;
			case RedRoomType.Items:
				break;
			}
		}

		public void TriggerRoom()
		{
			if (!m_hasTriggered)
			{
				m_hasTriggered = true;
				if (Triggerable.GetComponent<IRoomTriggerable>() != null)
				{
					Triggerable.GetComponent<IRoomTriggerable>()?.Init(RoomSize, this);
				}
				if (DoDoorsShutWhenTriggered)
				{
					CloseDoors(playSound: true);
				}
				EntryTrigger.SetActive(value: false);
			}
		}

		public void SetTriggerable(GameObject trig)
		{
			Triggerable = trig;
			Triggerable.transform.position = base.transform.position;
		}

		public void OpenDoors(bool playSound)
		{
			if (playSound)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_DoorOpen, base.transform.position);
			}
			for (int i = 0; i < Doors.Length; i++)
			{
				Doors[i].localPosition = new Vector3(Doors[i].localPosition.x, 2.2f, Doors[i].localPosition.z);
			}
		}

		public void CloseDoors(bool playSound)
		{
			if (playSound)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_DoorClose, base.transform.position);
			}
			for (int i = 0; i < Doors.Length; i++)
			{
				Doors[i].localPosition = new Vector3(Doors[i].localPosition.x, 0f, Doors[i].localPosition.z);
			}
		}
	}
}
