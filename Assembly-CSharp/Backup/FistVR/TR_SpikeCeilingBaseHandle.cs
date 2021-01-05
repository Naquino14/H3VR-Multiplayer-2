// Decompiled with JetBrains decompiler
// Type: FistVR.TR_SpikeCeilingBaseHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_SpikeCeilingBaseHandle : FVRInteractiveObject
  {
    private float m_curRot;
    public GameObject JackBase;
    public Transform HandleBase;
    private IMG_HandlePumpable Pumpable;
    public AudioSource audsource;
    private float curVolume;
    private float tarVolume;
    public bool SendsPositiveDelta = true;

    protected override void Awake()
    {
      base.Awake();
      this.Pumpable = this.JackBase.GetComponent<IMG_HandlePumpable>();
      this.m_curRot = Random.Range(-33f, 33f);
      this.transform.localEulerAngles = new Vector3(this.m_curRot, 0.0f, 0.0f);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 from = Vector3.ProjectOnPlane(hand.transform.position - this.HandleBase.position, this.HandleBase.right);
      float num1 = Vector3.Angle(from, this.HandleBase.forward);
      if ((double) Vector3.Dot(Vector3.up, from.normalized) > 0.0)
        num1 = -num1;
      float num2 = Mathf.Clamp(num1, -33f, 33f);
      float delta = Mathf.Abs(num2 - this.m_curRot);
      this.Pump(delta);
      if (this.SendsPositiveDelta)
        this.Pumpable.Pump(delta);
      else
        this.Pumpable.Pump(-delta);
      this.m_curRot = num2;
      this.transform.localEulerAngles = new Vector3(this.m_curRot, 0.0f, 0.0f);
    }

    private void Pump(float delta) => this.tarVolume += delta * 0.02f;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.tarVolume -= Time.deltaTime * 5f;
      this.audsource.volume = this.tarVolume;
      if ((double) this.tarVolume <= 0.0)
      {
        this.tarVolume = 0.0f;
        if (!this.audsource.isPlaying)
          return;
        this.audsource.Stop();
      }
      else
      {
        if (this.audsource.isPlaying)
          return;
        this.audsource.Play();
      }
    }
  }
}
