// Decompiled with JetBrains decompiler
// Type: PTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(15000)]
[Serializable]
public class PTarget : MonoBehaviour, IPTargetUnsafeFunctions
{
  [Header("Asset references")]
  [Tooltip("Disable this if you want to profile the target, as background threads cannot be profiled.")]
  public Material targetMaterial;
  public PTargetProfile targetProfile;
  public GameObject targetPiecePrefab;
  public float targetPieceMass = 0.1f;
  public float targetPieceDrag = 1f;
  public float targetPieceAngularDrag = 1f;
  public float targetPieceRandomAngularVelocityScale = 4f;
  public float targetPieceMaxAngularVelocity = 4f;
  public float targetPieceColliderThickness = 0.01f;
  public bool decalsOnly;
  [Header("Debugging Tools")]
  public bool multithreaded = true;
  [NonSerialized]
  private PTargetGridJob gridJob;
  [NonSerialized]
  private List<IOnPTargetHit> onHitRecievers = new List<IOnPTargetHit>(8);
  [NonSerialized]
  private List<OnHitInfo> onHitInfos = new List<OnHitInfo>(8);
  [NonSerialized]
  private List<PTarget.AddBulletCommand> addBulletCommands = new List<PTarget.AddBulletCommand>(8);
  [NonSerialized]
  private List<PTarget.DecalInfo> bulletDecals = new List<PTarget.DecalInfo>(8);
  [NonSerialized]
  private List<PTarget.DamageInfo> damageCells = new List<PTarget.DamageInfo>(8);
  [NonSerialized]
  private List<Vector2> destroyCells = new List<Vector2>(8);
  [NonSerialized]
  private Camera renderTextureCamera;
  [NonSerialized]
  private CommandBuffer commands;
  [NonSerialized]
  private float renderTextureAspect = 1f;
  [NonSerialized]
  private RenderTexture backupRT;
  [NonSerialized]
  private RenderTexture rt;
  private List<PTargetPiece> pieces = new List<PTargetPiece>();
  [NonSerialized]
  private float gridHealthInitialMultiplier = 1f;
  [NonSerialized]
  private float[,] gridHealth;
  [NonSerialized]
  private bool isInitialized;
  [NonSerialized]
  public List<IOnPTargetChangeUnsafe> onTargetChange = new List<IOnPTargetChangeUnsafe>(128);

  [ContextMenu("Reset Target")]
  public void ResetTarget() => this.isInitialized = false;

  public void ResetTarget(PTargetProfile targetProfile, float healthMultiplier = 1f, bool decalsOnly = false)
  {
    this.targetProfile = targetProfile;
    this.gridHealthInitialMultiplier = healthMultiplier;
    this.decalsOnly = decalsOnly;
    this.isInitialized = false;
  }

  public void DestroyCell(Vector2 uvPosition) => this.destroyCells.Add(uvPosition);

  public Vector2 GetUV(Vector3 localPosition)
  {
    PTargetGridJob gridJob = this.GetOrCreateGridJob();
    float targetSizeX = gridJob.targetSizeX;
    float targetSizeY = gridJob.targetSizeY;
    return new Vector2((localPosition.x + targetSizeX * 0.5f) / targetSizeX, (localPosition.y + targetSizeY * 0.5f) / targetSizeY);
  }

  public void AddBulletDecal(int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale)
  {
    int length = this.targetProfile.bulletDecals.Length;
    if (decalIndex < 0 || decalIndex >= length)
      MonoBehaviour.print((object) ("Tried to add a bullet decal with index " + (object) decalIndex + ", this index doesn't match any decals in the profile: " + this.targetProfile.name));
    else
      this.bulletDecals.Add(new PTarget.DecalInfo(decalIndex, uvPosition, rotation, scale));
  }

  public void AddBulletDecal(
    int sourceLabel,
    int decalIndex,
    Vector2 uvPosition,
    float rotation,
    Vector2 scale)
  {
    int length = this.targetProfile.bulletDecals.Length;
    if (decalIndex < 0 || decalIndex >= length)
      MonoBehaviour.print((object) ("Tried to add a bullet decal with index " + (object) decalIndex + ", this index doesn't match any decals in the profile: " + this.targetProfile.name));
    else
      this.addBulletCommands.Add(new PTarget.AddBulletCommand(sourceLabel, decalIndex, uvPosition, rotation, scale, 0.0f));
  }

  public void AddBullet(
    int decalIndex,
    Vector2 uvPosition,
    float rotation,
    Vector2 scale,
    float damageSize)
  {
    int length = this.targetProfile.bulletDecals.Length;
    if (decalIndex < 0 || decalIndex >= length)
    {
      MonoBehaviour.print((object) ("Tried to add a bullet decal with index " + (object) decalIndex + ", this index doesn't match any decals in the profile: " + this.targetProfile.name));
    }
    else
    {
      this.bulletDecals.Add(new PTarget.DecalInfo(decalIndex, uvPosition, rotation, scale));
      damageSize = !this.decalsOnly ? damageSize : 0.0f;
      if ((double) damageSize <= 0.0 && !this.decalsOnly)
        return;
      this.damageCells.Add(new PTarget.DamageInfo(uvPosition, damageSize));
    }
  }

  public void AddBullet(
    int sourceLabel,
    int decalIndex,
    Vector2 uvPosition,
    float rotation,
    float damageSize)
  {
    this.addBulletCommands.Add(new PTarget.AddBulletCommand(sourceLabel, decalIndex, uvPosition, rotation, new Vector2(damageSize, damageSize), !this.decalsOnly ? damageSize : 0.0f));
  }

  public void AddBullet(
    int sourceLabel,
    int decalIndex,
    Vector2 uvPosition,
    float rotation,
    Vector2 scale,
    float damageSize)
  {
    damageSize = !this.decalsOnly ? damageSize : 0.0f;
    this.addBulletCommands.Add(new PTarget.AddBulletCommand(sourceLabel, decalIndex, uvPosition, rotation, scale, damageSize));
  }

  private void Update()
  {
    if ((UnityEngine.Object) this.targetPiecePrefab == (UnityEngine.Object) null)
    {
      Debug.Log((object) "No Target Piece Prefab assigned, disabling destructable target");
      this.enabled = false;
    }
    else if ((UnityEngine.Object) this.targetProfile == (UnityEngine.Object) null)
    {
      Debug.Log((object) "No Target Profile assigned, disabling destructable target");
      this.enabled = false;
    }
    else if ((UnityEngine.Object) this.targetMaterial == (UnityEngine.Object) null)
    {
      Debug.Log((object) "No Target Material assigned, disabling destructable target");
      this.enabled = false;
    }
    else
    {
      this.Initialize();
      bool isScheduled = this.gridJob.isScheduled;
      if (isScheduled)
      {
        this.gridJob.WaitForResults();
        this.CreateNewTargetPieces();
        this.UpdateTargetPieces();
      }
      PTarget.ApplyAddBulletCommands(this.gridJob.labelGrid, this.addBulletCommands, this.bulletDecals, this.damageCells);
      this.addBulletCommands.Clear();
      bool flag1 = this.bulletDecals.Count > 0;
      bool flag2 = this.damageCells.Count > 0;
      bool flag3 = this.destroyCells.Count > 0;
      if (flag1 || isScheduled)
      {
        this.UpdateRenderTexture(this.bulletDecals, this.gridJob.tearDecalStart, this.gridJob.tearDecalEnd);
        PTarget.ComputeScoreEvents(this.bulletDecals, this.onHitInfos, this.targetProfile.scoreMap, this.targetProfile.scores);
        this.bulletDecals.Clear();
        this.gridJob.tearDecalStart.Clear();
        this.gridJob.tearDecalEnd.Clear();
      }
      if (flag2)
      {
        PTarget.ApplyDamageCells(this.gridHealth, this.damageCells, this.destroyCells);
        this.damageCells.Clear();
      }
      if (flag3)
      {
        PTarget.ApplyDestroyCells(this.gridJob.grid, this.destroyCells);
        this.destroyCells.Clear();
        if (this.multithreaded)
          this.gridJob.Schedule();
        else
          this.gridJob.ScheduleNow();
      }
      if (this.onHitInfos.Count <= 0)
        return;
      this.onHitRecievers.Clear();
      this.GetComponents<IOnPTargetHit>(this.onHitRecievers);
      for (int index = 0; index < this.onHitRecievers.Count; ++index)
        this.onHitRecievers[index].OnTargetHit(this.onHitInfos);
      this.onHitInfos.Clear();
    }
  }

  private void Initialize()
  {
    if (this.isInitialized)
      return;
    this.isInitialized = true;
    for (int index = 0; index < this.pieces.Count; ++index)
    {
      PTargetPiece piece = this.pieces[index];
      if ((UnityEngine.Object) piece != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) piece.gameObject);
    }
    this.onTargetChange.Clear();
    this.pieces.Clear();
    this.bulletDecals.Clear();
    PTargetGridJob gridJob = this.GetOrCreateGridJob(this.targetProfile.gridSizeX, this.targetProfile.gridSizeY, this.targetProfile.targetWidth, this.targetProfile.targetHeight);
    if (gridJob.isAlive)
      gridJob.Abort();
    gridJob.ClearData();
    PTarget.ResetHealthGrid(this.gridHealth, this.targetProfile.cellArea, this.gridHealthInitialMultiplier * this.targetProfile.healthMultiplier);
    this.InitializeRenderTexture();
    this.ClearRenderTexture();
    this.UpdateRenderTexture(this.bulletDecals, gridJob.tearDecalStart, gridJob.tearDecalEnd);
    this.CreateTargetPiece(1, this.transform.position, this.transform.rotation);
    if (this.multithreaded)
    {
      gridJob.Start();
      gridJob.Schedule();
      gridJob.WaitForResults();
    }
    else
    {
      gridJob.StartNow();
      gridJob.ScheduleNow();
      gridJob.WaitForResults();
    }
    this.UpdateTargetPieces();
  }

  private PTargetGridJob GetOrCreateGridJob()
  {
    if (this.gridJob == null)
      this.gridJob = new PTargetGridJob(this.targetProfile.gridSizeX, this.targetProfile.gridSizeY, this.targetProfile.targetWidth, this.targetProfile.targetHeight);
    return this.gridJob;
  }

  private PTargetGridJob GetOrCreateGridJob(
    int gridSizeX,
    int gridSizeY,
    float targetSizeX,
    float targetSizeY)
  {
    if (this.gridJob == null)
      this.gridJob = new PTargetGridJob();
    if (this.gridHealth == null || this.gridHealth.GetLength(0) != gridSizeX || this.gridHealth.GetLength(1) != gridSizeY)
      this.gridHealth = new float[gridSizeX, gridSizeY];
    this.gridJob.SetGridData(gridSizeX, gridSizeY, targetSizeX, targetSizeY);
    return this.gridJob;
  }

  private CommandBuffer GetOrCreateCommandBuffer()
  {
    if (this.commands == null)
    {
      this.commands = new CommandBuffer();
      this.commands.name = "TargetCommands";
      this.renderTextureCamera.RemoveAllCommandBuffers();
      this.renderTextureCamera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, this.commands);
    }
    return this.commands;
  }

  private static bool IsBulletInLabel(int[,] labelGrid, int sourceLabel, Vector2 uvPosition)
  {
    int[,] numArray = labelGrid;
    int length1 = labelGrid.GetLength(0);
    int length2 = labelGrid.GetLength(1);
    int index1 = Mathf.FloorToInt(Mathf.Clamp(uvPosition.x * (float) length1, 0.0f, (float) length1 - 0.5f));
    int index2 = Mathf.FloorToInt(Mathf.Clamp(uvPosition.y * (float) length2, 0.0f, (float) length2 - 0.5f));
    int num1 = numArray[index1, index2];
    int num2 = numArray[index1, Mathf.Min(index2 + 1, length2 - 1)];
    int num3 = numArray[Mathf.Min(index1 + 1, length1 - 1), index2];
    int num4 = numArray[index1, Mathf.Max(index2 - 1, 0)];
    int num5 = numArray[Mathf.Max(index1 - 1, 0), index2];
    bool flag1 = num1 == sourceLabel;
    bool flag2 = num2 == sourceLabel;
    bool flag3 = num3 == sourceLabel;
    bool flag4 = num4 == sourceLabel;
    bool flag5 = num5 == sourceLabel;
    return flag1 || flag2 || (flag3 || flag4) || flag5;
  }

  private static void ApplyAddBulletCommands(
    int[,] labelGrid,
    List<PTarget.AddBulletCommand> addBulletCommands,
    List<PTarget.DecalInfo> bulletDecals,
    List<PTarget.DamageInfo> damageCells)
  {
    for (int index = 0; index < addBulletCommands.Count; ++index)
    {
      PTarget.AddBulletCommand addBulletCommand = addBulletCommands[index];
      if (PTarget.IsBulletInLabel(labelGrid, addBulletCommand.sourceLabel, addBulletCommand.uvPosition))
      {
        bulletDecals.Add(new PTarget.DecalInfo(addBulletCommand.decalIndex, addBulletCommand.uvPosition, addBulletCommand.rotation, addBulletCommand.scale));
        if ((double) addBulletCommand.damageSize > 0.0)
          damageCells.Add(new PTarget.DamageInfo(addBulletCommand.uvPosition, addBulletCommand.damageSize));
      }
    }
  }

  private static void ApplyDamageCells(
    float[,] gridHealth,
    List<PTarget.DamageInfo> damageCells,
    List<Vector2> destroyCells)
  {
    int length1 = gridHealth.GetLength(0);
    int length2 = gridHealth.GetLength(1);
    int count = damageCells.Count;
    for (int index1 = 0; index1 < count; ++index1)
    {
      PTarget.DamageInfo damageCell = damageCells[index1];
      Vector2 uvPosition = damageCell.uvPosition;
      int index2 = Mathf.FloorToInt(Mathf.Clamp(uvPosition.x * (float) length1, 0.0f, (float) length1 - 0.5f));
      int index3 = Mathf.FloorToInt(Mathf.Clamp(uvPosition.y * (float) length2, 0.0f, (float) length2 - 0.5f));
      float num1 = gridHealth[index2, index3];
      if ((double) num1 > 0.0)
      {
        float num2 = damageCell.damageSize * damageCell.damageSize;
        float num3 = num1 - num2;
        if ((double) num3 <= 0.0)
        {
          destroyCells.Add(uvPosition);
          num3 = 0.0f;
        }
        gridHealth[index2, index3] = num3;
      }
    }
  }

  private static void ApplyDestroyCells(bool[,] grid, List<Vector2> coords)
  {
    int length1 = grid.GetLength(0);
    int length2 = grid.GetLength(1);
    int count = coords.Count;
    for (int index1 = 0; index1 < count; ++index1)
    {
      Vector2 coord = coords[index1];
      int index2 = Mathf.FloorToInt(Mathf.Clamp(coord.x * (float) length1, 0.0f, (float) length1 - 0.5f));
      int index3 = Mathf.FloorToInt(Mathf.Clamp(coord.y * (float) length2, 0.0f, (float) length2 - 0.5f));
      grid[index2, index3] = false;
    }
  }

  private static void ComputeScoreEvents(
    List<PTarget.DecalInfo> bulletDecals,
    List<OnHitInfo> onHitInfos,
    Texture2D scoreMap,
    int[] scores)
  {
    onHitInfos.Clear();
    float width = (float) scoreMap.width;
    float height = (float) scoreMap.height;
    int num = scores.Length - 1;
    for (int index = 0; index < bulletDecals.Count; ++index)
    {
      Vector2 uvPosition = bulletDecals[index].uvPosition;
      int x = Mathf.FloorToInt(uvPosition.x * width);
      int y = Mathf.FloorToInt(uvPosition.y * height);
      float a = scoreMap.GetPixel(x, y).a;
      int score = scores[Mathf.RoundToInt(a * (float) num)];
      onHitInfos.Add(new OnHitInfo(uvPosition, score));
    }
  }

  private static void ResetHealthGrid(float[,] gridHealth, float cellArea, float healthMultiplier)
  {
    for (int index1 = 0; index1 < gridHealth.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < gridHealth.GetLength(0); ++index2)
        gridHealth[index2, index1] = cellArea * healthMultiplier;
    }
  }

  void IPTargetUnsafeFunctions.ApplyLabelMesh(int label, Mesh mesh)
  {
    PTargetGridJob gridJob = this.GetOrCreateGridJob();
    if (gridJob.meshBuilderMap[label] == null)
      return;
    gridJob.meshBuilderMap[label].Apply(mesh);
  }

  int IPTargetUnsafeFunctions.GetCurrentLabelFromLastLabel(int lastLabel) => this.GetOrCreateGridJob().differenceRemapping[lastLabel];

  Rect IPTargetUnsafeFunctions.GetLabelRect(int label) => this.GetOrCreateGridJob().labelRects[label];

  bool IPTargetUnsafeFunctions.IsAttached(int label) => this.GetOrCreateGridJob().attachedLabels.Contains(label);

  private static void InitializeGrid(bool[,] grid, int[,] lastLabelGrid, int[,] labelGrid)
  {
    PTarget.ClearGrid(grid);
    PTarget.ResetLabelGrid(lastLabelGrid);
    PTarget.ResetLabelGrid(labelGrid);
  }

  private void InitializeRenderTexture()
  {
    this.renderTextureAspect = this.targetProfile.targetWidth / this.targetProfile.targetHeight;
    if (this.commands == null)
      this.commands = new CommandBuffer();
    this.commands.name = "TargetCommands";
    int textureResolutionX = this.targetProfile.renderTextureResolutionX;
    int textureResolutionY = this.targetProfile.renderTextureResolutionY;
    if ((UnityEngine.Object) this.backupRT == (UnityEngine.Object) null || this.backupRT.width != textureResolutionX || this.backupRT.height != textureResolutionY)
    {
      this.backupRT = new RenderTexture(textureResolutionX, textureResolutionY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      this.backupRT.useMipMap = this.targetProfile.renderTextureUseMipmap;
      this.backupRT.filterMode = this.targetProfile.renderTextureFilterMode;
      this.backupRT.anisoLevel = this.targetProfile.renderTextureAnisoLevel;
    }
    if ((UnityEngine.Object) this.rt == (UnityEngine.Object) null || this.rt.width != textureResolutionX || this.rt.height != textureResolutionY)
      this.rt = new RenderTexture(textureResolutionX, textureResolutionY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    this.targetMaterial.mainTexture = (Texture) this.backupRT;
    bool flag = (UnityEngine.Object) this.renderTextureCamera != (UnityEngine.Object) null;
    GameObject gameObject = !flag ? new GameObject("DestructableTargetCamera") : this.renderTextureCamera.gameObject;
    Transform transform = gameObject.transform;
    transform.localPosition = new Vector3(0.5f * this.renderTextureAspect, 0.5f, -0.5f);
    transform.localRotation = Quaternion.identity;
    transform.localScale = new Vector3(1f, 1f, 1f);
    if (!flag)
      this.renderTextureCamera = gameObject.AddComponent<Camera>();
    this.renderTextureCamera.enabled = false;
    this.renderTextureCamera.aspect = this.renderTextureAspect;
    this.renderTextureCamera.clearFlags = CameraClearFlags.Nothing;
    this.renderTextureCamera.cullingMask = 0;
    this.renderTextureCamera.orthographic = true;
    this.renderTextureCamera.orthographicSize = 0.5f;
    this.renderTextureCamera.nearClipPlane = 0.0f;
    this.renderTextureCamera.farClipPlane = 1f;
    this.renderTextureCamera.useOcclusionCulling = false;
    this.renderTextureCamera.allowHDR = false;
    this.renderTextureCamera.allowMSAA = false;
    this.renderTextureCamera.targetTexture = this.rt;
    this.renderTextureCamera.RemoveAllCommandBuffers();
    this.renderTextureCamera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, this.commands);
  }

  private static void ClearGrid(bool[,] grid)
  {
    for (int index1 = 0; index1 < grid.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < grid.GetLength(0); ++index2)
        grid[index2, index1] = true;
    }
  }

  private static void ResetLabelGrid(int[,] labels)
  {
    for (int index1 = 0; index1 < labels.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < labels.GetLength(0); ++index2)
        labels[index2, index1] = 1;
    }
  }

  private void OnDestroy() => this.gridJob.Abort();

  private GameObject CreateTargetPiece(
    int pieceLabel,
    Vector3 position,
    Quaternion rotation)
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.targetPiecePrefab);
    Transform transform = gameObject.transform;
    transform.position = position;
    transform.rotation = rotation;
    transform.SetParent(this.transform);
    PTargetPiece component = gameObject.GetComponent<PTargetPiece>();
    this.pieces.Add(component);
    component.parentTarget = this;
    component.componentLabel = pieceLabel;
    component.Initialize();
    return gameObject;
  }

  private void ClearRenderTexture()
  {
    Transform transform = this.renderTextureCamera.transform;
    this.commands.Clear();
    this.commands.ClearRenderTarget(true, true, new Color(0.0f, 0.0f, 0.0f, 1f));
    this.commands.DrawMesh(this.targetProfile.background.mesh, Matrix4x4.TRS(transform.TransformPoint(Vector3.forward * 0.49f), transform.rotation, new Vector3(this.renderTextureAspect, 1f, 1f) * UnityEngine.Random.Range(this.targetProfile.background.scaleMultiplierMin, this.targetProfile.background.scaleMultiplierMax)), this.targetProfile.background.material);
    this.commands.Blit((Texture) this.rt, (RenderTargetIdentifier) (Texture) this.backupRT);
    this.renderTextureCamera.Render();
  }

  private void UpdateRenderTexture(
    List<PTarget.DecalInfo> bulletDecalPositions,
    List<Vector3> tearDecalStart,
    List<Vector3> tearDecalEnd)
  {
    CommandBuffer commandBuffer = this.GetOrCreateCommandBuffer();
    commandBuffer.Clear();
    commandBuffer.ClearRenderTarget(true, true, new Color(0.0f, 0.0f, 0.0f, 1f));
    commandBuffer.Blit((Texture) this.backupRT, (RenderTargetIdentifier) (Texture) this.rt);
    Transform transform = this.renderTextureCamera.transform;
    PTargetDecal tearDecal = this.targetProfile.tearDecals[0];
    Mesh mesh1 = tearDecal.mesh;
    Material material1 = tearDecal.material;
    float scaleMultiplierMin1 = tearDecal.scaleMultiplierMin;
    float scaleMultiplierMax1 = tearDecal.scaleMultiplierMax;
    float orthographicSize = this.renderTextureCamera.orthographicSize;
    int count1 = tearDecalStart.Count;
    float targetHeight = this.targetProfile.targetHeight;
    for (int index = 0; index < count1; ++index)
    {
      Vector3 vector3_1 = tearDecalStart[index];
      vector3_1.x *= this.renderTextureAspect;
      Vector3 vector3_2 = tearDecalEnd[index];
      vector3_2.x *= this.renderTextureAspect;
      Vector3 vector3_3 = vector3_2 - vector3_1;
      Vector3 normalized = vector3_3.normalized;
      Vector3 pos = (vector3_1 + vector3_2) * 0.5f;
      Quaternion q = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, normalized));
      Vector3 s = new Vector3(1f, 1f, 1f) * (vector3_3.magnitude * UnityEngine.Random.Range(scaleMultiplierMin1, scaleMultiplierMax1));
      commandBuffer.DrawMesh(mesh1, Matrix4x4.TRS(pos, q, s), material1);
    }
    int length = this.targetProfile.bulletDecals.Length;
    int count2 = bulletDecalPositions.Count;
    for (int index = 0; index < count2; ++index)
    {
      PTarget.DecalInfo bulletDecalPosition = bulletDecalPositions[index];
      if (bulletDecalPosition.decalIndex >= length || bulletDecalPosition.decalIndex < 0)
      {
        MonoBehaviour.print((object) ("Tried to render bullet decal with invalid index, there is no decal at index " + (object) bulletDecalPosition.decalIndex + " in the target profile: " + this.targetProfile.name));
      }
      else
      {
        PTargetDecal bulletDecal = this.targetProfile.bulletDecals[bulletDecalPosition.decalIndex];
        Mesh mesh2 = bulletDecal.mesh;
        Material material2 = bulletDecal.material;
        float scaleMultiplierMin2 = bulletDecal.scaleMultiplierMin;
        float scaleMultiplierMax2 = bulletDecal.scaleMultiplierMax;
        Vector2 uvPosition = bulletDecalPosition.uvPosition;
        Quaternion q = Quaternion.AngleAxis(bulletDecalPosition.rotation, new Vector3(0.0f, 0.0f, 1f));
        Vector3 s = new Vector3(bulletDecalPosition.scale.x, bulletDecalPosition.scale.y) * (UnityEngine.Random.Range(scaleMultiplierMin2, scaleMultiplierMax2) / targetHeight);
        uvPosition.x *= this.renderTextureAspect;
        commandBuffer.DrawMesh(mesh2, Matrix4x4.TRS((Vector3) uvPosition, q, s), material2);
      }
    }
    commandBuffer.Blit((Texture) this.rt, (RenderTargetIdentifier) (Texture) this.backupRT);
    this.renderTextureCamera.Render();
  }

  private void CreateNewTargetPieces()
  {
    PTargetGridJob gridJob = this.GetOrCreateGridJob();
    int count1 = gridJob.newLabelsFrontBuffer.Count;
    int count2 = this.pieces.Count;
    for (int index1 = 0; index1 < count2; ++index1)
    {
      PTargetPiece piece = this.pieces[index1];
      int componentLabel = piece.componentLabel;
      for (int index2 = 0; index2 < count1; ++index2)
      {
        if (gridJob.newLabelsBackBuffer[index2] == componentLabel)
          this.CreateTargetPiece(this.gridJob.newLabelsFrontBuffer[index2], piece.transform.position, piece.transform.rotation);
      }
    }
  }

  private void UpdateTargetPieces()
  {
    int count = this.onTargetChange.Count;
    for (int index = 0; index < count; ++index)
      this.onTargetChange[index].OnTargetChange();
  }

  private static void BuildCornerDebug(
    bool corner,
    bool left,
    bool right,
    Vector3 center,
    Vector3 leftVector,
    Vector3 rightVector)
  {
    if (corner)
    {
      if (left && !right)
      {
        Gizmos.DrawLine(center, center + rightVector);
      }
      else
      {
        if (left || !right)
          return;
        Gizmos.DrawLine(center, center + leftVector);
      }
    }
    else
    {
      if (left)
        Gizmos.DrawLine(center, center + leftVector + rightVector);
      if (!right)
        return;
      Gizmos.DrawLine(center + leftVector + rightVector, center);
    }
  }

  private static Vector2 GetLocalCellPosition(float x, float y, PTargetGridJob gridJob)
  {
    Vector2 vector2 = new Vector2(gridJob.cellSizeX * (float) gridJob.gridSizeX, gridJob.cellSizeY * (float) gridJob.gridSizeY) * 0.5f;
    return new Vector2(x * gridJob.cellSizeX, y * gridJob.cellSizeX) - vector2;
  }

  private void OnDrawGizmosSelected()
  {
    if ((UnityEngine.Object) this.targetProfile == (UnityEngine.Object) null)
      return;
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Gizmos.color = new Color(0.3f, 0.9f, 0.1f);
    float targetWidth = this.targetProfile.targetWidth;
    float targetHeight = this.targetProfile.targetHeight;
    int gridSizeX = this.targetProfile.gridSizeX;
    int gridSizeY = this.targetProfile.gridSizeY;
    Gizmos.DrawWireCube(Vector3.zero, new Vector3(targetWidth, targetHeight));
    float x1 = targetWidth * 0.5f;
    float y1 = targetHeight * 0.5f;
    float num1 = targetWidth / (float) gridSizeX;
    float num2 = targetHeight / (float) gridSizeY;
    for (float x2 = -x1 + num1; (double) x2 < (double) x1; x2 += num1)
      Gizmos.DrawLine(new Vector3(x2, -y1), new Vector3(x2, y1));
    for (float y2 = -y1 + num2; (double) y2 < (double) y1; y2 += num2)
      Gizmos.DrawLine(new Vector3(-x1, y2), new Vector3(x1, y2));
    Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
    Gizmos.DrawLine(new Vector3(0.0f, targetHeight * 0.5f, -0.1f), new Vector3(0.0f, targetHeight * 0.5f, -0.3f));
    Gizmos.DrawLine(new Vector3(0.05f, targetHeight * 0.5f, -0.2f), new Vector3(0.0f, targetHeight * 0.5f, -0.3f));
    Gizmos.DrawLine(new Vector3(-0.05f, targetHeight * 0.5f, -0.2f), new Vector3(0.0f, targetHeight * 0.5f, -0.3f));
    Gizmos.DrawLine(new Vector3(0.0f, (float) (-(double) targetHeight * 0.5), -0.1f), new Vector3(0.0f, (float) (-(double) targetHeight * 0.5), -0.3f));
    Gizmos.DrawLine(new Vector3(0.05f, (float) (-(double) targetHeight * 0.5), -0.2f), new Vector3(0.0f, (float) (-(double) targetHeight * 0.5), -0.3f));
    Gizmos.DrawLine(new Vector3(-0.05f, (float) (-(double) targetHeight * 0.5), -0.2f), new Vector3(0.0f, (float) (-(double) targetHeight * 0.5), -0.3f));
  }

  private struct DecalInfo
  {
    public int decalIndex;
    public Vector2 uvPosition;
    public float rotation;
    public Vector2 scale;

    public DecalInfo(int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale)
    {
      this.decalIndex = decalIndex;
      this.uvPosition = uvPosition;
      this.rotation = rotation;
      this.scale = scale;
    }
  }

  private struct DamageInfo
  {
    public Vector2 uvPosition;
    public float damageSize;

    public DamageInfo(Vector2 uvPosition, float damageSize)
    {
      this.uvPosition = uvPosition;
      this.damageSize = damageSize;
    }
  }

  private struct AddBulletCommand
  {
    public int sourceLabel;
    public int decalIndex;
    public Vector2 uvPosition;
    public float rotation;
    public Vector2 scale;
    public float damageSize;

    public AddBulletCommand(
      int sourceLabel,
      int decalIndex,
      Vector2 uvPosition,
      float rotation,
      Vector2 scale,
      float damageSize)
    {
      this.sourceLabel = sourceLabel;
      this.decalIndex = decalIndex;
      this.uvPosition = uvPosition;
      this.rotation = rotation;
      this.scale = scale;
      this.damageSize = damageSize;
    }
  }
}
