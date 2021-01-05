// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwCharcoalGrillChart
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RotrwCharcoalGrillChart : MonoBehaviour
  {
    public List<GameObject> HerbRecipeLines;
    public List<GameObject> HerbRecipeLines_Inverted;
    public List<GameObject> CoreNameLabels;
    public List<GameObject> CoreIntensityLines;
    public List<GameObject> CoreDurationLines;
    public List<GameObject> CoreSpecialLines;
    public RotrwMeatCore rw;

    public void RevealChartElements(RW_Powerup p)
    {
      if (!p.isInverted)
        this.HerbRecipeLines[(int) p.PowerupType].SetActive(true);
      else
        this.HerbRecipeLines_Inverted[(int) p.PowerupType].SetActive(true);
      RotrwMeatCore.CoreType mcMadeWith = p.GetMCMadeWith();
      this.CoreSpecialLines[(int) mcMadeWith].SetActive(true);
      this.CoreNameLabels[(int) mcMadeWith].SetActive(true);
      switch (p.PowerupType)
      {
        case PowerupType.Health:
          this.CoreIntensityLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.QuadDamage:
          this.CoreIntensityLines[(int) mcMadeWith].SetActive(true);
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.InfiniteAmmo:
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.Invincibility:
          this.CoreIntensityLines[(int) mcMadeWith].SetActive(true);
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.Ghosted:
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.FarOutMeat:
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.MuscleMeat:
          this.CoreIntensityLines[(int) mcMadeWith].SetActive(true);
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.SnakeEye:
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.Blort:
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.Regen:
          this.CoreIntensityLines[(int) mcMadeWith].SetActive(true);
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
        case PowerupType.Cyclops:
          this.CoreDurationLines[(int) mcMadeWith].SetActive(true);
          break;
      }
    }
  }
}
