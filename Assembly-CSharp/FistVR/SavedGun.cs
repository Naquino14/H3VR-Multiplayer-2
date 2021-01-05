// Decompiled with JetBrains decompiler
// Type: FistVR.SavedGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class SavedGun
  {
    public string FileName;
    public List<SavedGunComponent> Components = new List<SavedGunComponent>();
    public List<FireArmRoundClass> LoadedRoundsInMag = new List<FireArmRoundClass>();
    public List<FireArmRoundClass> LoadedRoundsInChambers = new List<FireArmRoundClass>();
    public List<string> SavedFlags = new List<string>();
    public DateTime DateMade = new DateTime();
  }
}
