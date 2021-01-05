// Decompiled with JetBrains decompiler
// Type: FistVR.MG_BurningMeatBall
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_BurningMeatBall : MonoBehaviour, IFVRDamageable
  {
    private Rigidbody RB;
    public Transform TargOverride;
    private bool m_isExploded;
    public GameObject[] Spawns;
    private float Life = 2500f;
    public AudioSource Source;
    public AudioLowPassFilter LowPassFilter;
    public AnimationCurve OcclusionFactorCurve;
    public AnimationCurve OcclusionVolumeCurve;
    public LayerMask OcclusionLM;
    private float fuse = 1f;

    private void Awake() => this.RB = this.GetComponent<Rigidbody>();

    private void FixedUpdate() => this.Tick();

    private float GetLowPassOcclusionValue(Vector3 start, Vector3 end)
    {
      if (!Physics.Linecast(start, end, (int) this.OcclusionLM, QueryTriggerInteraction.Ignore))
        return 22000f;
      float time = Vector3.Distance(start, end);
      this.Source.volume = 0.3f * this.OcclusionVolumeCurve.Evaluate(time);
      return this.OcclusionFactorCurve.Evaluate(time);
    }

    private void Tick()
    {
      if ((Object) GM.CurrentPlayerBody == (Object) null)
        return;
      this.LowPassFilter.cutoffFrequency = Mathf.MoveTowards(this.LowPassFilter.cutoffFrequency, this.GetLowPassOcclusionValue(this.transform.position, GM.CurrentPlayerBody.Head.position), Time.deltaTime * 20000f);
      Vector3 position = GM.CurrentPlayerBody.transform.position;
      float num = Vector3.Distance(position, this.transform.position);
      if ((double) num > 20.0)
      {
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        Vector3 lhs = position - this.transform.position;
        lhs.Normalize();
        this.RB.angularVelocity = Vector3.Lerp(this.RB.angularVelocity, -Vector3.Cross(lhs, Vector3.up) * 8f, Time.deltaTime * 20f);
        this.RB.AddForce(lhs * 3f, ForceMode.Acceleration);
        if ((double) num >= 1.20000004768372)
          return;
        this.fuse -= Time.deltaTime;
        if ((double) this.fuse >= 0.0)
          return;
        this.Explode();
      }
    }

    public void Damage(FistVR.Damage d)
    {
      this.Life -= d.Dam_TotalKinetic;
      if ((double) this.Life > 0.0)
        return;
      this.Explode();
    }

    private void Explode()
    {
      if (this.m_isExploded)
        return;
      this.m_isExploded = true;
      for (int index = 0; index < this.Spawns.Length; ++index)
        Object.Instantiate<GameObject>(this.Spawns[index], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
