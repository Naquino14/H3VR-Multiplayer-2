// Decompiled with JetBrains decompiler
// Type: FistVR.ShatterableWindow
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ShatterableWindow : MonoBehaviour, IFVRDamageable
  {
    public Collider Col;
    public List<Renderer> Rends;
    private bool m_isShattered;
    private float m_life = 300f;
    public List<Transform> Shatter_Points;
    public List<GameObject> Shatter_Shards;
    public AudioEvent AudEvent_Shatter;

    private void Awake() => this.m_life = Random.Range(200f, 800f);

    public void Damage(FistVR.Damage d)
    {
      if (this.m_isShattered || (double) d.Dam_TotalKinetic <= 0.0)
        return;
      this.m_life -= d.Dam_TotalKinetic;
      if ((double) this.m_life > 0.0)
        return;
      this.Shatter(d.point, d.strikeDir, 1f);
    }

    private void Shatter(Vector3 point, Vector3 dir, float magnitude)
    {
      if (this.m_isShattered)
        return;
      this.m_isShattered = true;
      this.Col.enabled = false;
      for (int index = 0; index < this.Rends.Count; ++index)
        this.Rends[index].enabled = false;
      for (int index = 0; index < this.Shatter_Points.Count; ++index)
        Object.Instantiate<GameObject>(this.Shatter_Shards[index], this.Shatter_Points[index].position, this.Shatter_Points[index].rotation).GetComponent<Rigidbody>().AddExplosionForce(Random.Range(1f, 3f), point, 3f, 0.1f, ForceMode.Impulse);
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_Shatter, this.transform.position, Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
    }
  }
}
