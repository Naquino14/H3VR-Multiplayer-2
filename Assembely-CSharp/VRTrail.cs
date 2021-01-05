using System;
using UnityEngine;

public class VRTrail : MonoBehaviour
{
	[ColorUsage(true, true, 0f, 50f, 0.125f, 3f)]
	public Color Color = Color.white;

	private Vector3[] m_positions;

	private int m_numPositions;

	public ComputeBuffer Buffer;

	private bool m_dirty;

	[NonSerialized]
	public Material Material;

	public int NumPositions => m_numPositions;

	private void Awake()
	{
		m_positions = new Vector3[256];
		AllocBuffer();
		VRTrailManager.Instance.Register(this);
		Material = new Material(VRTrailManager.Instance.TrailShader);
		Material.SetBuffer("_PosBuffer", Buffer);
	}

	private void AllocBuffer()
	{
		Buffer = new ComputeBuffer(m_positions.Length, 12);
	}

	private void OnDestroy()
	{
		Buffer.Dispose();
		VRTrailManager.Instance.Deregister(this);
	}

	public void AddPosition(Vector3 pos)
	{
		if (m_numPositions >= m_positions.Length)
		{
			Array.Resize(ref m_positions, m_numPositions + 256);
			Buffer.Dispose();
			AllocBuffer();
			SetData();
		}
		m_positions[m_numPositions] = pos;
		m_numPositions++;
		m_dirty = true;
	}

	public void SetData()
	{
		if (m_dirty)
		{
			Buffer.SetData(m_positions);
			m_dirty = false;
		}
	}
}
