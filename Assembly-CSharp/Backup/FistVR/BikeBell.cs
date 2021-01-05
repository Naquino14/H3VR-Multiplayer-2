// Decompiled with JetBrains decompiler
// Type: FistVR.BikeBell
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BikeBell : FVRInteractiveObject
  {
    private BikeBell.BellState Bstate;
    private float bLerp;
    public Transform Piece;
    public Vector2 PieceRange;
    public AudioEvent AudEvent_Ring;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      if (this.Bstate != BikeBell.BellState.Unrung)
        return;
      this.Bstate = BikeBell.BellState.Ringing;
      this.bLerp = 0.0f;
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Ring, this.transform.position);
      GM.CurrentSceneSettings.OnPerceiveableSound(40f, 12f, this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      switch (this.Bstate)
      {
        case BikeBell.BellState.Ringing:
          this.bLerp += Time.deltaTime * 6f;
          if ((double) this.bLerp >= 1.0)
          {
            this.bLerp = 1f;
            this.Bstate = BikeBell.BellState.Unringing;
          }
          this.Piece.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(this.PieceRange.x, this.PieceRange.y, this.bLerp), 0.0f);
          break;
        case BikeBell.BellState.Unringing:
          this.bLerp -= Time.deltaTime * 6f;
          if ((double) this.bLerp <= 0.0)
          {
            this.bLerp = 0.0f;
            this.Bstate = BikeBell.BellState.Unrung;
          }
          this.Piece.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(this.PieceRange.x, this.PieceRange.y, this.bLerp), 0.0f);
          break;
      }
    }

    public enum BellState
    {
      Unrung,
      Ringing,
      Unringing,
    }
  }
}
