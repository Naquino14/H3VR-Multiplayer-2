// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockFlintScrew
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockFlintScrew : FVRInteractiveObject
  {
    public FlintlockWeapon Weapon;
    public Transform Screw;
    public FlintlockFlintScrew.ScrewState SState;
    private float lerp;
    public Vector2 Heights = new Vector2(0.05455612f, 0.06048f);
    public AudioEvent AudEvent_Screw;
    public AudioEvent AudEvent_Unscrew;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      this.ToggleScrewState();
    }

    public override bool IsInteractable() => this.SState != FlintlockFlintScrew.ScrewState.Screwing && this.SState != FlintlockFlintScrew.ScrewState.Unscrewing && base.IsInteractable();

    private void ToggleScrewState()
    {
      if (this.SState == FlintlockFlintScrew.ScrewState.Screwed)
      {
        this.Weapon.PlayAudioAsHandling(this.AudEvent_Unscrew, this.transform.position);
        this.SState = FlintlockFlintScrew.ScrewState.Unscrewing;
        this.lerp = 0.0f;
      }
      else
      {
        if (this.SState != FlintlockFlintScrew.ScrewState.Unscrewed)
          return;
        this.Weapon.PlayAudioAsHandling(this.AudEvent_Screw, this.transform.position);
        this.SState = FlintlockFlintScrew.ScrewState.Screwing;
        this.lerp = 0.0f;
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.SState == FlintlockFlintScrew.ScrewState.Screwing)
      {
        this.lerp += Time.deltaTime * 1.4f;
        this.Screw.localPosition = new Vector3(this.Screw.localPosition.x, Mathf.Lerp(this.Heights.y, this.Heights.x, this.lerp), this.Screw.localPosition.z);
        this.Screw.localEulerAngles = new Vector3(90f, Mathf.Lerp(0.0f, 720f, this.lerp), 0.0f);
        if ((double) this.lerp < 1.0)
          return;
        this.SState = FlintlockFlintScrew.ScrewState.Screwed;
      }
      else
      {
        if (this.SState != FlintlockFlintScrew.ScrewState.Unscrewing)
          return;
        this.lerp += Time.deltaTime * 1.4f;
        this.Screw.localPosition = new Vector3(this.Screw.localPosition.x, Mathf.Lerp(this.Heights.x, this.Heights.y, this.lerp), this.Screw.localPosition.z);
        this.Screw.localEulerAngles = new Vector3(90f, Mathf.Lerp(720f, 0.0f, this.lerp), 0.0f);
        if ((double) this.lerp < 1.0)
          return;
        this.SState = FlintlockFlintScrew.ScrewState.Unscrewed;
      }
    }

    public enum ScrewState
    {
      Screwed,
      Screwing,
      Unscrewed,
      Unscrewing,
    }
  }
}
