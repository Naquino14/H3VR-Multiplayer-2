// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_Pose_Source_Map`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Valve.VR
{
  public class SteamVR_Action_Pose_Source_Map<Source> : SteamVR_Action_In_Source_Map<Source>
    where Source : SteamVR_Action_Pose_Source, new()
  {
    public void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newOrigin)
    {
      Dictionary<SteamVR_Input_Sources, Source>.Enumerator enumerator = this.sources.GetEnumerator();
      while (enumerator.MoveNext())
        enumerator.Current.Value.universeOrigin = newOrigin;
    }

    public virtual void UpdateValues(bool skipStateAndEventUpdates)
    {
      for (int index = 0; index < this.updatingSources.Count; ++index)
        this.sources[this.updatingSources[index]].UpdateValue(skipStateAndEventUpdates);
    }
  }
}
