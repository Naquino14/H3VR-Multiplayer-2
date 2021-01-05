// Decompiled with JetBrains decompiler
// Type: FistVR.PMaterialDefinition
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu]
  public class PMaterialDefinition : ScriptableObject
  {
    public PMaterial material;
    public float yieldStrength;
    public float roughness;
    public float stiffness;
    public float density;
    public float bounciness;
    public float toughness;
    public PMatSoundCategory soundCategory;
    public PMatImpactEffectCategory impactCategory;
  }
}
