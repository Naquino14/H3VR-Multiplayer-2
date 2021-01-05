// Decompiled with JetBrains decompiler
// Type: FistVR.SpinningBladeTrap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SpinningBladeTrap : ZosigQuestManager
  {
    public HingeJoint Hinge;
    private bool m_isRunning;
    private float curPower;
    private float tarPower;
    public float MotorForce = 10f;
    public float TargSpeed = -100f;
    public AudioSource Aud;
    private ZosigGameManager M;
    public string Flag;
    public int ValueWhenOn;
    private bool m_isGassed;
    private bool isJustMotor;

    public void TurnOn() => this.m_isGassed = true;

    public void TurnOff() => this.m_isGassed = false;

    public override void Init(ZosigGameManager m) => this.M = m;

    private void Start()
    {
      if ((Object) this.Hinge == (Object) null)
        this.isJustMotor = true;
      else
        this.Hinge.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 50f;
    }

    private void Update()
    {
      this.tarPower = !this.m_isRunning || !this.m_isGassed ? 0.0f : 1f;
      this.curPower = Mathf.MoveTowards(this.curPower, this.tarPower, Time.deltaTime * 0.25f);
      if ((double) this.curPower > 0.0)
      {
        if (this.Aud.isPlaying)
        {
          this.Aud.volume = this.curPower * 0.4f;
        }
        else
        {
          this.Aud.volume = this.curPower * 0.4f;
          this.Aud.Play();
        }
      }
      else if (this.Aud.isPlaying)
        this.Aud.Stop();
      if (this.isJustMotor)
        return;
      if (this.m_isRunning)
      {
        JointMotor motor = this.Hinge.motor;
        motor.targetVelocity = this.TargSpeed;
        motor.force = this.curPower * this.MotorForce;
        this.Hinge.motor = motor;
      }
      else
      {
        JointMotor motor = this.Hinge.motor;
        motor.force = 0.0f;
        motor.targetVelocity = 0.0f;
        this.Hinge.motor = motor;
      }
    }

    public void ON()
    {
      if (this.m_isRunning)
        return;
      if ((Object) GM.ZMaster != (Object) null)
        GM.ZMaster.FlagM.AddToFlag("s_t", 1);
      this.M.FlagM.SetFlag(this.Flag, this.ValueWhenOn);
      this.m_isRunning = true;
    }

    public void OFF() => this.m_isRunning = false;
  }
}
