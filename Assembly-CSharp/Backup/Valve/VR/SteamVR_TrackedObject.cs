// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_TrackedObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
  public class SteamVR_TrackedObject : MonoBehaviour
  {
    public SteamVR_TrackedObject.EIndex index;
    [Tooltip("If not set, relative to parent")]
    public Transform origin;
    private SteamVR_Events.Action newPosesAction;

    private SteamVR_TrackedObject() => this.newPosesAction = SteamVR_Events.NewPosesAction(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));

    public bool isValid { get; private set; }

    private void OnNewPoses(TrackedDevicePose_t[] poses)
    {
      if (this.index == SteamVR_TrackedObject.EIndex.None)
        return;
      int index = (int) this.index;
      this.isValid = false;
      if (poses.Length <= index || !poses[index].bDeviceIsConnected || !poses[index].bPoseIsValid)
        return;
      this.isValid = true;
      SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(poses[index].mDeviceToAbsoluteTracking);
      if ((UnityEngine.Object) this.origin != (UnityEngine.Object) null)
      {
        this.transform.position = this.origin.transform.TransformPoint(rigidTransform.pos);
        this.transform.rotation = this.origin.rotation * rigidTransform.rot;
      }
      else
      {
        this.transform.localPosition = rigidTransform.pos;
        this.transform.localRotation = rigidTransform.rot;
      }
    }

    private void Awake() => this.OnEnable();

    private void OnEnable()
    {
      if ((UnityEngine.Object) SteamVR_Render.instance == (UnityEngine.Object) null)
        this.enabled = false;
      else
        this.newPosesAction.enabled = true;
    }

    private void OnDisable()
    {
      this.newPosesAction.enabled = false;
      this.isValid = false;
    }

    public void SetDeviceIndex(int index)
    {
      if (!Enum.IsDefined(typeof (SteamVR_TrackedObject.EIndex), (object) index))
        return;
      this.index = (SteamVR_TrackedObject.EIndex) index;
    }

    public enum EIndex
    {
      None = -1, // 0xFFFFFFFF
      Hmd = 0,
      Device1 = 1,
      Device2 = 2,
      Device3 = 3,
      Device4 = 4,
      Device5 = 5,
      Device6 = 6,
      Device7 = 7,
      Device8 = 8,
      Device9 = 9,
      Device10 = 10, // 0x0000000A
      Device11 = 11, // 0x0000000B
      Device12 = 12, // 0x0000000C
      Device13 = 13, // 0x0000000D
      Device14 = 14, // 0x0000000E
      Device15 = 15, // 0x0000000F
    }
  }
}
