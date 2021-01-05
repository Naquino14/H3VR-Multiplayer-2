using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryDirectLight : MonoBehaviour
{
	public Color color = Color.white;

	public float intensity = 1f;

	public float shadowSpread = 0.01f;

	public int samples = 16;

	public int bitmask = 1;

	public bool bakeToIndirect;

	public bool shadowmask;

	public bool shadowmaskDenoise;

	public float indirectIntensity = 1f;

	public int UID;
}
