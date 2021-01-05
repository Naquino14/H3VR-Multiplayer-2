using System.Collections.Generic;
using UnityEngine;

public class PTargetGridJob : CynicatSimpleJob
{
	public int gridSizeX = 32;

	public int gridSizeY = 32;

	public float targetSizeX;

	public float targetSizeY;

	public int maxLabels;

	public float cellSizeX;

	public float cellSizeY;

	public bool[,] grid;

	private bool[,,] edgeGrid;

	private bool[,] lastStampMaskHorizontal;

	private bool[,] lastStampMaskVertical;

	private bool[,] lastStampMaskDiagonalLeft;

	private bool[,] lastStampMaskDiagonalRight;

	private bool[,] stampMaskHorizontal;

	private bool[,] stampMaskVertical;

	private bool[,] stampMaskDiagonalLeft;

	private bool[,] stampMaskDiagonalRight;

	private DisjointSet conflictTable;

	public Rect[] labelRects;

	public int[] differenceRemapping;

	public List<int> newLabelsBackBuffer;

	public List<int> newLabelsFrontBuffer;

	public List<Vector3> tearDecalStart;

	public List<Vector3> tearDecalEnd;

	public int[,] lastLabelGrid;

	public int[,] labelGrid;

	public List<int> attachedLabels;

	public List<MeshBuilder> meshBuilders;

	public MeshBuilder[] meshBuilderMap;

	public PTargetGridJob()
	{
	}

	public PTargetGridJob(int gridSizeX, int gridSizeY, float targetSizeX, float targetSizeY)
	{
		SetGridData(gridSizeX, gridSizeY, targetSizeX, targetSizeY);
	}

	public void Resize<T>(ref T[] array, int length)
	{
		if (array == null || array.Length != length)
		{
			array = new T[length];
		}
	}

	public void Resize<T>(ref T[,] array, int width, int height)
	{
		if (array == null || array.GetLength(0) != width || array.GetLength(1) != height)
		{
			array = new T[width, height];
		}
	}

	public void Resize<T>(ref T[,,] array, int width, int height, int depth)
	{
		if (array == null || array.GetLength(0) != width || array.GetLength(1) != height || array.GetLength(2) != depth)
		{
			array = new T[width, height, depth];
		}
	}

	public void SetGridData(int gridSizeX, int gridSizeY, float targetSizeX, float targetSizeY)
	{
		this.gridSizeX = gridSizeX;
		this.gridSizeY = gridSizeY;
		this.targetSizeX = targetSizeX;
		this.targetSizeY = targetSizeY;
		maxLabels = gridSizeX * (gridSizeY / 2) + 1;
		cellSizeX = targetSizeX / (float)gridSizeX;
		cellSizeY = targetSizeY / (float)gridSizeY;
		if (grid == null || grid.GetLength(0) != gridSizeX || grid.GetLength(1) != gridSizeY)
		{
			grid = new bool[gridSizeX, gridSizeY];
		}
		Resize(ref edgeGrid, gridSizeX + 2, gridSizeY + 2, 8);
		Resize(ref lastStampMaskHorizontal, gridSizeX + 1, gridSizeY);
		Resize(ref lastStampMaskVertical, gridSizeX, gridSizeY + 1);
		Resize(ref lastStampMaskDiagonalLeft, gridSizeX + 1, gridSizeY + 1);
		Resize(ref lastStampMaskDiagonalRight, gridSizeX + 1, gridSizeY + 1);
		Resize(ref stampMaskHorizontal, gridSizeX + 1, gridSizeY);
		Resize(ref stampMaskVertical, gridSizeX, gridSizeY + 1);
		Resize(ref stampMaskDiagonalLeft, gridSizeX + 1, gridSizeY + 1);
		Resize(ref stampMaskDiagonalRight, gridSizeX + 1, gridSizeY + 1);
		if (conflictTable.parent == null || conflictTable.rank == null || conflictTable.parent.Length != maxLabels || conflictTable.rank.Length != maxLabels)
		{
			conflictTable = new DisjointSet(maxLabels);
		}
		Resize(ref labelRects, maxLabels);
		Resize(ref differenceRemapping, maxLabels);
		if (newLabelsBackBuffer == null)
		{
			newLabelsBackBuffer = new List<int>();
		}
		if (newLabelsFrontBuffer == null)
		{
			newLabelsFrontBuffer = new List<int>();
		}
		if (tearDecalStart == null)
		{
			tearDecalStart = new List<Vector3>(8);
		}
		if (tearDecalEnd == null)
		{
			tearDecalEnd = new List<Vector3>(8);
		}
		Resize(ref lastLabelGrid, gridSizeX, gridSizeY);
		Resize(ref labelGrid, gridSizeX, gridSizeY);
		if (attachedLabels == null)
		{
			attachedLabels = new List<int>(8);
		}
		if (meshBuilders == null)
		{
			meshBuilders = new List<MeshBuilder>(128);
		}
		Resize(ref meshBuilderMap, maxLabels);
	}

	protected override void OnStart()
	{
	}

	public override void ClearData()
	{
		ClearGrid(grid, value: true);
		ClearGrid(edgeGrid, value: false);
		ClearGrid(lastStampMaskHorizontal, value: false);
		ClearGrid(lastStampMaskVertical, value: false);
		ClearGrid(lastStampMaskDiagonalLeft, value: false);
		ClearGrid(lastStampMaskDiagonalRight, value: false);
		ClearGrid(stampMaskHorizontal, value: false);
		ClearGrid(stampMaskVertical, value: false);
		ClearGrid(stampMaskDiagonalLeft, value: false);
		ClearGrid(stampMaskDiagonalRight, value: false);
		conflictTable.Clear();
		ClearArray(labelRects, new Rect(float.MinValue, float.MinValue, 0f, 0f));
		ClearDifferenceRemapping(differenceRemapping);
		newLabelsBackBuffer.Clear();
		newLabelsFrontBuffer.Clear();
		tearDecalStart.Clear();
		tearDecalEnd.Clear();
		ClearGrid(lastLabelGrid, 1);
		ClearGrid(labelGrid, 1);
		attachedLabels.Clear();
		ClearMeshBuilders(meshBuilders, meshBuilderMap);
	}

	protected override void OnSchedule()
	{
		SwapBackbuffer(ref lastStampMaskHorizontal, ref stampMaskHorizontal);
		SwapBackbuffer(ref lastStampMaskVertical, ref stampMaskVertical);
		SwapBackbuffer(ref lastStampMaskDiagonalLeft, ref stampMaskDiagonalLeft);
		SwapBackbuffer(ref lastStampMaskDiagonalRight, ref stampMaskDiagonalRight);
		SwapBackbuffer(ref lastLabelGrid, ref labelGrid);
	}

	protected override void OnExecute()
	{
		BuildEdgeGrid(grid, edgeGrid);
		BuildStampMask(edgeGrid, stampMaskHorizontal, stampMaskVertical, stampMaskDiagonalLeft, stampMaskDiagonalRight);
		ConnectedComponents(grid, labelGrid, conflictTable);
		BuildLabelDifferances(lastLabelGrid, labelGrid, differenceRemapping, newLabelsFrontBuffer, newLabelsBackBuffer);
		BuildLabelRects(cellSizeX, cellSizeY, labelGrid, labelRects);
		BuildPieceMeshes(cellSizeX, cellSizeY, grid, labelGrid, meshBuilders, meshBuilderMap);
		BuildTearDecals(gridSizeX, gridSizeY, stampMaskHorizontal, stampMaskVertical, stampMaskDiagonalLeft, stampMaskDiagonalRight, lastStampMaskHorizontal, lastStampMaskVertical, lastStampMaskDiagonalLeft, lastStampMaskDiagonalRight, tearDecalStart, tearDecalEnd);
		BuildAttachedLabels(labelGrid, attachedLabels);
	}

	private static void ClearArray<T>(T[] array, T value)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = value;
		}
	}

	private static void ClearGrid<T>(T[,] grid, T value)
	{
		for (int i = 0; i < grid.GetLength(1); i++)
		{
			for (int j = 0; j < grid.GetLength(0); j++)
			{
				grid[j, i] = value;
			}
		}
	}

	private static void ClearGrid<T>(T[,,] grid, T value)
	{
		for (int i = 0; i < grid.GetLength(2); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				for (int k = 0; k < grid.GetLength(0); k++)
				{
					grid[k, j, i] = value;
				}
			}
		}
	}

	private static void ClearMeshBuilders(List<MeshBuilder> meshBuilders, MeshBuilder[] meshBuilderMap)
	{
		int count = meshBuilders.Count;
		for (int i = 0; i < count; i++)
		{
			meshBuilders[i].Clear();
		}
		for (int j = 0; j < meshBuilderMap.Length; j++)
		{
			meshBuilderMap[j] = null;
		}
	}

	private static void ClearDifferenceRemapping(int[] differenceRemapping)
	{
		for (int i = 0; i < differenceRemapping.Length; i++)
		{
			differenceRemapping[i] = 0;
		}
		differenceRemapping[1] = 1;
	}

	private static void GetAdjascent(int[,] labelGrid, int x, int y, out int bottomLabel, out int topLabel, out int leftLabel, out int rightLabel, out int bottomLeftLabel, out int bottomRightLabel, out int topLeftLabel, out int topRightLabel)
	{
		int length = labelGrid.GetLength(0);
		int length2 = labelGrid.GetLength(1);
		int num = length - 1;
		int num2 = length2 - 1;
		int num3 = x - 1;
		int num4 = x + 1;
		int num5 = y - 1;
		int num6 = y + 1;
		bottomLabel = labelGrid[(x >= 0) ? ((x <= num) ? x : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0];
		topLabel = labelGrid[(x >= 0) ? ((x <= num) ? x : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0];
		leftLabel = labelGrid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (y >= 0) ? ((y <= num2) ? y : num2) : 0];
		rightLabel = labelGrid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (y >= 0) ? ((y <= num2) ? y : num2) : 0];
		bottomLeftLabel = labelGrid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0];
		bottomRightLabel = labelGrid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0];
		topLeftLabel = labelGrid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0];
		topRightLabel = labelGrid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0];
	}

	private static void GetAdjascent(int[,] labelGrid, int x, int y, int label, out bool bottomLabel, out bool topLabel, out bool leftLabel, out bool rightLabel, out bool bottomLeftLabel, out bool bottomRightLabel, out bool topLeftLabel, out bool topRightLabel)
	{
		int length = labelGrid.GetLength(0);
		int length2 = labelGrid.GetLength(1);
		int num = length - 1;
		int num2 = length2 - 1;
		int num3 = x - 1;
		int num4 = x + 1;
		int num5 = y - 1;
		int num6 = y + 1;
		bottomLabel = labelGrid[(x >= 0) ? ((x <= num) ? x : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0] == label;
		topLabel = labelGrid[(x >= 0) ? ((x <= num) ? x : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0] == label;
		leftLabel = labelGrid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (y >= 0) ? ((y <= num2) ? y : num2) : 0] == label;
		rightLabel = labelGrid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (y >= 0) ? ((y <= num2) ? y : num2) : 0] == label;
		bottomLeftLabel = labelGrid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0] == label;
		bottomRightLabel = labelGrid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0] == label;
		topLeftLabel = labelGrid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0] == label;
		topRightLabel = labelGrid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0] == label;
	}

	private static void GetAdjascent(bool[,] grid, int x, int y, out bool bottomLabel, out bool topLabel, out bool leftLabel, out bool rightLabel, out bool bottomLeftLabel, out bool bottomRightLabel, out bool topLeftLabel, out bool topRightLabel)
	{
		int length = grid.GetLength(0);
		int length2 = grid.GetLength(1);
		int num = length - 1;
		int num2 = length2 - 1;
		int num3 = x - 1;
		int num4 = x + 1;
		int num5 = y - 1;
		int num6 = y + 1;
		bottomLabel = grid[(x >= 0) ? ((x <= num) ? x : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0];
		topLabel = grid[(x >= 0) ? ((x <= num) ? x : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0];
		leftLabel = grid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (y >= 0) ? ((y <= num2) ? y : num2) : 0];
		rightLabel = grid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (y >= 0) ? ((y <= num2) ? y : num2) : 0];
		bottomLeftLabel = grid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0];
		bottomRightLabel = grid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (num5 >= 0) ? ((num5 <= num2) ? num5 : num2) : 0];
		topLeftLabel = grid[(num3 >= 0) ? ((num3 <= num) ? num3 : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0];
		topRightLabel = grid[(num4 >= 0) ? ((num4 <= num) ? num4 : num) : 0, (num6 >= 0) ? ((num6 <= num2) ? num6 : num2) : 0];
	}

	private static void BuildCornerEdges(bool corner, bool left, bool right, ref bool cornerOutput, ref bool leftOutput, ref bool rightOutput)
	{
		rightOutput |= corner && left && !right;
		leftOutput |= corner && !left && right;
		cornerOutput |= !corner && (left || right);
	}

	private static void BuildEdgeGrid(bool[,] grid, bool[,,] edgeGrid)
	{
		int length = grid.GetLength(0);
		int length2 = grid.GetLength(1);
		int length3 = edgeGrid.GetLength(0);
		int length4 = edgeGrid.GetLength(1);
		for (int i = 0; i < length4; i++)
		{
			for (int j = 0; j < length3; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					edgeGrid[j, i, k] = false;
				}
				if (!grid[Mathf.Clamp(j - 1, 0, length - 1), Mathf.Clamp(i - 1, 0, length2 - 1)])
				{
					GetAdjascent(grid, j - 1, i - 1, out var bottomLabel, out var topLabel, out var leftLabel, out var rightLabel, out var bottomLeftLabel, out var bottomRightLabel, out var topLeftLabel, out var topRightLabel);
					BuildCornerEdges(topLeftLabel, leftLabel, topLabel, ref edgeGrid[j, i, 7], ref edgeGrid[j, i, 6], ref edgeGrid[j, i, 0]);
					BuildCornerEdges(topRightLabel, topLabel, rightLabel, ref edgeGrid[j, i, 1], ref edgeGrid[j, i, 0], ref edgeGrid[j, i, 2]);
					BuildCornerEdges(bottomRightLabel, rightLabel, bottomLabel, ref edgeGrid[j, i, 3], ref edgeGrid[j, i, 2], ref edgeGrid[j, i, 4]);
					BuildCornerEdges(bottomLeftLabel, bottomLabel, leftLabel, ref edgeGrid[j, i, 5], ref edgeGrid[j, i, 4], ref edgeGrid[j, i, 6]);
				}
			}
		}
	}

	private static void SwapBackbuffer<T>(ref T[,] backBuffer, ref T[,] frontBuffer)
	{
		T[,] array = backBuffer;
		backBuffer = frontBuffer;
		frontBuffer = array;
	}

	private static void BuildStampMask(bool[,,] edgeGrid, bool[,] stampMaskHorizontal, bool[,] stampMaskVertical, bool[,] stampMaskDiagonalLeft, bool[,] stampMaskDiagonalRight)
	{
		int length = edgeGrid.GetLength(0);
		int length2 = edgeGrid.GetLength(1);
		for (int i = 1; i < length2 - 1; i++)
		{
			for (int j = 0; j < length - 1; j++)
			{
				stampMaskHorizontal[j, i - 1] = edgeGrid[Mathf.Clamp(j, 0, length - 1), Mathf.Clamp(i, 0, length2 - 1), 2] || edgeGrid[Mathf.Clamp(j + 1, 0, length - 1), Mathf.Clamp(i, 0, length2 - 1), 6];
			}
		}
		for (int k = 0; k < length2 - 1; k++)
		{
			for (int l = 1; l < length - 1; l++)
			{
				stampMaskVertical[l - 1, k] = edgeGrid[Mathf.Clamp(l, 0, length - 1), Mathf.Clamp(k, 0, length2 - 1), 0] || edgeGrid[Mathf.Clamp(l, 0, length - 1), Mathf.Clamp(k + 1, 0, length2 - 1), 4];
			}
		}
		for (int m = 0; m < length2 - 1; m++)
		{
			for (int n = 0; n < length - 1; n++)
			{
				stampMaskDiagonalRight[n, m] = edgeGrid[Mathf.Clamp(n, 0, length - 1), Mathf.Clamp(m, 0, length2 - 1), 1] || edgeGrid[Mathf.Clamp(n + 1, 0, length - 1), Mathf.Clamp(m + 1, 0, length2 - 1), 5];
				stampMaskDiagonalLeft[n, m] = edgeGrid[Mathf.Clamp(n, 0, length - 1), Mathf.Clamp(m + 1, 0, length2 - 1), 3] || edgeGrid[Mathf.Clamp(n + 1, 0, length - 1), Mathf.Clamp(m, 0, length2 - 1), 7];
			}
		}
	}

	private static void ConnectedComponents(bool[,] grid, int[,] labels, DisjointSet conflicts)
	{
		int length = grid.GetLength(0);
		int length2 = grid.GetLength(1);
		for (int i = 0; i < length2; i++)
		{
			for (int j = 0; j < length; j++)
			{
				labels[j, i] = 0;
			}
		}
		conflicts.Clear();
		int num = 1;
		if (grid[0, 0])
		{
			labels[0, 0] = num++;
		}
		for (int k = 1; k < length; k++)
		{
			if (grid[k, 0])
			{
				bool flag = grid[k - 1, 0];
				int num2 = labels[k - 1, 0];
				if (flag)
				{
					labels[k, 0] = num2;
				}
				else
				{
					labels[k, 0] = num++;
				}
			}
		}
		for (int l = 1; l < length2; l++)
		{
			if (grid[0, l])
			{
				bool flag2 = grid[0, l - 1];
				int num3 = labels[0, l - 1];
				if (flag2)
				{
					labels[0, l] = num3;
				}
				else
				{
					labels[0, l] = num++;
				}
			}
			for (int m = 1; m < length; m++)
			{
				if (grid[m, l])
				{
					bool flag3 = grid[m, l - 1];
					int num4 = labels[m, l - 1];
					bool flag4 = grid[m - 1, l];
					int num5 = labels[m - 1, l];
					if (flag4 && !flag3)
					{
						labels[m, l] = num5;
					}
					else if (!flag4 && flag3)
					{
						labels[m, l] = num4;
					}
					else if (flag4 && flag3)
					{
						int num6 = Mathf.Min(num5, num4);
						int y = Mathf.Max(num5, num4);
						labels[m, l] = num6;
						conflicts.Union(num6, y);
					}
					else
					{
						labels[m, l] = num++;
					}
				}
			}
		}
		for (int n = 0; n < length2; n++)
		{
			for (int num7 = 0; num7 < length; num7++)
			{
				labels[num7, n] = conflicts.Find(labels[num7, n]);
			}
		}
	}

	private static void BuildLabelDifferances(int[,] backBuffer, int[,] frontBuffer, int[] differenceRemapping, List<int> newLabelsFrontBuffer, List<int> newLabelsBackBuffer)
	{
		for (int i = 0; i < differenceRemapping.Length; i++)
		{
			differenceRemapping[i] = 0;
		}
		newLabelsFrontBuffer.Clear();
		newLabelsBackBuffer.Clear();
		for (int j = 0; j < backBuffer.GetLength(1); j++)
		{
			for (int k = 0; k < backBuffer.GetLength(0); k++)
			{
				int num = backBuffer[k, j];
				int num2 = frontBuffer[k, j];
				if (num != 0 && num2 != 0)
				{
					int num3 = differenceRemapping[num];
					if (num3 == 0)
					{
						differenceRemapping[num] = num2;
					}
					else if (num3 != num2 && !newLabelsFrontBuffer.Contains(num2))
					{
						newLabelsBackBuffer.Add(num);
						newLabelsFrontBuffer.Add(num2);
					}
				}
			}
		}
	}

	private static void BuildLabelRects(float cellSizeX, float cellSizeY, int[,] labelGrid, Rect[] labelRects)
	{
		for (int i = 0; i < labelRects.Length; i++)
		{
			ref Rect reference = ref labelRects[i];
			reference = new Rect(float.MinValue, float.MinValue, 0f, 0f);
		}
		int length = labelGrid.GetLength(0);
		int length2 = labelGrid.GetLength(1);
		int num = length / 2;
		int num2 = length2 / 2;
		float num3 = cellSizeX * 0.5f;
		float num4 = cellSizeY * 0.5f;
		float num5 = cellSizeX * (float)num;
		float num6 = cellSizeY * (float)num2;
		float num7 = 0f - num5 + num3;
		float num8 = 0f - num6 + num3;
		for (int j = 0; j < length2; j++)
		{
			num7 = 0f - num5 + num3;
			for (int k = 0; k < length; k++)
			{
				float num9 = num7 - num3;
				float num10 = num8 - num4;
				float num11 = num7 + num3;
				float num12 = num8 + num4;
				int num13 = labelGrid[k, j];
				Rect rect = labelRects[num13];
				if (!(rect.x > -1000f) && !(rect.y > -1000f))
				{
					float width = num11 - num9;
					float height = num12 - num10;
					rect.x = num9;
					rect.y = num10;
					rect.width = width;
					rect.height = height;
				}
				else
				{
					float x = rect.x;
					float y = rect.y;
					float num14 = x + rect.width;
					float num15 = y + rect.height;
					float num16 = ((!(num9 < x)) ? x : num9);
					float num17 = ((!(num10 < y)) ? y : num10);
					float num18 = ((!(num11 > num14)) ? num14 : num11);
					float num19 = ((!(num12 > num15)) ? num15 : num12);
					rect.x = num16;
					rect.y = num17;
					rect.width = num18 - num16;
					rect.height = num19 - num17;
				}
				labelRects[num13] = rect;
				num7 += cellSizeX;
			}
			num8 += cellSizeY;
		}
	}

	private static MeshBuilder GetOrCreateMeshBuilder(int label, List<MeshBuilder> meshBuilders, MeshBuilder[] meshBuilderMap)
	{
		int count = meshBuilders.Count;
		MeshBuilder meshBuilder = meshBuilderMap[label];
		if (meshBuilder != null)
		{
			return meshBuilder;
		}
		for (int i = 0; i < count; i++)
		{
			meshBuilder = meshBuilders[i];
			if (meshBuilder.label == 0)
			{
				meshBuilder.label = label;
				meshBuilderMap[label] = meshBuilder;
				return meshBuilder;
			}
		}
		MeshBuilder meshBuilder2 = new MeshBuilder();
		meshBuilder2.label = label;
		meshBuilders.Add(meshBuilder2);
		meshBuilderMap[label] = meshBuilder2;
		return meshBuilder2;
	}

	private static void BuildPieceMeshes(float cellSizeX, float cellSizeY, bool[,] grid, int[,] labelGrid, List<MeshBuilder> meshBuilders, MeshBuilder[] meshBuilderMap)
	{
		ClearMeshBuilders(meshBuilders, meshBuilderMap);
		int length = grid.GetLength(0);
		int length2 = grid.GetLength(1);
		int num = length / 2;
		int num2 = length2 / 2;
		Vector3 item = new Vector3(0f, 0f, -1f);
		float num3 = cellSizeX * 0.5f;
		float num4 = cellSizeY * 0.5f;
		float num5 = cellSizeX * (float)num;
		float num6 = cellSizeY * (float)num2;
		float num7 = 1f / (float)length;
		float num8 = 1f / (float)length2;
		float num9 = num7 * 0.5f;
		float num10 = num8 * 0.5f;
		float num11 = 0f - num5;
		float num12 = 0f - num6;
		float num13 = 0f;
		float num14 = 0f;
		for (int i = 0; i < length2; i++)
		{
			for (int j = 0; j < length; j++)
			{
				float x = num11 + cellSizeX;
				float y = num12 + cellSizeY;
				float x2 = num13 + num7;
				float y2 = num14 + num8;
				if (grid[j, i])
				{
					MeshBuilder orCreateMeshBuilder = GetOrCreateMeshBuilder(labelGrid[j, i], meshBuilders, meshBuilderMap);
					int count = orCreateMeshBuilder.vertices.Count;
					orCreateMeshBuilder.vertices.Add(new Vector3(num11, num12));
					orCreateMeshBuilder.vertices.Add(new Vector3(num11, y));
					orCreateMeshBuilder.vertices.Add(new Vector3(x, y));
					orCreateMeshBuilder.vertices.Add(new Vector3(x, num12));
					orCreateMeshBuilder.normals.Add(item);
					orCreateMeshBuilder.normals.Add(item);
					orCreateMeshBuilder.normals.Add(item);
					orCreateMeshBuilder.normals.Add(item);
					orCreateMeshBuilder.uvs.Add(new Vector2(num13, num14));
					orCreateMeshBuilder.uvs.Add(new Vector2(num13, y2));
					orCreateMeshBuilder.uvs.Add(new Vector2(x2, y2));
					orCreateMeshBuilder.uvs.Add(new Vector2(x2, num14));
					orCreateMeshBuilder.triangles.Add(count);
					orCreateMeshBuilder.triangles.Add(count + 1);
					orCreateMeshBuilder.triangles.Add(count + 2);
					orCreateMeshBuilder.triangles.Add(count);
					orCreateMeshBuilder.triangles.Add(count + 2);
					orCreateMeshBuilder.triangles.Add(count + 3);
				}
				else
				{
					float x3 = num11 + num3;
					float y3 = num12 + num4;
					float x4 = num13 + num9;
					float y4 = num14 + num10;
					GetAdjascent(labelGrid, j, i, out var bottomLabel, out var topLabel, out var leftLabel, out var rightLabel, out var bottomLeftLabel, out var bottomRightLabel, out var topLeftLabel, out var topRightLabel);
					bool flag = leftLabel != 0;
					bool flag2 = rightLabel != 0;
					bool flag3 = topLabel != 0;
					bool flag4 = bottomLabel != 0;
					bool flag5 = topLeftLabel != 0;
					bool flag6 = topRightLabel != 0;
					bool flag7 = bottomRightLabel != 0;
					bool flag8 = bottomLeftLabel != 0;
					if (flag5)
					{
						if (flag || flag3)
						{
							MeshBuilder orCreateMeshBuilder2 = GetOrCreateMeshBuilder(topLeftLabel, meshBuilders, meshBuilderMap);
							int count2 = orCreateMeshBuilder2.vertices.Count;
							orCreateMeshBuilder2.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder2.vertices.Add(new Vector3(num11, y3));
							orCreateMeshBuilder2.vertices.Add(new Vector3(num11, y));
							orCreateMeshBuilder2.vertices.Add(new Vector3(x3, y));
							orCreateMeshBuilder2.normals.Add(item);
							orCreateMeshBuilder2.normals.Add(item);
							orCreateMeshBuilder2.normals.Add(item);
							orCreateMeshBuilder2.normals.Add(item);
							orCreateMeshBuilder2.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder2.uvs.Add(new Vector2(num13, y4));
							orCreateMeshBuilder2.uvs.Add(new Vector2(num13, y2));
							orCreateMeshBuilder2.uvs.Add(new Vector2(x4, y2));
							orCreateMeshBuilder2.triangles.Add(count2);
							orCreateMeshBuilder2.triangles.Add(count2 + 1);
							orCreateMeshBuilder2.triangles.Add(count2 + 2);
							orCreateMeshBuilder2.triangles.Add(count2);
							orCreateMeshBuilder2.triangles.Add(count2 + 2);
							orCreateMeshBuilder2.triangles.Add(count2 + 3);
						}
					}
					else
					{
						if (flag)
						{
							MeshBuilder orCreateMeshBuilder3 = GetOrCreateMeshBuilder(leftLabel, meshBuilders, meshBuilderMap);
							int count3 = orCreateMeshBuilder3.vertices.Count;
							orCreateMeshBuilder3.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder3.vertices.Add(new Vector3(num11, y3));
							orCreateMeshBuilder3.vertices.Add(new Vector3(num11, y));
							orCreateMeshBuilder3.normals.Add(item);
							orCreateMeshBuilder3.normals.Add(item);
							orCreateMeshBuilder3.normals.Add(item);
							orCreateMeshBuilder3.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder3.uvs.Add(new Vector2(num13, y4));
							orCreateMeshBuilder3.uvs.Add(new Vector2(num13, y2));
							orCreateMeshBuilder3.triangles.Add(count3);
							orCreateMeshBuilder3.triangles.Add(count3 + 1);
							orCreateMeshBuilder3.triangles.Add(count3 + 2);
						}
						if (flag3)
						{
							MeshBuilder orCreateMeshBuilder4 = GetOrCreateMeshBuilder(topLabel, meshBuilders, meshBuilderMap);
							int count4 = orCreateMeshBuilder4.vertices.Count;
							orCreateMeshBuilder4.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder4.vertices.Add(new Vector3(num11, y));
							orCreateMeshBuilder4.vertices.Add(new Vector3(x3, y));
							orCreateMeshBuilder4.normals.Add(item);
							orCreateMeshBuilder4.normals.Add(item);
							orCreateMeshBuilder4.normals.Add(item);
							orCreateMeshBuilder4.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder4.uvs.Add(new Vector2(num13, y2));
							orCreateMeshBuilder4.uvs.Add(new Vector2(x4, y2));
							orCreateMeshBuilder4.triangles.Add(count4);
							orCreateMeshBuilder4.triangles.Add(count4 + 1);
							orCreateMeshBuilder4.triangles.Add(count4 + 2);
						}
					}
					if (flag6)
					{
						if (flag3 || flag2)
						{
							MeshBuilder orCreateMeshBuilder5 = GetOrCreateMeshBuilder(topRightLabel, meshBuilders, meshBuilderMap);
							int count5 = orCreateMeshBuilder5.vertices.Count;
							orCreateMeshBuilder5.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder5.vertices.Add(new Vector3(x3, y));
							orCreateMeshBuilder5.vertices.Add(new Vector3(x, y));
							orCreateMeshBuilder5.vertices.Add(new Vector3(x, y3));
							orCreateMeshBuilder5.normals.Add(item);
							orCreateMeshBuilder5.normals.Add(item);
							orCreateMeshBuilder5.normals.Add(item);
							orCreateMeshBuilder5.normals.Add(item);
							orCreateMeshBuilder5.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder5.uvs.Add(new Vector2(x4, y2));
							orCreateMeshBuilder5.uvs.Add(new Vector2(x2, y2));
							orCreateMeshBuilder5.uvs.Add(new Vector2(x2, y4));
							orCreateMeshBuilder5.triangles.Add(count5);
							orCreateMeshBuilder5.triangles.Add(count5 + 1);
							orCreateMeshBuilder5.triangles.Add(count5 + 2);
							orCreateMeshBuilder5.triangles.Add(count5);
							orCreateMeshBuilder5.triangles.Add(count5 + 2);
							orCreateMeshBuilder5.triangles.Add(count5 + 3);
						}
					}
					else
					{
						if (flag3)
						{
							MeshBuilder orCreateMeshBuilder6 = GetOrCreateMeshBuilder(topLabel, meshBuilders, meshBuilderMap);
							int count6 = orCreateMeshBuilder6.vertices.Count;
							orCreateMeshBuilder6.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder6.vertices.Add(new Vector3(x3, y));
							orCreateMeshBuilder6.vertices.Add(new Vector3(x, y));
							orCreateMeshBuilder6.normals.Add(item);
							orCreateMeshBuilder6.normals.Add(item);
							orCreateMeshBuilder6.normals.Add(item);
							orCreateMeshBuilder6.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder6.uvs.Add(new Vector2(x4, y2));
							orCreateMeshBuilder6.uvs.Add(new Vector2(x2, y2));
							orCreateMeshBuilder6.triangles.Add(count6);
							orCreateMeshBuilder6.triangles.Add(count6 + 1);
							orCreateMeshBuilder6.triangles.Add(count6 + 2);
						}
						if (flag2)
						{
							MeshBuilder orCreateMeshBuilder7 = GetOrCreateMeshBuilder(rightLabel, meshBuilders, meshBuilderMap);
							int count7 = orCreateMeshBuilder7.vertices.Count;
							orCreateMeshBuilder7.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder7.vertices.Add(new Vector3(x, y));
							orCreateMeshBuilder7.vertices.Add(new Vector3(x, y3));
							orCreateMeshBuilder7.normals.Add(item);
							orCreateMeshBuilder7.normals.Add(item);
							orCreateMeshBuilder7.normals.Add(item);
							orCreateMeshBuilder7.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder7.uvs.Add(new Vector2(x2, y2));
							orCreateMeshBuilder7.uvs.Add(new Vector2(x2, y4));
							orCreateMeshBuilder7.triangles.Add(count7);
							orCreateMeshBuilder7.triangles.Add(count7 + 1);
							orCreateMeshBuilder7.triangles.Add(count7 + 2);
						}
					}
					if (flag7)
					{
						if (flag2 || flag4)
						{
							MeshBuilder orCreateMeshBuilder8 = GetOrCreateMeshBuilder(bottomRightLabel, meshBuilders, meshBuilderMap);
							int count8 = orCreateMeshBuilder8.vertices.Count;
							orCreateMeshBuilder8.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder8.vertices.Add(new Vector3(x, y3));
							orCreateMeshBuilder8.vertices.Add(new Vector3(x, num12));
							orCreateMeshBuilder8.vertices.Add(new Vector3(x3, num12));
							orCreateMeshBuilder8.normals.Add(item);
							orCreateMeshBuilder8.normals.Add(item);
							orCreateMeshBuilder8.normals.Add(item);
							orCreateMeshBuilder8.normals.Add(item);
							orCreateMeshBuilder8.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder8.uvs.Add(new Vector2(x2, y4));
							orCreateMeshBuilder8.uvs.Add(new Vector2(x2, num14));
							orCreateMeshBuilder8.uvs.Add(new Vector2(x4, num14));
							orCreateMeshBuilder8.triangles.Add(count8);
							orCreateMeshBuilder8.triangles.Add(count8 + 1);
							orCreateMeshBuilder8.triangles.Add(count8 + 2);
							orCreateMeshBuilder8.triangles.Add(count8);
							orCreateMeshBuilder8.triangles.Add(count8 + 2);
							orCreateMeshBuilder8.triangles.Add(count8 + 3);
						}
					}
					else
					{
						if (flag2)
						{
							MeshBuilder orCreateMeshBuilder9 = GetOrCreateMeshBuilder(rightLabel, meshBuilders, meshBuilderMap);
							int count9 = orCreateMeshBuilder9.vertices.Count;
							orCreateMeshBuilder9.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder9.vertices.Add(new Vector3(x, y3));
							orCreateMeshBuilder9.vertices.Add(new Vector3(x, num12));
							orCreateMeshBuilder9.normals.Add(item);
							orCreateMeshBuilder9.normals.Add(item);
							orCreateMeshBuilder9.normals.Add(item);
							orCreateMeshBuilder9.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder9.uvs.Add(new Vector2(x2, y4));
							orCreateMeshBuilder9.uvs.Add(new Vector2(x2, num14));
							orCreateMeshBuilder9.triangles.Add(count9);
							orCreateMeshBuilder9.triangles.Add(count9 + 1);
							orCreateMeshBuilder9.triangles.Add(count9 + 2);
						}
						if (flag4)
						{
							MeshBuilder orCreateMeshBuilder10 = GetOrCreateMeshBuilder(bottomLabel, meshBuilders, meshBuilderMap);
							int count10 = orCreateMeshBuilder10.vertices.Count;
							orCreateMeshBuilder10.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder10.vertices.Add(new Vector3(x, num12));
							orCreateMeshBuilder10.vertices.Add(new Vector3(x3, num12));
							orCreateMeshBuilder10.normals.Add(item);
							orCreateMeshBuilder10.normals.Add(item);
							orCreateMeshBuilder10.normals.Add(item);
							orCreateMeshBuilder10.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder10.uvs.Add(new Vector2(x2, num14));
							orCreateMeshBuilder10.uvs.Add(new Vector2(x4, num14));
							orCreateMeshBuilder10.triangles.Add(count10);
							orCreateMeshBuilder10.triangles.Add(count10 + 1);
							orCreateMeshBuilder10.triangles.Add(count10 + 2);
						}
					}
					if (flag8)
					{
						if (flag4 || flag)
						{
							MeshBuilder orCreateMeshBuilder11 = GetOrCreateMeshBuilder(bottomLeftLabel, meshBuilders, meshBuilderMap);
							int count11 = orCreateMeshBuilder11.vertices.Count;
							orCreateMeshBuilder11.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder11.vertices.Add(new Vector3(x3, num12));
							orCreateMeshBuilder11.vertices.Add(new Vector3(num11, num12));
							orCreateMeshBuilder11.vertices.Add(new Vector3(num11, y3));
							orCreateMeshBuilder11.normals.Add(item);
							orCreateMeshBuilder11.normals.Add(item);
							orCreateMeshBuilder11.normals.Add(item);
							orCreateMeshBuilder11.normals.Add(item);
							orCreateMeshBuilder11.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder11.uvs.Add(new Vector2(x4, num14));
							orCreateMeshBuilder11.uvs.Add(new Vector2(num13, num14));
							orCreateMeshBuilder11.uvs.Add(new Vector2(num13, y4));
							orCreateMeshBuilder11.triangles.Add(count11);
							orCreateMeshBuilder11.triangles.Add(count11 + 1);
							orCreateMeshBuilder11.triangles.Add(count11 + 2);
							orCreateMeshBuilder11.triangles.Add(count11);
							orCreateMeshBuilder11.triangles.Add(count11 + 2);
							orCreateMeshBuilder11.triangles.Add(count11 + 3);
						}
					}
					else
					{
						if (flag4)
						{
							MeshBuilder orCreateMeshBuilder12 = GetOrCreateMeshBuilder(bottomLabel, meshBuilders, meshBuilderMap);
							int count12 = orCreateMeshBuilder12.vertices.Count;
							orCreateMeshBuilder12.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder12.vertices.Add(new Vector3(x3, num12));
							orCreateMeshBuilder12.vertices.Add(new Vector3(num11, num12));
							orCreateMeshBuilder12.normals.Add(item);
							orCreateMeshBuilder12.normals.Add(item);
							orCreateMeshBuilder12.normals.Add(item);
							orCreateMeshBuilder12.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder12.uvs.Add(new Vector2(x4, num14));
							orCreateMeshBuilder12.uvs.Add(new Vector2(num13, num14));
							orCreateMeshBuilder12.triangles.Add(count12);
							orCreateMeshBuilder12.triangles.Add(count12 + 1);
							orCreateMeshBuilder12.triangles.Add(count12 + 2);
						}
						if (flag)
						{
							MeshBuilder orCreateMeshBuilder13 = GetOrCreateMeshBuilder(leftLabel, meshBuilders, meshBuilderMap);
							int count13 = orCreateMeshBuilder13.vertices.Count;
							orCreateMeshBuilder13.vertices.Add(new Vector3(x3, y3));
							orCreateMeshBuilder13.vertices.Add(new Vector3(num11, num12));
							orCreateMeshBuilder13.vertices.Add(new Vector3(num11, y3));
							orCreateMeshBuilder13.normals.Add(item);
							orCreateMeshBuilder13.normals.Add(item);
							orCreateMeshBuilder13.normals.Add(item);
							orCreateMeshBuilder13.uvs.Add(new Vector2(x4, y4));
							orCreateMeshBuilder13.uvs.Add(new Vector2(num13, num14));
							orCreateMeshBuilder13.uvs.Add(new Vector2(num13, y4));
							orCreateMeshBuilder13.triangles.Add(count13);
							orCreateMeshBuilder13.triangles.Add(count13 + 1);
							orCreateMeshBuilder13.triangles.Add(count13 + 2);
						}
					}
				}
				num11 += cellSizeX;
				num13 += num7;
			}
			num11 = 0f - num5;
			num13 = 0f;
			num12 += cellSizeY;
			num14 += num8;
		}
	}

	private static void BuildTearDecals(int gridSizeX, int gridSizeY, bool[,] stampMaskHorizontal, bool[,] stampMaskVertical, bool[,] stampMaskDiagonalLeft, bool[,] stampMaskDiagonalRight, bool[,] lastStampMaskHorizontal, bool[,] lastStampMaskVertical, bool[,] lastStampMaskDiagonalLeft, bool[,] lastStampMaskDiagonalRight, List<Vector3> tearDecalStart, List<Vector3> tearDecalEnd)
	{
		tearDecalStart.Clear();
		tearDecalEnd.Clear();
		for (int i = 0; i < stampMaskHorizontal.GetLength(1); i++)
		{
			float y = ((float)i + 0.5f) / (float)gridSizeY;
			for (int j = 0; j < stampMaskHorizontal.GetLength(0); j++)
			{
				if (stampMaskHorizontal[j, i] && !lastStampMaskHorizontal[j, i])
				{
					tearDecalStart.Add(new Vector2(((float)j - 0.5f) / (float)gridSizeX, y));
					tearDecalEnd.Add(new Vector2(((float)j + 0.5f) / (float)gridSizeX, y));
				}
			}
		}
		for (int k = 0; k < stampMaskVertical.GetLength(0); k++)
		{
			float x = ((float)k + 0.5f) / (float)gridSizeX;
			for (int l = 0; l < stampMaskVertical.GetLength(1); l++)
			{
				if (stampMaskVertical[k, l] && !lastStampMaskVertical[k, l])
				{
					tearDecalStart.Add(new Vector2(x, ((float)l - 0.5f) / (float)gridSizeY));
					tearDecalEnd.Add(new Vector2(x, ((float)l + 0.5f) / (float)gridSizeY));
				}
			}
		}
		for (int m = 0; m < stampMaskDiagonalLeft.GetLength(0); m++)
		{
			for (int n = 0; n < stampMaskDiagonalLeft.GetLength(1); n++)
			{
				if (stampMaskDiagonalLeft[m, n] && !lastStampMaskDiagonalLeft[m, n])
				{
					tearDecalStart.Add(new Vector2(((float)m + 0.5f) / (float)gridSizeX, ((float)n - 0.5f) / (float)gridSizeY));
					tearDecalEnd.Add(new Vector2(((float)m - 0.5f) / (float)gridSizeX, ((float)n + 0.5f) / (float)gridSizeY));
				}
				if (stampMaskDiagonalRight[m, n] && !lastStampMaskDiagonalRight[m, n])
				{
					tearDecalStart.Add(new Vector2(((float)m + 0.5f) / (float)gridSizeX, ((float)n + 0.5f) / (float)gridSizeY));
					tearDecalEnd.Add(new Vector2(((float)m - 0.5f) / (float)gridSizeX, ((float)n - 0.5f) / (float)gridSizeY));
				}
			}
		}
	}

	private static void BuildAttachedLabels(int[,] labelGrid, List<int> attachedLabels)
	{
		attachedLabels.Clear();
		int length = labelGrid.GetLength(0);
		int length2 = labelGrid.GetLength(1);
		int num = length2 - 1;
		int num2 = 0;
		for (int i = 0; i < length; i++)
		{
			int num3 = labelGrid[i, num];
			if (num2 == 0 && num3 != 0 && !attachedLabels.Contains(num3))
			{
				attachedLabels.Add(num3);
			}
			num2 = num3;
		}
	}
}
