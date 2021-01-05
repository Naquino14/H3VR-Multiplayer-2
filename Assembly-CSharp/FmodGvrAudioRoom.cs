// Decompiled with JetBrains decompiler
// Type: FmodGvrAudioRoom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("GoogleVR/Audio/FmodGvrAudioRoom")]
public class FmodGvrAudioRoom : MonoBehaviour
{
  public FmodGvrAudioRoom.SurfaceMaterial leftWall = FmodGvrAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;
  public FmodGvrAudioRoom.SurfaceMaterial rightWall = FmodGvrAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;
  public FmodGvrAudioRoom.SurfaceMaterial floor = FmodGvrAudioRoom.SurfaceMaterial.ParquetOnConcrete;
  public FmodGvrAudioRoom.SurfaceMaterial ceiling = FmodGvrAudioRoom.SurfaceMaterial.PlasterRough;
  public FmodGvrAudioRoom.SurfaceMaterial backWall = FmodGvrAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;
  public FmodGvrAudioRoom.SurfaceMaterial frontWall = FmodGvrAudioRoom.SurfaceMaterial.ConcreteBlockCoarse;
  public float reflectivity = 1f;
  public float reverbGainDb;
  public float reverbBrightness;
  public float reverbTime = 1f;
  public Vector3 size = Vector3.one;

  private void OnEnable() => FmodGvrAudio.UpdateAudioRoom(this, FmodGvrAudio.IsListenerInsideRoom(this));

  private void OnDisable() => FmodGvrAudio.UpdateAudioRoom(this, false);

  private void Update() => FmodGvrAudio.UpdateAudioRoom(this, FmodGvrAudio.IsListenerInsideRoom(this));

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.DrawWireCube(Vector3.zero, this.size);
  }

  public enum SurfaceMaterial
  {
    Transparent,
    AcousticCeilingTiles,
    BrickBare,
    BrickPainted,
    ConcreteBlockCoarse,
    ConcreteBlockPainted,
    CurtainHeavy,
    FiberglassInsulation,
    GlassThin,
    GlassThick,
    Grass,
    LinoleumOnConcrete,
    Marble,
    Metal,
    ParquetOnConcrete,
    PlasterRough,
    PlasterSmooth,
    PlywoodPanel,
    PolishedConcreteOrTile,
    Sheetrock,
    WaterOrIceSurface,
    WoodCeiling,
    WoodPanel,
  }
}
