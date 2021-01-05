// Decompiled with JetBrains decompiler
// Type: FistVR.MF2Flags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class MF2Flags
  {
    public MF_TeamColor PlayerTeam;
    public int PlayerSetting_HealthIndex = 4;
    public float[] PlayerSetting_HealthSettings = new float[6]
    {
      1000f,
      2500f,
      5000f,
      10000f,
      50000f,
      100000f
    };
    public MF_BlastJumping PlayerSetting_BlastJumping;
    public MF_BlastJumpingSelfDamage PlayerSetting_BlastJumpingSelfDamage = MF_BlastJumpingSelfDamage.Arcade;
    public int[] TDM_TeamSizes = new int[6]
    {
      0,
      1,
      3,
      6,
      10,
      16
    };
    public MF_TeamColor TDMOption_PlayerTeam;
    public int TDMOption_RedTeamSizeIndex = 4;
    public int TDMOption_BlueTeamSizeIndex = 4;
    public MF_SpawnSpeed TDMOption_RedTeamSpeed = MF_SpawnSpeed.Standard;
    public MF_SpawnSpeed TDMOption_BlueTeamSpeed = MF_SpawnSpeed.Standard;
    public MF_PlayArea TDMOption_PlayArea;

    public void InitializeFromSaveFile()
    {
      if (ES2.Exists("MF.txt"))
      {
        Debug.Log((object) "MF.txt exists, initializing from it");
        using (ES2Reader es2Reader = ES2Reader.Create("MF.txt"))
        {
          if (es2Reader.TagExists("PlayerSetting_HealthIndex"))
            this.PlayerSetting_HealthIndex = es2Reader.Read<int>("PlayerSetting_HealthIndex");
          if (es2Reader.TagExists("PlayerSetting_BlastJumping"))
            this.PlayerSetting_BlastJumping = es2Reader.Read<MF_BlastJumping>("PlayerSetting_BlastJumping");
          if (es2Reader.TagExists("PlayerSetting_BlastJumpingSelfDamage"))
            this.PlayerSetting_BlastJumpingSelfDamage = es2Reader.Read<MF_BlastJumpingSelfDamage>("PlayerSetting_BlastJumpingSelfDamage");
          if (es2Reader.TagExists("TDMOption_PlayerTeam"))
            this.TDMOption_PlayerTeam = es2Reader.Read<MF_TeamColor>("TDMOption_PlayerTeam");
          if (es2Reader.TagExists("TDMOption_RedTeamSizeIndex"))
            this.TDMOption_RedTeamSizeIndex = es2Reader.Read<int>("TDMOption_RedTeamSizeIndex");
          if (es2Reader.TagExists("TDMOption_BlueTeamSizeIndex"))
            this.TDMOption_BlueTeamSizeIndex = es2Reader.Read<int>("TDMOption_BlueTeamSizeIndex");
          if (es2Reader.TagExists("TDMOption_RedTeamSpeed"))
            this.TDMOption_RedTeamSpeed = es2Reader.Read<MF_SpawnSpeed>("TDMOption_RedTeamSpeed");
          if (es2Reader.TagExists("TDMOption_BlueTeamSpeed"))
            this.TDMOption_BlueTeamSpeed = es2Reader.Read<MF_SpawnSpeed>("TDMOption_BlueTeamSpeed");
          if (!es2Reader.TagExists("TDMOption_PlayArea"))
            return;
          this.TDMOption_PlayArea = es2Reader.Read<MF_PlayArea>("TDMOption_PlayArea");
        }
      }
      else
      {
        Debug.Log((object) "MF.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer es2Writer = ES2Writer.Create("MF.txt"))
      {
        es2Writer.Write<int>(this.PlayerSetting_HealthIndex, "PlayerSetting_HealthIndex");
        es2Writer.Write<MF_BlastJumping>(this.PlayerSetting_BlastJumping, "PlayerSetting_BlastJumping");
        es2Writer.Write<MF_BlastJumpingSelfDamage>(this.PlayerSetting_BlastJumpingSelfDamage, "PlayerSetting_BlastJumpingSelfDamage");
        es2Writer.Write<MF_TeamColor>(this.TDMOption_PlayerTeam, "TDMOption_PlayerTeam");
        es2Writer.Write<int>(this.TDMOption_RedTeamSizeIndex, "TDMOption_RedTeamSizeIndex");
        es2Writer.Write<int>(this.TDMOption_BlueTeamSizeIndex, "TDMOption_BlueTeamSizeIndex");
        es2Writer.Write<MF_SpawnSpeed>(this.TDMOption_RedTeamSpeed, "TDMOption_RedTeamSpeed");
        es2Writer.Write<MF_SpawnSpeed>(this.TDMOption_BlueTeamSpeed, "TDMOption_BlueTeamSpeed");
        es2Writer.Write<MF_PlayArea>(this.TDMOption_PlayArea, "TDMOption_PlayArea");
        es2Writer.Save();
      }
    }
  }
}
