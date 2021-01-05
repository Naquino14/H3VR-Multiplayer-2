// Decompiled with JetBrains decompiler
// Type: FistVR.SosigVomitter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SosigVomitter : MonoBehaviour
  {
    public AudioEvent AudEvent_Vomit;
    public ParticleSystem PSystem_Vomit;
    private float m_tickDownToVomit = 0.25f;

    private void Start() => this.m_tickDownToVomit = Random.Range(0.6f, 2.8f);

    private void Update()
    {
      this.m_tickDownToVomit -= Time.deltaTime;
      if ((double) this.m_tickDownToVomit > 0.0)
        return;
      this.m_tickDownToVomit = Random.Range(0.6f, 2.8f);
      this.PSystem_Vomit.Emit(10);
      SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, this.AudEvent_Vomit, this.transform.position);
    }
  }
}
