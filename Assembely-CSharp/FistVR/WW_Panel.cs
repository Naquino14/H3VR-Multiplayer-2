using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class WW_Panel : MonoBehaviour
	{
		[Serializable]
		public class wwMessage
		{
			public string Tit;

			public AudioClip AudClip;
		}

		[Header("Page Stuff")]
		public List<GameObject> Pages;

		public GameObject Radar;

		public AudioEvent AudEvent_Beep;

		public AudioEvent AudEvent_Boop;

		public WW_TeleportMaster Master;

		public List<Image> Message_Buttons;

		public Sprite Message_Unread;

		public Sprite Message_Read;

		public Text LBL_NumMessagesUnread;

		public Text LBL_SelectedMessage;

		public GameObject BTN_PlayMessage;

		public AudioSource AudSource_MessagePlayback;

		public AudioEvent AudEvent_MessageReceived;

		public AudioEvent AudEvent_MessageStart;

		public AudioEvent AudEvent_MessageEnd;

		public List<wwMessage> Messages;

		private List<int> m_messageDisplayIndices;

		private int m_curPage;

		private int m_maxPage;

		[Header("Radar Stuff")]
		public TAH_Reticle Reticle;

		public List<Transform> SatcommList;

		public GameObject SatcommSyncPulse;

		[Header("Map Stuff")]
		public List<GameObject> MapPieces;

		private float maxPos = 515f;

		public Transform MapExtentsMin;

		public Transform MapExtentsMax;

		public Transform MapPing;

		private int m_curMessage;

		public Color MapRingSolid;

		public Color MapRingTrans;

		private float m_bunkerpipPulse;

		public List<Image> BunkerSprites;

		public Transform BunkerMapExtentsMin;

		public Transform BunkerMapExtentsMax;

		public void SetPage(int p)
		{
			for (int i = 0; i < Pages.Count; i++)
			{
				if (i == p)
				{
					Pages[i].SetActive(value: true);
				}
				else
				{
					Pages[i].SetActive(value: false);
				}
			}
			if (p == 1)
			{
				Radar.SetActive(value: true);
				MapPing.gameObject.SetActive(value: true);
			}
			else
			{
				Radar.SetActive(value: false);
				MapPing.gameObject.SetActive(value: false);
			}
			UpdateMessageDisplay();
		}

		public void UpdateMessageDisplay()
		{
			int num = 0;
			for (int i = 0; i < Message_Buttons.Count; i++)
			{
				if (i < Messages.Count)
				{
					if (GM.Options.XmasFlags.MessagesAcquired[i])
					{
						Message_Buttons[i].gameObject.SetActive(value: true);
						if (GM.Options.XmasFlags.MessagesRead[i])
						{
							Message_Buttons[i].sprite = Message_Read;
							continue;
						}
						num++;
						Message_Buttons[i].sprite = Message_Unread;
					}
					else
					{
						Message_Buttons[i].gameObject.SetActive(value: false);
					}
				}
				else
				{
					Message_Buttons[i].gameObject.SetActive(value: false);
				}
			}
			LBL_NumMessagesUnread.text = num + " Unread Messages";
			LBL_SelectedMessage.text = m_curMessage + 1 + ". " + Messages[m_curMessage].Tit;
		}

		private void RegisterInitialContacts()
		{
			for (int i = 0; i < SatcommList.Count; i++)
			{
				Reticle.RegisterTrackedObject(SatcommList[i], TAH_ReticleContact.ContactType.Supply);
			}
		}

		public void BTN_SetMessage(int i)
		{
			m_curMessage = i;
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Beep, base.transform.position);
			UpdateMessageDisplay();
		}

		public void BTN_PlayCurrentMessage()
		{
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_MessageStart, base.transform.position);
			AudSource_MessagePlayback.Stop();
			AudSource_MessagePlayback.clip = Messages[m_curMessage].AudClip;
			AudSource_MessagePlayback.Play();
			GM.Options.XmasFlags.MessagesRead[m_curMessage] = true;
			GM.Options.SaveToFile();
			UpdateMessageDisplay();
		}

		private void Start()
		{
			for (int i = 0; i < 5; i++)
			{
				GM.Options.XmasFlags.MessagesAcquired[i] = true;
			}
			UpdateMessageDisplay();
			RegisterInitialContacts();
			UpdateMap();
		}

		private void Update()
		{
			RadarCheck();
		}

		private void RadarCheck()
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			position.x = (position.x + 515f) / 1030f;
			position.z = (position.z + 515f) / 1030f;
			Vector3 localPosition = new Vector3(Mathf.Lerp(MapExtentsMin.localPosition.x, MapExtentsMax.localPosition.x, position.x), MapPing.localPosition.y, Mathf.Lerp(MapExtentsMin.localPosition.z, MapExtentsMax.localPosition.z, position.z));
			MapPing.localPosition = localPosition;
			for (int i = 0; i < SatcommList.Count; i++)
			{
				if (!GM.Options.XmasFlags.TowersActive[i])
				{
					Vector3 a = new Vector3(SatcommList[i].transform.position.x, 0f, SatcommList[i].transform.position.z);
					Vector3 b = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
					float num = Vector3.Distance(a, b);
					if (num < 6f)
					{
						SetSatcomm(i);
						UpdateMap();
						Master.BunkerUnlockedUpdate();
					}
				}
			}
			for (int j = 0; j < BunkerSprites.Count; j++)
			{
				if (Master.Bunkers[j].IsLockDown || Master.Bunkers[j].IsUnlocked)
				{
					BunkerSprites[j].gameObject.SetActive(value: false);
					continue;
				}
				float num2 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, Master.Bunkers[j].transform.position);
				m_bunkerpipPulse += Time.deltaTime * 0.2f;
				m_bunkerpipPulse = Mathf.Repeat(m_bunkerpipPulse, (float)Math.PI * 2f);
				if (num2 < 300f)
				{
					float t = num2 / 250f;
					Color color = Color.Lerp(MapRingSolid, MapRingTrans, t) * Mathf.Abs(Mathf.Sin(m_bunkerpipPulse));
					float num3 = Mathf.Lerp(0.5f, 1.5f, t);
					BunkerSprites[j].gameObject.SetActive(value: true);
					position = Master.Bunkers[j].transform.position;
					position.x = (position.x + 515f) / 1030f;
					position.y = (position.z + 515f) / 1030f;
					position.z = 0f;
					localPosition = new Vector3(Mathf.Lerp(BunkerMapExtentsMin.localPosition.x, BunkerMapExtentsMax.localPosition.x, position.x), Mathf.Lerp(BunkerMapExtentsMin.localPosition.y, BunkerMapExtentsMax.localPosition.y, position.y), BunkerSprites[j].transform.localPosition.z);
					BunkerSprites[j].transform.localPosition = localPosition;
					BunkerSprites[j].transform.localScale = new Vector3(num3, num3, num3);
					BunkerSprites[j].color = color;
				}
				else
				{
					BunkerSprites[j].gameObject.SetActive(value: false);
				}
			}
		}

		private void UpdateMap()
		{
			for (int i = 0; i < MapPieces.Count; i++)
			{
				MapPieces[i].SetActive(GM.Options.XmasFlags.TowersActive[i]);
			}
		}

		private void SetSatcomm(int i)
		{
			UnityEngine.Object.Instantiate(SatcommSyncPulse, base.transform.position, Quaternion.identity);
			GM.Options.XmasFlags.TowersActive[i] = true;
			GM.Options.SaveToFile();
		}
	}
}
