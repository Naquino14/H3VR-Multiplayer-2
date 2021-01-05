// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PropertySheet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PropertySheet
  {
    internal PropertySheet(Material material)
    {
      this.material = material;
      this.properties = new MaterialPropertyBlock();
    }

    public MaterialPropertyBlock properties { get; private set; }

    internal Material material { get; private set; }

    public void ClearKeywords() => this.material.shaderKeywords = (string[]) null;

    public void EnableKeyword(string keyword) => this.material.EnableKeyword(keyword);

    public void DisableKeyword(string keyword) => this.material.DisableKeyword(keyword);

    internal void Release()
    {
      RuntimeUtilities.Destroy((Object) this.material);
      this.material = (Material) null;
    }
  }
}
