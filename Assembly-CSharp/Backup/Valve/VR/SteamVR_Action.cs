// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public abstract class SteamVR_Action : IEquatable<SteamVR_Action>, ISteamVR_Action, ISteamVR_Action_Source
  {
    [SerializeField]
    protected string actionPath;
    [SerializeField]
    protected bool needsReinit;
    public static bool startUpdatingSourceOnAccess = true;
    [NonSerialized]
    private string cachedShortName;

    public static CreateType Create<CreateType>(string newActionPath) where CreateType : SteamVR_Action, new()
    {
      CreateType createType = new CreateType();
      createType.PreInitialize(newActionPath);
      return createType;
    }

    public static CreateType CreateUninitialized<CreateType>(
      string setName,
      SteamVR_ActionDirections direction,
      string newActionName,
      bool caseSensitive)
      where CreateType : SteamVR_Action, new()
    {
      CreateType createType = new CreateType();
      createType.CreateUninitialized(setName, direction, newActionName, caseSensitive);
      return createType;
    }

    public static CreateType CreateUninitialized<CreateType>(string actionPath, bool caseSensitive) where CreateType : SteamVR_Action, new()
    {
      CreateType createType = new CreateType();
      createType.CreateUninitialized(actionPath, caseSensitive);
      return createType;
    }

    public CreateType GetCopy<CreateType>() where CreateType : SteamVR_Action, new()
    {
      if (!SteamVR_Input.ShouldMakeCopy())
        return (CreateType) this;
      CreateType createType = new CreateType();
      createType.InitializeCopy(this.actionPath, this.GetSourceMap());
      return createType;
    }

    public abstract string TryNeedsInitData();

    protected abstract void InitializeCopy(string newActionPath, SteamVR_Action_Source_Map newData);

    public abstract string fullPath { get; }

    public abstract ulong handle { get; }

    public abstract SteamVR_ActionSet actionSet { get; }

    public abstract SteamVR_ActionDirections direction { get; }

    public bool setActive => this.actionSet.IsActive(SteamVR_Input_Sources.Any);

    public abstract bool active { get; }

    public abstract bool activeBinding { get; }

    public abstract bool lastActive { get; }

    public abstract bool lastActiveBinding { get; }

    public abstract void PreInitialize(string newActionPath);

    protected abstract void CreateUninitialized(string newActionPath, bool caseSensitive);

    protected abstract void CreateUninitialized(
      string newActionSet,
      SteamVR_ActionDirections direction,
      string newAction,
      bool caseSensitive);

    public abstract void Initialize(bool createNew = false, bool throwNotSetError = true);

    public abstract float GetTimeLastChanged(SteamVR_Input_Sources inputSource);

    public abstract SteamVR_Action_Source_Map GetSourceMap();

    public abstract bool GetActive(SteamVR_Input_Sources inputSource);

    public bool GetSetActive(SteamVR_Input_Sources inputSource) => this.actionSet.IsActive(inputSource);

    public abstract bool GetActiveBinding(SteamVR_Input_Sources inputSource);

    public abstract bool GetLastActive(SteamVR_Input_Sources inputSource);

    public abstract bool GetLastActiveBinding(SteamVR_Input_Sources inputSource);

    public string GetPath() => this.actionPath;

    public abstract bool IsUpdating(SteamVR_Input_Sources inputSource);

    public override int GetHashCode() => this.actionPath == null ? 0 : this.actionPath.GetHashCode();

    public bool Equals(SteamVR_Action other) => !object.ReferenceEquals((object) null, (object) other) && this.actionPath == other.actionPath;

    public override bool Equals(object other)
    {
      if (object.ReferenceEquals((object) null, other))
        return string.IsNullOrEmpty(this.actionPath) || this.GetSourceMap() == null;
      if (object.ReferenceEquals((object) this, other))
        return true;
      return (object) (other as SteamVR_Action) != null && this.Equals((SteamVR_Action) other);
    }

    public static bool operator !=(SteamVR_Action action1, SteamVR_Action action2) => !(action1 == action2);

    public static bool operator ==(SteamVR_Action action1, SteamVR_Action action2)
    {
      bool flag1 = object.ReferenceEquals((object) null, (object) action1) || string.IsNullOrEmpty(action1.actionPath) || action1.GetSourceMap() == null;
      bool flag2 = object.ReferenceEquals((object) null, (object) action2) || string.IsNullOrEmpty(action2.actionPath) || action2.GetSourceMap() == null;
      if (flag1 && flag2)
        return true;
      return flag1 == flag2 && action1.Equals(action2);
    }

    public static SteamVR_Action FindExistingActionForPartialPath(string path)
    {
      if (string.IsNullOrEmpty(path) || path.IndexOf('/') == -1)
        return (SteamVR_Action) null;
      string[] strArray = path.Split('/');
      return strArray.Length < 2 || !string.IsNullOrEmpty(strArray[2]) ? SteamVR_Input.GetBaseActionFromPath(path) : SteamVR_Input.GetBaseAction(strArray[2], strArray[4]);
    }

    public string GetShortName()
    {
      if (this.cachedShortName == null)
        this.cachedShortName = SteamVR_Input_ActionFile.GetShortName(this.fullPath);
      return this.cachedShortName;
    }

    public void ShowOrigins()
    {
      int num = (int) OpenVR.Input.ShowActionOrigins(this.actionSet.handle, this.handle);
    }

    public void HideOrigins()
    {
      int num = (int) OpenVR.Input.ShowActionOrigins(0UL, 0UL);
    }
  }
}
