// Decompiled with JetBrains decompiler
// Type: FistVR.FVRBeamer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRBeamer : FVRPhysicalObject
  {
    [Header("Beamer Config")]
    public Renderer Light0;
    public Renderer Light1;
    public Renderer Light2;
    public Material MatLightGreen;
    public Material MatLightYellow;
    public Material MatLightRed;
    public FVRBeamerSwitch BatterySwitch;
    public FVRBeamerSwitch CapacitorSwitch;
    public FVRBeamerSwitch MotorSwitch;
    private bool m_isBatterySwitchedOn;
    private bool m_isCapacitorSwitchedOn;
    private bool m_isMotorSwitchedOn;
    private FVRBeamer.BeamerStatusLight m_batteryLight;
    private FVRBeamer.BeamerStatusLight m_capacitorLight;
    private FVRBeamer.BeamerStatusLight m_motorLight;
    private float curMotorSpeed;
    private float tarMotorSpeed;
    public AudioSource AudioMotor;
    public AudioSource AudioDrone;
    public AudioSource AudioDroneActive;
    public AudioSource AudioElectricity;
    public AudioSource AudioShunt;
    private FVRBeamer.BeamerPowerState m_powerState;
    private bool m_isManipulating;
    public Transform[] SpinnyParts;
    public Transform Aperture;
    public ParticleSystem RotorParticles;
    private AudioSource m_aud;
    [Header("Beamer Locus Config")]
    public FVRBeamerLocus GravLocus;
    private float m_locusDistance = 1f;
    private float m_locusMinDistance = 0.2f;
    private float m_locusMaxDistance = 50f;
    private float m_locusMover;
    private bool m_hasTriggeredUpSinceFiring = true;

    public void SetLocusMover(float l) => this.m_locusMover = l;

    protected override void Awake()
    {
      base.Awake();
      this.m_aud = this.GetComponent<AudioSource>();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.m_powerState = !this.m_isBatterySwitchedOn || !this.m_isCapacitorSwitchedOn || !this.m_isMotorSwitchedOn ? FVRBeamer.BeamerPowerState.Off : FVRBeamer.BeamerPowerState.On;
      this.tarMotorSpeed = this.m_powerState != FVRBeamer.BeamerPowerState.Off ? (!this.m_isManipulating ? 0.5f : 1f) : 0.0f;
      this.curMotorSpeed = Mathf.Lerp(this.curMotorSpeed, this.tarMotorSpeed, Time.deltaTime * 1.6f);
      if ((double) this.curMotorSpeed < 0.0500000007450581)
      {
        this.AudioMotor.Stop();
        this.AudioDrone.Stop();
        this.AudioDroneActive.Stop();
        ParticleSystem.EmissionModule emission = this.RotorParticles.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.constantMax = 0.0f;
        emission.rate = rate;
      }
      else
      {
        if (!this.AudioMotor.isPlaying)
          this.AudioMotor.Play();
        if (!this.AudioDrone.isPlaying)
          this.AudioDrone.Play();
        if (!this.AudioDroneActive.isPlaying)
          this.AudioDroneActive.Play();
        this.AudioMotor.volume = (float) (((double) this.curMotorSpeed * 0.25 - 0.100000001490116) * 0.300000011920929);
        this.AudioMotor.pitch = (float) ((double) this.curMotorSpeed * 0.5 + 0.5);
        this.AudioDrone.volume = (float) (((double) this.curMotorSpeed * 0.400000005960464 - 0.100000001490116) * 0.300000011920929);
        this.AudioDrone.pitch = (float) ((double) this.curMotorSpeed * 0.300000011920929 + 0.699999988079071);
        this.AudioDroneActive.volume = (float) (((double) this.curMotorSpeed * 1.10000002384186 - 0.5) * 0.300000011920929);
        this.AudioDroneActive.pitch = (float) ((double) this.curMotorSpeed * 0.5 + 0.5);
        ParticleSystem.EmissionModule emission = this.RotorParticles.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.constantMax = (float) (((double) this.curMotorSpeed - 0.5) * 20.0);
        emission.rate = rate;
      }
      foreach (Transform spinnyPart in this.SpinnyParts)
      {
        float num = this.curMotorSpeed * 2000f;
        spinnyPart.localEulerAngles = new Vector3(spinnyPart.localEulerAngles.x, spinnyPart.localEulerAngles.y, spinnyPart.localEulerAngles.z + num * Time.deltaTime);
      }
      if (this.m_powerState == FVRBeamer.BeamerPowerState.On)
      {
        if (!this.GravLocus.gameObject.activeSelf)
          this.GravLocus.gameObject.transform.position = this.Aperture.position + this.Aperture.forward * this.m_locusDistance;
        this.GravLocus.gameObject.SetActive(true);
        this.GravLocus.SetExistence(true);
        if ((double) Mathf.Abs(this.m_locusMover) < 3.0)
          this.m_locusMover = 0.0f;
        this.m_locusDistance += (float) ((double) this.m_locusMover * (double) Time.deltaTime * 0.150000005960464);
        this.m_locusDistance = Mathf.Clamp(this.m_locusDistance, this.m_locusMinDistance, this.m_locusMaxDistance);
        this.GravLocus.SetTargetPoint(this.Aperture.position + this.Aperture.forward * this.m_locusDistance);
        if (this.m_isManipulating)
          this.GravLocus.SetGrav(true);
        else
          this.GravLocus.SetGrav(false);
      }
      else
        this.GravLocus.SetExistence(false);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.m_powerState == FVRBeamer.BeamerPowerState.On)
      {
        if (hand.Input.TriggerPressed && this.m_hasTriggeredUpSinceFiring)
        {
          this.m_isManipulating = true;
          if ((double) Random.value > 0.75)
            FXM.InitiateMuzzleFlash(this.GravLocus.transform.position, this.Aperture.forward, Random.Range(0.5f, 1.5f), Color.white, Random.Range(0.5f, 2f));
          if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown || !hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
          {
            this.GravLocus.Shunt();
            this.m_locusDistance = 0.25f;
            this.curMotorSpeed = 1.6f;
            this.m_isManipulating = false;
            this.AudioShunt.Stop();
            this.AudioShunt.Play();
            this.m_hasTriggeredUpSinceFiring = false;
          }
        }
        else
          this.m_isManipulating = false;
      }
      if (!hand.Input.TriggerUp)
        return;
      this.m_hasTriggeredUpSinceFiring = true;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_isManipulating = false;
    }

    public void Shunt() => this.m_locusDistance = 0.25f;

    public void SetSwitchState(int index, bool b)
    {
      switch (index)
      {
        case 0:
          this.m_isBatterySwitchedOn = b;
          break;
        case 1:
          this.m_isCapacitorSwitchedOn = b;
          if (b)
          {
            this.AudioElectricity.Play();
            break;
          }
          break;
        case 2:
          this.m_isMotorSwitchedOn = b;
          break;
      }
      this.UpdateStatusLights();
    }

    private void UpdateStatusLights()
    {
      this.m_batteryLight = !this.m_isBatterySwitchedOn ? FVRBeamer.BeamerStatusLight.Red : FVRBeamer.BeamerStatusLight.Green;
      this.m_capacitorLight = !this.m_isBatterySwitchedOn || !this.m_isCapacitorSwitchedOn ? (!this.m_isCapacitorSwitchedOn ? FVRBeamer.BeamerStatusLight.Red : FVRBeamer.BeamerStatusLight.Yellow) : FVRBeamer.BeamerStatusLight.Green;
      this.m_motorLight = !this.m_isBatterySwitchedOn || !this.m_isCapacitorSwitchedOn || !this.m_isMotorSwitchedOn ? (!this.m_isMotorSwitchedOn ? FVRBeamer.BeamerStatusLight.Red : FVRBeamer.BeamerStatusLight.Yellow) : FVRBeamer.BeamerStatusLight.Green;
      this.UpdateLightDisplay(this.Light0, this.m_batteryLight);
      this.UpdateLightDisplay(this.Light1, this.m_capacitorLight);
      this.UpdateLightDisplay(this.Light2, this.m_motorLight);
    }

    private void UpdateLightDisplay(Renderer rend, FVRBeamer.BeamerStatusLight lightStatus)
    {
      switch (lightStatus)
      {
        case FVRBeamer.BeamerStatusLight.Red:
          rend.material = this.MatLightRed;
          break;
        case FVRBeamer.BeamerStatusLight.Yellow:
          rend.material = this.MatLightYellow;
          break;
        case FVRBeamer.BeamerStatusLight.Green:
          rend.material = this.MatLightGreen;
          break;
      }
    }

    public enum BeamerStatusLight
    {
      Red,
      Yellow,
      Green,
    }

    public enum BeamerPowerState
    {
      Off,
      On,
    }
  }
}
