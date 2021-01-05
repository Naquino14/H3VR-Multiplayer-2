// Decompiled with JetBrains decompiler
// Type: FistVR.TrapLever
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TrapLever : FVRInteractiveObject
  {
    public float MaxRot = 30f;
    public Transform RefVector;
    public float ValvePos = 0.5f;
    public List<GameObject> MessageTargets;
    public AudioEvent AudEvent_Release;
    public Transform Lever;

    protected override void Awake()
    {
      base.Awake();
      this.ValvePos = 0.0f;
      this.Lever.localEulerAngles = new Vector3(0.0f, -this.MaxRot, 0.0f);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector3_1 = Vector3.ProjectOnPlane(hand.transform.position - this.Lever.position, this.Lever.up);
      float a = Vector3.Angle(this.RefVector.forward, vector3_1);
      Vector3 vector3_2 = Vector3.RotateTowards(this.RefVector.forward, vector3_1, Mathf.Min(a, this.MaxRot) * 0.0174533f, 0.0f);
      this.Lever.rotation = Quaternion.LookRotation(vector3_2, this.RefVector.up);
      this.ValvePos = (float) (((double) Vector3.Angle(this.RefVector.right, vector3_2) - 90.0) / ((double) this.MaxRot * 2.0) + 0.5);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Release, this.transform.position);
      if ((double) this.ValvePos < 0.5)
      {
        this.ValvePos = 1f;
        this.Lever.localEulerAngles = new Vector3(0.0f, this.MaxRot, 0.0f);
        for (int index = 0; index < this.MessageTargets.Count; ++index)
          this.MessageTargets[index].BroadcastMessage("ON", SendMessageOptions.DontRequireReceiver);
      }
      else
      {
        this.ValvePos = 0.0f;
        this.Lever.localEulerAngles = new Vector3(0.0f, -this.MaxRot, 0.0f);
        for (int index = 0; index < this.MessageTargets.Count; ++index)
          this.MessageTargets[index].BroadcastMessage("OFF", SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}
