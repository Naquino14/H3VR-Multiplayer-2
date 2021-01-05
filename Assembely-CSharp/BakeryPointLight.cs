using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class BakeryPointLight : MonoBehaviour
{
	public enum ftLightProjectionMode
	{
		Omni,
		Cookie,
		Cubemap,
		IES,
		Cone
	}

	public int UID;

	public Color color = Color.white;

	public float intensity = 1f;

	public float shadowSpread = 0.05f;

	public float cutoff = 10f;

	public bool realisticFalloff;

	public int samples = 8;

	public ftLightProjectionMode projMode;

	public Texture2D cookie;

	public float angle = 30f;

	public float innerAngle;

	public Cubemap cubemap;

	public Object iesFile;

	public int bitmask = 1;

	public bool bakeToIndirect;

	public bool shadowmask;

	public float indirectIntensity = 1f;

	public float falloffMinRadius = 1f;

	private const float GIZMO_MAXSIZE = 0.1f;

	private const float GIZMO_SCALE = 0.01f;

	private float screenRadius = 0.1f;
}
