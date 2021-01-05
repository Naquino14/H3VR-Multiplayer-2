using UnityEngine;

public class VrTrailTest : MonoBehaviour
{
	private int m_render;

	private float m_lisA;

	private float m_lisB;

	private Vector3 m_offset;

	private VRTrail m_trail;

	private void Awake()
	{
		m_trail = GetComponent<VRTrail>();
		m_lisA = Random.value / 10f;
		m_lisB = Random.value / 10f;
		m_offset = base.transform.position;
	}

	private void Update()
	{
		m_trail.AddPosition(m_offset + new Vector3(Mathf.Sin(m_lisA * (float)m_render), Mathf.Sin(m_lisB * (float)m_render)));
		m_render++;
	}
}
