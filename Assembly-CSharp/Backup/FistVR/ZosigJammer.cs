// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigJammer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class ZosigJammer : ZosigQuestManager
  {
    public Text Label;
    public List<ZosigJammerBox> Boxes;
    public List<GameObject> Jamming;
    private float m_checkTick = 0.1f;
    private ZosigGameManager M;
    public string Flag;
    public int ValueWhenDestroyed;
    private bool m_isDisabled;

    public override void Init(ZosigGameManager m)
    {
      this.M = m;
      if (this.M.FlagM.GetFlagValue(this.Flag) <= 0)
        return;
      for (int index = 0; index < this.Jamming.Count; ++index)
        this.Jamming[index].SetActive(false);
      this.m_isDisabled = true;
      for (int index = 0; index < this.Boxes.Count; ++index)
        this.Boxes[index].SetDestroyed();
    }

    private void Awake()
    {
    }

    private void Update()
    {
      this.m_checkTick -= Time.deltaTime;
      if ((double) this.m_checkTick > 0.0)
        return;
      this.m_checkTick = 1f;
      this.Check();
    }

    private void Check()
    {
      int num = 0;
      for (int index = 0; index < this.Boxes.Count; ++index)
      {
        if (this.Boxes[index].BState == ZosigJammerBox.JammerBoxState.Functioning)
          ++num;
      }
      this.Label.text = num != 0 ? num.ToString() + "JAMMERS ONLINE" : "JAMMER OFFLINE";
      if (num != 0 || this.m_isDisabled)
        return;
      this.m_isDisabled = true;
      this.ShutDown();
    }

    private void ShutDown()
    {
      this.M.FlagM.SetFlag(this.Flag, this.ValueWhenDestroyed);
      for (int index = 0; index < this.Jamming.Count; ++index)
        this.Jamming[index].SetActive(false);
    }
  }
}
