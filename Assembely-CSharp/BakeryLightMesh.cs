using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryLightMesh : MonoBehaviour
{
	public int UID;

	public static List<MeshFilter> All = new List<MeshFilter>();

	public Color color = Color.white;

	public float intensity = 1f;

	public Texture2D texture;

	public float cutoff = 100f;

	public int samples = 256;

	public int samples2 = 16;

	public int bitmask = 1;

	public bool selfShadow = true;

	public bool bakeToIndirect = true;

	public float indirectIntensity = 1f;

	public int lmid = -2;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		if (component != null)
		{
			Gizmos.DrawWireSphere(component.bounds.center, cutoff);
		}
	}
}
