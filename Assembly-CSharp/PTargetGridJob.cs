// Decompiled with JetBrains decompiler
// Type: PTargetGridJob
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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

  public PTargetGridJob(int gridSizeX, int gridSizeY, float targetSizeX, float targetSizeY) => this.SetGridData(gridSizeX, gridSizeY, targetSizeX, targetSizeY);

  public void Resize<T>(ref T[] array, int length)
  {
    if (array != null && array.Length == length)
      return;
    array = new T[length];
  }

  public void Resize<T>(ref T[,] array, int width, int height)
  {
    if (array != null && array.GetLength(0) == width && array.GetLength(1) == height)
      return;
    array = new T[width, height];
  }

  public void Resize<T>(ref T[,,] array, int width, int height, int depth)
  {
    if (array != null && array.GetLength(0) == width && (array.GetLength(1) == height && array.GetLength(2) == depth))
      return;
    array = new T[width, height, depth];
  }

  public void SetGridData(int gridSizeX, int gridSizeY, float targetSizeX, float targetSizeY)
  {
    this.gridSizeX = gridSizeX;
    this.gridSizeY = gridSizeY;
    this.targetSizeX = targetSizeX;
    this.targetSizeY = targetSizeY;
    this.maxLabels = gridSizeX * (gridSizeY / 2) + 1;
    this.cellSizeX = targetSizeX / (float) gridSizeX;
    this.cellSizeY = targetSizeY / (float) gridSizeY;
    if (this.grid == null || this.grid.GetLength(0) != gridSizeX || this.grid.GetLength(1) != gridSizeY)
      this.grid = new bool[gridSizeX, gridSizeY];
    this.Resize<bool>(ref this.edgeGrid, gridSizeX + 2, gridSizeY + 2, 8);
    this.Resize<bool>(ref this.lastStampMaskHorizontal, gridSizeX + 1, gridSizeY);
    this.Resize<bool>(ref this.lastStampMaskVertical, gridSizeX, gridSizeY + 1);
    this.Resize<bool>(ref this.lastStampMaskDiagonalLeft, gridSizeX + 1, gridSizeY + 1);
    this.Resize<bool>(ref this.lastStampMaskDiagonalRight, gridSizeX + 1, gridSizeY + 1);
    this.Resize<bool>(ref this.stampMaskHorizontal, gridSizeX + 1, gridSizeY);
    this.Resize<bool>(ref this.stampMaskVertical, gridSizeX, gridSizeY + 1);
    this.Resize<bool>(ref this.stampMaskDiagonalLeft, gridSizeX + 1, gridSizeY + 1);
    this.Resize<bool>(ref this.stampMaskDiagonalRight, gridSizeX + 1, gridSizeY + 1);
    if (this.conflictTable.parent == null || this.conflictTable.rank == null || (this.conflictTable.parent.Length != this.maxLabels || this.conflictTable.rank.Length != this.maxLabels))
      this.conflictTable = new DisjointSet(this.maxLabels);
    this.Resize<Rect>(ref this.labelRects, this.maxLabels);
    this.Resize<int>(ref this.differenceRemapping, this.maxLabels);
    if (this.newLabelsBackBuffer == null)
      this.newLabelsBackBuffer = new List<int>();
    if (this.newLabelsFrontBuffer == null)
      this.newLabelsFrontBuffer = new List<int>();
    if (this.tearDecalStart == null)
      this.tearDecalStart = new List<Vector3>(8);
    if (this.tearDecalEnd == null)
      this.tearDecalEnd = new List<Vector3>(8);
    this.Resize<int>(ref this.lastLabelGrid, gridSizeX, gridSizeY);
    this.Resize<int>(ref this.labelGrid, gridSizeX, gridSizeY);
    if (this.attachedLabels == null)
      this.attachedLabels = new List<int>(8);
    if (this.meshBuilders == null)
      this.meshBuilders = new List<MeshBuilder>(128);
    this.Resize<MeshBuilder>(ref this.meshBuilderMap, this.maxLabels);
  }

  protected override void OnStart()
  {
  }

  public override void ClearData()
  {
    PTargetGridJob.ClearGrid<bool>(this.grid, true);
    PTargetGridJob.ClearGrid<bool>(this.edgeGrid, false);
    PTargetGridJob.ClearGrid<bool>(this.lastStampMaskHorizontal, false);
    PTargetGridJob.ClearGrid<bool>(this.lastStampMaskVertical, false);
    PTargetGridJob.ClearGrid<bool>(this.lastStampMaskDiagonalLeft, false);
    PTargetGridJob.ClearGrid<bool>(this.lastStampMaskDiagonalRight, false);
    PTargetGridJob.ClearGrid<bool>(this.stampMaskHorizontal, false);
    PTargetGridJob.ClearGrid<bool>(this.stampMaskVertical, false);
    PTargetGridJob.ClearGrid<bool>(this.stampMaskDiagonalLeft, false);
    PTargetGridJob.ClearGrid<bool>(this.stampMaskDiagonalRight, false);
    this.conflictTable.Clear();
    PTargetGridJob.ClearArray<Rect>(this.labelRects, new Rect(float.MinValue, float.MinValue, 0.0f, 0.0f));
    PTargetGridJob.ClearDifferenceRemapping(this.differenceRemapping);
    this.newLabelsBackBuffer.Clear();
    this.newLabelsFrontBuffer.Clear();
    this.tearDecalStart.Clear();
    this.tearDecalEnd.Clear();
    PTargetGridJob.ClearGrid<int>(this.lastLabelGrid, 1);
    PTargetGridJob.ClearGrid<int>(this.labelGrid, 1);
    this.attachedLabels.Clear();
    PTargetGridJob.ClearMeshBuilders(this.meshBuilders, this.meshBuilderMap);
  }

  protected override void OnSchedule()
  {
    PTargetGridJob.SwapBackbuffer<bool>(ref this.lastStampMaskHorizontal, ref this.stampMaskHorizontal);
    PTargetGridJob.SwapBackbuffer<bool>(ref this.lastStampMaskVertical, ref this.stampMaskVertical);
    PTargetGridJob.SwapBackbuffer<bool>(ref this.lastStampMaskDiagonalLeft, ref this.stampMaskDiagonalLeft);
    PTargetGridJob.SwapBackbuffer<bool>(ref this.lastStampMaskDiagonalRight, ref this.stampMaskDiagonalRight);
    PTargetGridJob.SwapBackbuffer<int>(ref this.lastLabelGrid, ref this.labelGrid);
  }

  protected override void OnExecute()
  {
    PTargetGridJob.BuildEdgeGrid(this.grid, this.edgeGrid);
    PTargetGridJob.BuildStampMask(this.edgeGrid, this.stampMaskHorizontal, this.stampMaskVertical, this.stampMaskDiagonalLeft, this.stampMaskDiagonalRight);
    PTargetGridJob.ConnectedComponents(this.grid, this.labelGrid, this.conflictTable);
    PTargetGridJob.BuildLabelDifferances(this.lastLabelGrid, this.labelGrid, this.differenceRemapping, this.newLabelsFrontBuffer, this.newLabelsBackBuffer);
    PTargetGridJob.BuildLabelRects(this.cellSizeX, this.cellSizeY, this.labelGrid, this.labelRects);
    PTargetGridJob.BuildPieceMeshes(this.cellSizeX, this.cellSizeY, this.grid, this.labelGrid, this.meshBuilders, this.meshBuilderMap);
    PTargetGridJob.BuildTearDecals(this.gridSizeX, this.gridSizeY, this.stampMaskHorizontal, this.stampMaskVertical, this.stampMaskDiagonalLeft, this.stampMaskDiagonalRight, this.lastStampMaskHorizontal, this.lastStampMaskVertical, this.lastStampMaskDiagonalLeft, this.lastStampMaskDiagonalRight, this.tearDecalStart, this.tearDecalEnd);
    PTargetGridJob.BuildAttachedLabels(this.labelGrid, this.attachedLabels);
  }

  private static void ClearArray<T>(T[] array, T value)
  {
    for (int index = 0; index < array.Length; ++index)
      array[index] = value;
  }

  private static void ClearGrid<T>(T[,] grid, T value)
  {
    for (int index1 = 0; index1 < grid.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < grid.GetLength(0); ++index2)
        grid[index2, index1] = value;
    }
  }

  private static void ClearGrid<T>(T[,,] grid, T value)
  {
    for (int index1 = 0; index1 < grid.GetLength(2); ++index1)
    {
      for (int index2 = 0; index2 < grid.GetLength(1); ++index2)
      {
        for (int index3 = 0; index3 < grid.GetLength(0); ++index3)
          grid[index3, index2, index1] = value;
      }
    }
  }

  private static void ClearMeshBuilders(
    List<MeshBuilder> meshBuilders,
    MeshBuilder[] meshBuilderMap)
  {
    int count = meshBuilders.Count;
    for (int index = 0; index < count; ++index)
      meshBuilders[index].Clear();
    for (int index = 0; index < meshBuilderMap.Length; ++index)
      meshBuilderMap[index] = (MeshBuilder) null;
  }

  private static void ClearDifferenceRemapping(int[] differenceRemapping)
  {
    for (int index = 0; index < differenceRemapping.Length; ++index)
      differenceRemapping[index] = 0;
    differenceRemapping[1] = 1;
  }

  private static void GetAdjascent(
    int[,] labelGrid,
    int x,
    int y,
    out int bottomLabel,
    out int topLabel,
    out int leftLabel,
    out int rightLabel,
    out int bottomLeftLabel,
    out int bottomRightLabel,
    out int topLeftLabel,
    out int topRightLabel)
  {
    int length1 = labelGrid.GetLength(0);
    int length2 = labelGrid.GetLength(1);
    int num1 = length1 - 1;
    int num2 = length2 - 1;
    int num3 = x - 1;
    int num4 = x + 1;
    int num5 = y - 1;
    int num6 = y + 1;
    bottomLabel = labelGrid[x >= 0 ? (x <= num1 ? x : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0];
    topLabel = labelGrid[x >= 0 ? (x <= num1 ? x : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0];
    leftLabel = labelGrid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, y >= 0 ? (y <= num2 ? y : num2) : 0];
    rightLabel = labelGrid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, y >= 0 ? (y <= num2 ? y : num2) : 0];
    bottomLeftLabel = labelGrid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0];
    bottomRightLabel = labelGrid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0];
    topLeftLabel = labelGrid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0];
    topRightLabel = labelGrid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0];
  }

  private static void GetAdjascent(
    int[,] labelGrid,
    int x,
    int y,
    int label,
    out bool bottomLabel,
    out bool topLabel,
    out bool leftLabel,
    out bool rightLabel,
    out bool bottomLeftLabel,
    out bool bottomRightLabel,
    out bool topLeftLabel,
    out bool topRightLabel)
  {
    int length1 = labelGrid.GetLength(0);
    int length2 = labelGrid.GetLength(1);
    int num1 = length1 - 1;
    int num2 = length2 - 1;
    int num3 = x - 1;
    int num4 = x + 1;
    int num5 = y - 1;
    int num6 = y + 1;
    bottomLabel = labelGrid[x >= 0 ? (x <= num1 ? x : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0] == label;
    topLabel = labelGrid[x >= 0 ? (x <= num1 ? x : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0] == label;
    leftLabel = labelGrid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, y >= 0 ? (y <= num2 ? y : num2) : 0] == label;
    rightLabel = labelGrid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, y >= 0 ? (y <= num2 ? y : num2) : 0] == label;
    bottomLeftLabel = labelGrid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0] == label;
    bottomRightLabel = labelGrid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0] == label;
    topLeftLabel = labelGrid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0] == label;
    topRightLabel = labelGrid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0] == label;
  }

  private static void GetAdjascent(
    bool[,] grid,
    int x,
    int y,
    out bool bottomLabel,
    out bool topLabel,
    out bool leftLabel,
    out bool rightLabel,
    out bool bottomLeftLabel,
    out bool bottomRightLabel,
    out bool topLeftLabel,
    out bool topRightLabel)
  {
    int length1 = grid.GetLength(0);
    int length2 = grid.GetLength(1);
    int num1 = length1 - 1;
    int num2 = length2 - 1;
    int num3 = x - 1;
    int num4 = x + 1;
    int num5 = y - 1;
    int num6 = y + 1;
    bottomLabel = grid[x >= 0 ? (x <= num1 ? x : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0];
    topLabel = grid[x >= 0 ? (x <= num1 ? x : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0];
    leftLabel = grid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, y >= 0 ? (y <= num2 ? y : num2) : 0];
    rightLabel = grid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, y >= 0 ? (y <= num2 ? y : num2) : 0];
    bottomLeftLabel = grid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0];
    bottomRightLabel = grid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, num5 >= 0 ? (num5 <= num2 ? num5 : num2) : 0];
    topLeftLabel = grid[num3 >= 0 ? (num3 <= num1 ? num3 : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0];
    topRightLabel = grid[num4 >= 0 ? (num4 <= num1 ? num4 : num1) : 0, num6 >= 0 ? (num6 <= num2 ? num6 : num2) : 0];
  }

  private static void BuildCornerEdges(
    bool corner,
    bool left,
    bool right,
    ref bool cornerOutput,
    ref bool leftOutput,
    ref bool rightOutput)
  {
    rightOutput = ((rightOutput ? 1 : 0) | (!corner || !left ? 0 : (!right ? 1 : 0))) != 0;
    leftOutput = ((leftOutput ? 1 : 0) | (!corner || left ? 0 : (right ? 1 : 0))) != 0;
    cornerOutput = ((cornerOutput ? 1 : 0) | (corner ? 0 : (left ? 1 : (right ? 1 : 0)))) != 0;
  }

  private static void BuildEdgeGrid(bool[,] grid, bool[,,] edgeGrid)
  {
    int length1 = grid.GetLength(0);
    int length2 = grid.GetLength(1);
    int length3 = edgeGrid.GetLength(0);
    int length4 = edgeGrid.GetLength(1);
    for (int index1 = 0; index1 < length4; ++index1)
    {
      for (int index2 = 0; index2 < length3; ++index2)
      {
        for (int index3 = 0; index3 < 8; ++index3)
          edgeGrid[index2, index1, index3] = false;
        if (!grid[Mathf.Clamp(index2 - 1, 0, length1 - 1), Mathf.Clamp(index1 - 1, 0, length2 - 1)])
        {
          bool bottomLabel;
          bool topLabel;
          bool leftLabel;
          bool rightLabel;
          bool bottomLeftLabel;
          bool bottomRightLabel;
          bool topLeftLabel;
          bool topRightLabel;
          PTargetGridJob.GetAdjascent(grid, index2 - 1, index1 - 1, out bottomLabel, out topLabel, out leftLabel, out rightLabel, out bottomLeftLabel, out bottomRightLabel, out topLeftLabel, out topRightLabel);
          PTargetGridJob.BuildCornerEdges(topLeftLabel, leftLabel, topLabel, ref edgeGrid.Address(index2, index1, 7), ref edgeGrid.Address(index2, index1, 6), ref edgeGrid.Address(index2, index1, 0));
          PTargetGridJob.BuildCornerEdges(topRightLabel, topLabel, rightLabel, ref edgeGrid.Address(index2, index1, 1), ref edgeGrid.Address(index2, index1, 0), ref edgeGrid.Address(index2, index1, 2));
          PTargetGridJob.BuildCornerEdges(bottomRightLabel, rightLabel, bottomLabel, ref edgeGrid.Address(index2, index1, 3), ref edgeGrid.Address(index2, index1, 2), ref edgeGrid.Address(index2, index1, 4));
          PTargetGridJob.BuildCornerEdges(bottomLeftLabel, bottomLabel, leftLabel, ref edgeGrid.Address(index2, index1, 5), ref edgeGrid.Address(index2, index1, 4), ref edgeGrid.Address(index2, index1, 6));
        }
      }
    }
  }

  private static void SwapBackbuffer<T>(ref T[,] backBuffer, ref T[,] frontBuffer)
  {
    T[,] objArray = backBuffer;
    backBuffer = frontBuffer;
    frontBuffer = objArray;
  }

  private static void BuildStampMask(
    bool[,,] edgeGrid,
    bool[,] stampMaskHorizontal,
    bool[,] stampMaskVertical,
    bool[,] stampMaskDiagonalLeft,
    bool[,] stampMaskDiagonalRight)
  {
    int length1 = edgeGrid.GetLength(0);
    int length2 = edgeGrid.GetLength(1);
    for (int index1 = 1; index1 < length2 - 1; ++index1)
    {
      for (int index2 = 0; index2 < length1 - 1; ++index2)
        stampMaskHorizontal[index2, index1 - 1] = edgeGrid[Mathf.Clamp(index2, 0, length1 - 1), Mathf.Clamp(index1, 0, length2 - 1), 2] || edgeGrid[Mathf.Clamp(index2 + 1, 0, length1 - 1), Mathf.Clamp(index1, 0, length2 - 1), 6];
    }
    for (int index1 = 0; index1 < length2 - 1; ++index1)
    {
      for (int index2 = 1; index2 < length1 - 1; ++index2)
        stampMaskVertical[index2 - 1, index1] = edgeGrid[Mathf.Clamp(index2, 0, length1 - 1), Mathf.Clamp(index1, 0, length2 - 1), 0] || edgeGrid[Mathf.Clamp(index2, 0, length1 - 1), Mathf.Clamp(index1 + 1, 0, length2 - 1), 4];
    }
    for (int index1 = 0; index1 < length2 - 1; ++index1)
    {
      for (int index2 = 0; index2 < length1 - 1; ++index2)
      {
        stampMaskDiagonalRight[index2, index1] = edgeGrid[Mathf.Clamp(index2, 0, length1 - 1), Mathf.Clamp(index1, 0, length2 - 1), 1] || edgeGrid[Mathf.Clamp(index2 + 1, 0, length1 - 1), Mathf.Clamp(index1 + 1, 0, length2 - 1), 5];
        stampMaskDiagonalLeft[index2, index1] = edgeGrid[Mathf.Clamp(index2, 0, length1 - 1), Mathf.Clamp(index1 + 1, 0, length2 - 1), 3] || edgeGrid[Mathf.Clamp(index2 + 1, 0, length1 - 1), Mathf.Clamp(index1, 0, length2 - 1), 7];
      }
    }
  }

  private static void ConnectedComponents(bool[,] grid, int[,] labels, DisjointSet conflicts)
  {
    int length1 = grid.GetLength(0);
    int length2 = grid.GetLength(1);
    for (int index1 = 0; index1 < length2; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
        labels[index2, index1] = 0;
    }
    conflicts.Clear();
    int num = 1;
    if (grid[0, 0])
      labels[0, 0] = num++;
    for (int index = 1; index < length1; ++index)
    {
      if (grid[index, 0])
      {
        bool flag = grid[index - 1, 0];
        int label = labels[index - 1, 0];
        labels[index, 0] = !flag ? num++ : label;
      }
    }
    for (int index1 = 1; index1 < length2; ++index1)
    {
      if (grid[0, index1])
      {
        bool flag = grid[0, index1 - 1];
        int label = labels[0, index1 - 1];
        labels[0, index1] = !flag ? num++ : label;
      }
      for (int index2 = 1; index2 < length1; ++index2)
      {
        if (grid[index2, index1])
        {
          bool flag1 = grid[index2, index1 - 1];
          int label1 = labels[index2, index1 - 1];
          bool flag2 = grid[index2 - 1, index1];
          int label2 = labels[index2 - 1, index1];
          if (flag2 && !flag1)
            labels[index2, index1] = label2;
          else if (!flag2 && flag1)
            labels[index2, index1] = label1;
          else if (flag2 && flag1)
          {
            int x = Mathf.Min(label2, label1);
            int y = Mathf.Max(label2, label1);
            labels[index2, index1] = x;
            conflicts.Union(x, y);
          }
          else
            labels[index2, index1] = num++;
        }
      }
    }
    for (int index1 = 0; index1 < length2; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
        labels[index2, index1] = conflicts.Find(labels[index2, index1]);
    }
  }

  private static void BuildLabelDifferances(
    int[,] backBuffer,
    int[,] frontBuffer,
    int[] differenceRemapping,
    List<int> newLabelsFrontBuffer,
    List<int> newLabelsBackBuffer)
  {
    for (int index = 0; index < differenceRemapping.Length; ++index)
      differenceRemapping[index] = 0;
    newLabelsFrontBuffer.Clear();
    newLabelsBackBuffer.Clear();
    for (int index1 = 0; index1 < backBuffer.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < backBuffer.GetLength(0); ++index2)
      {
        int index3 = backBuffer[index2, index1];
        int num1 = frontBuffer[index2, index1];
        if (index3 != 0 && num1 != 0)
        {
          int num2 = differenceRemapping[index3];
          if (num2 == 0)
            differenceRemapping[index3] = num1;
          else if (num2 != num1 && !newLabelsFrontBuffer.Contains(num1))
          {
            newLabelsBackBuffer.Add(index3);
            newLabelsFrontBuffer.Add(num1);
          }
        }
      }
    }
  }

  private static void BuildLabelRects(
    float cellSizeX,
    float cellSizeY,
    int[,] labelGrid,
    Rect[] labelRects)
  {
    for (int index = 0; index < labelRects.Length; ++index)
      labelRects[index] = new Rect(float.MinValue, float.MinValue, 0.0f, 0.0f);
    int length1 = labelGrid.GetLength(0);
    int length2 = labelGrid.GetLength(1);
    int num1 = length1 / 2;
    int num2 = length2 / 2;
    float num3 = cellSizeX;
    float num4 = cellSizeY;
    float num5 = num3 * 0.5f;
    float num6 = num4 * 0.5f;
    float num7 = num3 * (float) num1;
    float num8 = num4 * (float) num2;
    float num9 = -num7 + num5;
    float num10 = -num8 + num5;
    for (int index1 = 0; index1 < length2; ++index1)
    {
      float num11 = -num7 + num5;
      for (int index2 = 0; index2 < length1; ++index2)
      {
        float num12 = num11 - num5;
        float num13 = num10 - num6;
        float num14 = num11 + num5;
        float num15 = num10 + num6;
        int index3 = labelGrid[index2, index1];
        Rect labelRect = labelRects[index3];
        if ((double) labelRect.x <= -1000.0 && (double) labelRect.y <= -1000.0)
        {
          float num16 = num14 - num12;
          float num17 = num15 - num13;
          labelRect.x = num12;
          labelRect.y = num13;
          labelRect.width = num16;
          labelRect.height = num17;
        }
        else
        {
          float x = labelRect.x;
          float y = labelRect.y;
          float num16 = x + labelRect.width;
          float num17 = y + labelRect.height;
          float num18 = (double) num12 >= (double) x ? x : num12;
          float num19 = (double) num13 >= (double) y ? y : num13;
          float num20 = (double) num14 <= (double) num16 ? num16 : num14;
          float num21 = (double) num15 <= (double) num17 ? num17 : num15;
          labelRect.x = num18;
          labelRect.y = num19;
          labelRect.width = num20 - num18;
          labelRect.height = num21 - num19;
        }
        labelRects[index3] = labelRect;
        num11 += num3;
      }
      num10 += num4;
    }
  }

  private static MeshBuilder GetOrCreateMeshBuilder(
    int label,
    List<MeshBuilder> meshBuilders,
    MeshBuilder[] meshBuilderMap)
  {
    int count = meshBuilders.Count;
    MeshBuilder meshBuilder1 = meshBuilderMap[label];
    if (meshBuilder1 != null)
      return meshBuilder1;
    for (int index = 0; index < count; ++index)
    {
      MeshBuilder meshBuilder2 = meshBuilders[index];
      if (meshBuilder2.label == 0)
      {
        meshBuilder2.label = label;
        meshBuilderMap[label] = meshBuilder2;
        return meshBuilder2;
      }
    }
    MeshBuilder meshBuilder3 = new MeshBuilder();
    meshBuilder3.label = label;
    meshBuilders.Add(meshBuilder3);
    meshBuilderMap[label] = meshBuilder3;
    return meshBuilder3;
  }

  private static void BuildPieceMeshes(
    float cellSizeX,
    float cellSizeY,
    bool[,] grid,
    int[,] labelGrid,
    List<MeshBuilder> meshBuilders,
    MeshBuilder[] meshBuilderMap)
  {
    PTargetGridJob.ClearMeshBuilders(meshBuilders, meshBuilderMap);
    int length1 = grid.GetLength(0);
    int length2 = grid.GetLength(1);
    int num1 = length1 / 2;
    int num2 = length2 / 2;
    Vector3 vector3 = new Vector3(0.0f, 0.0f, -1f);
    float num3 = cellSizeX;
    float num4 = cellSizeY;
    float num5 = num3 * 0.5f;
    float num6 = num4 * 0.5f;
    float num7 = num3 * (float) num1;
    float num8 = num4 * (float) num2;
    float num9 = 1f / (float) length1;
    float num10 = 1f / (float) length2;
    float num11 = num9 * 0.5f;
    float num12 = num10 * 0.5f;
    float x1 = -num7;
    float y1 = -num8;
    float x2 = 0.0f;
    float y2 = 0.0f;
    for (int y3 = 0; y3 < length2; ++y3)
    {
      for (int x3 = 0; x3 < length1; ++x3)
      {
        float x4 = x1 + num3;
        float y4 = y1 + num4;
        float x5 = x2 + num9;
        float y5 = y2 + num10;
        if (grid[x3, y3])
        {
          MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(labelGrid[x3, y3], meshBuilders, meshBuilderMap);
          int count = meshBuilder.vertices.Count;
          meshBuilder.vertices.Add(new Vector3(x1, y1));
          meshBuilder.vertices.Add(new Vector3(x1, y4));
          meshBuilder.vertices.Add(new Vector3(x4, y4));
          meshBuilder.vertices.Add(new Vector3(x4, y1));
          meshBuilder.normals.Add(vector3);
          meshBuilder.normals.Add(vector3);
          meshBuilder.normals.Add(vector3);
          meshBuilder.normals.Add(vector3);
          meshBuilder.uvs.Add(new Vector2(x2, y2));
          meshBuilder.uvs.Add(new Vector2(x2, y5));
          meshBuilder.uvs.Add(new Vector2(x5, y5));
          meshBuilder.uvs.Add(new Vector2(x5, y2));
          meshBuilder.triangles.Add(count);
          meshBuilder.triangles.Add(count + 1);
          meshBuilder.triangles.Add(count + 2);
          meshBuilder.triangles.Add(count);
          meshBuilder.triangles.Add(count + 2);
          meshBuilder.triangles.Add(count + 3);
        }
        else
        {
          float x6 = x1 + num5;
          float y6 = y1 + num6;
          float x7 = x2 + num11;
          float y7 = y2 + num12;
          int bottomLabel;
          int topLabel;
          int leftLabel;
          int rightLabel;
          int bottomLeftLabel;
          int bottomRightLabel;
          int topLeftLabel;
          int topRightLabel;
          PTargetGridJob.GetAdjascent(labelGrid, x3, y3, out bottomLabel, out topLabel, out leftLabel, out rightLabel, out bottomLeftLabel, out bottomRightLabel, out topLeftLabel, out topRightLabel);
          bool flag1 = leftLabel != 0;
          bool flag2 = rightLabel != 0;
          bool flag3 = topLabel != 0;
          bool flag4 = bottomLabel != 0;
          bool flag5 = topLeftLabel != 0;
          bool flag6 = topRightLabel != 0;
          bool flag7 = bottomRightLabel != 0;
          bool flag8 = bottomLeftLabel != 0;
          if (flag5)
          {
            if (flag1 || flag3)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(topLeftLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x1, y6));
              meshBuilder.vertices.Add(new Vector3(x1, y4));
              meshBuilder.vertices.Add(new Vector3(x6, y4));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x2, y7));
              meshBuilder.uvs.Add(new Vector2(x2, y5));
              meshBuilder.uvs.Add(new Vector2(x7, y5));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count + 3);
            }
          }
          else
          {
            if (flag1)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(leftLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x1, y6));
              meshBuilder.vertices.Add(new Vector3(x1, y4));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x2, y7));
              meshBuilder.uvs.Add(new Vector2(x2, y5));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
            if (flag3)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(topLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x1, y4));
              meshBuilder.vertices.Add(new Vector3(x6, y4));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x2, y5));
              meshBuilder.uvs.Add(new Vector2(x7, y5));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
          }
          if (flag6)
          {
            if (flag3 || flag2)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(topRightLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x6, y4));
              meshBuilder.vertices.Add(new Vector3(x4, y4));
              meshBuilder.vertices.Add(new Vector3(x4, y6));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x7, y5));
              meshBuilder.uvs.Add(new Vector2(x5, y5));
              meshBuilder.uvs.Add(new Vector2(x5, y7));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count + 3);
            }
          }
          else
          {
            if (flag3)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(topLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x6, y4));
              meshBuilder.vertices.Add(new Vector3(x4, y4));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x7, y5));
              meshBuilder.uvs.Add(new Vector2(x5, y5));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
            if (flag2)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(rightLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x4, y4));
              meshBuilder.vertices.Add(new Vector3(x4, y6));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x5, y5));
              meshBuilder.uvs.Add(new Vector2(x5, y7));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
          }
          if (flag7)
          {
            if (flag2 || flag4)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(bottomRightLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x4, y6));
              meshBuilder.vertices.Add(new Vector3(x4, y1));
              meshBuilder.vertices.Add(new Vector3(x6, y1));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x5, y7));
              meshBuilder.uvs.Add(new Vector2(x5, y2));
              meshBuilder.uvs.Add(new Vector2(x7, y2));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count + 3);
            }
          }
          else
          {
            if (flag2)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(rightLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x4, y6));
              meshBuilder.vertices.Add(new Vector3(x4, y1));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x5, y7));
              meshBuilder.uvs.Add(new Vector2(x5, y2));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
            if (flag4)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(bottomLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x4, y1));
              meshBuilder.vertices.Add(new Vector3(x6, y1));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x5, y2));
              meshBuilder.uvs.Add(new Vector2(x7, y2));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
          }
          if (flag8)
          {
            if (flag4 || flag1)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(bottomLeftLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x6, y1));
              meshBuilder.vertices.Add(new Vector3(x1, y1));
              meshBuilder.vertices.Add(new Vector3(x1, y6));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x7, y2));
              meshBuilder.uvs.Add(new Vector2(x2, y2));
              meshBuilder.uvs.Add(new Vector2(x2, y7));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 2);
              meshBuilder.triangles.Add(count + 3);
            }
          }
          else
          {
            if (flag4)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(bottomLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x6, y1));
              meshBuilder.vertices.Add(new Vector3(x1, y1));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x7, y2));
              meshBuilder.uvs.Add(new Vector2(x2, y2));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
            if (flag1)
            {
              MeshBuilder meshBuilder = PTargetGridJob.GetOrCreateMeshBuilder(leftLabel, meshBuilders, meshBuilderMap);
              int count = meshBuilder.vertices.Count;
              meshBuilder.vertices.Add(new Vector3(x6, y6));
              meshBuilder.vertices.Add(new Vector3(x1, y1));
              meshBuilder.vertices.Add(new Vector3(x1, y6));
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.normals.Add(vector3);
              meshBuilder.uvs.Add(new Vector2(x7, y7));
              meshBuilder.uvs.Add(new Vector2(x2, y2));
              meshBuilder.uvs.Add(new Vector2(x2, y7));
              meshBuilder.triangles.Add(count);
              meshBuilder.triangles.Add(count + 1);
              meshBuilder.triangles.Add(count + 2);
            }
          }
        }
        x1 += num3;
        x2 += num9;
      }
      x1 = -num7;
      x2 = 0.0f;
      y1 += num4;
      y2 += num10;
    }
  }

  private static void BuildTearDecals(
    int gridSizeX,
    int gridSizeY,
    bool[,] stampMaskHorizontal,
    bool[,] stampMaskVertical,
    bool[,] stampMaskDiagonalLeft,
    bool[,] stampMaskDiagonalRight,
    bool[,] lastStampMaskHorizontal,
    bool[,] lastStampMaskVertical,
    bool[,] lastStampMaskDiagonalLeft,
    bool[,] lastStampMaskDiagonalRight,
    List<Vector3> tearDecalStart,
    List<Vector3> tearDecalEnd)
  {
    tearDecalStart.Clear();
    tearDecalEnd.Clear();
    for (int index1 = 0; index1 < stampMaskHorizontal.GetLength(1); ++index1)
    {
      float y = ((float) index1 + 0.5f) / (float) gridSizeY;
      for (int index2 = 0; index2 < stampMaskHorizontal.GetLength(0); ++index2)
      {
        if (stampMaskHorizontal[index2, index1] && !lastStampMaskHorizontal[index2, index1])
        {
          tearDecalStart.Add((Vector3) new Vector2(((float) index2 - 0.5f) / (float) gridSizeX, y));
          tearDecalEnd.Add((Vector3) new Vector2(((float) index2 + 0.5f) / (float) gridSizeX, y));
        }
      }
    }
    for (int index1 = 0; index1 < stampMaskVertical.GetLength(0); ++index1)
    {
      float x = ((float) index1 + 0.5f) / (float) gridSizeX;
      for (int index2 = 0; index2 < stampMaskVertical.GetLength(1); ++index2)
      {
        if (stampMaskVertical[index1, index2] && !lastStampMaskVertical[index1, index2])
        {
          tearDecalStart.Add((Vector3) new Vector2(x, ((float) index2 - 0.5f) / (float) gridSizeY));
          tearDecalEnd.Add((Vector3) new Vector2(x, ((float) index2 + 0.5f) / (float) gridSizeY));
        }
      }
    }
    for (int index1 = 0; index1 < stampMaskDiagonalLeft.GetLength(0); ++index1)
    {
      for (int index2 = 0; index2 < stampMaskDiagonalLeft.GetLength(1); ++index2)
      {
        if (stampMaskDiagonalLeft[index1, index2] && !lastStampMaskDiagonalLeft[index1, index2])
        {
          tearDecalStart.Add((Vector3) new Vector2(((float) index1 + 0.5f) / (float) gridSizeX, ((float) index2 - 0.5f) / (float) gridSizeY));
          tearDecalEnd.Add((Vector3) new Vector2(((float) index1 - 0.5f) / (float) gridSizeX, ((float) index2 + 0.5f) / (float) gridSizeY));
        }
        if (stampMaskDiagonalRight[index1, index2] && !lastStampMaskDiagonalRight[index1, index2])
        {
          tearDecalStart.Add((Vector3) new Vector2(((float) index1 + 0.5f) / (float) gridSizeX, ((float) index2 + 0.5f) / (float) gridSizeY));
          tearDecalEnd.Add((Vector3) new Vector2(((float) index1 - 0.5f) / (float) gridSizeX, ((float) index2 - 0.5f) / (float) gridSizeY));
        }
      }
    }
  }

  private static void BuildAttachedLabels(int[,] labelGrid, List<int> attachedLabels)
  {
    attachedLabels.Clear();
    int length = labelGrid.GetLength(0);
    int index1 = labelGrid.GetLength(1) - 1;
    int num1 = 0;
    for (int index2 = 0; index2 < length; ++index2)
    {
      int num2 = labelGrid[index2, index1];
      if (num1 == 0 && num2 != 0 && !attachedLabels.Contains(num2))
        attachedLabels.Add(num2);
      num1 = num2;
    }
  }
}
