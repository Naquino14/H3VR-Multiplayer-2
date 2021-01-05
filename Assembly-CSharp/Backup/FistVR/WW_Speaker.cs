// Decompiled with JetBrains decompiler
// Type: FistVR.WW_Speaker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class WW_Speaker : MonoBehaviour, IFVRDamageable
  {
    public WW_TeleportMaster Master;
    public AudioSource AudioSource;
    public AudioLowPassFilter LPF;
    public bool HasMessage;
    public int MessageToUnlock = 30;
    public Transform SpawnPoint;
    public List<GameObject> SpawnOnDestroy;
    private bool m_isDestroyed;

    private void Update()
    {
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
      if ((double) num > 50.0 && this.AudioSource.isPlaying)
      {
        this.AudioSource.Stop();
      }
      else
      {
        if ((double) num >= 50.0)
          return;
        if (!this.AudioSource.isPlaying)
          this.AudioSource.Play();
        float t = Mathf.Clamp(Vector3.Angle(this.transform.forward, GM.CurrentPlayerBody.transform.position - this.transform.position), 0.0f, 120f) / 120f;
        this.AudioSource.volume = (float) (0.75 - (double) t * 0.400000005960464);
        this.LPF.cutoffFrequency = Mathf.Lerp(22000f, 1200f, t);
      }
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      if (this.HasMessage && !GM.Options.XmasFlags.MessagesAcquired[this.MessageToUnlock])
        this.Master.UnlockMessage(this.MessageToUnlock);
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnPoint.position, Quaternion.identity);
      this.gameObject.SetActive(false);
    }
  }
}
