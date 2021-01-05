// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action`2
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Valve.VR
{
  [Serializable]
  public abstract class SteamVR_Action<SourceMap, SourceElement> : SteamVR_Action, ISteamVR_Action, ISteamVR_Action_Source
    where SourceMap : SteamVR_Action_Source_Map<SourceElement>, new()
    where SourceElement : SteamVR_Action_Source, new()
  {
    [NonSerialized]
    protected SourceMap sourceMap;
    [NonSerialized]
    protected bool initialized;

    public virtual SourceElement this[SteamVR_Input_Sources inputSource] => this.sourceMap[inputSource];

    public override string fullPath => this.sourceMap.fullPath;

    public override ulong handle => this.sourceMap.handle;

    public override SteamVR_ActionSet actionSet => this.sourceMap.actionSet;

    public override SteamVR_ActionDirections direction => this.sourceMap.direction;

    public override bool active => this.sourceMap[SteamVR_Input_Sources.Any].active;

    public override bool lastActive => this.sourceMap[SteamVR_Input_Sources.Any].lastActive;

    public override bool activeBinding => this.sourceMap[SteamVR_Input_Sources.Any].activeBinding;

    public override bool lastActiveBinding => this.sourceMap[SteamVR_Input_Sources.Any].lastActiveBinding;

    public override void PreInitialize(string newActionPath)
    {
      this.actionPath = newActionPath;
      this.sourceMap = new SourceMap();
      this.sourceMap.PreInitialize((SteamVR_Action) this, this.actionPath, true);
      this.initialized = true;
    }

    protected override void CreateUninitialized(string newActionPath, bool caseSensitive)
    {
      this.actionPath = newActionPath;
      this.sourceMap = new SourceMap();
      this.sourceMap.PreInitialize((SteamVR_Action) this, this.actionPath, false);
      this.needsReinit = true;
      this.initialized = false;
    }

    protected override void CreateUninitialized(
      string newActionSet,
      SteamVR_ActionDirections direction,
      string newAction,
      bool caseSensitive)
    {
      this.actionPath = SteamVR_Input_ActionFile_Action.CreateNewName(newActionSet, direction, newAction);
      this.sourceMap = new SourceMap();
      this.sourceMap.PreInitialize((SteamVR_Action) this, this.actionPath, false);
      this.needsReinit = true;
      this.initialized = false;
    }

    public override string TryNeedsInitData()
    {
      if (this.needsReinit && this.actionPath != null)
      {
        SteamVR_Action actionForPartialPath = SteamVR_Action.FindExistingActionForPartialPath(this.actionPath);
        if (actionForPartialPath == (SteamVR_Action) null)
        {
          this.sourceMap = (SourceMap) null;
        }
        else
        {
          this.actionPath = actionForPartialPath.fullPath;
          this.sourceMap = (SourceMap) actionForPartialPath.GetSourceMap();
          this.initialized = true;
          this.needsReinit = false;
          return this.actionPath;
        }
      }
      return (string) null;
    }

    public override void Initialize(bool createNew = false, bool throwErrors = true)
    {
      if (this.needsReinit)
        this.TryNeedsInitData();
      if (createNew)
      {
        this.sourceMap.Initialize();
      }
      else
      {
        this.sourceMap = SteamVR_Input.GetActionDataFromPath<SourceMap>(this.actionPath);
        if ((object) this.sourceMap != null)
          ;
      }
      this.initialized = true;
    }

    public override SteamVR_Action_Source_Map GetSourceMap() => (SteamVR_Action_Source_Map) this.sourceMap;

    protected override void InitializeCopy(string newActionPath, SteamVR_Action_Source_Map newData)
    {
      this.actionPath = newActionPath;
      this.sourceMap = (SourceMap) newData;
      this.initialized = true;
    }

    protected void InitAfterDeserialize()
    {
      if ((object) this.sourceMap != null)
      {
        if (this.sourceMap.fullPath != this.actionPath)
        {
          this.needsReinit = true;
          this.TryNeedsInitData();
        }
        if (string.IsNullOrEmpty(this.actionPath))
          this.sourceMap = (SourceMap) null;
      }
      if (this.initialized)
        return;
      this.Initialize(false, false);
    }

    public override bool GetActive(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].active;

    public override bool GetActiveBinding(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].activeBinding;

    public override bool GetLastActive(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastActive;

    public override bool GetLastActiveBinding(SteamVR_Input_Sources inputSource) => this.sourceMap[inputSource].lastActiveBinding;
  }
}
