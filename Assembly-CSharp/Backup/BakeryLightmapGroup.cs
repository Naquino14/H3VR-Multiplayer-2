// Decompiled with JetBrains decompiler
// Type: BakeryLightmapGroup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Bakery lightmap group")]
public class BakeryLightmapGroup : ScriptableObject
{
  [SerializeField]
  [Range(1f, 8192f)]
  public int resolution = 512;
  [SerializeField]
  public int bitmask = 1;
  [SerializeField]
  public int id = -1;
  public int sortingID = -1;
  [SerializeField]
  public bool isImplicit;
  [SerializeField]
  public float area;
  [SerializeField]
  public int totalVertexCount;
  [SerializeField]
  public int vertexCounter;
  [SerializeField]
  public int sceneLodLevel = -1;
  [SerializeField]
  public string sceneName;
  [SerializeField]
  public bool containsTerrains;
  [SerializeField]
  public bool probes;
  [SerializeField]
  public BakeryLightmapGroup.ftLMGroupMode mode = BakeryLightmapGroup.ftLMGroupMode.PackAtlas;
  [SerializeField]
  public BakeryLightmapGroup.RenderMode renderMode = BakeryLightmapGroup.RenderMode.Auto;
  [SerializeField]
  public BakeryLightmapGroup.RenderDirMode renderDirMode = BakeryLightmapGroup.RenderDirMode.Auto;
  [SerializeField]
  public bool computeSSS;
  [SerializeField]
  public int sssSamples = 16;
  [SerializeField]
  public float sssDensity = 10f;
  [SerializeField]
  public Color sssColor = Color.white;
  [SerializeField]
  public float fakeShadowBias;
  [SerializeField]
  public bool transparentSelfShadow;
  [SerializeField]
  public bool flipNormal;
  [SerializeField]
  public string overridePath = string.Empty;

  public BakeryLightmapGroupPlain GetPlainStruct()
  {
    BakeryLightmapGroupPlain lightmapGroupPlain;
    lightmapGroupPlain.name = this.name;
    lightmapGroupPlain.id = this.id;
    lightmapGroupPlain.resolution = this.resolution;
    lightmapGroupPlain.vertexBake = this.mode == BakeryLightmapGroup.ftLMGroupMode.Vertex;
    lightmapGroupPlain.isImplicit = this.isImplicit;
    lightmapGroupPlain.renderMode = (int) this.renderMode;
    lightmapGroupPlain.renderDirMode = (int) this.renderDirMode;
    lightmapGroupPlain.computeSSS = this.computeSSS;
    lightmapGroupPlain.sssSamples = this.sssSamples;
    lightmapGroupPlain.sssDensity = this.sssDensity;
    lightmapGroupPlain.sssR = this.sssColor.r;
    lightmapGroupPlain.sssG = this.sssColor.g;
    lightmapGroupPlain.sssB = this.sssColor.b;
    lightmapGroupPlain.containsTerrains = this.containsTerrains;
    lightmapGroupPlain.probes = this.probes;
    lightmapGroupPlain.fakeShadowBias = this.fakeShadowBias;
    lightmapGroupPlain.transparentSelfShadow = this.transparentSelfShadow;
    lightmapGroupPlain.flipNormal = this.flipNormal;
    return lightmapGroupPlain;
  }

  public enum ftLMGroupMode
  {
    OriginalUV,
    PackAtlas,
    Vertex,
  }

  public enum RenderMode
  {
    FullLighting = 0,
    Indirect = 1,
    Shadowmask = 2,
    Subtractive = 3,
    AmbientOcclusionOnly = 4,
    Auto = 1000, // 0x000003E8
  }

  public enum RenderDirMode
  {
    None = 0,
    BakedNormalMaps = 1,
    DominantDirection = 2,
    RNM = 3,
    SH = 4,
    ProbeSH = 5,
    Auto = 1000, // 0x000003E8
  }
}
