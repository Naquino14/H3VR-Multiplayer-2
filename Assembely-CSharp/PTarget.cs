using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[DefaultExecutionOrder(15000)]
public class PTarget : MonoBehaviour, IPTargetUnsafeFunctions
{
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

		public AddBulletCommand(int sourceLabel, int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale, float damageSize)
		{
			this.sourceLabel = sourceLabel;
			this.decalIndex = decalIndex;
			this.uvPosition = uvPosition;
			this.rotation = rotation;
			this.scale = scale;
			this.damageSize = damageSize;
		}
	}

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
	private List<AddBulletCommand> addBulletCommands = new List<AddBulletCommand>(8);

	[NonSerialized]
	private List<DecalInfo> bulletDecals = new List<DecalInfo>(8);

	[NonSerialized]
	private List<DamageInfo> damageCells = new List<DamageInfo>(8);

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
	public void ResetTarget()
	{
		isInitialized = false;
	}

	public void ResetTarget(PTargetProfile targetProfile, float healthMultiplier = 1f, bool decalsOnly = false)
	{
		this.targetProfile = targetProfile;
		gridHealthInitialMultiplier = healthMultiplier;
		this.decalsOnly = decalsOnly;
		isInitialized = false;
	}

	public void DestroyCell(Vector2 uvPosition)
	{
		destroyCells.Add(uvPosition);
	}

	public Vector2 GetUV(Vector3 localPosition)
	{
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob();
		float targetSizeX = orCreateGridJob.targetSizeX;
		float targetSizeY = orCreateGridJob.targetSizeY;
		float x = (localPosition.x + targetSizeX * 0.5f) / targetSizeX;
		float y = (localPosition.y + targetSizeY * 0.5f) / targetSizeY;
		return new Vector2(x, y);
	}

	public void AddBulletDecal(int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale)
	{
		int num = targetProfile.bulletDecals.Length;
		if (decalIndex < 0 || decalIndex >= num)
		{
			MonoBehaviour.print("Tried to add a bullet decal with index " + decalIndex + ", this index doesn't match any decals in the profile: " + targetProfile.name);
		}
		else
		{
			bulletDecals.Add(new DecalInfo(decalIndex, uvPosition, rotation, scale));
		}
	}

	public void AddBulletDecal(int sourceLabel, int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale)
	{
		int num = targetProfile.bulletDecals.Length;
		if (decalIndex < 0 || decalIndex >= num)
		{
			MonoBehaviour.print("Tried to add a bullet decal with index " + decalIndex + ", this index doesn't match any decals in the profile: " + targetProfile.name);
		}
		else
		{
			addBulletCommands.Add(new AddBulletCommand(sourceLabel, decalIndex, uvPosition, rotation, scale, 0f));
		}
	}

	public void AddBullet(int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale, float damageSize)
	{
		int num = targetProfile.bulletDecals.Length;
		if (decalIndex < 0 || decalIndex >= num)
		{
			MonoBehaviour.print("Tried to add a bullet decal with index " + decalIndex + ", this index doesn't match any decals in the profile: " + targetProfile.name);
		}
		else
		{
			bulletDecals.Add(new DecalInfo(decalIndex, uvPosition, rotation, scale));
			damageSize = ((!decalsOnly) ? damageSize : 0f);
			if (damageSize > 0f || decalsOnly)
			{
				damageCells.Add(new DamageInfo(uvPosition, damageSize));
			}
		}
	}

	public void AddBullet(int sourceLabel, int decalIndex, Vector2 uvPosition, float rotation, float damageSize)
	{
		addBulletCommands.Add(new AddBulletCommand(sourceLabel, decalIndex, uvPosition, rotation, new Vector2(damageSize, damageSize), (!decalsOnly) ? damageSize : 0f));
	}

	public void AddBullet(int sourceLabel, int decalIndex, Vector2 uvPosition, float rotation, Vector2 scale, float damageSize)
	{
		damageSize = ((!decalsOnly) ? damageSize : 0f);
		addBulletCommands.Add(new AddBulletCommand(sourceLabel, decalIndex, uvPosition, rotation, scale, damageSize));
	}

	private void Update()
	{
		if (targetPiecePrefab == null)
		{
			Debug.Log("No Target Piece Prefab assigned, disabling destructable target");
			base.enabled = false;
			return;
		}
		if (targetProfile == null)
		{
			Debug.Log("No Target Profile assigned, disabling destructable target");
			base.enabled = false;
			return;
		}
		if (targetMaterial == null)
		{
			Debug.Log("No Target Material assigned, disabling destructable target");
			base.enabled = false;
			return;
		}
		Initialize();
		bool isScheduled = gridJob.isScheduled;
		if (isScheduled)
		{
			gridJob.WaitForResults();
			CreateNewTargetPieces();
			UpdateTargetPieces();
		}
		ApplyAddBulletCommands(gridJob.labelGrid, addBulletCommands, bulletDecals, damageCells);
		addBulletCommands.Clear();
		bool flag = bulletDecals.Count > 0;
		bool flag2 = damageCells.Count > 0;
		bool flag3 = destroyCells.Count > 0;
		if (flag || isScheduled)
		{
			UpdateRenderTexture(bulletDecals, gridJob.tearDecalStart, gridJob.tearDecalEnd);
			ComputeScoreEvents(bulletDecals, onHitInfos, targetProfile.scoreMap, targetProfile.scores);
			bulletDecals.Clear();
			gridJob.tearDecalStart.Clear();
			gridJob.tearDecalEnd.Clear();
		}
		if (flag2)
		{
			ApplyDamageCells(gridHealth, damageCells, destroyCells);
			damageCells.Clear();
		}
		if (flag3)
		{
			ApplyDestroyCells(gridJob.grid, destroyCells);
			destroyCells.Clear();
			if (multithreaded)
			{
				gridJob.Schedule();
			}
			else
			{
				gridJob.ScheduleNow();
			}
		}
		if (onHitInfos.Count > 0)
		{
			onHitRecievers.Clear();
			GetComponents(onHitRecievers);
			for (int i = 0; i < onHitRecievers.Count; i++)
			{
				IOnPTargetHit onPTargetHit = onHitRecievers[i];
				onPTargetHit.OnTargetHit(onHitInfos);
			}
			onHitInfos.Clear();
		}
	}

	private void Initialize()
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		for (int i = 0; i < pieces.Count; i++)
		{
			PTargetPiece pTargetPiece = pieces[i];
			if (pTargetPiece != null)
			{
				UnityEngine.Object.Destroy(pTargetPiece.gameObject);
			}
		}
		onTargetChange.Clear();
		pieces.Clear();
		bulletDecals.Clear();
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob(targetProfile.gridSizeX, targetProfile.gridSizeY, targetProfile.targetWidth, targetProfile.targetHeight);
		if (orCreateGridJob.isAlive)
		{
			orCreateGridJob.Abort();
		}
		orCreateGridJob.ClearData();
		ResetHealthGrid(gridHealth, targetProfile.cellArea, gridHealthInitialMultiplier * targetProfile.healthMultiplier);
		InitializeRenderTexture();
		ClearRenderTexture();
		UpdateRenderTexture(bulletDecals, orCreateGridJob.tearDecalStart, orCreateGridJob.tearDecalEnd);
		CreateTargetPiece(1, base.transform.position, base.transform.rotation);
		if (multithreaded)
		{
			orCreateGridJob.Start();
			orCreateGridJob.Schedule();
			orCreateGridJob.WaitForResults();
		}
		else
		{
			orCreateGridJob.StartNow();
			orCreateGridJob.ScheduleNow();
			orCreateGridJob.WaitForResults();
		}
		UpdateTargetPieces();
	}

	private PTargetGridJob GetOrCreateGridJob()
	{
		if (gridJob == null)
		{
			gridJob = new PTargetGridJob(targetProfile.gridSizeX, targetProfile.gridSizeY, targetProfile.targetWidth, targetProfile.targetHeight);
		}
		return gridJob;
	}

	private PTargetGridJob GetOrCreateGridJob(int gridSizeX, int gridSizeY, float targetSizeX, float targetSizeY)
	{
		if (gridJob == null)
		{
			gridJob = new PTargetGridJob();
		}
		if (gridHealth == null || gridHealth.GetLength(0) != gridSizeX || gridHealth.GetLength(1) != gridSizeY)
		{
			gridHealth = new float[gridSizeX, gridSizeY];
		}
		gridJob.SetGridData(gridSizeX, gridSizeY, targetSizeX, targetSizeY);
		return gridJob;
	}

	private CommandBuffer GetOrCreateCommandBuffer()
	{
		if (commands == null)
		{
			commands = new CommandBuffer();
			commands.name = "TargetCommands";
			renderTextureCamera.RemoveAllCommandBuffers();
			renderTextureCamera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, commands);
		}
		return commands;
	}

	private static bool IsBulletInLabel(int[,] labelGrid, int sourceLabel, Vector2 uvPosition)
	{
		int length = labelGrid.GetLength(0);
		int length2 = labelGrid.GetLength(1);
		int num = Mathf.FloorToInt(Mathf.Clamp(uvPosition.x * (float)length, 0f, (float)length - 0.5f));
		int num2 = Mathf.FloorToInt(Mathf.Clamp(uvPosition.y * (float)length2, 0f, (float)length2 - 0.5f));
		int num3 = labelGrid[num, num2];
		int num4 = labelGrid[num, Mathf.Min(num2 + 1, length2 - 1)];
		int num5 = labelGrid[Mathf.Min(num + 1, length - 1), num2];
		int num6 = labelGrid[num, Mathf.Max(num2 - 1, 0)];
		int num7 = labelGrid[Mathf.Max(num - 1, 0), num2];
		bool flag = num3 == sourceLabel;
		bool flag2 = num4 == sourceLabel;
		bool flag3 = num5 == sourceLabel;
		bool flag4 = num6 == sourceLabel;
		bool flag5 = num7 == sourceLabel;
		return flag || flag2 || flag3 || flag4 || flag5;
	}

	private static void ApplyAddBulletCommands(int[,] labelGrid, List<AddBulletCommand> addBulletCommands, List<DecalInfo> bulletDecals, List<DamageInfo> damageCells)
	{
		for (int i = 0; i < addBulletCommands.Count; i++)
		{
			AddBulletCommand addBulletCommand = addBulletCommands[i];
			if (IsBulletInLabel(labelGrid, addBulletCommand.sourceLabel, addBulletCommand.uvPosition))
			{
				bulletDecals.Add(new DecalInfo(addBulletCommand.decalIndex, addBulletCommand.uvPosition, addBulletCommand.rotation, addBulletCommand.scale));
				if (addBulletCommand.damageSize > 0f)
				{
					damageCells.Add(new DamageInfo(addBulletCommand.uvPosition, addBulletCommand.damageSize));
				}
			}
		}
	}

	private static void ApplyDamageCells(float[,] gridHealth, List<DamageInfo> damageCells, List<Vector2> destroyCells)
	{
		int length = gridHealth.GetLength(0);
		int length2 = gridHealth.GetLength(1);
		int count = damageCells.Count;
		for (int i = 0; i < count; i++)
		{
			DamageInfo damageInfo = damageCells[i];
			Vector2 uvPosition = damageInfo.uvPosition;
			int num = Mathf.FloorToInt(Mathf.Clamp(uvPosition.x * (float)length, 0f, (float)length - 0.5f));
			int num2 = Mathf.FloorToInt(Mathf.Clamp(uvPosition.y * (float)length2, 0f, (float)length2 - 0.5f));
			float num3 = gridHealth[num, num2];
			if (!(num3 <= 0f))
			{
				float num4 = damageInfo.damageSize * damageInfo.damageSize;
				float num5 = num3 - num4;
				if (num5 <= 0f)
				{
					destroyCells.Add(uvPosition);
					num5 = 0f;
				}
				gridHealth[num, num2] = num5;
			}
		}
	}

	private static void ApplyDestroyCells(bool[,] grid, List<Vector2> coords)
	{
		int length = grid.GetLength(0);
		int length2 = grid.GetLength(1);
		int count = coords.Count;
		for (int i = 0; i < count; i++)
		{
			Vector2 vector = coords[i];
			int num = Mathf.FloorToInt(Mathf.Clamp(vector.x * (float)length, 0f, (float)length - 0.5f));
			int num2 = Mathf.FloorToInt(Mathf.Clamp(vector.y * (float)length2, 0f, (float)length2 - 0.5f));
			grid[num, num2] = false;
		}
	}

	private static void ComputeScoreEvents(List<DecalInfo> bulletDecals, List<OnHitInfo> onHitInfos, Texture2D scoreMap, int[] scores)
	{
		onHitInfos.Clear();
		float num = scoreMap.width;
		float num2 = scoreMap.height;
		int num3 = scores.Length - 1;
		for (int i = 0; i < bulletDecals.Count; i++)
		{
			Vector2 uvPosition = bulletDecals[i].uvPosition;
			int x = Mathf.FloorToInt(uvPosition.x * num);
			int y = Mathf.FloorToInt(uvPosition.y * num2);
			float a = scoreMap.GetPixel(x, y).a;
			int score = scores[Mathf.RoundToInt(a * (float)num3)];
			onHitInfos.Add(new OnHitInfo(uvPosition, score));
		}
	}

	private static void ResetHealthGrid(float[,] gridHealth, float cellArea, float healthMultiplier)
	{
		for (int i = 0; i < gridHealth.GetLength(1); i++)
		{
			for (int j = 0; j < gridHealth.GetLength(0); j++)
			{
				gridHealth[j, i] = cellArea * healthMultiplier;
			}
		}
	}

	void IPTargetUnsafeFunctions.ApplyLabelMesh(int label, Mesh mesh)
	{
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob();
		if (orCreateGridJob.meshBuilderMap[label] != null)
		{
			orCreateGridJob.meshBuilderMap[label].Apply(mesh);
		}
	}

	int IPTargetUnsafeFunctions.GetCurrentLabelFromLastLabel(int lastLabel)
	{
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob();
		return orCreateGridJob.differenceRemapping[lastLabel];
	}

	Rect IPTargetUnsafeFunctions.GetLabelRect(int label)
	{
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob();
		return orCreateGridJob.labelRects[label];
	}

	bool IPTargetUnsafeFunctions.IsAttached(int label)
	{
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob();
		return orCreateGridJob.attachedLabels.Contains(label);
	}

	private static void InitializeGrid(bool[,] grid, int[,] lastLabelGrid, int[,] labelGrid)
	{
		ClearGrid(grid);
		ResetLabelGrid(lastLabelGrid);
		ResetLabelGrid(labelGrid);
	}

	private void InitializeRenderTexture()
	{
		float targetWidth = targetProfile.targetWidth;
		float targetHeight = targetProfile.targetHeight;
		renderTextureAspect = targetWidth / targetHeight;
		if (commands == null)
		{
			commands = new CommandBuffer();
		}
		commands.name = "TargetCommands";
		int renderTextureResolutionX = targetProfile.renderTextureResolutionX;
		int renderTextureResolutionY = targetProfile.renderTextureResolutionY;
		if (backupRT == null || backupRT.width != renderTextureResolutionX || backupRT.height != renderTextureResolutionY)
		{
			backupRT = new RenderTexture(renderTextureResolutionX, renderTextureResolutionY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			backupRT.useMipMap = targetProfile.renderTextureUseMipmap;
			backupRT.filterMode = targetProfile.renderTextureFilterMode;
			backupRT.anisoLevel = targetProfile.renderTextureAnisoLevel;
		}
		if (rt == null || rt.width != renderTextureResolutionX || rt.height != renderTextureResolutionY)
		{
			rt = new RenderTexture(renderTextureResolutionX, renderTextureResolutionY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		}
		targetMaterial.mainTexture = backupRT;
		bool flag = renderTextureCamera != null;
		GameObject gameObject = ((!flag) ? new GameObject("DestructableTargetCamera") : renderTextureCamera.gameObject);
		Transform transform = gameObject.transform;
		transform.localPosition = new Vector3(0.5f * renderTextureAspect, 0.5f, -0.5f);
		transform.localRotation = Quaternion.identity;
		transform.localScale = new Vector3(1f, 1f, 1f);
		if (!flag)
		{
			renderTextureCamera = gameObject.AddComponent<Camera>();
		}
		renderTextureCamera.enabled = false;
		renderTextureCamera.aspect = renderTextureAspect;
		renderTextureCamera.clearFlags = CameraClearFlags.Nothing;
		renderTextureCamera.cullingMask = 0;
		renderTextureCamera.orthographic = true;
		renderTextureCamera.orthographicSize = 0.5f;
		renderTextureCamera.nearClipPlane = 0f;
		renderTextureCamera.farClipPlane = 1f;
		renderTextureCamera.useOcclusionCulling = false;
		renderTextureCamera.allowHDR = false;
		renderTextureCamera.allowMSAA = false;
		renderTextureCamera.targetTexture = rt;
		renderTextureCamera.RemoveAllCommandBuffers();
		renderTextureCamera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, commands);
	}

	private static void ClearGrid(bool[,] grid)
	{
		for (int i = 0; i < grid.GetLength(1); i++)
		{
			for (int j = 0; j < grid.GetLength(0); j++)
			{
				grid[j, i] = true;
			}
		}
	}

	private static void ResetLabelGrid(int[,] labels)
	{
		for (int i = 0; i < labels.GetLength(1); i++)
		{
			for (int j = 0; j < labels.GetLength(0); j++)
			{
				labels[j, i] = 1;
			}
		}
	}

	private void OnDestroy()
	{
		gridJob.Abort();
	}

	private GameObject CreateTargetPiece(int pieceLabel, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(targetPiecePrefab);
		Transform transform = gameObject.transform;
		transform.position = position;
		transform.rotation = rotation;
		transform.SetParent(base.transform);
		PTargetPiece component = gameObject.GetComponent<PTargetPiece>();
		pieces.Add(component);
		component.parentTarget = this;
		component.componentLabel = pieceLabel;
		component.Initialize();
		return gameObject;
	}

	private void ClearRenderTexture()
	{
		Transform transform = renderTextureCamera.transform;
		commands.Clear();
		commands.ClearRenderTarget(clearDepth: true, clearColor: true, new Color(0f, 0f, 0f, 1f));
		commands.DrawMesh(targetProfile.background.mesh, Matrix4x4.TRS(transform.TransformPoint(Vector3.forward * 0.49f), transform.rotation, new Vector3(renderTextureAspect, 1f, 1f) * UnityEngine.Random.Range(targetProfile.background.scaleMultiplierMin, targetProfile.background.scaleMultiplierMax)), targetProfile.background.material);
		commands.Blit(rt, backupRT);
		renderTextureCamera.Render();
	}

	private void UpdateRenderTexture(List<DecalInfo> bulletDecalPositions, List<Vector3> tearDecalStart, List<Vector3> tearDecalEnd)
	{
		CommandBuffer orCreateCommandBuffer = GetOrCreateCommandBuffer();
		orCreateCommandBuffer.Clear();
		orCreateCommandBuffer.ClearRenderTarget(clearDepth: true, clearColor: true, new Color(0f, 0f, 0f, 1f));
		orCreateCommandBuffer.Blit(backupRT, rt);
		Transform transform = renderTextureCamera.transform;
		PTargetDecal pTargetDecal = targetProfile.tearDecals[0];
		Mesh mesh = pTargetDecal.mesh;
		Material material = pTargetDecal.material;
		float scaleMultiplierMin = pTargetDecal.scaleMultiplierMin;
		float scaleMultiplierMax = pTargetDecal.scaleMultiplierMax;
		float orthographicSize = renderTextureCamera.orthographicSize;
		int count = tearDecalStart.Count;
		float targetHeight = targetProfile.targetHeight;
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = tearDecalStart[i];
			vector.x *= renderTextureAspect;
			Vector3 vector2 = tearDecalEnd[i];
			vector2.x *= renderTextureAspect;
			Vector3 vector3 = vector2 - vector;
			Vector3 normalized = vector3.normalized;
			Vector3 pos = (vector + vector2) * 0.5f;
			Quaternion q = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, normalized));
			Vector3 s = new Vector3(1f, 1f, 1f) * (vector3.magnitude * UnityEngine.Random.Range(scaleMultiplierMin, scaleMultiplierMax));
			orCreateCommandBuffer.DrawMesh(mesh, Matrix4x4.TRS(pos, q, s), material);
		}
		int num = targetProfile.bulletDecals.Length;
		int count2 = bulletDecalPositions.Count;
		for (int j = 0; j < count2; j++)
		{
			DecalInfo decalInfo = bulletDecalPositions[j];
			if (decalInfo.decalIndex >= num || decalInfo.decalIndex < 0)
			{
				MonoBehaviour.print("Tried to render bullet decal with invalid index, there is no decal at index " + decalInfo.decalIndex + " in the target profile: " + targetProfile.name);
				continue;
			}
			PTargetDecal pTargetDecal2 = targetProfile.bulletDecals[decalInfo.decalIndex];
			Mesh mesh2 = pTargetDecal2.mesh;
			Material material2 = pTargetDecal2.material;
			float scaleMultiplierMin2 = pTargetDecal2.scaleMultiplierMin;
			float scaleMultiplierMax2 = pTargetDecal2.scaleMultiplierMax;
			Vector2 uvPosition = decalInfo.uvPosition;
			Quaternion q2 = Quaternion.AngleAxis(decalInfo.rotation, new Vector3(0f, 0f, 1f));
			Vector3 s2 = new Vector3(decalInfo.scale.x, decalInfo.scale.y) * (UnityEngine.Random.Range(scaleMultiplierMin2, scaleMultiplierMax2) / targetHeight);
			uvPosition.x *= renderTextureAspect;
			orCreateCommandBuffer.DrawMesh(mesh2, Matrix4x4.TRS(uvPosition, q2, s2), material2);
		}
		orCreateCommandBuffer.Blit(rt, backupRT);
		renderTextureCamera.Render();
	}

	private void CreateNewTargetPieces()
	{
		PTargetGridJob orCreateGridJob = GetOrCreateGridJob();
		int count = orCreateGridJob.newLabelsFrontBuffer.Count;
		int count2 = pieces.Count;
		for (int i = 0; i < count2; i++)
		{
			PTargetPiece pTargetPiece = pieces[i];
			int componentLabel = pTargetPiece.componentLabel;
			for (int j = 0; j < count; j++)
			{
				if (orCreateGridJob.newLabelsBackBuffer[j] == componentLabel)
				{
					CreateTargetPiece(gridJob.newLabelsFrontBuffer[j], pTargetPiece.transform.position, pTargetPiece.transform.rotation);
				}
			}
		}
	}

	private void UpdateTargetPieces()
	{
		int count = onTargetChange.Count;
		for (int i = 0; i < count; i++)
		{
			onTargetChange[i].OnTargetChange();
		}
	}

	private static void BuildCornerDebug(bool corner, bool left, bool right, Vector3 center, Vector3 leftVector, Vector3 rightVector)
	{
		if (corner)
		{
			if (left && !right)
			{
				Gizmos.DrawLine(center, center + rightVector);
			}
			else if (!left && right)
			{
				Gizmos.DrawLine(center, center + leftVector);
			}
			return;
		}
		if (left)
		{
			Gizmos.DrawLine(center, center + leftVector + rightVector);
		}
		if (right)
		{
			Gizmos.DrawLine(center + leftVector + rightVector, center);
		}
	}

	private static Vector2 GetLocalCellPosition(float x, float y, PTargetGridJob gridJob)
	{
		Vector2 vector = new Vector2(gridJob.cellSizeX * (float)gridJob.gridSizeX, gridJob.cellSizeY * (float)gridJob.gridSizeY);
		Vector2 vector2 = vector * 0.5f;
		return new Vector2(x * gridJob.cellSizeX, y * gridJob.cellSizeX) - vector2;
	}

	private void OnDrawGizmosSelected()
	{
		if (!(targetProfile == null))
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = new Color(0.3f, 0.9f, 0.1f);
			float targetWidth = targetProfile.targetWidth;
			float targetHeight = targetProfile.targetHeight;
			int gridSizeX = targetProfile.gridSizeX;
			int gridSizeY = targetProfile.gridSizeY;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(targetWidth, targetHeight));
			float num = targetWidth * 0.5f;
			float num2 = targetHeight * 0.5f;
			float num3 = targetWidth / (float)gridSizeX;
			float num4 = targetHeight / (float)gridSizeY;
			for (float num5 = 0f - num + num3; num5 < num; num5 += num3)
			{
				Gizmos.DrawLine(new Vector3(num5, 0f - num2), new Vector3(num5, num2));
			}
			for (float num6 = 0f - num2 + num4; num6 < num2; num6 += num4)
			{
				Gizmos.DrawLine(new Vector3(0f - num, num6), new Vector3(num, num6));
			}
			Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
			Gizmos.DrawLine(new Vector3(0f, targetHeight * 0.5f, -0.1f), new Vector3(0f, targetHeight * 0.5f, -0.3f));
			Gizmos.DrawLine(new Vector3(0.05f, targetHeight * 0.5f, -0.2f), new Vector3(0f, targetHeight * 0.5f, -0.3f));
			Gizmos.DrawLine(new Vector3(-0.05f, targetHeight * 0.5f, -0.2f), new Vector3(0f, targetHeight * 0.5f, -0.3f));
			Gizmos.DrawLine(new Vector3(0f, (0f - targetHeight) * 0.5f, -0.1f), new Vector3(0f, (0f - targetHeight) * 0.5f, -0.3f));
			Gizmos.DrawLine(new Vector3(0.05f, (0f - targetHeight) * 0.5f, -0.2f), new Vector3(0f, (0f - targetHeight) * 0.5f, -0.3f));
			Gizmos.DrawLine(new Vector3(-0.05f, (0f - targetHeight) * 0.5f, -0.2f), new Vector3(0f, (0f - targetHeight) * 0.5f, -0.3f));
		}
	}
}
