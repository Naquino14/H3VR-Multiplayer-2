using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VRTrailManager : MonoBehaviour
{
	public static VRTrailManager Instance;

	public Shader TrailShader;

	private List<VRTrail> m_trails = new List<VRTrail>();

	private HashSet<Camera> m_setCameras = new HashSet<Camera>();

	private CommandBuffer m_cmdBuffer;

	private int m_colId;

	private void Awake()
	{
		m_colId = Shader.PropertyToID("_Color");
		Instance = this;
		m_cmdBuffer = new CommandBuffer();
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(OnCam));
	}

	private void OnDestroy()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(OnCam));
	}

	public void Register(VRTrail trail)
	{
		m_trails.Add(trail);
	}

	public void Deregister(VRTrail trail)
	{
		int num = m_trails.IndexOf(trail);
		if (num != -1)
		{
			m_trails[num] = m_trails[m_trails.Count - 1];
			m_trails.RemoveAt(m_trails.Count - 1);
		}
	}

	private void Update()
	{
		Matrix4x4 identity = Matrix4x4.identity;
		m_cmdBuffer.Clear();
		for (int i = 0; i < m_trails.Count; i++)
		{
			VRTrail vRTrail = m_trails[i];
			if (vRTrail.NumPositions > 1)
			{
				vRTrail.SetData();
				vRTrail.Material.SetColor(m_colId, vRTrail.Color);
				m_cmdBuffer.DrawProcedural(identity, vRTrail.Material, 0, MeshTopology.LineStrip, vRTrail.NumPositions - 1, 1);
			}
		}
	}

	private void OnCam(Camera cam)
	{
		if (m_setCameras.Add(cam))
		{
			cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, m_cmdBuffer);
		}
	}
}
