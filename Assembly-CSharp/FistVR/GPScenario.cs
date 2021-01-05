// Decompiled with JetBrains decompiler
// Type: FistVR.GPScenario
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class GPScenario
  {
    public string levelname;
    public int versionNumber;
    public string description;
    public SerializableStringDictionary SceneFlags;
    public List<GPSavedPlaceable> SavedPlaceables;
  }
}
