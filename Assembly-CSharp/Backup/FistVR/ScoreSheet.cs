// Decompiled with JetBrains decompiler
// Type: FistVR.ScoreSheet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ScoreSheet : FVRInteractiveObject
  {
    public PaperTarget PaperTarget;
    public Text ScoreTabulation;

    public override void Poke(FVRViveHand hand)
    {
      base.Poke(hand);
      if (this.PaperTarget.CurrentShots.Count <= 0)
        return;
      Vector3 vector3 = this.PaperTarget.ClearHolesAndReportScore();
      this.ScoreTabulation.text += "\n" + (object) (int) vector3.x + " Shots - " + (object) (int) ((double) vector3.z * 3.28083992004395) + " Feet - " + (object) (int) vector3.y + " Points";
    }
  }
}
