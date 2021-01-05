// Decompiled with JetBrains decompiler
// Type: FistVR.Russet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Russet : FVRPhysicalObject
  {
    public Transform Beam;
    public LayerMask LM_Beam;
    private RaycastHit m_hit;
    private FVRPhysicalObject m_selectedObj;
    private bool m_isObjectInTransit;
    private float m_reset = 1f;
    public AudioEvent AudEvent_PewPew;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (hand.Input.TriggerDown && (Object) this.m_selectedObj == (Object) null)
        this.CastToGrab();
      if (hand.Input.TriggerUp && !this.m_isObjectInTransit)
        this.m_selectedObj = (FVRPhysicalObject) null;
      if (!((Object) this.m_selectedObj != (Object) null) || this.m_isObjectInTransit)
        return;
      float num = 3f;
      if ((double) Mathf.Abs(hand.Input.VelAngularLocal.x) <= (double) num && (double) Mathf.Abs(hand.Input.VelAngularLocal.y) <= (double) num)
        return;
      this.BeginFlick(this.m_selectedObj);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.m_reset < 0.0 || !this.m_isObjectInTransit)
        return;
      this.m_reset -= Time.deltaTime;
      if ((double) this.m_reset > 0.0)
        return;
      this.m_isObjectInTransit = false;
      this.m_selectedObj = (FVRPhysicalObject) null;
    }

    private void BeginFlick(FVRPhysicalObject o)
    {
      Vector3 vector3_1 = this.transform.position - o.transform.position;
      float proj_speed = Mathf.Clamp(Vector3.Distance(this.transform.position, o.transform.position) * 2f, 3f, 10f);
      Vector3 s0;
      int num = fts.solve_ballistic_arc(o.transform.position, proj_speed, this.transform.position, Mathf.Abs(Physics.gravity.y), out s0, out Vector3 _);
      Debug.Log((object) num);
      if (num < 1)
      {
        this.m_isObjectInTransit = false;
        this.m_selectedObj = (FVRPhysicalObject) null;
      }
      else
      {
        Vector3 vector3_2 = s0;
        SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_PewPew, this.transform.position);
        this.m_selectedObj.RootRigidbody.velocity = vector3_2;
        this.m_isObjectInTransit = true;
        this.m_reset = 2f;
      }
    }

    private void CastToGrab()
    {
      if (!Physics.Raycast(this.Beam.position, this.Beam.forward, out this.m_hit, 20f, (int) this.LM_Beam, QueryTriggerInteraction.Collide) || !((Object) this.m_hit.collider.attachedRigidbody != (Object) null))
        return;
      FVRPhysicalObject component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if (!((Object) component != (Object) null))
        return;
      this.m_selectedObj = component;
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_PewPew, this.transform.position);
    }
  }
}
