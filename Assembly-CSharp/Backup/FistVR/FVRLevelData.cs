// Decompiled with JetBrains decompiler
// Type: FistVR.FVRLevelData
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class FVRLevelData
  {
    public string UniqueID = "unassigned";
    public string LevelName = "unnamed";
    public string SceneName = "unassigned";
    public string SaveSystemVersion = "v0.1";
    public string Description = "new user level";
    public List<FVREntityProxyData> ProxyDataList = new List<FVREntityProxyData>();

    public FVRLevelData(string levelName, string sceneName)
    {
      this.UniqueID = Guid.NewGuid().ToString();
      this.LevelName = levelName;
      this.SceneName = sceneName;
    }

    private bool IsUniqueIDValid() => this.UniqueID != "unassigned";

    private bool IsLevelNameValid() => this.LevelName != string.Empty && this.LevelName != "unnamed";

    private bool IsSceneNameValid() => this.SceneName != string.Empty && this.SceneName != "unassigned";

    public static FVRLevelData Parse(string s) => JsonUtility.FromJson<FVRLevelData>(s);

    public override string ToString() => JsonUtility.ToJson((object) this);
  }
}
