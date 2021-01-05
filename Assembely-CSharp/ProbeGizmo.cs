using UnityEngine;

public class ProbeGizmo : MonoBehaviour
{
	public ReflectionProbe probe;

	public bool Draw;

	private void OnDrawGizmos()
	{
		if (Draw)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(base.transform.position + probe.center, new Vector3(probe.size.x, probe.size.y, probe.size.z));
		}
	}

	[ContextMenu("attach")]
	public void attach()
	{
		probe = GetComponent<ReflectionProbe>();
	}
}
