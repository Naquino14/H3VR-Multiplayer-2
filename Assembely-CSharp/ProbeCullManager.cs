using System;
using UnityEngine;

public class ProbeCullManager : MonoBehaviour
{
	public Camera Camera;

	private CullingGroup m_cullGroup;

	private BoundingSphere[] m_bounds = new BoundingSphere[64];

	private void Awake()
	{
		Camera = Camera.main;
		m_cullGroup = new CullingGroup();
		m_cullGroup.SetBoundingSphereCount(0);
		m_cullGroup.SetBoundingSpheres(m_bounds);
		m_cullGroup.targetCamera = Camera;
	}

	private void Update()
	{
		while (m_bounds.Length < Tracker<ProbeCullTrack>.Count)
		{
			Array.Resize(ref m_bounds, m_bounds.Length * 2);
			m_cullGroup.SetBoundingSpheres(m_bounds);
		}
		m_cullGroup.SetBoundingSphereCount(Tracker<ProbeCullTrack>.Count);
		for (int i = 0; i < Tracker<ProbeCullTrack>.Count; i++)
		{
			ref BoundingSphere reference = ref m_bounds[i];
			reference = Tracker<ProbeCullTrack>.All[i].BoundingSphere;
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < Tracker<ProbeCullTrack>.Count; i++)
		{
			bool flag = m_cullGroup.IsVisible(i);
			ProbeCullTrack probeCullTrack = Tracker<ProbeCullTrack>.All[i];
			if (probeCullTrack.Enabled != flag)
			{
				probeCullTrack.Probe.enabled = flag;
				Tracker<ProbeCullTrack>.All[i].Enabled = flag;
			}
		}
	}

	private void OnDestroy()
	{
		m_cullGroup.Dispose();
	}
}
