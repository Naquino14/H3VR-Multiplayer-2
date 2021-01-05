// Decompiled with JetBrains decompiler
// Type: FistVR.HG_ModeManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_ModeManager : MonoBehaviour
  {
    public HG_GameManager M;
    protected bool IsPlaying;
    protected HG_ModeManager.HG_Mode m_mode;
    protected Vector3 InitialRespawnPos;
    protected Quaternion InitialRespawnRot;

    public virtual void InitMode(HG_ModeManager.HG_Mode mode)
    {
    }

    public virtual bool IsModeComplete() => true;

    public virtual void HandlePlayerDeath()
    {
    }

    public virtual void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
    {
      this.M.IsNoLongerPlaying();
      this.M.FadeOutMusic();
      if (doesInvokeTeleport)
        this.Invoke("TeleportPlayerBackAndRegisterScore", 5f);
      if (!immediateTeleportBackAndScore)
        return;
      this.M.EndModeScore();
    }

    private void TeleportPlayerBackAndRegisterScore()
    {
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, true, GM.CurrentSceneSettings.DeathResetPoint.forward);
      this.M.EndModeScore();
    }

    public virtual int GetScore() => 0;

    public virtual List<string> GetScoringReadOuts() => (List<string>) null;

    public virtual void TargetDestroyed(HG_Target t)
    {
    }

    public enum HG_Mode
    {
      None,
      TargetRelay_Sprint,
      TargetRelay_Jog,
      TargetRelay_Marathon,
      AssaultNPepper_Skirmish,
      AssaultNPepper_Brawl,
      AssaultNPepper_Maelstrom,
      MeatNMetal_Neophyte,
      MeatNMetal_Warrior,
      MeatNMetal_Veteran,
      BattlePetite_Open,
      BattlePetite_Sosiggun,
      BattlePetite_Melee,
      KingOfTheGrill_Invasion,
      KingOfTheGrill_Resurrection,
      KingOfTheGrill_Anachronism,
      MeatleGear_Open,
      MeatleGear_ScavengingSnake,
      MeatleGear_ThirdSnake,
    }
  }
}
