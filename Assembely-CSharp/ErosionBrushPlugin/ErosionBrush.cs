using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErosionBrushPlugin
{
	[ExecuteInEditMode]
	public class ErosionBrush : MonoBehaviour
	{
		public struct UndoStep
		{
			private float[,] heights;

			private int heightsOffsetX;

			private int heightsOffsetZ;

			private float[,,] splats;

			private int splatsOffsetX;

			private int splatsOffsetZ;

			public UndoStep(float[,] heights, float[,,] splats, int heightsOffsetX, int heightsOffsetZ, int splatsOffsetX, int splatsOffsetZ)
			{
				if (heightsOffsetX < 0)
				{
					heightsOffsetX = 0;
				}
				if (heightsOffsetZ < 0)
				{
					heightsOffsetZ = 0;
				}
				if (splatsOffsetX < 0)
				{
					splatsOffsetX = 0;
				}
				if (splatsOffsetZ < 0)
				{
					splatsOffsetZ = 0;
				}
				this.heightsOffsetX = heightsOffsetX;
				this.heightsOffsetZ = heightsOffsetZ;
				this.splatsOffsetX = splatsOffsetX;
				this.splatsOffsetZ = splatsOffsetZ;
				this.heights = heights.Clone() as float[,];
				if (splats != null)
				{
					this.splats = splats.Clone() as float[,,];
				}
				else
				{
					this.splats = null;
				}
			}

			public void Perform(TerrainData data)
			{
				data.SetHeights(heightsOffsetX, heightsOffsetZ, heights);
				if (splats != null)
				{
					data.SetAlphamaps(splatsOffsetX, splatsOffsetZ, splats);
				}
			}
		}

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

		public List<List<UndoStep>> undoList = new List<List<UndoStep>>();

		public bool allowUndo;

		public Terrain terrain
		{
			get
			{
				if (_terrain == null)
				{
					_terrain = GetComponent<Terrain>();
				}
				return _terrain;
			}
			set
			{
				_terrain = value;
			}
		}

		public void ApplyBrush(Rect worldRect, bool useFallof = true, bool newUndo = false)
		{
			TerrainData terrainData = terrain.terrainData;
			bool flag = preset.foreground.apply || preset.background.apply;
			if (terrainData.alphamapLayers == 0)
			{
				flag = false;
			}
			int num = terrainData.heightmapResolution - 1;
			int alphamapResolution = terrainData.alphamapResolution;
			CoordRect coordRect = new CoordRect(worldRect.x * (float)num, worldRect.y * (float)num, worldRect.width * (float)num, worldRect.height * (float)num);
			CoordRect coordRect2 = new CoordRect(worldRect.x * (float)alphamapResolution, worldRect.y * (float)alphamapResolution, worldRect.width * (float)alphamapResolution, worldRect.height * (float)alphamapResolution);
			CoordRect newRect = coordRect / preset.downscale;
			srcHeight.ChangeRect(coordRect);
			CoordRect c = CoordRect.Intersect(coordRect, new CoordRect(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution));
			float[,] heights = terrainData.GetHeights(c.offset.x, c.offset.z, c.size.x, c.size.z);
			CoordRect centerRect = CoordRect.Intersect(coordRect, c);
			Coord min = centerRect.Min;
			Coord max = centerRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					srcHeight[i, j] = heights[j - c.offset.z, i - c.offset.x];
				}
			}
			srcHeight.RemoveBorders(centerRect);
			CoordRect c2 = CoordRect.Intersect(coordRect2, new CoordRect(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution));
			float[,,] alphamaps = terrainData.GetAlphamaps(c2.offset.x, c2.offset.z, c2.size.x, c2.size.z);
			if (recordUndo)
			{
				if (newUndo)
				{
					if (undoList.Count > 10)
					{
						undoList.RemoveAt(0);
					}
					undoList.Add(new List<UndoStep>());
				}
				if (undoList.Count == 0)
				{
					undoList.Add(new List<UndoStep>());
				}
				undoList[undoList.Count - 1].Add(new UndoStep(heights, alphamaps, c.offset.x, c.offset.z, c2.offset.x, c2.offset.z));
			}
			wrkHeight = srcHeight.Resize(newRect, wrkHeight);
			wrkCliff.ChangeRect(newRect);
			wrkCliff.Clear();
			wrkSediment.ChangeRect(newRect);
			wrkSediment.Clear();
			dstHeight = wrkHeight.Resize(coordRect, dstHeight);
			if (preset.downscale != 1)
			{
				Matrix matrix = dstHeight;
				float intensity = preset.downscale / 4;
				matrix.Blur(null, intensity);
			}
			dstCliff = wrkCliff.Resize(coordRect2, dstCliff);
			dstSediment = wrkSediment.Resize(coordRect2, dstSediment);
			if (preset.downscale != 1 && preset.preserveDetail)
			{
				Matrix matrix2 = srcHeight.Copy();
				matrix2 = matrix2.Resize(newRect);
				matrix2 = matrix2.Resize(coordRect);
				Matrix matrix3 = matrix2;
				float intensity = preset.downscale / 4;
				matrix3.Blur(null, intensity);
				for (int k = 0; k < dstHeight.count; k++)
				{
					float num2 = srcHeight.array[k] - matrix2.array[k];
					dstHeight.array[k] += num2;
				}
			}
			min = coordRect.Min;
			max = coordRect.Max;
			Coord center = coordRect.Center;
			float num3 = (float)coordRect.size.x / 2f;
			for (int l = min.x; l < max.x; l++)
			{
				for (int m = min.z; m < max.z; m++)
				{
					float num4 = (num3 - Coord.Distance(new Coord(l, m), center)) / (num3 - num3 * preset.brushFallof);
					if (num4 < 0f)
					{
						num4 = 0f;
					}
					if (num4 > 1f)
					{
						num4 = 1f;
					}
					num4 = 3f * num4 * num4 - 2f * num4 * num4 * num4;
					dstHeight[l, m] = srcHeight[l, m] * (1f - num4) + dstHeight[l, m] * num4;
				}
			}
			min = coordRect2.Min;
			max = coordRect2.Max;
			center = coordRect2.Center;
			num3 = (float)coordRect2.size.x / 2f;
			for (int n = min.x; n < max.x; n++)
			{
				for (int num5 = min.z; num5 < max.z; num5++)
				{
					float num6 = (num3 - Coord.Distance(new Coord(n, num5), center)) / (num3 - num3 * preset.brushFallof);
					if (num6 < 0f)
					{
						num6 = 0f;
					}
					if (num6 > 1f)
					{
						num6 = 1f;
					}
					num6 = 3f * num6 * num6 - 2f * num6 * num6 * num6;
					dstCliff[n, num5] *= num6;
					dstSediment[n, num5] *= num6;
				}
			}
			centerRect = CoordRect.Intersect(coordRect, c);
			min = centerRect.Min;
			max = centerRect.Max;
			for (int num7 = min.x; num7 < max.x; num7++)
			{
				for (int num8 = min.z; num8 < max.z; num8++)
				{
					heights[num8 - c.offset.z, num7 - c.offset.x] = dstHeight[num7, num8];
				}
			}
			terrainData.SetHeightsDelayLOD(c.offset.x, c.offset.z, heights);
			if (!flag)
			{
				return;
			}
			centerRect = CoordRect.Intersect(coordRect2, c2);
			min = centerRect.Min;
			max = centerRect.Max;
			for (int num9 = min.x; num9 < max.x; num9++)
			{
				for (int num10 = min.z; num10 < max.z; num10++)
				{
					alphamaps[num10 - c2.offset.z, num9 - c2.offset.x, preset.foreground.num] += dstCliff[num9, num10];
				}
			}
			alphamaps.Normalize(preset.foreground.num);
			for (int num11 = min.x; num11 < max.x; num11++)
			{
				for (int num12 = min.z; num12 < max.z; num12++)
				{
					alphamaps[num12 - c2.offset.z, num11 - c2.offset.x, preset.background.num] += dstSediment[num11, num12];
				}
			}
			alphamaps.Normalize(preset.background.num);
			terrainData.SetAlphamaps(c2.offset.x, c2.offset.z, alphamaps);
		}
	}
}
