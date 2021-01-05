// Decompiled with JetBrains decompiler
// Type: FistVR.ShieldBars
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ShieldBars : MonoBehaviour
  {
    public Transform FollowTarg;
    public Transform ShieldBar;
    public Cubegame Game;
    public Renderer Rend;

    private void Update()
    {
      this.ShieldBar.transform.localScale = new Vector3((float) ((double) this.Game.Health * 0.00999999977648258 + 0.00999999977648258), 1f, 1f);
      this.transform.position = this.FollowTarg.position;
      this.transform.rotation = this.FollowTarg.rotation;
      float g = this.Game.Health * 0.01f;
      this.Rend.material.SetColor("_TintColor", new Color(1f - g, g, 0.0f, 1f));
    }
  }
}
