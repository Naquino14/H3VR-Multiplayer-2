// Decompiled with JetBrains decompiler
// Type: FistVR.TR_SpikeCeilingBase
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TR_SpikeCeilingBase : MonoBehaviour, IMG_HandlePumpable
  {
    public TR_SpikeCeilingBaseHandle Handle;
    public Transform Jack;
    public float CurHeight = 0.8f;
    public float MinHeight = 0.8f;
    public float MaxHeight = 4.2f;
    public ParticleSystem Sparks;

    public void Pump(float delta)
    {
      this.CurHeight += delta * (1f / 500f);
      this.CurHeight = Mathf.Clamp(this.CurHeight, this.MinHeight, this.MaxHeight);
      this.Jack.localPosition = new Vector3(this.Jack.localPosition.x, this.CurHeight, this.Jack.localPosition.z);
    }

    public void LowerTo(float max)
    {
      if ((double) this.CurHeight <= (double) max)
        return;
      this.CurHeight = Mathf.Clamp(max, this.MinHeight, this.MaxHeight);
      this.Jack.localPosition = new Vector3(this.Jack.localPosition.x, this.CurHeight, this.Jack.localPosition.z);
      this.Sparks.Emit(1);
    }
  }
}
