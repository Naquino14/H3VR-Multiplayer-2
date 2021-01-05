// Decompiled with JetBrains decompiler
// Type: FistVR.PumpLottoRoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PumpLottoRoom : MonoBehaviour, IRoomTriggerable
  {
    public RedRoom m_room;
    private bool m_isTriggered;
    public MR_PumpLottoBase[] Bases;

    public void Init(int size, RedRoom room)
    {
      this.m_room = room;
      if (!this.m_isTriggered)
        this.m_isTriggered = true;
      this.m_room.CloseDoors(true);
      for (int index = 0; index < this.Bases.Length; ++index)
        this.Bases[index].gameObject.SetActive(true);
    }

    public void SetRoom(RedRoom room)
    {
      this.m_room = room;
      this.SetUpPumps();
    }

    public void SetUpPumps()
    {
      this.ShuffleBases();
      this.Bases[0].SetPLType(MR_PumpLottoBase.PumpLottoType.OpenDoor);
      float num1 = Random.Range(0.0f, 1f);
      if ((double) num1 > 0.949999988079071)
        this.Bases[1].SetPLType(MR_PumpLottoBase.PumpLottoType.WeinerBot);
      else if ((double) num1 > 0.5)
        this.Bases[1].SetPLType(MR_PumpLottoBase.PumpLottoType.Loot);
      else
        this.Bases[1].SetPLType(MR_PumpLottoBase.PumpLottoType.CloseDoor);
      float num2 = Random.Range(0.0f, 1f);
      if ((double) num2 > 0.800000011920929)
        this.Bases[2].SetPLType(MR_PumpLottoBase.PumpLottoType.WeinerBot);
      else if ((double) num2 > 0.300000011920929)
        this.Bases[2].SetPLType(MR_PumpLottoBase.PumpLottoType.Loot);
      else
        this.Bases[2].SetPLType(MR_PumpLottoBase.PumpLottoType.Goof);
      float num3 = Random.Range(0.0f, 1f);
      if ((double) num3 > 0.800000011920929)
        this.Bases[3].SetPLType(MR_PumpLottoBase.PumpLottoType.Slicer);
      else if ((double) num3 > 0.300000011920929)
        this.Bases[3].SetPLType(MR_PumpLottoBase.PumpLottoType.Loot);
      else
        this.Bases[3].SetPLType(MR_PumpLottoBase.PumpLottoType.Goof);
      for (int index = 0; index < this.Bases.Length; ++index)
        this.Bases[index].gameObject.SetActive(false);
    }

    private void ShuffleBases()
    {
      for (int min = 0; min < this.Bases.Length; ++min)
      {
        MR_PumpLottoBase mrPumpLottoBase = this.Bases[min];
        int index = Random.Range(min, this.Bases.Length);
        this.Bases[min] = this.Bases[index];
        this.Bases[index] = mrPumpLottoBase;
      }
    }
  }
}
