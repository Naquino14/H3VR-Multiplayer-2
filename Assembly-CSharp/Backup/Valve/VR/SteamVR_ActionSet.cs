// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_ActionSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_ActionSet : IEquatable<SteamVR_ActionSet>, ISteamVR_ActionSet, ISerializationCallbackReceiver
  {
    [SerializeField]
    private string actionSetPath;
    [NonSerialized]
    protected SteamVR_ActionSet_Data setData;
    [NonSerialized]
    protected bool initialized;
    private VRActiveActionSet_t[] emptySetCache = new VRActiveActionSet_t[0];
    private VRActiveActionSet_t[] setCache = new VRActiveActionSet_t[1];

    public SteamVR_Action[] allActions
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.allActions;
      }
    }

    public ISteamVR_Action_In[] nonVisualInActions
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.nonVisualInActions;
      }
    }

    public ISteamVR_Action_In[] visualActions
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.visualActions;
      }
    }

    public SteamVR_Action_Pose[] poseActions
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.poseActions;
      }
    }

    public SteamVR_Action_Skeleton[] skeletonActions
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.skeletonActions;
      }
    }

    public ISteamVR_Action_Out[] outActionArray
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.outActionArray;
      }
    }

    public string fullPath
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.fullPath;
      }
    }

    public string usage
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.usage;
      }
    }

    public ulong handle
    {
      get
      {
        if (!this.initialized)
          this.Initialize();
        return this.setData.handle;
      }
    }

    public static CreateType Create<CreateType>(string newSetPath) where CreateType : SteamVR_ActionSet, new()
    {
      CreateType createType = new CreateType();
      createType.PreInitialize(newSetPath);
      return createType;
    }

    public static CreateType CreateFromName<CreateType>(string newSetName) where CreateType : SteamVR_ActionSet, new()
    {
      CreateType createType = new CreateType();
      createType.PreInitialize(SteamVR_Input_ActionFile_ActionSet.GetPathFromName(newSetName));
      return createType;
    }

    public void PreInitialize(string newActionPath)
    {
      this.actionSetPath = newActionPath;
      this.setData = new SteamVR_ActionSet_Data();
      this.setData.fullPath = this.actionSetPath;
      this.setData.PreInitialize();
      this.initialized = true;
    }

    public virtual void FinishPreInitialize() => this.setData.FinishPreInitialize();

    public virtual void Initialize(bool createNew = false, bool throwErrors = true)
    {
      if (createNew)
      {
        this.setData.Initialize();
      }
      else
      {
        this.setData = SteamVR_Input.GetActionSetDataFromPath(this.actionSetPath);
        if (this.setData != null)
          ;
      }
      this.initialized = true;
    }

    public string GetPath() => this.actionSetPath;

    public bool IsActive(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any) => this.setData.IsActive(source);

    public float GetTimeLastChanged(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any) => this.setData.GetTimeLastChanged(source);

    public void Activate(
      SteamVR_Input_Sources activateForSource = SteamVR_Input_Sources.Any,
      int priority = 0,
      bool disableAllOtherActionSets = false)
    {
      this.setData.Activate(activateForSource, priority, disableAllOtherActionSets);
    }

    public void Deactivate(SteamVR_Input_Sources forSource = SteamVR_Input_Sources.Any) => this.setData.Deactivate(forSource);

    public string GetShortName() => this.setData.GetShortName();

    public bool ShowBindingHints(ISteamVR_Action_In originToHighlight = null)
    {
      if (originToHighlight == null)
      {
        for (int index = 0; index < this.allActions.Length; ++index)
        {
          if (this.allActions[index].direction == SteamVR_ActionDirections.In && this.allActions[index].active)
          {
            originToHighlight = (ISteamVR_Action_In) this.allActions[index];
            break;
          }
        }
      }
      if (originToHighlight == null)
        return false;
      this.setCache[0].ulActionSet = this.handle;
      int num = (int) OpenVR.Input.ShowBindingsForActionSet(this.setCache, 1U, originToHighlight.activeOrigin);
      return true;
    }

    public void HideBindingHints()
    {
      int num = (int) OpenVR.Input.ShowBindingsForActionSet(this.emptySetCache, 0U, 0UL);
    }

    public bool ReadRawSetActive(SteamVR_Input_Sources inputSource) => this.setData.ReadRawSetActive(inputSource);

    public float ReadRawSetLastChanged(SteamVR_Input_Sources inputSource) => this.setData.ReadRawSetLastChanged(inputSource);

    public int ReadRawSetPriority(SteamVR_Input_Sources inputSource) => this.setData.ReadRawSetPriority(inputSource);

    public SteamVR_ActionSet_Data GetActionSetData() => this.setData;

    public CreateType GetCopy<CreateType>() where CreateType : SteamVR_ActionSet, new()
    {
      if (!SteamVR_Input.ShouldMakeCopy())
        return (CreateType) this;
      CreateType createType = new CreateType();
      createType.actionSetPath = this.actionSetPath;
      createType.setData = this.setData;
      createType.initialized = true;
      return createType;
    }

    public bool Equals(SteamVR_ActionSet other) => !object.ReferenceEquals((object) null, (object) other) && this.actionSetPath == other.actionSetPath;

    public override bool Equals(object other)
    {
      if (object.ReferenceEquals((object) null, other))
        return string.IsNullOrEmpty(this.actionSetPath);
      if (object.ReferenceEquals((object) this, other))
        return true;
      return (object) (other as SteamVR_ActionSet) != null && this.Equals((SteamVR_ActionSet) other);
    }

    public override int GetHashCode() => this.actionSetPath == null ? 0 : this.actionSetPath.GetHashCode();

    public static bool operator !=(SteamVR_ActionSet set1, SteamVR_ActionSet set2) => !(set1 == set2);

    public static bool operator ==(SteamVR_ActionSet set1, SteamVR_ActionSet set2)
    {
      bool flag1 = object.ReferenceEquals((object) null, (object) set1) || string.IsNullOrEmpty(set1.actionSetPath) || set1.GetActionSetData() == null;
      bool flag2 = object.ReferenceEquals((object) null, (object) set2) || string.IsNullOrEmpty(set2.actionSetPath) || set2.GetActionSetData() == null;
      if (flag1 && flag2)
        return true;
      return flag1 == flag2 && set1.Equals(set2);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      if (this.setData != null && this.setData.fullPath != this.actionSetPath)
        this.setData = SteamVR_Input.GetActionSetDataFromPath(this.actionSetPath);
      if (this.initialized)
        return;
      this.Initialize(throwErrors: false);
    }
  }
}
