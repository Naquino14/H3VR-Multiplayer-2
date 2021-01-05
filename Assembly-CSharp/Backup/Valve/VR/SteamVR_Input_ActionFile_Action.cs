// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ActionFile_Action
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_ActionFile_Action
  {
    [JsonIgnore]
    private static string[] _requirementValues;
    public string name;
    public string type;
    public string scope;
    public string skeleton;
    public string requirement;
    private const string nameTemplate = "/actions/{0}/{1}/{2}";
    protected const string prefix = "/actions/";

    [JsonIgnore]
    public static string[] requirementValues
    {
      get
      {
        if (SteamVR_Input_ActionFile_Action._requirementValues == null)
          SteamVR_Input_ActionFile_Action._requirementValues = Enum.GetNames(typeof (SteamVR_Input_ActionFile_Action_Requirements));
        return SteamVR_Input_ActionFile_Action._requirementValues;
      }
    }

    public SteamVR_Input_ActionFile_Action GetCopy() => new SteamVR_Input_ActionFile_Action()
    {
      name = this.name,
      type = this.type,
      scope = this.scope,
      skeleton = this.skeleton,
      requirement = this.requirement
    };

    [JsonIgnore]
    public SteamVR_Input_ActionFile_Action_Requirements requirementEnum
    {
      get
      {
        for (int index = 0; index < SteamVR_Input_ActionFile_Action.requirementValues.Length; ++index)
        {
          if (string.Equals(SteamVR_Input_ActionFile_Action.requirementValues[index], this.requirement, StringComparison.CurrentCultureIgnoreCase))
            return (SteamVR_Input_ActionFile_Action_Requirements) index;
        }
        return SteamVR_Input_ActionFile_Action_Requirements.suggested;
      }
      set => this.requirement = value.ToString();
    }

    [JsonIgnore]
    public string codeFriendlyName => SteamVR_Input_ActionFile.GetCodeFriendlyName(this.name);

    [JsonIgnore]
    public string shortName => SteamVR_Input_ActionFile.GetShortName(this.name);

    [JsonIgnore]
    public string path
    {
      get
      {
        int num = this.name.LastIndexOf('/');
        return num != -1 && num + 1 < this.name.Length ? this.name.Substring(0, num + 1) : this.name;
      }
    }

    public static string CreateNewName(string actionSet, string direction) => string.Format("/actions/{0}/{1}/{2}", (object) actionSet, (object) direction, (object) "NewAction");

    public static string CreateNewName(
      string actionSet,
      SteamVR_ActionDirections direction,
      string actionName)
    {
      return string.Format("/actions/{0}/{1}/{2}", (object) actionSet, (object) direction.ToString().ToLower(), (object) actionName);
    }

    public static SteamVR_Input_ActionFile_Action CreateNew(
      string actionSet,
      SteamVR_ActionDirections direction,
      string actionType)
    {
      return new SteamVR_Input_ActionFile_Action()
      {
        name = SteamVR_Input_ActionFile_Action.CreateNewName(actionSet, direction.ToString().ToLower()),
        type = actionType
      };
    }

    [JsonIgnore]
    public SteamVR_ActionDirections direction => this.type.ToLower() == SteamVR_Input_ActionFile_ActionTypes.vibration ? SteamVR_ActionDirections.Out : SteamVR_ActionDirections.In;

    [JsonIgnore]
    public string actionSet
    {
      get
      {
        int length = this.name.IndexOf('/', "/actions/".Length);
        return length == -1 ? string.Empty : this.name.Substring(0, length);
      }
    }

    public void SetNewActionSet(string newSetName) => this.name = string.Format("/actions/{0}/{1}/{2}", (object) newSetName, (object) this.direction.ToString().ToLower(), (object) this.shortName);

    public override string ToString() => this.shortName;

    public override bool Equals(object obj)
    {
      if (!(obj is SteamVR_Input_ActionFile_Action))
        return base.Equals(obj);
      SteamVR_Input_ActionFile_Action actionFileAction = (SteamVR_Input_ActionFile_Action) obj;
      return this == obj || this.name == actionFileAction.name && this.type == actionFileAction.type && (this.skeleton == actionFileAction.skeleton && this.requirement == actionFileAction.requirement);
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
