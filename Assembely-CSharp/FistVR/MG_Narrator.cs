using UnityEngine;

namespace FistVR
{
	public class MG_Narrator : MonoBehaviour
	{
		public MeatGrinderMaster Master;

		[Header("Intercom System")]
		public Transform[] Intercoms;

		public AudioSource AUD;

		private bool m_isPlaying;

		[Header("Intro")]
		public AudioClip AC_Pt1;

		public AudioClip AC_Pt2;

		public AudioClip[] AC_IntrosShort;

		[Header("Trap Rooms")]
		public AudioClip[] AC_TrapRoomInit;

		public AudioClip[] AC_TrapRoomFailing;

		[Header("MeatRoom Sets")]
		public AudioClip[] AC_MeatRoomDiscover;

		public AudioClip[] AC_MeatRoomAcquire;

		public AudioClip[] AC_MeatRoomFeedIn;

		[Header("Monster Closet")]
		public AudioClip[] AC_MonsterCloset;

		public AudioClip[] AC_MonsterRebuilt;

		[Header("JumpScareSets")]
		public AudioClip[] AC_JumpScareReactions;

		[Header("AreaEntry")]
		public AudioClip[] AC_AreaEntryBoiler;

		public AudioClip[] AC_ColdStorage;

		public AudioClip[] AC_Office;

		public AudioClip[] AC_Restaurant;

		[Header("FoundThings")]
		public AudioClip[] AC_FoundSpecialItem;

		public AudioClip[] AC_FoundJunkItem;

		public AudioClip[] AC_FoundNormalWeapon;

		public AudioClip[] AC_FoundRareWeapon;

		[Header("Death")]
		public AudioClip[] AC_PlayerAboutToDie;

		public AudioClip[] AC_PlayerDiedCheating;

		public AudioClip[] AC_PlayerDiedOutOfHealth;

		[Header("Winning")]
		public AudioClip AC_WonFirstTime;

		public AudioClip[] AC_WonAgain;

		[Header("TimeRemaining")]
		public AudioClip[] AC_TimeRemaining;

		private int curClipPriority;

		private int m_curTrapRoomInitIndex;

		private int m_curTrapRoomFailingIndex;

		private int m_curAEBoilerIndex;

		private int m_curAEColdStorageIndex;

		private int m_curAEOfficeIndex;

		private int m_curAERestaurantIndex;

		private int m_curFoundSpecialItemIndex;

		private int m_curFoundJunkItemIndex;

		private int m_curFoundNormalItemIndex;

		private int m_curFoundRareItemIndex;

		private int m_curAboutToDieIndex;

		private float TimeSinceObjectHook;

		private float TimeSinceAreaHook;

		private float TimeSinceHealthHook;

		private float TimeSinceJumpScareHook;

		private float TimeSinceTrapRoomFailingHook;

		private float m_intercomCheckTick = 1f;

		private Transform m_curIntercom;

		private Transform m_lastIntercom;

		private void Awake()
		{
			ShuffleClips(AC_TrapRoomInit);
			ShuffleClips(AC_TrapRoomFailing);
			ShuffleClips(AC_FoundSpecialItem);
			ShuffleClips(AC_FoundJunkItem);
			ShuffleClips(AC_FoundNormalWeapon);
			ShuffleClips(AC_FoundRareWeapon);
			ShuffleClips(AC_PlayerAboutToDie);
			CheckForClosestIntercom();
			if (GM.Options.MeatGrinderFlags.NarratorMode == MeatGrinderFlags.MeatGrinderNarratorMode.Silent)
			{
				AUD.volume = 0f;
			}
		}

		private bool CanPlay(int i)
		{
			if (GM.Options.MeatGrinderFlags.NarratorMode == MeatGrinderFlags.MeatGrinderNarratorMode.Silent)
			{
				return false;
			}
			if (GM.Options.MeatGrinderFlags.NarratorMode == MeatGrinderFlags.MeatGrinderNarratorMode.Terse && i < 4)
			{
				return false;
			}
			if (i < curClipPriority && AUD.isPlaying)
			{
				return false;
			}
			return true;
		}

		public void PlayIntroPt1()
		{
			curClipPriority = 4;
			AUD.clip = AC_Pt1;
			AUD.Play();
		}

		public void PlayIntroPt2()
		{
			curClipPriority = 4;
			AUD.clip = AC_Pt2;
			AUD.Play();
			GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro = true;
		}

		public void PlayIntroShort(int i)
		{
			curClipPriority = 4;
			AUD.clip = AC_IntrosShort[i];
			AUD.Play();
		}

		public void PlayWonFirstTime()
		{
			curClipPriority = 5;
			AUD.clip = AC_WonFirstTime;
			AUD.Play();
		}

		public void PlayWonAgain(int i)
		{
			curClipPriority = 5;
			AUD.clip = AC_WonAgain[i];
			AUD.Play();
		}

		public void PlayTrapRoomInit()
		{
			if (CanPlay(3) && m_curTrapRoomInitIndex < AC_TrapRoomInit.Length)
			{
				curClipPriority = 3;
				AUD.clip = AC_TrapRoomInit[m_curTrapRoomInitIndex];
				AUD.Play();
				m_curTrapRoomInitIndex++;
			}
		}

		public void PlayTrapRoomFailing()
		{
			if (CanPlay(3) && m_curTrapRoomFailingIndex < AC_TrapRoomFailing.Length && !(TimeSinceTrapRoomFailingHook < 15f))
			{
				TimeSinceTrapRoomFailingHook = 0f;
				curClipPriority = 3;
				AUD.clip = AC_TrapRoomFailing[Random.Range(0, AC_TrapRoomFailing.Length)];
				AUD.Play();
				m_curTrapRoomFailingIndex++;
			}
		}

		public void PlayMeatDiscover(int i)
		{
			if (CanPlay(4))
			{
				curClipPriority = 4;
				AUD.clip = AC_MeatRoomDiscover[i];
				AUD.Play();
			}
		}

		public void PlayMeatAcquire(int i)
		{
			if (CanPlay(4))
			{
				curClipPriority = 4;
				AUD.clip = AC_MeatRoomAcquire[i];
				AUD.Play();
			}
		}

		public void PlayMeatFeedIn(int i)
		{
			if (CanPlay(4))
			{
				curClipPriority = 4;
				AUD.clip = AC_MeatRoomFeedIn[i];
				AUD.Play();
			}
		}

		public void PlayMonsterCloset()
		{
			if (CanPlay(3))
			{
				curClipPriority = 3;
				AUD.clip = AC_MonsterCloset[Random.Range(0, AC_MonsterCloset.Length)];
				AUD.Play();
			}
		}

		public void PlayMonsterRebuilt()
		{
			if (CanPlay(1))
			{
				curClipPriority = 1;
				AUD.clip = AC_MonsterRebuilt[Random.Range(0, AC_MonsterRebuilt.Length)];
				AUD.Play();
			}
		}

		public void PlayJumpScare()
		{
			if (CanPlay(1) && !(TimeSinceJumpScareHook < 20f))
			{
				TimeSinceJumpScareHook = 0f;
				curClipPriority = 1;
				AUD.clip = AC_JumpScareReactions[Random.Range(0, AC_JumpScareReactions.Length)];
				AUD.Play();
			}
		}

		public void PlayAreaEntryBoiler()
		{
			if (CanPlay(3) && m_curAEBoilerIndex < AC_AreaEntryBoiler.Length && !(TimeSinceAreaHook < 120f))
			{
				TimeSinceAreaHook = 0f;
				curClipPriority = 3;
				AUD.clip = AC_AreaEntryBoiler[m_curAEBoilerIndex];
				AUD.Play();
				m_curAEBoilerIndex++;
			}
		}

		public void PlayAreaEntryColdStorage()
		{
			if (CanPlay(3) && m_curAEColdStorageIndex < AC_ColdStorage.Length && !(TimeSinceAreaHook < 120f))
			{
				TimeSinceAreaHook = 0f;
				curClipPriority = 3;
				AUD.clip = AC_ColdStorage[m_curAEColdStorageIndex];
				AUD.Play();
				m_curAEColdStorageIndex++;
			}
		}

		public void PlayAreaEntryOffice()
		{
			if (CanPlay(3) && m_curAEOfficeIndex < AC_Office.Length && !(TimeSinceAreaHook < 120f))
			{
				TimeSinceAreaHook = 0f;
				curClipPriority = 3;
				AUD.clip = AC_Office[m_curAEOfficeIndex];
				AUD.Play();
				m_curAEOfficeIndex++;
			}
		}

		public void PlayAreaEntryRestaurant()
		{
			if (CanPlay(3) && m_curAERestaurantIndex < AC_Restaurant.Length && !(TimeSinceAreaHook < 120f))
			{
				TimeSinceAreaHook = 0f;
				curClipPriority = 3;
				AUD.clip = AC_Restaurant[m_curAERestaurantIndex];
				AUD.Play();
				m_curAERestaurantIndex++;
			}
		}

		public void PlayFoundSpecialItem()
		{
			if (CanPlay(2) && m_curFoundSpecialItemIndex < AC_FoundSpecialItem.Length && !(TimeSinceObjectHook < 20f))
			{
				TimeSinceObjectHook = 0f;
				curClipPriority = 2;
				AUD.clip = AC_FoundSpecialItem[m_curFoundSpecialItemIndex];
				AUD.Play();
				m_curFoundSpecialItemIndex++;
			}
		}

		public void PlayFoundJunkItem()
		{
			if (CanPlay(2) && m_curFoundJunkItemIndex < AC_FoundJunkItem.Length && !(TimeSinceObjectHook < 20f))
			{
				TimeSinceObjectHook = 0f;
				curClipPriority = 2;
				AUD.clip = AC_FoundJunkItem[m_curFoundJunkItemIndex];
				AUD.Play();
				m_curFoundJunkItemIndex++;
			}
		}

		public void PlayFoundNormalItem()
		{
			if (CanPlay(2) && m_curFoundNormalItemIndex < AC_FoundNormalWeapon.Length && !(TimeSinceObjectHook < 20f))
			{
				TimeSinceObjectHook = 0f;
				curClipPriority = 2;
				AUD.clip = AC_FoundNormalWeapon[m_curFoundNormalItemIndex];
				AUD.Play();
				m_curFoundNormalItemIndex++;
			}
		}

		public void PlayFoundRareItem()
		{
			if (CanPlay(2) && m_curFoundRareItemIndex < AC_FoundRareWeapon.Length && !(TimeSinceObjectHook < 20f))
			{
				TimeSinceObjectHook = 0f;
				curClipPriority = 2;
				AUD.clip = AC_FoundRareWeapon[m_curFoundRareItemIndex];
				AUD.Play();
				m_curFoundRareItemIndex++;
			}
		}

		public void PlayAboutToDie()
		{
			if (CanPlay(4) && m_curAboutToDieIndex < AC_PlayerAboutToDie.Length && !(TimeSinceHealthHook < 15f))
			{
				TimeSinceHealthHook = 0f;
				curClipPriority = 4;
				AUD.clip = AC_PlayerAboutToDie[m_curAboutToDieIndex];
				AUD.Play();
				m_curAboutToDieIndex++;
			}
		}

		public void PlayDiedCheating()
		{
			if (CanPlay(4))
			{
				curClipPriority = 4;
				AUD.clip = AC_PlayerDiedCheating[Random.Range(0, AC_PlayerDiedCheating.Length)];
				AUD.Play();
			}
		}

		public void PlayDiedOutOfHealth()
		{
			if (CanPlay(5))
			{
				curClipPriority = 5;
				AUD.clip = AC_PlayerDiedOutOfHealth[Random.Range(0, AC_PlayerDiedOutOfHealth.Length)];
				AUD.Play();
			}
		}

		public void PlayTimeWarning(int i)
		{
			if (CanPlay(3))
			{
				curClipPriority = 3;
				AUD.clip = AC_TimeRemaining[i];
				AUD.Play();
			}
		}

		private void Update()
		{
			if (m_intercomCheckTick > 0f)
			{
				m_intercomCheckTick -= Time.deltaTime;
			}
			else
			{
				m_intercomCheckTick = Random.Range(1.5f, 2f);
				CheckForClosestIntercom();
			}
			if (m_curIntercom == null)
			{
				CheckForClosestIntercom();
			}
			TimeSinceObjectHook += Time.deltaTime;
			TimeSinceObjectHook += Time.deltaTime;
			TimeSinceAreaHook += Time.deltaTime;
			TimeSinceHealthHook += Time.deltaTime;
			TimeSinceJumpScareHook += Time.deltaTime;
			TimeSinceTrapRoomFailingHook += Time.deltaTime;
		}

		private void CheckForClosestIntercom()
		{
			float num = 100f;
			int num2 = 0;
			Vector3 position = Camera.main.transform.position;
			for (int i = 0; i < Intercoms.Length; i++)
			{
				float num3 = Vector3.Distance(Intercoms[i].position, position);
				if (num3 <= num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (m_curIntercom != Intercoms[num2])
			{
				m_lastIntercom = m_curIntercom;
				m_curIntercom = Intercoms[num2];
				base.transform.position = m_curIntercom.position;
			}
		}

		private void ShuffleClips(AudioClip[] clips)
		{
			for (int i = 0; i < clips.Length; i++)
			{
				AudioClip audioClip = clips[i];
				int num = Random.Range(i, clips.Length);
				clips[i] = clips[num];
				clips[num] = audioClip;
			}
		}
	}
}
