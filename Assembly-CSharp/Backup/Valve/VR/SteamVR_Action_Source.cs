// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public abstract class SteamVR_Action_Source : ISteamVR_Action_Source
  {
    protected ulong inputSourceHandle;
    protected SteamVR_Action action;

    public string fullPath => this.action.fullPath;

    public ulong handle => this.action.handle;

    public SteamVR_ActionSet actionSet => this.action.actionSet;

    public SteamVR_ActionDirections direction => this.action.direction;

    public SteamVR_Input_Sources inputSource { get; protected set; }

    public bool setActive => this.actionSet.IsActive(this.inputSource);

    public abstract bool active { get; }

    public abstract bool activeBinding { get; }

    public abstract bool lastActive { get; protected set; }

    public abstract bool lastActiveBinding { get; }

    public virtual void Preinitialize(
      SteamVR_Action wrappingAction,
      SteamVR_Input_Sources forInputSource)
    {
      this.action = wrappingAction;
      this.inputSource = forInputSource;
    }

    public virtual void Initialize() => this.inputSourceHandle = SteamVR_Input_Source.GetHandle(this.inputSource);
  }
}
