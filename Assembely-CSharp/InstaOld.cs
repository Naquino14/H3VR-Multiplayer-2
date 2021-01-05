using UnityEngine;

[ExecuteInEditMode]
public class InstaOld : MonoBehaviour
{
	private static class Uniforms
	{
		internal static readonly int _Grain_Params1 = Shader.PropertyToID("_Grain_Params1");

		internal static readonly int _Grain_Params2 = Shader.PropertyToID("_Grain_Params2");

		internal static readonly int _GrainTex = Shader.PropertyToID("_GrainTex");

		internal static readonly int _Phase = Shader.PropertyToID("_Phase");
	}

	public Texture2D Lut;

	public float VignetteStrength;

	public Color VignetteColor;

	public ShaderVariantCollection Collection;

	[Header("Grain")]
	[Tooltip("Enable the use of colored grain.")]
	public bool colored;

	[Range(0f, 1f)]
	[Tooltip("Grain strength. Higher means more visible grain.")]
	public float intensity;

	[Range(0.3f, 3f)]
	[Tooltip("Grain particle size.")]
	public float size;

	[Range(0f, 1f)]
	[Tooltip("Controls the noisiness response curve based on scene luminance. Lower values mean less noise in dark areas.")]
	public float luminanceContribution;

	private RenderTexture m_GrainLookupRT;

	private Material m_grainGenMat;

	private void OnEnable()
	{
		m_grainGenMat = new Material(Shader.Find("Hidden/Post FX/Grain Generator"));
		m_grainGenMat.hideFlags = HideFlags.HideAndDontSave;
	}

	public void OnDisable()
	{
		Object.DestroyImmediate(m_GrainLookupRT);
		m_GrainLookupRT = null;
	}

	public void BindMaterial(Material mat)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float value = Random.value;
		float value2 = Random.value;
		if (m_GrainLookupRT == null || !m_GrainLookupRT.IsCreated())
		{
			Object.DestroyImmediate(m_GrainLookupRT);
			m_GrainLookupRT = new RenderTexture(192, 192, 0, RenderTextureFormat.ARGBHalf)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Repeat,
				anisoLevel = 0,
				name = "Grain Lookup Texture"
			};
			m_GrainLookupRT.Create();
		}
		m_grainGenMat.SetFloat(Uniforms._Phase, realtimeSinceStartup / 20f);
		Graphics.Blit(null, m_GrainLookupRT, m_grainGenMat, colored ? 1 : 0);
		mat.SetTexture(Uniforms._GrainTex, m_GrainLookupRT);
		mat.SetVector(Uniforms._Grain_Params1, new Vector2(luminanceContribution, intensity * 20f));
		mat.SetVector(Uniforms._Grain_Params2, new Vector4((float)Camera.current.pixelWidth / (float)m_GrainLookupRT.width / size, (float)Camera.current.pixelHeight / (float)m_GrainLookupRT.height / size, value, value2));
		mat.SetTexture("_InstaOldLUT", Lut);
		mat.SetFloat("_VignetteStrength", VignetteStrength);
		mat.SetColor("_VignetteColor", VignetteColor);
	}
}
