using UnityEngine;

public class LightProbeTools : MonoBehaviour
{
	public LayerMask projectMask = -1;

	public float projectOffset = 1f;

	public float projectMaxDistance = 100000f;

	[ContextMenu("Project Down")]
	public void ProjectDown()
	{
	}
}
