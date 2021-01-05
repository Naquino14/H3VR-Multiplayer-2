// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ActionFile_ActionSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_ActionFile_ActionSet
  {
    [JsonIgnore]
    private const string actionSetInstancePrefix = "instance_";
    public string name;
    public string usage;
    private const string nameTemplate = "/actions/{0}";
    [JsonIgnore]
    public List<SteamVR_Input_ActionFile_Action> actionsInList = new List<SteamVR_Input_ActionFile_Action>();
    [JsonIgnore]
    public List<SteamVR_Input_ActionFile_Action> actionsOutList = new List<SteamVR_Input_ActionFile_Action>();
    [JsonIgnore]
    public List<SteamVR_Input_ActionFile_Action> actionsList = new List<SteamVR_Input_ActionFile_Action>();

    [JsonIgnore]
    public string codeFriendlyName => SteamVR_Input_ActionFile.GetCodeFriendlyName(this.name);

    [JsonIgnore]
    public string shortName => this.name.LastIndexOf('/') == this.name.Length - 1 ? string.Empty : SteamVR_Input_ActionFile.GetShortName(this.name);

    public void SetNewShortName(string newShortName) => this.name = SteamVR_Input_ActionFile_ActionSet.GetPathFromName(newShortName);

    public static string CreateNewName() => SteamVR_Input_ActionFile_ActionSet.GetPathFromName("NewSet");

    public static string GetPathFromName(string name) => string.Format("/actions/{0}", (object) name);

    public static SteamVR_Input_ActionFile_ActionSet CreateNew() => new SteamVR_Input_ActionFile_ActionSet()
    {
      name = SteamVR_Input_ActionFile_ActionSet.CreateNewName()
    };

    public SteamVR_Input_ActionFile_ActionSet GetCopy() => new SteamVR_Input_ActionFile_ActionSet()
    {
      name = this.name,
      usage = this.usage
    };

    public override bool Equals(object obj)
    {
      if (!(obj is SteamVR_Input_ActionFile_ActionSet))
        return base.Equals(obj);
      SteamVR_Input_ActionFile_ActionSet actionFileActionSet = (SteamVR_Input_ActionFile_ActionSet) obj;
      return actionFileActionSet == this || actionFileActionSet.name == this.name;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
