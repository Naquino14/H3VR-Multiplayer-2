// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_ActivateActionSetOnLoad
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_ActivateActionSetOnLoad : MonoBehaviour
  {
    public SteamVR_ActionSet actionSet = SteamVR_Input.GetActionSet("default");
    public SteamVR_Input_Sources forSources;
    public bool disableAllOtherActionSets;
    public bool activateOnStart = true;
    public bool deactivateOnDestroy = true;

    private void Start()
    {
      if (!(this.actionSet != (SteamVR_ActionSet) null) || !this.activateOnStart)
        return;
      this.actionSet.Activate(this.forSources, 0, this.disableAllOtherActionSets);
    }

    private void OnDestroy()
    {
      if (!(this.actionSet != (SteamVR_ActionSet) null) || !this.deactivateOnDestroy)
        return;
      this.actionSet.Deactivate(this.forSources);
    }
  }
}
