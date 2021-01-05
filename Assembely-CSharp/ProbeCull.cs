using UnityEngine;

public class ProbeCull : MonoBehaviour
{
	private ProbeCullTrack m_track;

	public float RadiusScale = 1f;

	private ReflectionProbe m_probe;

	private void OnEnable()
	{
		m_track.Probe = GetComponent<ReflectionProbe>();
		m_track.BoundingSphere = new BoundingSphere(base.transform.position, m_track.Probe.bounds.extents.magnitude * RadiusScale);
		m_track.Enabled = m_track.Probe.enabled;
		Tracker<ProbeCullTrack>.Register(m_track);
	}

	private void OnDisable()
	{
		Tracker<ProbeCullTrack>.Deregister(m_track);
	}
}
