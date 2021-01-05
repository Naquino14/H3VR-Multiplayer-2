// Decompiled with JetBrains decompiler
// Type: FistVR.GronchCorpseGrinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GronchCorpseGrinder : MonoBehaviour
  {
    private bool m_isJobStarted;
    private GronchJobManager m_M;
    public ParticleSystem PSystem_Sparks;
    public ParticleSystem PSystem_Sauce;
    public Transform[] Rollers;
    private float m_roll;
    public AudioSource Aud;
    public AudioClip Aud_Grind;
    public AudioSource Aud_GrindLoop;
    private float GrindTick = 0.1f;

    public void BeginJob(GronchJobManager m)
    {
      this.m_M = m;
      this.Aud_GrindLoop.Play();
    }

    public void EndJob(GronchJobManager m)
    {
      this.m_M = (GronchJobManager) null;
      this.Aud_GrindLoop.Stop();
    }

    private void Update()
    {
      this.m_roll += Time.deltaTime * 1720f;
      this.m_roll = Mathf.Repeat(this.m_roll, 360f);
      this.Rollers[0].localEulerAngles = new Vector3(0.0f, 0.0f, -this.m_roll);
      this.Rollers[1].localEulerAngles = new Vector3(0.0f, 0.0f, this.m_roll);
      if ((double) this.GrindTick <= 0.0)
        return;
      this.GrindTick -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider col) => this.CheckCol(col);

    private void CheckCol(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      SosigLink component = col.attachedRigidbody.gameObject.GetComponent<SosigLink>();
      if ((Object) component == (Object) null || (Object) component.O.QuickbeltSlot != (Object) null || component.O.m_isHardnessed)
        return;
      if ((double) this.GrindTick <= 0.0 && (Object) this.Aud_Grind != (Object) null)
      {
        this.GrindTick = Random.Range(0.2f, 0.5f);
        this.PSystem_Sparks.Emit(50);
        this.Aud.pitch = Random.Range(0.85f, 1.05f);
        this.Aud.PlayOneShot(this.Aud_Grind, 0.5f);
      }
      component.Damage(new Damage()
      {
        hitNormal = Vector3.up,
        Class = Damage.DamageClass.Environment,
        damageSize = 0.1f,
        Dam_Cutting = 100000f,
        Dam_Piercing = 100000f,
        Dam_Blunt = 100000f,
        point = component.transform.position,
        strikeDir = Vector3.up
      });
      this.m_M.DidJobStuff();
    }
  }
}
