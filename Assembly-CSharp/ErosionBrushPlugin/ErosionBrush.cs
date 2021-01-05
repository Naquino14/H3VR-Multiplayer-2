// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.ErosionBrush
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErosionBrushPlugin
{
  [ExecuteInEditMode]
  public class ErosionBrush : MonoBehaviour
  {
    private Terrain _terrain;
    public Preset preset = new Preset();
    public Preset[] presets = new Preset[0];
    public int guiSelectedPreset;
    public bool paint;
    public bool wasPaint;
    public bool moveDown;
    public Transform moveTfm;
    public bool gen;
    public bool undo;
    [NonSerialized]
    public Texture2D guiHydraulicIcon;
    [NonSerialized]
    public Texture2D guiWindIcon;
    [NonSerialized]
    public Texture2D guiPluginIcon;
    public int guiApplyIterations = 1;
    public int[] guiChannels;
    public string[] guiChannelNames;
    public Color guiBrushColor = new Color(1f, 0.7f, 0.3f);
    public float guiBrushThickness = 4f;
    public int guiBrushNumCorners = 32;
    public bool recordUndo = true;
    public bool unity5positioning;
    public bool focusOnBrush = true;
    public bool guiShowPreset = true;
    public bool guiShowBrush = true;
    public bool guiShowGenerator = true;
    public bool guiShowTextures = true;
    public bool guiShowGlobal;
    public bool guiShowSettings;
    public bool guiShowAbout;
    public int guiMaxBrushSize = 100;
    public bool guiSelectPresetsUsingNumkeys = true;
    [NonSerialized]
    private Matrix srcHeight = new Matrix(new CoordRect(0, 0, 0, 0));
    [NonSerialized]
    private Matrix wrkHeight = new Matrix(new CoordRect(0, 0, 0, 0));
    [NonSerialized]
    private Matrix wrkCliff = new Matrix(new CoordRect(0, 0, 0, 0));
    [NonSerialized]
    private Matrix wrkSediment = new Matrix(new CoordRect(0, 0, 0, 0));
    [NonSerialized]
    private Matrix dstHeight = new Matrix(new CoordRect(0, 0, 0, 0));
    [NonSerialized]
    private Matrix dstCliff = new Matrix(new CoordRect(0, 0, 0, 0));
    [NonSerialized]
    private Matrix dstSediment = new Matrix(new CoordRect(0, 0, 0, 0));
    public List<List<ErosionBrush.UndoStep>> undoList = new List<List<ErosionBrush.UndoStep>>();
    public bool allowUndo;

    public Terrain terrain
    {
      get
      {
        if ((UnityEngine.Object) this._terrain == (UnityEngine.Object) null)
          this._terrain = this.GetComponent<Terrain>();
        return this._terrain;
      }
      set => this._terrain = value;
    }

    public void ApplyBrush(Rect worldRect, bool useFallof = true, bool newUndo = false)
    {
      TerrainData terrainData = this.terrain.terrainData;
      bool flag = this.preset.foreground.apply || this.preset.background.apply;
      if (terrainData.alphamapLayers == 0)
        flag = false;
      int num1 = terrainData.heightmapResolution - 1;
      int alphamapResolution = terrainData.alphamapResolution;
      CoordRect coordRect1 = new CoordRect(worldRect.x * (float) num1, worldRect.y * (float) num1, worldRect.width * (float) num1, worldRect.height * (float) num1);
      CoordRect coordRect2 = new CoordRect(worldRect.x * (float) alphamapResolution, worldRect.y * (float) alphamapResolution, worldRect.width * (float) alphamapResolution, worldRect.height * (float) alphamapResolution);
      CoordRect newRect = coordRect1 / this.preset.downscale;
      this.srcHeight.ChangeRect(coordRect1);
      CoordRect c2_1 = CoordRect.Intersect(coordRect1, new CoordRect(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution));
      float[,] heights = terrainData.GetHeights(c2_1.offset.x, c2_1.offset.z, c2_1.size.x, c2_1.size.z);
      CoordRect centerRect = CoordRect.Intersect(coordRect1, c2_1);
      Coord min = centerRect.Min;
      Coord max1 = centerRect.Max;
      for (int x = min.x; x < max1.x; ++x)
      {
        for (int z = min.z; z < max1.z; ++z)
          this.srcHeight[x, z] = heights[z - c2_1.offset.z, x - c2_1.offset.x];
      }
      this.srcHeight.RemoveBorders(centerRect);
      CoordRect c2_2 = CoordRect.Intersect(coordRect2, new CoordRect(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution));
      float[,,] alphamaps = terrainData.GetAlphamaps(c2_2.offset.x, c2_2.offset.z, c2_2.size.x, c2_2.size.z);
      if (this.recordUndo)
      {
        if (newUndo)
        {
          if (this.undoList.Count > 10)
            this.undoList.RemoveAt(0);
          this.undoList.Add(new List<ErosionBrush.UndoStep>());
        }
        if (this.undoList.Count == 0)
          this.undoList.Add(new List<ErosionBrush.UndoStep>());
        this.undoList[this.undoList.Count - 1].Add(new ErosionBrush.UndoStep(heights, alphamaps, c2_1.offset.x, c2_1.offset.z, c2_2.offset.x, c2_2.offset.z));
      }
      this.wrkHeight = this.srcHeight.Resize(newRect, this.wrkHeight);
      this.wrkCliff.ChangeRect(newRect);
      this.wrkCliff.Clear();
      this.wrkSediment.ChangeRect(newRect);
      this.wrkSediment.Clear();
      this.dstHeight = this.wrkHeight.Resize(coordRect1, this.dstHeight);
      if (this.preset.downscale != 1)
        this.dstHeight.Blur(intensity: ((float) (this.preset.downscale / 4)));
      this.dstCliff = this.wrkCliff.Resize(coordRect2, this.dstCliff);
      this.dstSediment = this.wrkSediment.Resize(coordRect2, this.dstSediment);
      if (this.preset.downscale != 1 && this.preset.preserveDetail)
      {
        Matrix matrix = this.srcHeight.Copy().Resize(newRect).Resize(coordRect1);
        matrix.Blur(intensity: ((float) (this.preset.downscale / 4)));
        for (int index = 0; index < this.dstHeight.count; ++index)
        {
          float num2 = this.srcHeight.array[index] - matrix.array[index];
          this.dstHeight.array[index] += num2;
        }
      }
      min = coordRect1.Min;
      Coord max2 = coordRect1.Max;
      Coord center1 = coordRect1.Center;
      float num3 = (float) coordRect1.size.x / 2f;
      for (int x = min.x; x < max2.x; ++x)
      {
        for (int z = min.z; z < max2.z; ++z)
        {
          float num2 = (float) (((double) num3 - (double) Coord.Distance(new Coord(x, z), center1)) / ((double) num3 - (double) num3 * (double) this.preset.brushFallof));
          if ((double) num2 < 0.0)
            num2 = 0.0f;
          if ((double) num2 > 1.0)
            num2 = 1f;
          float num4 = (float) (3.0 * (double) num2 * (double) num2 - 2.0 * (double) num2 * (double) num2 * (double) num2);
          this.dstHeight[x, z] = (float) ((double) this.srcHeight[x, z] * (1.0 - (double) num4) + (double) this.dstHeight[x, z] * (double) num4);
        }
      }
      min = coordRect2.Min;
      Coord max3 = coordRect2.Max;
      Coord center2 = coordRect2.Center;
      float num5 = (float) coordRect2.size.x / 2f;
      for (int x1 = min.x; x1 < max3.x; ++x1)
      {
        for (int z1 = min.z; z1 < max3.z; ++z1)
        {
          float num2 = (float) (((double) num5 - (double) Coord.Distance(new Coord(x1, z1), center2)) / ((double) num5 - (double) num5 * (double) this.preset.brushFallof));
          if ((double) num2 < 0.0)
            num2 = 0.0f;
          if ((double) num2 > 1.0)
            num2 = 1f;
          float num4 = (float) (3.0 * (double) num2 * (double) num2 - 2.0 * (double) num2 * (double) num2 * (double) num2);
          Matrix dstCliff;
          int x2;
          int z2;
          (dstCliff = this.dstCliff)[x2 = x1, z2 = z1] = dstCliff[x2, z2] * num4;
          Matrix dstSediment;
          int x3;
          int z3;
          (dstSediment = this.dstSediment)[x3 = x1, z3 = z1] = dstSediment[x3, z3] * num4;
        }
      }
      centerRect = CoordRect.Intersect(coordRect1, c2_1);
      min = centerRect.Min;
      Coord max4 = centerRect.Max;
      for (int x = min.x; x < max4.x; ++x)
      {
        for (int z = min.z; z < max4.z; ++z)
          heights[z - c2_1.offset.z, x - c2_1.offset.x] = this.dstHeight[x, z];
      }
      terrainData.SetHeightsDelayLOD(c2_1.offset.x, c2_1.offset.z, heights);
      if (!flag)
        return;
      centerRect = CoordRect.Intersect(coordRect2, c2_2);
      min = centerRect.Min;
      Coord max5 = centerRect.Max;
      for (int x = min.x; x < max5.x; ++x)
      {
        for (int z = min.z; z < max5.z; ++z)
          alphamaps[z - c2_2.offset.z, x - c2_2.offset.x, this.preset.foreground.num] += this.dstCliff[x, z];
      }
      alphamaps.Normalize(this.preset.foreground.num);
      for (int x = min.x; x < max5.x; ++x)
      {
        for (int z = min.z; z < max5.z; ++z)
          alphamaps[z - c2_2.offset.z, x - c2_2.offset.x, this.preset.background.num] += this.dstSediment[x, z];
      }
      alphamaps.Normalize(this.preset.background.num);
      terrainData.SetAlphamaps(c2_2.offset.x, c2_2.offset.z, alphamaps);
    }

    public struct UndoStep
    {
      private float[,] heights;
      private int heightsOffsetX;
      private int heightsOffsetZ;
      private float[,,] splats;
      private int splatsOffsetX;
      private int splatsOffsetZ;

      public UndoStep(
        float[,] heights,
        float[,,] splats,
        int heightsOffsetX,
        int heightsOffsetZ,
        int splatsOffsetX,
        int splatsOffsetZ)
      {
        if (heightsOffsetX < 0)
          heightsOffsetX = 0;
        if (heightsOffsetZ < 0)
          heightsOffsetZ = 0;
        if (splatsOffsetX < 0)
          splatsOffsetX = 0;
        if (splatsOffsetZ < 0)
          splatsOffsetZ = 0;
        this.heightsOffsetX = heightsOffsetX;
        this.heightsOffsetZ = heightsOffsetZ;
        this.splatsOffsetX = splatsOffsetX;
        this.splatsOffsetZ = splatsOffsetZ;
        this.heights = heights.Clone() as float[,];
        if (splats != null)
          this.splats = splats.Clone() as float[,,];
        else
          this.splats = (float[,,]) null;
      }

      public void Perform(TerrainData data)
      {
        data.SetHeights(this.heightsOffsetX, this.heightsOffsetZ, this.heights);
        if (this.splats == null)
          return;
        data.SetAlphamaps(this.splatsOffsetX, this.splatsOffsetZ, this.splats);
      }
    }
  }
}
