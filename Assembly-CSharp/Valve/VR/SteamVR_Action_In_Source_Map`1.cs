// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Action_In_Source_Map`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Valve.VR
{
  public class SteamVR_Action_In_Source_Map<SourceElement> : SteamVR_Action_Source_Map<SourceElement>
    where SourceElement : SteamVR_Action_In_Source, new()
  {
    protected List<SteamVR_Input_Sources> updatingSources = new List<SteamVR_Input_Sources>();

    public bool IsUpdating(SteamVR_Input_Sources inputSource)
    {
      for (int index = 0; index < this.updatingSources.Count; ++index)
      {
        if (inputSource == this.updatingSources[index])
          return true;
      }
      return false;
    }

    protected override void OnAccessSource(SteamVR_Input_Sources inputSource)
    {
      if (!SteamVR_Action.startUpdatingSourceOnAccess)
        return;
      this.ForceAddSourceToUpdateList(inputSource);
    }

    public void ForceAddSourceToUpdateList(SteamVR_Input_Sources inputSource)
    {
      if (this.sources[inputSource].isUpdating)
        return;
      this.updatingSources.Add(inputSource);
      this.sources[inputSource].isUpdating = true;
      if (SteamVR_Input.isStartupFrame)
        return;
      this.sources[inputSource].UpdateValue();
    }

    public void UpdateValues()
    {
      for (int index = 0; index < this.updatingSources.Count; ++index)
        this.sources[this.updatingSources[index]].UpdateValue();
    }
  }
}
