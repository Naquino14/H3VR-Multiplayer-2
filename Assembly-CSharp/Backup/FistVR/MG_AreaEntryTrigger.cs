// Decompiled with JetBrains decompiler
// Type: FistVR.MG_AreaEntryTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_AreaEntryTrigger : MonoBehaviour
  {
    public string Area;
    public RedRoom.Quadrant Quadrant;

    private void OnTriggerEnter()
    {
      if (this.Quadrant == GM.MGMaster.PlayerQuadrant)
        return;
      GM.MGMaster.PlayerQuadrant = this.Quadrant;
      switch (this.Area)
      {
        case "boiler":
          GM.MGMaster.Narrator.PlayAreaEntryBoiler();
          break;
        case "office":
          GM.MGMaster.Narrator.PlayAreaEntryOffice();
          break;
        case "restaurant":
          GM.MGMaster.Narrator.PlayAreaEntryRestaurant();
          break;
        case "coldstorage":
          GM.MGMaster.Narrator.PlayAreaEntryColdStorage();
          break;
      }
      Object.Destroy((Object) this.gameObject);
    }
  }
}
