using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakerySkyLight : MonoBehaviour
{
	public string texName = "sky.dds";

	public Color color = Color.white;

	public float intensity = 1f;

	public int samples = 32;

	public bool hemispherical;

	public int bitmask = 1;

	public bool bakeToIndirect = true;

	public float indirectIntensity = 1f;

	public bool tangentSH;

	public Cubemap cubemap;

	public int UID;
}
