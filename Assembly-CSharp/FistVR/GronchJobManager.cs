// Decompiled with JetBrains decompiler
// Type: FistVR.GronchJobManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class GronchJobManager : MonoBehaviour
  {
    private GronchJobManager.GronchJobType m_curActiveJob = GronchJobManager.GronchJobType.None;
    private bool m_isWorking;
    [Header("JOB PANEL")]
    public Text LBL_JobName;
    public Text LBL_TimeTilFired;
    public Text LBL_TimeTilNextPay;
    public Text LBL_TimeTilNextPromotion;
    public Text LBL_GBucksEarned;
    public Text LBL_Wage;
    public Transform JobPanel;
    public List<GronchJobManager.GronchJobObjs> JobObjs;
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
      if (GM.MMFlags.hasGenWPH)
        return;
      GM.MMFlags.hasGenWPH = true;
      GM.MMFlags.WPH.Add(0);
      GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
      GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
      GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
      GM.MMFlags.WPH.Add(UnityEngine.Random.Range(725, 1200));
    }

    public void HandlePlayerDeath()
    {
      if (!this.m_isWorking)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Gronch_Yeah, this.transform.position);
      for (int index = 0; index < this.JobObjs[(int) this.m_curActiveJob].ActivateOnStartJob.Count; ++index)
        this.JobObjs[(int) this.m_curActiveJob].ActivateOnStartJob[index].BroadcastMessage("PlayerDied", (object) this, SendMessageOptions.DontRequireReceiver);
    }

    public void SetJobAndStart(int i)
    {
      this.m_curActiveJob = (GronchJobManager.GronchJobType) i;
      this.m_isWorking = true;
      this.BeginSelectedJob();
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Gronch_Hired, this.transform.position);
      this.JobPanel.gameObject.GetComponent<AudioSource>().clip = this.JobObjs[(int) this.m_curActiveJob].Song;
      this.JobPanel.gameObject.GetComponent<AudioSource>().Play();
      this.ppPay = this.JobObjs[(int) this.m_curActiveJob].StartingPay;
    }

    public void BeginSelectedJob()
    {
      if (this.m_curActiveJob == GronchJobManager.GronchJobType.None)
        return;
      for (int index = 0; index < this.JobObjs[(int) this.m_curActiveJob].ActivateOnStartJob.Count; ++index)
        this.JobObjs[(int) this.m_curActiveJob].ActivateOnStartJob[index].BroadcastMessage("BeginJob", (object) this, SendMessageOptions.DontRequireReceiver);
      this.JobPanel.position = this.JobObjs[(int) this.m_curActiveJob].JobScreenPoint.position;
      this.JobPanel.rotation = this.JobObjs[(int) this.m_curActiveJob].JobScreenPoint.rotation;
      this.m_tickToFired = this.JobObjs[(int) this.m_curActiveJob].BaseJobTime;
      this.m_maxfiretime = this.JobObjs[(int) this.m_curActiveJob].BaseJobTime;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(this.JobObjs[(int) this.m_curActiveJob].RespawnPoint.position, true, this.JobObjs[(int) this.m_curActiveJob].RespawnPoint.forward);
      this.m_origSpawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
      this.m_origSpawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
      GM.CurrentSceneSettings.DeathResetPoint.position = this.JobObjs[(int) this.m_curActiveJob].RespawnPoint.position;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.JobObjs[(int) this.m_curActiveJob].RespawnPoint.rotation;
    }

    public void Promotion()
    {
      ++this.ppPay;
      this.m_maxfiretime -= this.JobObjs[(int) this.m_curActiveJob].DeductPerPromotion;
      this.m_maxfiretime = Mathf.Clamp(this.m_maxfiretime, 5f, this.m_maxfiretime);
    }

    public void DidJobStuff() => this.m_tickToFired = this.m_maxfiretime;

    private void Update()
    {
      if (!this.m_isWorking)
        return;
      this.m_tickToFired -= Time.deltaTime;
      if ((double) this.m_tickToFired <= 0.0)
        this.Fired();
      if (this.JobObjs[(int) this.m_curActiveJob].HasPromotionTick)
      {
        this.m_tickTilNextPromotion -= Time.deltaTime;
        if ((double) this.m_tickTilNextPromotion <= 0.0)
        {
          this.m_tickTilNextPromotion = 60f;
          ++this.ppPay;
          --this.m_maxfiretime;
          this.m_maxfiretime = Mathf.Clamp(this.m_maxfiretime, 5f, this.m_maxfiretime);
        }
      }
      this.m_tickTilNextPay -= Time.deltaTime;
      if ((double) this.m_tickTilNextPay <= 0.0)
      {
        this.m_tickTilNextPay = 20f;
        this.GBucksEarned += this.ppPay;
        this.smeme.DrawGlobal();
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Payout, this.transform.position);
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Gronch_Yeah, this.transform.position);
        GM.MMFlags.AGB(this.ppPay);
        GM.MMFlags.SaveToFile();
      }
      this.LBL_JobName.text = this.m_curActiveJob.ToString();
      this.LBL_TimeTilFired.text = this.m_tickToFired.ToString("00");
      this.LBL_TimeTilNextPay.text = this.m_tickTilNextPay.ToString("00");
      this.LBL_GBucksEarned.text = "G" + ((float) this.GBucksEarned * 0.01f).ToString("C", (IFormatProvider) new CultureInfo("en-US"));
      this.LBL_TimeTilNextPromotion.text = !this.JobObjs[(int) this.m_curActiveJob].HasPromotionTick ? this.JobObjs[(int) this.m_curActiveJob].CustomPromotionText : this.m_tickTilNextPromotion.ToString("00");
      this.LBL_Wage.text = "G" + ((float) ((double) this.ppPay * 60.0 * 0.00999999977648258)).ToString("C", (IFormatProvider) new CultureInfo("en-US"));
    }

    private void Fired()
    {
      for (int index = 0; index < this.JobObjs[(int) this.m_curActiveJob].ActivateOnStartJob.Count; ++index)
        this.JobObjs[(int) this.m_curActiveJob].ActivateOnStartJob[index].BroadcastMessage("EndJob", (object) this, SendMessageOptions.DontRequireReceiver);
      this.m_isWorking = false;
      this.m_curActiveJob = GronchJobManager.GronchJobType.None;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fired, this.transform.position);
      this.Invoke("FiredSound", 0.25f);
      GM.CurrentSceneSettings.DeathResetPoint.position = this.m_origSpawnPos;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.m_origSpawnRot;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, true, GM.CurrentSceneSettings.DeathResetPoint.forward);
      this.JobPanel.gameObject.GetComponent<AudioSource>().Stop();
      this.m_tickTilNextPay = 60f;
      this.GBucksEarned = 0;
    }

    private void FiredSound() => SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Gronch_Fired, this.transform.position);

    public enum GronchJobType
    {
      None = -1, // 0xFFFFFFFF
      SosigCorpseDisposal = 0,
      PowerUpGrilling = 1,
      CartridgeSorting = 2,
      TurretBallisticTesting = 3,
      MagazineReloading = 4,
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
  }
}
