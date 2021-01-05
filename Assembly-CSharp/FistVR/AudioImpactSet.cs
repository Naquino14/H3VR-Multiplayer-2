// Decompiled with JetBrains decompiler
// Type: FistVR.AudioImpactSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New AudioImpactSet", menuName = "AudioPooling/AudioImpactSet", order = 0)]
  public class AudioImpactSet : ScriptableObject
  {
    public ImpactType ImpactType;
    public Vector2 PitchRange = new Vector2(1f, 1.03f);
    public AudioImpactMaterialGroup Carpet;
    public AudioImpactMaterialGroup HardSurface;
    public AudioImpactMaterialGroup LooseSurface;
    public AudioImpactMaterialGroup Meat;
    public AudioImpactMaterialGroup Metal;
    public AudioImpactMaterialGroup Plastic;
    public AudioImpactMaterialGroup SoftSurface;
    public AudioImpactMaterialGroup Tile;
    public AudioImpactMaterialGroup Water;
    public AudioImpactMaterialGroup Wood;
  }
}
