// Decompiled with JetBrains decompiler
// Type: ftLightmapsStorage
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ftLightmapsStorage : MonoBehaviour
{
  public List<Texture2D> maps = new List<Texture2D>();
  public List<Texture2D> masks = new List<Texture2D>();
  public List<Texture2D> dirMaps = new List<Texture2D>();
  public List<Texture2D> rnmMaps0 = new List<Texture2D>();
  public List<Texture2D> rnmMaps1 = new List<Texture2D>();
  public List<Texture2D> rnmMaps2 = new List<Texture2D>();
  public List<int> mapsMode = new List<int>();
  public List<Renderer> bakedRenderers = new List<Renderer>();
  public List<int> bakedIDs = new List<int>();
  public List<Vector4> bakedScaleOffset = new List<Vector4>();
  public List<Mesh> bakedVertexColorMesh = new List<Mesh>();
  public List<Renderer> nonBakedRenderers = new List<Renderer>();
  public List<Light> bakedLights = new List<Light>();
  public List<int> bakedLightChannels = new List<int>();
  public List<Terrain> bakedRenderersTerrain = new List<Terrain>();
  public List<int> bakedIDsTerrain = new List<int>();
  public List<Vector4> bakedScaleOffsetTerrain = new List<Vector4>();
  public List<string> assetList = new List<string>();
  public List<int> uvOverlapAssetList = new List<int>();
  public int[] idremap;
  public bool usesRealtimeGI;
  public Texture2D emptyDirectionTex;

  private void Awake() => ftLightmaps.RefreshScene(this.gameObject.scene, this);

  private void Start() => ftLightmaps.RefreshScene2(this.gameObject.scene, this);

  private void OnDestroy() => ftLightmaps.UnloadScene(this);
}
