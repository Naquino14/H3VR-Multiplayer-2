// Decompiled with JetBrains decompiler
// Type: PTargetProfile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu]
public class PTargetProfile : ScriptableObject
{
  public string displayName;
  public Sprite displayIcon;
  public string displayDetails;
  public float healthMultiplier = 1f;
  public int gridSizeX = 32;
  public int gridSizeY = 32;
  public float targetWidth = 0.5f;
  public float targetHeight = 0.5f;
  public int renderTextureResolutionX = 1024;
  public int renderTextureResolutionY = 1024;
  public bool renderTextureUseMipmap = true;
  public FilterMode renderTextureFilterMode = FilterMode.Trilinear;
  public int renderTextureAnisoLevel = 4;
  public Texture2D scoreMap;
  public int[] scores;
  public PTargetDecal background;
  public PTargetDecal[] tearDecals;
  public PTargetDecal[] bulletDecals;

  public float cellWidth => this.targetWidth / (float) this.gridSizeX;

  public float cellHeight => this.targetHeight / (float) this.gridSizeY;

  public float cellArea => this.cellWidth * this.cellHeight;

  public float cellHealth => this.cellArea * this.healthMultiplier;
}
