// Decompiled with JetBrains decompiler
// Type: FistVR.LemonDoor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class LemonDoor : ZosigQuestManager
  {
    private ZosigGameManager M;
    public string FlagToOpen;
    private bool isUp = true;
    public AudioEvent AudEvent_DoorShut;

    public override void Init(ZosigGameManager m)
    {
      this.M = m;
      if (this.M.FlagM.GetFlagValue(this.FlagToOpen) <= 0)
        return;
      this.gameObject.SetActive(false);
      this.isUp = false;
    }

    private void Update()
    {
      if (!this.isUp || this.M.FlagM.GetFlagValue(this.FlagToOpen) <= 0)
        return;
      this.isUp = false;
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_DoorShut, this.transform.position);
      this.gameObject.SetActive(false);
    }
  }
}
