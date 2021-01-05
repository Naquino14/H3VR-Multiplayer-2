using System;
using UnityEngine;

namespace FistVR
{
	public class wwFinaleManager : MonoBehaviour
	{
		[Serializable]
		public class EndSequenceSoundEvent
		{
			public AudioEvent AudioEvent;

			public AudioClip Clip;

			public bool HasPlayed;

			public float TimeIndex;

			public float Volume = 1f;
		}

		[Serializable]
		public class EndSequenceLightEvent
		{
			public float TimeIndex;

			public int LightToSwitchTo;

			public bool HasFired;
		}

		public wwParkManager ParkManager;

		public GameObject OutdoorLight;

		public GameObject[] FinaleLights;

		public GameObject[] BlackOuts;

		public wwFinaleDoor[] Doors;

		public Transform EndSoundsPlacement;

		private bool m_isEndingMonologuePlaying;

		public float m_endingMonologueTick;

		public EndSequenceSoundEvent[] End_Marc;

		public EndSequenceSoundEvent[] End_SoundEvents;

		public EndSequenceLightEvent[] End_LightEvents;

		public AudioSource AUD_MechanicalLoop;

		public AudioSource AUD_Ending_Marc;

		public AudioSource AUD_Ending_Sounds;

		public Transform ParkModel;

		public Vector3 ParkModelUp;

		public Vector3 ParkModelDown;

		private float m_parkModelLerp;

		public Transform BotDoor;

		public Vector3 BotDoorUp;

		public Vector3 BotDoorDown;

		private float m_botDoorLerp;

		public GameObject FinalBot;

		public Rigidbody[] FinalBotPieces;

		public Transform BotPoint1;

		public Transform BotPoint2;

		public Transform BotPoint3;

		private bool hasBotSploded;

		public Transform ExplosionPoint;

		public GameObject ExplosionPrefab;

		private bool m_hasMonologueConcluded;

		public void SwitchToFinaleLight(int index)
		{
			OutdoorLight.SetActive(value: false);
			for (int i = 0; i < FinaleLights.Length; i++)
			{
				if (i == index)
				{
					FinaleLights[i].SetActive(value: true);
				}
				else
				{
					FinaleLights[i].SetActive(value: false);
				}
			}
		}

		public void DisableAllFinaleLights()
		{
			GameObject[] finaleLights = FinaleLights;
			foreach (GameObject gameObject in finaleLights)
			{
				gameObject.SetActive(value: false);
			}
		}

		public void EnableOutdoorLight()
		{
			OutdoorLight.SetActive(value: true);
		}

		public void OpenDoor(int index)
		{
			BlackOuts[index].SetActive(value: false);
			Doors[index].OpenDoor();
		}

		private void Start()
		{
		}

		public void BeginEnding()
		{
			if (!m_hasMonologueConcluded && !m_isEndingMonologuePlaying)
			{
				AUD_MechanicalLoop.Play();
				m_isEndingMonologuePlaying = true;
				Doors[6].ConfigureDoorState(0);
			}
		}

		public void ConfigureBlackOuts(int[] b)
		{
			for (int i = 0; i < b.Length; i++)
			{
				if (b[i] == 0)
				{
					BlackOuts[i].SetActive(value: true);
				}
				else
				{
					BlackOuts[i].SetActive(value: false);
				}
			}
		}

		private void Update()
		{
			if (m_isEndingMonologuePlaying)
			{
				EndingSequencer();
			}
		}

		private void EndingSequencer()
		{
			m_endingMonologueTick += Time.deltaTime;
			if (m_endingMonologueTick > 56.2f && m_parkModelLerp < 1f)
			{
				if (!ParkModel.gameObject.activeSelf)
				{
					ParkModel.gameObject.SetActive(value: true);
				}
				m_parkModelLerp += Time.deltaTime * 0.008f;
				ParkModel.transform.localPosition = Vector3.Lerp(ParkModelUp, ParkModelDown, m_parkModelLerp);
			}
			if (m_endingMonologueTick > 201f && m_botDoorLerp < 1f)
			{
				if (FinalBot != null && !FinalBot.activeSelf)
				{
					FinalBot.SetActive(value: true);
				}
				m_botDoorLerp += Time.deltaTime * 0.175f;
				BotDoor.transform.localPosition = Vector3.Lerp(BotDoorUp, BotDoorDown, m_botDoorLerp);
			}
			if (m_endingMonologueTick >= 209f && m_endingMonologueTick < 213f)
			{
				float t = (m_endingMonologueTick - 209f) * 0.25f;
				FinalBot.transform.position = Vector3.Lerp(BotPoint1.position, BotPoint2.position, t);
			}
			else if (m_endingMonologueTick >= 213f && m_endingMonologueTick < 215f)
			{
				float num = (m_endingMonologueTick - 213f) * 0.5f;
				FinalBot.transform.position = Vector3.Lerp(BotPoint2.position, BotPoint3.position, num * num);
				FinalBot.transform.rotation = Quaternion.Slerp(BotPoint2.rotation, BotPoint3.rotation, num * num);
			}
			else if (m_endingMonologueTick > 215f && !hasBotSploded)
			{
				hasBotSploded = true;
				Rigidbody[] finalBotPieces = FinalBotPieces;
				foreach (Rigidbody rigidbody in finalBotPieces)
				{
					rigidbody.gameObject.SetActive(value: true);
					rigidbody.transform.SetParent(null);
				}
				UnityEngine.Object.Instantiate(ExplosionPrefab, ExplosionPoint.position, ExplosionPoint.rotation);
				UnityEngine.Object.Destroy(FinalBot);
			}
			if (m_endingMonologueTick > 215f && AUD_MechanicalLoop.isPlaying)
			{
				AUD_MechanicalLoop.Stop();
				if (ParkModel.gameObject.activeSelf)
				{
					ParkModel.gameObject.SetActive(value: false);
				}
			}
			for (int j = 0; j < End_Marc.Length; j++)
			{
				if (!End_Marc[j].HasPlayed && m_endingMonologueTick > End_Marc[j].TimeIndex && m_endingMonologueTick < End_Marc[j].TimeIndex + 1f)
				{
					AUD_Ending_Marc.PlayOneShot(End_Marc[j].Clip, End_Marc[j].Volume);
					End_Marc[j].HasPlayed = true;
				}
			}
			for (int k = 0; k < End_SoundEvents.Length; k++)
			{
				if (!End_SoundEvents[k].HasPlayed && m_endingMonologueTick > End_SoundEvents[k].TimeIndex && m_endingMonologueTick < End_SoundEvents[k].TimeIndex + 1f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, End_SoundEvents[k].AudioEvent, EndSoundsPlacement.position);
					End_SoundEvents[k].HasPlayed = true;
				}
			}
			for (int l = 0; l < End_LightEvents.Length; l++)
			{
				if (!End_LightEvents[l].HasFired && m_endingMonologueTick > End_LightEvents[l].TimeIndex && m_endingMonologueTick < End_LightEvents[l].TimeIndex + 1f)
				{
					SwitchToFinaleLight(End_LightEvents[l].LightToSwitchTo);
					End_LightEvents[l].HasFired = true;
				}
			}
			if (m_endingMonologueTick > 260f && !m_hasMonologueConcluded)
			{
				m_hasMonologueConcluded = true;
				m_isEndingMonologuePlaying = false;
				Doors[6].ConfigureDoorState(1);
			}
		}
	}
}
