// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Source_Map
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public abstract class SteamVR_Action_Source_Map
  {
    public SteamVR_Action action;

    public string fullPath { get; protected set; }

    public ulong handle { get; protected set; }

    public SteamVR_ActionSet actionSet { get; protected set; }

    public SteamVR_ActionDirections direction { get; protected set; }

    public virtual void PreInitialize(
      SteamVR_Action wrappingAction,
      string actionPath,
      bool throwErrors = true)
    {
      this.fullPath = actionPath;
      this.action = wrappingAction;
      this.actionSet = SteamVR_Input.GetActionSetFromPath(this.GetActionSetPath());
      this.direction = this.GetActionDirection();
      foreach (SteamVR_Input_Sources allSource in SteamVR_Input_Source.GetAllSources())
        this.PreinitializeMap(allSource, wrappingAction);
    }

    protected abstract void PreinitializeMap(
      SteamVR_Input_Sources inputSource,
      SteamVR_Action wrappingAction);

    public virtual void Initialize()
    {
      ulong pHandle = 0;
      EVRInputError actionHandle = OpenVR.Input.GetActionHandle(this.fullPath.ToLower(), ref pHandle);
      this.handle = pHandle;
      if (actionHandle == EVRInputError.None)
        return;
      Debug.LogError((object) ("<b>[SteamVR]</b> GetActionHandle (" + this.fullPath.ToLower() + ") error: " + actionHandle.ToString()));
    }

    private string GetActionSetPath() => this.fullPath.Substring(0, this.fullPath.IndexOf('/', this.fullPath.IndexOf('/', 1) + 1));

    private SteamVR_ActionDirections GetActionDirection()
    {
      int num = this.fullPath.IndexOf('/', this.fullPath.IndexOf('/', 1) + 1);
      int length = this.fullPath.IndexOf('/', num + 1) - num - 1;
      string str = this.fullPath.Substring(num + 1, length);
      if (str == "in")
        return SteamVR_ActionDirections.In;
      if (str == "out")
        return SteamVR_ActionDirections.Out;
      Debug.LogError((object) ("Could not find match for direction: " + str));
      return SteamVR_ActionDirections.In;
    }
  }
}
