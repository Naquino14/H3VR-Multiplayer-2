// Decompiled with JetBrains decompiler
// Type: FistVR.GBeamer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GBeamer : FVRFireArm
  {
    [Header("Beamer")]
    public GBeamerModeLever Lever;
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
    private float m_capacitorCharge;
    private GBeamer.BeamerStatusLight m_batteryLight;
    private GBeamer.BeamerStatusLight m_capacitorLight;
    private GBeamer.BeamerStatusLight m_motorLight;
    private float curMotorSpeed;
    private float tarMotorSpeed;
    private List<float> m_rots = new List<float>()
    {
      0.0f,
      0.0f,
      0.0f
    };
    private GBeamer.BeamerPowerState m_powerState;
    [Header("Transforms")]
    public Transform Aperture;
    public Transform[] SpinnyParts;
    [Header("VFX")]
    public ParticleSystem RotorParticles;
    public ParticleSystem WindParticles;
    public List<Transform> Flaps;
    [Header("Audio")]
    public AudioEvent AudEvent_Capacitor;
    public AudioSource AudioMotor;
    public AudioSource AudioDrone;
    private bool m_isManipulating;
    private bool m_hasObject;
    private FVRPhysicalObject m_obj;
    [Header("ObjectSearch")]
    public LayerMask LM_ObjectHunt;
    public LayerMask LM_Blockers;
    public GameObject ShuntSplosion;
    public Transform ShuntSplosionPos;
    public Transform Locus;
    private RaycastHit m_hit;
    private HashSet<FVRPhysicalObject> pos = new HashSet<FVRPhysicalObject>();
    private float m_timeSinceLastSearch = 0.1f;
    private bool m_isObjSpinner;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      this.m_powerState = !this.m_isBatterySwitchedOn || !this.m_isCapacitorSwitchedOn || !this.m_isMotorSwitchedOn ? GBeamer.BeamerPowerState.Off : GBeamer.BeamerPowerState.On;
      if (this.m_powerState == GBeamer.BeamerPowerState.Off)
      {
        this.tarMotorSpeed = 0.0f;
        this.m_hasObject = false;
        this.m_isManipulating = false;
      }
      else
        this.tarMotorSpeed = !this.m_isManipulating ? 0.3f : 1f;
      if ((double) this.m_capacitorCharge < 1.0)
        this.m_capacitorCharge += Time.deltaTime;
      this.curMotorSpeed = Mathf.MoveTowards(this.curMotorSpeed, this.tarMotorSpeed, Time.deltaTime * 3f);
      for (int index1 = 0; index1 < this.SpinnyParts.Length; ++index1)
      {
        float num = this.curMotorSpeed * 2000f;
        List<float> rots;
        int index2;
        (rots = this.m_rots)[index2 = index1] = rots[index2] + num * Time.deltaTime;
        this.m_rots[index1] = Mathf.Repeat(this.m_rots[index1], 360f);
        this.SpinnyParts[index1].localEulerAngles = index1 != 1 ? new Vector3(0.0f, 0.0f, this.m_rots[index1]) : new Vector3(0.0f, 0.0f, -this.m_rots[index1]);
      }
      if ((double) this.curMotorSpeed < 0.0500000007450581)
      {
        if (this.AudioMotor.isPlaying)
          this.AudioMotor.Stop();
      }
      else
      {
        if (!this.AudioMotor.isPlaying)
          this.AudioMotor.Play();
        this.AudioMotor.volume = (float) (((double) this.curMotorSpeed * 0.300000011920929 - 0.0500000007450581) * 0.400000005960464);
        this.AudioMotor.pitch = (float) ((double) this.curMotorSpeed * 0.300000011920929 + 0.300000011920929);
      }
      if ((double) this.curMotorSpeed < 0.300000011920929)
      {
        if (this.AudioDrone.isPlaying)
          this.AudioDrone.Stop();
      }
      else
      {
        if (!this.AudioDrone.isPlaying)
          this.AudioDrone.Play();
        if (this.m_hasObject)
        {
          this.AudioDrone.volume = (float) (((double) this.curMotorSpeed * 0.400000005960464 - 0.0500000007450581) * 1.0);
          this.AudioDrone.pitch = (float) ((double) this.curMotorSpeed * 0.5 + 0.800000011920929);
        }
        else
        {
          this.AudioDrone.volume = (float) (((double) this.curMotorSpeed * 0.400000005960464 - 0.0500000007450581) * 0.699999988079071);
          this.AudioDrone.pitch = (float) ((double) this.curMotorSpeed * 0.300000011920929 + 0.699999988079071);
        }
      }
      if ((double) this.curMotorSpeed > 0.400000005960464)
      {
        ParticleSystem.EmissionModule emission = this.RotorParticles.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.constantMax = (float) (((double) this.curMotorSpeed - 0.5) * 20.0);
        emission.rate = rate;
      }
      else
      {
        ParticleSystem.EmissionModule emission = this.RotorParticles.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.constantMax = 0.0f;
        emission.rate = rate;
      }
      if (this.m_isManipulating)
      {
        if ((Object) this.m_obj == (Object) null)
          this.m_hasObject = false;
        if (!this.m_hasObject)
        {
          this.ObjectSearch();
          this.Locus.gameObject.SetActive(false);
        }
        if (this.m_hasObject)
        {
          this.Locus.gameObject.SetActive(true);
          this.Locus.position = this.m_obj.RootRigidbody.worldCenterOfMass;
          Vector3 vector3_1 = this.Aperture.position + this.Aperture.forward * 0.75f;
          Quaternion rotation1 = this.Aperture.rotation;
          Vector3 worldCenterOfMass = this.m_obj.RootRigidbody.worldCenterOfMass;
          Quaternion quaternion1 = this.m_obj.RootRigidbody.rotation;
          this.m_isObjSpinner = false;
          if (this.m_obj.MP.CanNewStab)
            quaternion1 = Quaternion.LookRotation(this.m_obj.MP.StabDirection.forward, this.m_obj.transform.right);
          else if (this.m_obj.MP.HighDamageVectors.Count > 0)
          {
            this.m_isObjSpinner = true;
            quaternion1 = Quaternion.LookRotation(this.m_obj.MP.HighDamageVectors[0].forward, this.m_obj.transform.right);
          }
          Vector3 vector3_2 = worldCenterOfMass;
          Quaternion rotation2 = quaternion1;
          Vector3 vector3_3 = vector3_1;
          Quaternion quaternion2 = rotation1;
          Vector3 vector3_4 = vector3_3 - vector3_2;
          Quaternion quaternion3 = quaternion2 * Quaternion.Inverse(rotation2);
          float deltaTime = Time.deltaTime;
          float angle;
          Vector3 axis;
          quaternion3.ToAngleAxis(out angle, out axis);
          if ((double) angle > 180.0)
            angle -= 360f;
          if ((double) angle != 0.0)
            this.m_obj.RootRigidbody.angularVelocity = Vector3.MoveTowards(this.m_obj.RootRigidbody.angularVelocity, deltaTime * angle * axis * 6f, 50f * Time.fixedDeltaTime);
          this.m_obj.RootRigidbody.velocity = Vector3.MoveTowards(this.m_obj.RootRigidbody.velocity, vector3_4 * 450f * deltaTime, 50f * deltaTime);
        }
      }
      else
        this.Locus.gameObject.SetActive(false);
      this.UpdateBolts();
      if (!((Object) this.m_obj != (Object) null) || this.m_obj.IsDistantGrabbable())
        return;
      this.m_obj = (FVRPhysicalObject) null;
      this.m_hasObject = false;
    }

    private void UpdateBolts()
    {
      if (this.m_isManipulating)
      {
        if (this.m_hasObject)
        {
          for (int index = 0; index < this.Flaps.Count; ++index)
            this.Flaps[index].localEulerAngles = new Vector3(0.0f, Random.Range(0.0f, 3f) - 70f, 0.0f);
        }
        else
        {
          for (int index = 0; index < this.Flaps.Count; ++index)
            this.Flaps[index].localEulerAngles = new Vector3(0.0f, Random.Range(0.0f, 10f) - 45f, 0.0f);
        }
      }
      else
      {
        for (int index = 0; index < this.Flaps.Count; ++index)
          this.Flaps[index].localEulerAngles = new Vector3(0.0f, 17.7f, 0.0f);
      }
    }

    private void ObjectSearch()
    {
      if ((double) this.m_timeSinceLastSearch > 0.0)
      {
        this.m_timeSinceLastSearch -= Time.deltaTime;
      }
      else
      {
        this.m_timeSinceLastSearch = Random.Range(0.1f, 0.2f);
        this.pos.Clear();
        Collider[] colliderArray = Physics.OverlapCapsule(this.Aperture.position + this.Aperture.forward * 0.5f, this.Aperture.position + this.Aperture.forward * 4.5f, 0.5f, (int) this.LM_ObjectHunt, QueryTriggerInteraction.Collide);
        for (int index = 0; index < colliderArray.Length; ++index)
        {
          if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
          {
            FVRPhysicalObject component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
            if ((Object) component != (Object) null && component.IsDistantGrabbable() && (Object) component != (Object) this)
              this.pos.Add(component);
          }
        }
        float num1 = 45f;
        float num2 = 10f;
        FVRPhysicalObject fvrPhysicalObject = (FVRPhysicalObject) null;
        foreach (FVRPhysicalObject po in this.pos)
        {
          Vector3 from = po.transform.position - this.Aperture.position;
          float magnitude = from.magnitude;
          float num3 = Vector3.Angle(from, this.Aperture.forward);
          if ((double) num3 < (double) num1 && (double) magnitude < (double) num2)
          {
            num1 = num3;
            num2 = magnitude;
            fvrPhysicalObject = po;
          }
        }
        if (!((Object) fvrPhysicalObject != (Object) null))
          return;
        this.m_hasObject = true;
        this.m_obj = fvrPhysicalObject;
      }
    }

    private void ShuntHeldObject()
    {
      if (!this.m_hasObject)
        return;
      if (this.m_obj.IsHeld)
        this.m_obj.ForceBreakInteraction();
      this.m_obj.RootRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
      this.m_obj.RootRigidbody.velocity = this.Aperture.forward * 40f;
      if (!this.m_isObjSpinner)
        ;
      this.m_obj = (FVRPhysicalObject) null;
      this.m_hasObject = false;
      this.PlayAudioGunShot(true, FVRTailSoundClass.SuppressedLarge, FVRTailSoundClass.SuppressedLarge, SM.GetSoundEnvironment(this.transform.position));
      this.Recoil(true, true, false);
      this.WindParticles.Emit(20);
      this.RotorParticles.Emit(20);
      this.m_capacitorCharge = 0.0f;
      this.PlayAudioAsHandling(this.AudEvent_Capacitor, this.transform.position);
    }

    private void WideShunt()
    {
      this.pos.Clear();
      Collider[] colliderArray = Physics.OverlapCapsule(this.Aperture.position + this.Aperture.forward * 0.5f, this.Aperture.position + this.Aperture.forward * 4.5f, 0.5f, (int) this.LM_ObjectHunt, QueryTriggerInteraction.Collide);
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
        {
          FVRPhysicalObject component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
          if ((Object) component != (Object) null && component.IsDistantGrabbable() && (Object) component != (Object) this)
            this.pos.Add(component);
        }
      }
      foreach (FVRPhysicalObject po in this.pos)
      {
        Vector3 from = po.transform.position - this.Aperture.position;
        float magnitude = from.magnitude;
        float num1 = Vector3.Angle(from, this.Aperture.forward);
        float num2 = Mathf.Lerp(1f, 0.1f, magnitude / 5f);
        float num3 = Mathf.Lerp(1f, 0.4f, num1 / 75f);
        po.RootRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        po.RootRigidbody.velocity = this.Aperture.forward * 40f * num2 * num3;
      }
      this.m_obj = (FVRPhysicalObject) null;
      this.m_hasObject = false;
      this.PlayAudioGunShot(true, FVRTailSoundClass.SuppressedLarge, FVRTailSoundClass.SuppressedLarge, SM.GetSoundEnvironment(this.transform.position));
      this.Recoil(true, true, false);
      this.WindParticles.Emit(20);
      this.RotorParticles.Emit(20);
      this.m_capacitorCharge = 0.0f;
      this.PlayAudioAsHandling(this.AudEvent_Capacitor, this.transform.position);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin || this.IsAltHeld)
        return;
      if (this.Lever.Mode == GBeamerModeLever.LeverMode.Rearward)
      {
        if (hand.Input.TriggerPressed && (double) this.m_capacitorCharge >= 1.0)
          this.m_isManipulating = true;
      }
      else if (this.Lever.Mode == GBeamerModeLever.LeverMode.Forward && hand.Input.TriggerDown && (double) this.m_capacitorCharge >= 1.0)
      {
        Object.Instantiate<GameObject>(this.ShuntSplosion, this.ShuntSplosionPos.position, this.ShuntSplosionPos.rotation);
        this.Invoke("WideShunt", 0.08f);
      }
      if (!hand.Input.TriggerUp)
        return;
      this.ShuntHeldObject();
      this.m_isManipulating = false;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_isManipulating = false;
    }

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
            this.PlayAudioAsHandling(this.AudEvent_Capacitor, this.transform.position);
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
      this.m_batteryLight = !this.m_isBatterySwitchedOn ? GBeamer.BeamerStatusLight.Red : GBeamer.BeamerStatusLight.Green;
      this.m_capacitorLight = !this.m_isBatterySwitchedOn || !this.m_isCapacitorSwitchedOn || (double) this.m_capacitorCharge < 1.0 ? (!this.m_isCapacitorSwitchedOn ? GBeamer.BeamerStatusLight.Red : GBeamer.BeamerStatusLight.Yellow) : GBeamer.BeamerStatusLight.Green;
      this.m_motorLight = !this.m_isBatterySwitchedOn || !this.m_isCapacitorSwitchedOn || !this.m_isMotorSwitchedOn ? (!this.m_isMotorSwitchedOn ? GBeamer.BeamerStatusLight.Red : GBeamer.BeamerStatusLight.Yellow) : GBeamer.BeamerStatusLight.Green;
      this.UpdateLightDisplay(this.Light0, this.m_batteryLight);
      this.UpdateLightDisplay(this.Light1, this.m_capacitorLight);
      this.UpdateLightDisplay(this.Light2, this.m_motorLight);
    }

    private void UpdateLightDisplay(Renderer rend, GBeamer.BeamerStatusLight lightStatus)
    {
      switch (lightStatus)
      {
        case GBeamer.BeamerStatusLight.Red:
          rend.material = this.MatLightRed;
          break;
        case GBeamer.BeamerStatusLight.Yellow:
          rend.material = this.MatLightYellow;
          break;
        case GBeamer.BeamerStatusLight.Green:
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
