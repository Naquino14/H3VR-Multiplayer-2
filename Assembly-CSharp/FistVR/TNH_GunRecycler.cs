// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_GunRecycler
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TNH_GunRecycler : MonoBehaviour
  {
    public TNH_Manager M;
    public Transform Spawnpoint_Token;
    public Transform ScanningVolume;
    public LayerMask ScanningLM;
    public AudioEvent AudEvent_Fail;
    public AudioEvent AudEvent_Spawn;
    private List<FVRFireArm> m_detectedFirearms = new List<FVRFireArm>();
    private Collider[] colbuffer;
    private float m_scanTick = 1f;

    private void Start() => this.colbuffer = new Collider[50];

    public void Button_Recycler()
    {
      if (this.m_detectedFirearms.Count <= 0)
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
      }
      else
      {
        if ((Object) this.m_detectedFirearms[0] != (Object) null)
          Object.Destroy((Object) this.m_detectedFirearms[0].gameObject);
        this.m_detectedFirearms.Clear();
        this.M.AddTokens(1, false);
        this.M.EnqueueTokenLine(1);
      }
    }

    private void Update()
    {
      this.m_scanTick -= Time.deltaTime;
      if ((double) this.m_scanTick > 0.0)
        return;
      this.m_scanTick = Random.Range(0.8f, 1f);
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) >= 12.0)
        return;
      this.Scan();
    }

    private void Scan()
    {
      int num = Physics.OverlapBoxNonAlloc(this.ScanningVolume.position, this.ScanningVolume.localScale * 0.5f, this.colbuffer, this.ScanningVolume.rotation, (int) this.ScanningLM, QueryTriggerInteraction.Collide);
      this.m_detectedFirearms.Clear();
      for (int index = 0; index < num; ++index)
      {
        if ((Object) this.colbuffer[index].attachedRigidbody != (Object) null)
        {
          FVRFireArm component = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
          if ((Object) component != (Object) null && !component.SpawnLockable && (!component.IsHeld && (Object) component.QuickbeltSlot == (Object) null) && !this.m_detectedFirearms.Contains(component))
            this.m_detectedFirearms.Add(component);
        }
      }
    }
  }
}
