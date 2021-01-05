// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_In_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
  public abstract class SteamVR_Action_In_Source : SteamVR_Action_Source, ISteamVR_Action_In_Source, ISteamVR_Action_Source
  {
    protected static uint inputOriginInfo_size;
    protected InputOriginInfo_t inputOriginInfo = new InputOriginInfo_t();
    protected InputOriginInfo_t lastInputOriginInfo = new InputOriginInfo_t();

    public bool isUpdating { get; set; }

    public float updateTime { get; protected set; }

    public abstract ulong activeOrigin { get; }

    public abstract ulong lastActiveOrigin { get; }

    public abstract bool changed { get; protected set; }

    public abstract bool lastChanged { get; protected set; }

    public SteamVR_Input_Sources activeDevice
    {
      get
      {
        this.UpdateOriginTrackedDeviceInfo();
        return SteamVR_Input_Source.GetSource(this.inputOriginInfo.devicePath);
      }
    }

    public uint trackedDeviceIndex
    {
      get
      {
        this.UpdateOriginTrackedDeviceInfo();
        return this.inputOriginInfo.trackedDeviceIndex;
      }
    }

    public string renderModelComponentName
    {
      get
      {
        this.UpdateOriginTrackedDeviceInfo();
        return this.inputOriginInfo.rchRenderModelComponentName;
      }
    }

    public string localizedOriginName
    {
      get
      {
        this.UpdateOriginTrackedDeviceInfo();
        return this.GetLocalizedOrigin();
      }
    }

    public float changedTime { get; protected set; }

    protected int lastOriginGetFrame { get; set; }

    public abstract void UpdateValue();

    public override void Initialize()
    {
      base.Initialize();
      if (SteamVR_Action_In_Source.inputOriginInfo_size != 0U)
        return;
      SteamVR_Action_In_Source.inputOriginInfo_size = (uint) Marshal.SizeOf(typeof (InputOriginInfo_t));
    }

    protected void UpdateOriginTrackedDeviceInfo()
    {
      if (this.lastOriginGetFrame == Time.frameCount)
        return;
      EVRInputError trackedDeviceInfo = OpenVR.Input.GetOriginTrackedDeviceInfo(this.activeOrigin, ref this.inputOriginInfo, SteamVR_Action_In_Source.inputOriginInfo_size);
      if (trackedDeviceInfo != EVRInputError.None)
        Debug.LogError((object) ("<b>[SteamVR]</b> GetOriginTrackedDeviceInfo error (" + this.fullPath + "): " + trackedDeviceInfo.ToString() + " handle: " + this.handle.ToString() + " activeOrigin: " + this.activeOrigin.ToString() + " active: " + (object) this.active));
      this.lastInputOriginInfo = this.inputOriginInfo;
      this.lastOriginGetFrame = Time.frameCount;
    }

    public string GetLocalizedOriginPart(params EVRInputStringBits[] localizedParts)
    {
      this.UpdateOriginTrackedDeviceInfo();
      return this.active ? SteamVR_Input.GetLocalizedName(this.activeOrigin, localizedParts) : (string) null;
    }

    public string GetLocalizedOrigin()
    {
      this.UpdateOriginTrackedDeviceInfo();
      if (!this.active)
        return (string) null;
      return SteamVR_Input.GetLocalizedName(this.activeOrigin, EVRInputStringBits.VRInputString_All);
    }
  }
}
