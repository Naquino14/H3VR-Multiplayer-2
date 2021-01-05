// Decompiled with JetBrains decompiler
// Type: FistVR.AutoMeaterBlade
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AutoMeaterBlade : MonoBehaviour
  {
    public AutoMeater M;
    public AudioEvent AudEvent_Hit;
    public HingeJoint Joint;
    public ParticleSystem PSystem_Sparks;
    private ParticleSystem.EmitParams emitParams;
    private bool m_isShutDown;
    private float m_timeSinceDamage = 1f;

    private void Update()
    {
      if ((double) this.m_timeSinceDamage >= 1.0)
        return;
      this.m_timeSinceDamage += Time.deltaTime;
    }

    public void ShutDown()
    {
      if (this.m_isShutDown)
        return;
      this.m_isShutDown = true;
      this.Joint.useMotor = false;
    }

    public void Reactivate()
    {
      if (!this.m_isShutDown)
        return;
      this.m_isShutDown = false;
      this.Joint.useMotor = true;
    }

    private void OnCollisionEnter(Collision col)
    {
      if (this.m_isShutDown || (double) this.m_timeSinceDamage < 0.300000011920929)
        return;
      this.m_timeSinceDamage = Random.Range(0.0f, 0.1f);
      SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, this.AudEvent_Hit, col.contacts[0].point);
      int num = 0;
      for (int index = 0; index < col.contacts.Length; ++index)
      {
        if (num < 3)
        {
          ++num;
          this.emitParams.position = col.contacts[index].point;
          Vector3 vector3 = Vector3.Cross((col.contacts[index].point - this.transform.position).normalized, this.transform.up) * 30f + Random.onUnitSphere * 3f;
          this.emitParams.velocity = vector3;
          this.PSystem_Sparks.Emit(this.emitParams, 1);
          this.emitParams.velocity = vector3 * 0.2f + Random.onUnitSphere * 15f;
          this.PSystem_Sparks.Emit(this.emitParams, 1);
        }
      }
      bool flag1 = false;
      if (!((Object) col.collider.attachedRigidbody != (Object) null))
        return;
      flag1 = true;
      float magnitude = col.relativeVelocity.magnitude;
      bool flag2 = false;
      Damage dam = new Damage();
      IFVRDamageable component = col.contacts[0].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component == null && (Object) col.contacts[0].otherCollider.attachedRigidbody != (Object) null)
        component = col.contacts[0].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
      if (component == null)
        return;
      IFVRDamageable fvrDamageable = component;
      flag2 = true;
      dam.Class = Damage.DamageClass.Melee;
      dam.point = col.contacts[0].point;
      dam.hitNormal = col.contacts[0].normal;
      dam.strikeDir = this.M.RB.GetPointVelocity(col.contacts[0].point).normalized;
      dam.damageSize = 0.02f;
      dam.Source_IFF = this.M.E.IFFCode;
      dam.edgeNormal = this.transform.forward;
      dam.Dam_Blunt = 50f;
      dam.Dam_Cutting = 400f;
      dam.Dam_Piercing = 50f;
      dam.Dam_TotalKinetic = dam.Dam_Blunt + dam.Dam_Cutting + dam.Dam_Piercing;
      fvrDamageable.Damage(dam);
    }
  }
}
