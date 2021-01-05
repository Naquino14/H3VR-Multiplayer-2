// Decompiled with JetBrains decompiler
// Type: FistVR.DropTrap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DropTrap : ZosigQuestManager
  {
    private ZosigGameManager M;
    public string Flag;
    public int ValueWhenOn;
    private bool m_isEnabled;
    public Transform Cable;
    public Rigidbody Logs;
    public Transform LogTarget;
    private bool m_isGassed;

    public override void Init(ZosigGameManager m) => this.M = m;

    private void Start()
    {
    }

    public void TurnOn() => this.m_isGassed = true;

    public void TurnOff() => this.m_isGassed = false;

    private void Update()
    {
      if (this.m_isEnabled && this.m_isGassed)
      {
        float y1 = this.Logs.transform.position.y;
        float y2 = Mathf.MoveTowards(y1, this.LogTarget.position.y, Time.deltaTime * 0.25f);
        if ((double) Mathf.Abs(y1 - y2) > 1.0 / 1000.0)
          this.Logs.MovePosition(new Vector3(this.Logs.transform.position.x, y2, this.Logs.transform.position.z));
      }
      this.Cable.localScale = new Vector3(0.03f, 0.03f, Mathf.Max((this.Cable.transform.position - this.Logs.transform.position).magnitude, 0.01f));
    }

    public void ON()
    {
      if (!this.m_isEnabled && (Object) GM.ZMaster != (Object) null)
        GM.ZMaster.FlagM.AddToFlag("s_t", 1);
      this.M.FlagM.SetFlag(this.Flag, this.ValueWhenOn);
      this.m_isEnabled = true;
      this.Logs.isKinematic = true;
    }

    public void OFF()
    {
      this.m_isEnabled = false;
      this.Logs.isKinematic = false;
      this.Logs.velocity = Vector3.up * -9.81f;
    }
  }
}
