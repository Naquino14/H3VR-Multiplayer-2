using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class GronchJobManager : MonoBehaviour
	{
		public enum GronchJobType
		{
			None = -1,
			SosigCorpseDisposal,
			PowerUpGrilling,
			CartridgeSorting,
			TurretBallisticTesting,
			MagazineReloading
		}

		[Serializable]
		public class GronchJobObjs
		{
			public Transform RespawnPoint;

			public Transform JobScreenPoint;

			public List<GameObject> ActivateOnStartJob;

			public bool HasPromotionTick = true;

			public int StartingPay = 12;

			public AudioClip Song;

			public string CustomPromotionText;

			public float BaseJobTime = 30f;

			public float DeductPerPromotion = 1f;
		}

		private GronchJobType m_curActiveJob = GronchJobType.None;

		private bool m_isWorking;

		[Header("JOB PANEL")]
		public Text LBL_JobName;

		public Text LBL_TimeTilFired;

		public Text LBL_TimeTilNextPay;

		public Text LBL_TimeTilNextPromotion;

		public Text LBL_GBucksEarned;

		public Text LBL_Wage;

		public Transform JobPanel;

		public List<GronchJobObjs> JobObjs;

		[Header("Audio")]
		public AudioEvent AudEvent_Payout;

		public AudioEvent AudEvent_Fired;

		public AudioEvent AudEvent_Gronch_Hired;

		public AudioEvent AudEvent_Gronch_Yeah;

		public AudioEvent AudEvent_Gronch_Fired;

		private float m_tickToFired = 30f;

		public SMEME smeme;

		private Vector3 m_origSpawnPos;

		private Quaternion m_origSpawnRot;

		private int GBucksEarned;

		private float m_tickTilNextPay = 20f;

		private float m_tickTilNextPromotion = 60f;

		private float m_maxfiretime = 30f;

		private int ppPay = 4;

		private void Start()
		{
			if (!GM.MMFlags.hasGenWPH)
			{
				GM.MMFlags.hasGenWPH = true;
				GM.MMFlags.WPH.Add(0);
				GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
				GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
				GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
				GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
			}
		}

		public void HandlePlayerDeath()
		{
			if (m_isWorking)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Gronch_Yeah, base.transform.position);
				for (int i = 0; i < JobObjs[(int)m_curActiveJob].ActivateOnStartJob.Count; i++)
				{
					JobObjs[(int)m_curActiveJob].ActivateOnStartJob[i].BroadcastMessage("PlayerDied", this, SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		public void SetJobAndStart(int i)
		{
			m_curActiveJob = (GronchJobType)i;
			m_isWorking = true;
			BeginSelectedJob();
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Gronch_Hired, base.transform.position);
			JobPanel.gameObject.GetComponent<AudioSource>().clip = JobObjs[(int)m_curActiveJob].Song;
			JobPanel.gameObject.GetComponent<AudioSource>().Play();
			ppPay = JobObjs[(int)m_curActiveJob].StartingPay;
		}

		public void BeginSelectedJob()
		{
			if (m_curActiveJob != GronchJobType.None)
			{
				for (int i = 0; i < JobObjs[(int)m_curActiveJob].ActivateOnStartJob.Count; i++)
				{
					JobObjs[(int)m_curActiveJob].ActivateOnStartJob[i].BroadcastMessage("BeginJob", this, SendMessageOptions.DontRequireReceiver);
				}
				JobPanel.position = JobObjs[(int)m_curActiveJob].JobScreenPoint.position;
				JobPanel.rotation = JobObjs[(int)m_curActiveJob].JobScreenPoint.rotation;
				m_tickToFired = JobObjs[(int)m_curActiveJob].BaseJobTime;
				m_maxfiretime = JobObjs[(int)m_curActiveJob].BaseJobTime;
				GM.CurrentMovementManager.TeleportToPoint(JobObjs[(int)m_curActiveJob].RespawnPoint.position, isAbsolute: true, JobObjs[(int)m_curActiveJob].RespawnPoint.forward);
				m_origSpawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
				m_origSpawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
				GM.CurrentSceneSettings.DeathResetPoint.position = JobObjs[(int)m_curActiveJob].RespawnPoint.position;
				GM.CurrentSceneSettings.DeathResetPoint.rotation = JobObjs[(int)m_curActiveJob].RespawnPoint.rotation;
			}
		}

		public void Promotion()
		{
			ppPay++;
			m_maxfiretime -= JobObjs[(int)m_curActiveJob].DeductPerPromotion;
			m_maxfiretime = Mathf.Clamp(m_maxfiretime, 5f, m_maxfiretime);
		}

		public void DidJobStuff()
		{
			m_tickToFired = m_maxfiretime;
		}

		private void Update()
		{
			if (!m_isWorking)
			{
				return;
			}
			m_tickToFired -= Time.deltaTime;
			if (m_tickToFired <= 0f)
			{
				Fired();
			}
			if (JobObjs[(int)m_curActiveJob].HasPromotionTick)
			{
				m_tickTilNextPromotion -= Time.deltaTime;
				if (m_tickTilNextPromotion <= 0f)
				{
					m_tickTilNextPromotion = 60f;
					ppPay++;
					m_maxfiretime -= 1f;
					m_maxfiretime = Mathf.Clamp(m_maxfiretime, 5f, m_maxfiretime);
				}
			}
			m_tickTilNextPay -= Time.deltaTime;
			if ((double)m_tickTilNextPay <= 0.0)
			{
				m_tickTilNextPay = 20f;
				GBucksEarned += ppPay;
				smeme.DrawGlobal();
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Payout, base.transform.position);
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Gronch_Yeah, base.transform.position);
				GM.MMFlags.AGB(ppPay);
				GM.MMFlags.SaveToFile();
			}
			LBL_JobName.text = m_curActiveJob.ToString();
			LBL_TimeTilFired.text = m_tickToFired.ToString("00");
			LBL_TimeTilNextPay.text = m_tickTilNextPay.ToString("00");
			LBL_GBucksEarned.text = "G" + ((float)GBucksEarned * 0.01f).ToString("C", new CultureInfo("en-US"));
			if (JobObjs[(int)m_curActiveJob].HasPromotionTick)
			{
				LBL_TimeTilNextPromotion.text = m_tickTilNextPromotion.ToString("00");
			}
			else
			{
				LBL_TimeTilNextPromotion.text = JobObjs[(int)m_curActiveJob].CustomPromotionText;
			}
			LBL_Wage.text = "G" + ((float)ppPay * 60f * 0.01f).ToString("C", new CultureInfo("en-US"));
		}

		private void Fired()
		{
			for (int i = 0; i < JobObjs[(int)m_curActiveJob].ActivateOnStartJob.Count; i++)
			{
				JobObjs[(int)m_curActiveJob].ActivateOnStartJob[i].BroadcastMessage("EndJob", this, SendMessageOptions.DontRequireReceiver);
			}
			m_isWorking = false;
			m_curActiveJob = GronchJobType.None;
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fired, base.transform.position);
			Invoke("FiredSound", 0.25f);
			GM.CurrentSceneSettings.DeathResetPoint.position = m_origSpawnPos;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = m_origSpawnRot;
			GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, isAbsolute: true, GM.CurrentSceneSettings.DeathResetPoint.forward);
			JobPanel.gameObject.GetComponent<AudioSource>().Stop();
			m_tickTilNextPay = 60f;
			GBucksEarned = 0;
		}

		private void FiredSound()
		{
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Gronch_Fired, base.transform.position);
		}
	}
}
