// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.TeleportMarkerBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public abstract class TeleportMarkerBase : MonoBehaviour
  {
    public bool locked;
    public bool markerActive = true;

    public virtual bool showReticle => true;

    public void SetLocked(bool locked)
    {
      this.locked = locked;
      this.UpdateVisuals();
    }

    public virtual void TeleportPlayer(Vector3 pointedAtPosition)
    {
    }

    public abstract void UpdateVisuals();

    public abstract void Highlight(bool highlight);

    public abstract void SetAlpha(float tintAlpha, float alphaPercent);

    public abstract bool ShouldActivate(Vector3 playerPosition);

    public abstract bool ShouldMovePlayer();
  }
}
