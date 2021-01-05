// Decompiled with JetBrains decompiler
// Type: FistVR.TR_Grinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_Grinder : MonoBehaviour
  {
    public TR_GrinderContoller GController;
    public Transform TopGrinder;
    public Transform BottomGrinder;
    private float m_curRot1;
    private float m_curRot2 = 45f;
    private float m_curSpeed;
    private float m_tarSpeed;
    private float m_transitionMult = 1f;
    private float m_MaxSpinSpeed = 1000f;
    public ParticleSystem Sparks;
    private ParticleSystem.EmitParams emitParams;
    public Transform ParticlePoint1;
    public Transform ParticlePoint2;
    public GameObject[] Triggers;
    public TR_GrinderDamageTrigger[] DamageTriggers;
    public GameObject[] SmokePEffects;
    public GameObject[] FirePEffects;

    private void Start() => this.StartSpinning();

    public void SetGController(TR_GrinderContoller c)
    {
      this.GController = c;
      for (int index = 0; index < this.DamageTriggers.Length; ++index)
        this.DamageTriggers[index].GController = c;
    }

    public void StartSpinning()
    {
      this.m_tarSpeed = this.m_MaxSpinSpeed;
      for (int index = 0; index < this.Triggers.Length; ++index)
        this.Triggers[index].SetActive(true);
    }

    public void StopSpinning()
    {
      this.m_tarSpeed = 0.0f;
      this.m_transitionMult = 2f;
      for (int index = 0; index < this.Triggers.Length; ++index)
        this.Triggers[index].SetActive(false);
    }

    public void EmitEvent(Vector3 point)
    {
      Vector3 pos = point;
      FXM.InitiateMuzzleFlash(pos, this.Sparks.transform.forward, Random.Range(0.25f, 1f), Color.white, Random.Range(0.25f, 1.5f));
      for (int index = 0; index < Random.Range(15, 20); ++index)
      {
        Vector3 vector3 = this.Sparks.transform.forward * Random.Range(2f, 8f) * 1f + Random.onUnitSphere * 12f;
        this.emitParams.position = pos;
        this.emitParams.velocity = vector3;
        this.Sparks.Emit(this.emitParams, 1);
      }
    }

    private void Update()
    {
      this.m_curSpeed = Mathf.Lerp(this.m_curSpeed, this.m_tarSpeed, Time.deltaTime * this.m_transitionMult);
      this.m_curRot1 += this.m_curSpeed * Time.deltaTime;
      this.m_curRot1 = Mathf.Repeat(this.m_curRot1, 360f);
      this.m_curRot2 += this.m_curSpeed * Time.deltaTime;
      this.m_curRot2 = Mathf.Repeat(this.m_curRot2, 360f);
      this.TopGrinder.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curRot1);
      this.BottomGrinder.localEulerAngles = new Vector3(0.0f, 0.0f, -this.m_curRot2);
      float num = this.m_curSpeed / this.m_MaxSpinSpeed;
      if ((double) num <= 0.100000001490116 || Random.Range(0, 10) != 0)
        return;
      Vector3 pos = Vector3.Lerp(this.ParticlePoint1.position, this.ParticlePoint2.position, Random.Range(0.0f, 1f));
      FXM.InitiateMuzzleFlash(pos, this.Sparks.transform.forward, Random.Range(0.25f, 1f), Color.white, Random.Range(0.25f, 1.5f));
      for (int index = 0; index < Random.Range(3, 9); ++index)
      {
        Vector3 vector3 = this.Sparks.transform.forward * Random.Range(2f, 15f) * num + Random.onUnitSphere * 6f;
        this.emitParams.position = pos;
        this.emitParams.velocity = vector3;
        this.Sparks.Emit(this.emitParams, 1);
      }
    }
  }
}
