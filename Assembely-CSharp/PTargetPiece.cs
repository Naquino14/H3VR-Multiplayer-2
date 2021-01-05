using System;
using FistVR;
using UnityEngine;

[Serializable]
public class PTargetPiece : MonoBehaviour, IOnPTargetChangeUnsafe, IFVRDamageable
{
	private const float RANDOMIZE_COLLIDER_SCALE = 0.002f;

	[HideInInspector]
	public int componentLabel = 1;

	[HideInInspector]
	public int lastComponentLabel = 1;

	public PTarget parentTarget;

	public IPTargetUnsafeFunctions unsafeFunctions;

	public MeshFilter meshFilter;

	public BoxCollider boxCollider;

	[HideInInspector]
	public int initialLabel = -1;

	[NonSerialized]
	private Mesh mesh;

	[NonSerialized]
	private bool isAttachedToTarget = true;

	[NonSerialized]
	private bool initialized;

	[NonSerialized]
	private bool firstUpdate = true;

	[NonSerialized]
	private float thickness = 0.05f;

	public void Initialize()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		if (parentTarget == null)
		{
			MonoBehaviour.print("No Parent target found, Please make sure the parentTarget variable is assigned before initializing the target piece.");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		unsafeFunctions = parentTarget;
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		if (meshFilter == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		if (boxCollider == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		thickness = parentTarget.targetPieceColliderThickness + UnityEngine.Random.Range(-0.002f, 0.002f);
		parentTarget.onTargetChange.Add(this);
		mesh = new Mesh();
		meshFilter.sharedMesh = mesh;
		lastComponentLabel = componentLabel;
		unsafeFunctions.ApplyLabelMesh(componentLabel, mesh);
	}

	private void OnDestroy()
	{
		if (parentTarget != null && parentTarget.onTargetChange != null)
		{
			parentTarget.onTargetChange.Remove(this);
		}
	}

	void IOnPTargetChangeUnsafe.OnTargetChange()
	{
		Initialize();
		if (parentTarget == null)
		{
			return;
		}
		if (!firstUpdate)
		{
			int currentLabelFromLastLabel = unsafeFunctions.GetCurrentLabelFromLastLabel(componentLabel);
			if (currentLabelFromLastLabel != componentLabel)
			{
				lastComponentLabel = componentLabel;
			}
			componentLabel = currentLabelFromLastLabel;
		}
		else
		{
			initialLabel = unsafeFunctions.GetCurrentLabelFromLastLabel(componentLabel);
		}
		firstUpdate = false;
		if (componentLabel == 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		unsafeFunctions.ApplyLabelMesh(componentLabel, mesh);
		Rect labelRect = unsafeFunctions.GetLabelRect(componentLabel);
		Vector3 center = boxCollider.center;
		Vector3 size = boxCollider.size;
		Vector3 vector = labelRect.center;
		Vector3 size2 = labelRect.size;
		size2.z = thickness;
		if (!Mathf.Approximately(center.x, vector.x) || !Mathf.Approximately(center.y, vector.y) || !Mathf.Approximately(center.z, vector.z) || !Mathf.Approximately(size.x, size2.x) || !Mathf.Approximately(size.y, size2.y) || !Mathf.Approximately(size.z, size2.z))
		{
			boxCollider.center = labelRect.center;
			boxCollider.size = size2;
		}
		bool flag = unsafeFunctions.IsAttached(componentLabel);
		if (isAttachedToTarget && !flag)
		{
			base.transform.SetParent(null);
			Rigidbody component = base.gameObject.GetComponent<Rigidbody>();
			component.isKinematic = false;
			component.mass = parentTarget.targetPieceMass;
			component.drag = parentTarget.targetPieceDrag;
			component.angularDrag = parentTarget.targetPieceAngularDrag;
			component.maxAngularVelocity = parentTarget.targetPieceMaxAngularVelocity;
			float targetPieceRandomAngularVelocityScale = parentTarget.targetPieceRandomAngularVelocityScale;
			component.angularVelocity = new Vector3(UnityEngine.Random.Range(0f - targetPieceRandomAngularVelocityScale, targetPieceRandomAngularVelocityScale), UnityEngine.Random.Range(0f - targetPieceRandomAngularVelocityScale, targetPieceRandomAngularVelocityScale), UnityEngine.Random.Range(0f - targetPieceRandomAngularVelocityScale, targetPieceRandomAngularVelocityScale));
			isAttachedToTarget = flag;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		BoxCollider component = GetComponent<BoxCollider>();
		if (!(component == null))
		{
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
			Gizmos.DrawWireCube(component.center, component.size);
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.25f);
			Gizmos.DrawWireCube(component.center, component.size);
		}
	}

	public void AddBullet(Vector3 rayOrigin, Vector3 rayDir, float damageSize)
	{
		int decalIndex = 0;
		if (parentTarget != null)
		{
			PTargetProfile targetProfile = parentTarget.targetProfile;
			if (targetProfile != null)
			{
				decalIndex = UnityEngine.Random.Range(0, targetProfile.bulletDecals.Length);
			}
		}
		AddBullet(rayOrigin, rayDir, damageSize, decalIndex);
	}

	public void AddBullet(Vector3 rayOrigin, Vector3 rayDir, float damageSize, int decalIndex)
	{
		Transform transform = base.transform;
		Vector3 forward = transform.forward;
		if (!(Vector3.Dot(rayDir, forward) < 0f) || !isAttachedToTarget)
		{
			Vector3 position = transform.position;
			if (new Plane(forward, position).Raycast(new Ray(rayOrigin, rayDir), out var enter))
			{
				Vector3 position2 = rayOrigin + rayDir * enter;
				Vector3 localPosition = transform.InverseTransformPoint(position2);
				Vector2 uV = parentTarget.GetUV(localPosition);
				parentTarget.AddBullet(componentLabel, decalIndex, uV, 0f, damageSize);
			}
		}
	}

	void IFVRDamageable.Damage(Damage dam)
	{
		Transform transform = base.transform;
		Vector3 strikeDir = dam.strikeDir;
		Vector3 rayOrigin = dam.point - strikeDir;
		AddBullet(rayOrigin, strikeDir, dam.damageSize);
	}
}
