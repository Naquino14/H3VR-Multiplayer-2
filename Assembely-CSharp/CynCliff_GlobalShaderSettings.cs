using UnityEngine;

[ExecuteInEditMode]
public class CynCliff_GlobalShaderSettings : MonoBehaviour
{
	[Header("Grass Settings")]
	public Texture2D breakupVariationTex;

	public float breakupVariationTiling = 0.003f;

	[Range(0f, 1f)]
	public float breakupIntensity = 1f;

	[Range(0f, 1f)]
	public float variationIntensity = 1f;

	[ColorUsage(false, true, 0f, 3f, 1f, 1f)]
	public Color grassVariationTintDark = new Color(0.6f, 0.46f, 0.25f, 1f);

	[ColorUsage(false, true, 0f, 3f, 1f, 1f)]
	public Color grassVariationTintLight = new Color(1.05f, 1.05f, 0.87f, 1f);

	[Range(0f, 3f)]
	public float grassFuzzMultiplier = 1.2f;

	public float grassAngleMin = 0.92f;

	public float grassAngleMax = 1.45f;

	public float grassAnglePower = 5.61f;

	private void UpdateGlobalShaderParameters()
	{
		if (breakupVariationTex != null)
		{
			Shader.SetGlobalTexture("_BreakupVariationTex", breakupVariationTex);
		}
		Shader.SetGlobalFloat("_BreakupVariationTiling", breakupVariationTiling);
		Shader.SetGlobalFloat("_BreakupIntensity", breakupIntensity);
		Shader.SetGlobalFloat("_VariationIntensity", variationIntensity);
		Shader.SetGlobalColor("_GrassVariationTintDark", grassVariationTintDark);
		Shader.SetGlobalColor("_GrassVariationTintLight", grassVariationTintLight);
		Shader.SetGlobalFloat("_GrassFuzzMultiplier", grassFuzzMultiplier);
		Shader.SetGlobalFloat("_GrassAngleMin", grassAngleMin);
		Shader.SetGlobalFloat("_GrassAngleMax", grassAngleMax);
		Shader.SetGlobalFloat("_GrassAnglePower", grassAnglePower);
	}

	private void Start()
	{
		UpdateGlobalShaderParameters();
	}
}
