// Decompiled with JetBrains decompiler
// Type: FistVR.PMat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PMat : MonoBehaviour
  {
    public PMaterialDefinition Def;
    public MatDef MatDef;
    private float m_condition = 1f;

    public float Condition
    {
      get => this.m_condition;
      set => this.m_condition = Mathf.Clamp(value, 0.0f, 1f);
    }

    public float GetYieldStrength() => Mathf.Lerp(1f / 1000f, this.Def.yieldStrength, this.Condition);

    public float GetRoughness() => Mathf.Lerp(1f / 1000f, this.Def.roughness, this.Condition);

    public float GetStiffness() => Mathf.Lerp(this.Def.stiffness * 0.2f, this.Def.stiffness, this.Condition);

    public float GetDensity() => Mathf.Lerp(this.Def.density * 0.2f, this.Def.density, this.Condition);

    public float GetBounciness() => Mathf.Lerp(this.Def.bounciness * 0.3f, this.Def.bounciness, this.Condition);

    public float GetToughness() => Mathf.Lerp(1f / 1000f, this.Def.toughness, this.Condition);
  }
}
