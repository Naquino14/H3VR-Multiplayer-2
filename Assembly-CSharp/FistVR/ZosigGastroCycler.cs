// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigGastroCycler
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigGastroCycler : ZosigQuestManager
  {
    [Header("Health")]
    public Renderer HealthRingRend;
    public Color Health_Full;
    public Color Health_Empty;
    public List<ZosigGastroCyclerPointZone> PointingZones;
    public ZosigGastroCycler.GastroState State;
    [Header("Groups")]
    public Transform Root_Menu;
    public Transform Root_Cores;
    public Transform Root_Fruit;
    public Transform Root_Banger;
    public List<Transform> Group_Menu;
    public List<Transform> Group_Cores;
    public List<Transform> Group_Fruit;
    public List<Transform> Group_Banger;
    private ZosigGameManager M;
    private int m_curPointIndex = -1;

    public override void Init(ZosigGameManager m)
    {
      this.M = m;
      this.SetState(ZosigGastroCycler.GastroState.Disabled);
      this.M.SetGCycler(this);
    }

    public void InitiatePoint(int i)
    {
    }

    public void EndPoint(int i)
    {
    }

    public void UpdateState() => this.SetState(this.State);

    public void SetState(ZosigGastroCycler.GastroState s)
    {
      this.State = s;
      switch (this.State)
      {
        case ZosigGastroCycler.GastroState.Disabled:
          this.SetState_Disabled();
          break;
        case ZosigGastroCycler.GastroState.Menu:
          this.SetState_Menu();
          break;
        case ZosigGastroCycler.GastroState.Cores:
          this.SetState_Cores();
          break;
        case ZosigGastroCycler.GastroState.Fruit:
          this.SetState_Fruit();
          break;
        case ZosigGastroCycler.GastroState.Banger:
          this.SetState_Banger();
          break;
      }
    }

    public void SetState_Disabled()
    {
      this.PointingZones[0].IsEnabled = true;
      for (int index = 1; index < this.PointingZones.Count; ++index)
        this.PointingZones[index].IsEnabled = false;
      this.Root_Menu.gameObject.SetActive(false);
      this.Root_Cores.gameObject.SetActive(false);
      this.Root_Fruit.gameObject.SetActive(false);
      this.Root_Banger.gameObject.SetActive(false);
    }

    public void SetState_Menu()
    {
      this.Root_Menu.gameObject.SetActive(true);
      this.Root_Cores.gameObject.SetActive(false);
      this.Root_Fruit.gameObject.SetActive(false);
      this.Root_Banger.gameObject.SetActive(false);
      for (int index = 0; index < 4; ++index)
        this.PointingZones[index].IsEnabled = true;
      for (int index = 4; index < this.PointingZones.Count; ++index)
        this.PointingZones[index].IsEnabled = false;
    }

    public void SetState_Cores()
    {
      this.Root_Menu.gameObject.SetActive(false);
      this.Root_Cores.gameObject.SetActive(true);
      this.Root_Fruit.gameObject.SetActive(false);
      this.Root_Banger.gameObject.SetActive(false);
      this.PointingZones[0].IsEnabled = true;
      this.PointingZones[1].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreA") > 0;
      this.Group_Cores[1].gameObject.SetActive(this.PointingZones[1].IsEnabled);
      this.PointingZones[2].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreB") > 0;
      this.Group_Cores[2].gameObject.SetActive(this.PointingZones[2].IsEnabled);
      this.PointingZones[3].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreC") > 0;
      this.Group_Cores[3].gameObject.SetActive(this.PointingZones[3].IsEnabled);
      this.PointingZones[4].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreD") > 0;
      this.Group_Cores[4].gameObject.SetActive(this.PointingZones[4].IsEnabled);
      this.PointingZones[5].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreE") > 0;
      this.Group_Cores[5].gameObject.SetActive(this.PointingZones[5].IsEnabled);
      this.PointingZones[6].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreF") > 0;
      this.Group_Cores[6].gameObject.SetActive(this.PointingZones[6].IsEnabled);
      this.PointingZones[7].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreG") > 0;
      this.Group_Cores[7].gameObject.SetActive(this.PointingZones[7].IsEnabled);
      this.PointingZones[8].IsEnabled = this.M.FlagM.GetFlagValue("num_meatcoreH") > 0;
      this.Group_Cores[8].gameObject.SetActive(this.PointingZones[8].IsEnabled);
    }

    public void SetState_Fruit()
    {
      this.Root_Menu.gameObject.SetActive(false);
      this.Root_Cores.gameObject.SetActive(false);
      this.Root_Fruit.gameObject.SetActive(true);
      this.Root_Banger.gameObject.SetActive(false);
      this.PointingZones[0].IsEnabled = true;
      this.PointingZones[1].IsEnabled = this.M.FlagM.GetFlagValue("num_herbA") > 0;
      this.Group_Fruit[1].gameObject.SetActive(this.PointingZones[1].IsEnabled);
      this.PointingZones[2].IsEnabled = this.M.FlagM.GetFlagValue("num_herbB") > 0;
      this.Group_Fruit[2].gameObject.SetActive(this.PointingZones[2].IsEnabled);
      this.PointingZones[3].IsEnabled = this.M.FlagM.GetFlagValue("num_herbC") > 0;
      this.Group_Fruit[3].gameObject.SetActive(this.PointingZones[3].IsEnabled);
      this.PointingZones[4].IsEnabled = this.M.FlagM.GetFlagValue("num_herbD") > 0;
      this.Group_Fruit[4].gameObject.SetActive(this.PointingZones[4].IsEnabled);
      this.PointingZones[5].IsEnabled = this.M.FlagM.GetFlagValue("num_herbE") > 0;
      this.Group_Fruit[5].gameObject.SetActive(this.PointingZones[5].IsEnabled);
      for (int index = 6; index < this.PointingZones.Count; ++index)
        this.PointingZones[index].IsEnabled = false;
    }

    public void SetState_Banger()
    {
      this.Root_Menu.gameObject.SetActive(false);
      this.Root_Cores.gameObject.SetActive(false);
      this.Root_Fruit.gameObject.SetActive(false);
      this.Root_Banger.gameObject.SetActive(true);
      this.PointingZones[0].IsEnabled = true;
      this.PointingZones[1].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_TinCan_0") > 0 || this.M.FlagM.GetFlagValue("num_bangerJunk_TinCan_1") > 0 || this.M.FlagM.GetFlagValue("num_bangerJunk_TinCan_2") > 0;
      this.Group_Banger[1].gameObject.SetActive(this.PointingZones[1].IsEnabled);
      this.PointingZones[2].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_0") > 0 || this.M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_1") > 0 || this.M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_2") > 0 || this.M.FlagM.GetFlagValue("num_bangerJunk_CoffeeCan_3") > 0;
      this.Group_Banger[2].gameObject.SetActive(this.PointingZones[2].IsEnabled);
      this.PointingZones[3].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_Bucket") > 0;
      this.Group_Banger[3].gameObject.SetActive(this.PointingZones[3].IsEnabled);
      this.PointingZones[4].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_Bangsnaps") > 0;
      this.Group_Banger[4].gameObject.SetActive(this.PointingZones[4].IsEnabled);
      this.PointingZones[5].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_EggTimer") > 0;
      this.Group_Banger[5].gameObject.SetActive(this.PointingZones[5].IsEnabled);
      this.PointingZones[6].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_Radio") > 0;
      this.Group_Banger[6].gameObject.SetActive(this.PointingZones[6].IsEnabled);
      this.PointingZones[7].IsEnabled = this.M.FlagM.GetFlagValue("num_bangerJunk_FishFinder") > 0;
      this.Group_Banger[7].gameObject.SetActive(this.PointingZones[7].IsEnabled);
      for (int index = 8; index < this.PointingZones.Count; ++index)
        this.PointingZones[index].IsEnabled = false;
    }

    public void ClickPoint(int i)
    {
      switch (this.State)
      {
        case ZosigGastroCycler.GastroState.Disabled:
          this.ClickPoint_Disabled(i);
          break;
        case ZosigGastroCycler.GastroState.Menu:
          this.ClickPoint_Menu(i);
          break;
        case ZosigGastroCycler.GastroState.Cores:
          this.ClickPoint_Cores(i);
          break;
        case ZosigGastroCycler.GastroState.Fruit:
          this.ClickPoint_Fruit(i);
          break;
        case ZosigGastroCycler.GastroState.Banger:
          this.ClickPoint_Banger(i);
          break;
      }
    }

    private void ClickPoint_Disabled(int i)
    {
      if (i != 0)
        return;
      this.SetState(ZosigGastroCycler.GastroState.Menu);
    }

    private void ClickPoint_Menu(int i)
    {
      switch (i)
      {
        case 0:
          this.SetState(ZosigGastroCycler.GastroState.Disabled);
          break;
        case 1:
          this.SetState(ZosigGastroCycler.GastroState.Cores);
          break;
        case 2:
          this.SetState(ZosigGastroCycler.GastroState.Fruit);
          break;
        case 3:
          this.SetState(ZosigGastroCycler.GastroState.Banger);
          break;
      }
    }

    private void ClickPoint_Cores(int i)
    {
      if (i == 0)
      {
        this.SetState(ZosigGastroCycler.GastroState.Menu);
      }
      else
      {
        this.M.VomitCore(i - 1);
        this.SetState_Cores();
      }
    }

    private void ClickPoint_Fruit(int i)
    {
      if (i == 0)
      {
        this.SetState(ZosigGastroCycler.GastroState.Menu);
      }
      else
      {
        this.M.VomitHerb(i - 1);
        this.SetState_Fruit();
      }
    }

    private void ClickPoint_Banger(int i)
    {
      if (i == 0)
      {
        this.SetState(ZosigGastroCycler.GastroState.Menu);
      }
      else
      {
        this.M.VomitBangerJunk(i - 1);
        this.SetState_Banger();
      }
    }

    private void Update()
    {
      this.UpdateHealth();
      this.transform.position = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
    }

    private void UpdateHealth()
    {
      float playerHealth = GM.GetPlayerHealth();
      Color color = Color.Lerp(this.Health_Empty, this.Health_Full, playerHealth);
      this.HealthRingRend.material.SetTextureOffset("_MainTex", new Vector2(0.0f, -playerHealth + 0.5f));
      this.HealthRingRend.material.SetColor("_EmissionColor", color);
      this.HealthRingRend.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(GM.CurrentPlayerBody.Head.forward, this.transform.up), this.transform.up);
    }

    public enum GastroState
    {
      Disabled,
      Menu,
      Cores,
      Fruit,
      Banger,
    }
  }
}
