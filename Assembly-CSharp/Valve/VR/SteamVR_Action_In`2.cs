// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_In`2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Valve.VR
{
  [Serializable]
  public abstract class SteamVR_Action_In<SourceMap, SourceElement> : SteamVR_Action<SourceMap, SourceElement>, ISteamVR_Action_In, ISteamVR_Action, ISteamVR_Action_In_Source, ISteamVR_Action_Source
    where SourceMap : SteamVR_Action_In_Source_Map<SourceElement>, new()
    where SourceElement : SteamVR_Action_In_Source, new()
  {
    public bool changed => this.sourceMap[SteamVR_Input_Sources.Any].changed;

    public bool lastChanged => this.sourceMap[SteamVR_Input_Sources.Any].changed;

    public float changedTime => this.sourceMap[SteamVR_Input_Sources.Any].changedTime;

    public float updateTime => this.sourceMap[SteamVR_Input_Sources.Any].updateTime;

    public ulong activeOrigin => this.sourceMap[SteamVR_Input_Sources.Any].activeOrigin;

    public ulong lastActiveOrigin => this.sourceMap[SteamVR_Input_Sources.Any].lastActiveOrigin;

    public SteamVR_Input_Sources activeDevice => this.sourceMap[SteamVR_Input_Sources.Any].activeDevice;

    public uint trackedDeviceIndex => this.sourceMap[SteamVR_Input_Sources.Any].trackedDeviceIndex;

    public string renderModelComponentName => this.sourceMap[SteamVR_Input_Sources.Any].renderModelComponentName;

    public string localizedOriginName => this.sourceMap[SteamVR_Input_Sources.Any].localizedOriginName;

    public virtual void UpdateValues() => this.sourceMap.UpdateValues();

    public virtual string GetRenderModelComponentName(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].renderModelComponentName;

    public virtual SteamVR_Input_Sources GetActiveDevice(
      SteamVR_Input_Sources inputSource)
    {
      return this.sourceMap[inputSource].activeDevice;
    }

    public virtual uint GetDeviceIndex(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].trackedDeviceIndex;

    public virtual bool GetChanged(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].changed;

    public override float GetTimeLastChanged(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].changedTime;

    public string GetLocalizedOriginPart(
      SteamVR_Input_Sources inputSource,
      params EVRInputStringBits[] localizedParts)
    {
      return this.sourceMap[inputSource].GetLocalizedOriginPart(localizedParts);
    }

    public string GetLocalizedOrigin(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].GetLocalizedOrigin();

    public override bool IsUpdating(SteamVR_Input_Sources inputSource) => this.sourceMap.IsUpdating(inputSource);

    public void ForceAddSourceToUpdateList(SteamVR_Input_Sources inputSource) => this.sourceMap.ForceAddSourceToUpdateList(inputSource);
  }
}
