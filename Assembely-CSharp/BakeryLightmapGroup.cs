using UnityEngine;

[CreateAssetMenu(menuName = "Bakery lightmap group")]
public class BakeryLightmapGroup : ScriptableObject
{
	public enum ftLMGroupMode
	{
		OriginalUV,
		PackAtlas,
		Vertex
	}

	public enum RenderMode
	{
		FullLighting = 0,
		Indirect = 1,
		Shadowmask = 2,
		Subtractive = 3,
		AmbientOcclusionOnly = 4,
		Auto = 1000
	}

	public enum RenderDirMode
	{
		None = 0,
		BakedNormalMaps = 1,
		DominantDirection = 2,
		RNM = 3,
		SH = 4,
		ProbeSH = 5,
		Auto = 1000
	}

	[SerializeField]
	[Range(1f, 8192f)]
	public int resolution = 512;

	[SerializeField]
	public int bitmask = 1;

	[SerializeField]
	public int id = -1;

	public int sortingID = -1;

	[SerializeField]
	public bool isImplicit;

	[SerializeField]
	public float area;

	[SerializeField]
	public int totalVertexCount;

	[SerializeField]
	public int vertexCounter;

	[SerializeField]
	public int sceneLodLevel = -1;

	[SerializeField]
	public string sceneName;

	[SerializeField]
	public bool containsTerrains;

	[SerializeField]
	public bool probes;

	[SerializeField]
	public ftLMGroupMode mode = ftLMGroupMode.PackAtlas;

	[SerializeField]
	public RenderMode renderMode = RenderMode.Auto;

	[SerializeField]
	public RenderDirMode renderDirMode = RenderDirMode.Auto;

	[SerializeField]
	public bool computeSSS;

	[SerializeField]
	public int sssSamples = 16;

	[SerializeField]
	public float sssDensity = 10f;

	[SerializeField]
	public Color sssColor = Color.white;

	[SerializeField]
	public float fakeShadowBias;

	[SerializeField]
	public bool transparentSelfShadow;

	[SerializeField]
	public bool flipNormal;

	[SerializeField]
	public string overridePath = string.Empty;

	public BakeryLightmapGroupPlain GetPlainStruct()
	{
		BakeryLightmapGroupPlain result = default(BakeryLightmapGroupPlain);
		result.name = base.name;
		result.id = id;
		result.resolution = resolution;
		result.vertexBake = mode == ftLMGroupMode.Vertex;
		result.isImplicit = isImplicit;
		result.renderMode = (int)renderMode;
		result.renderDirMode = (int)renderDirMode;
		result.computeSSS = computeSSS;
		result.sssSamples = sssSamples;
		result.sssDensity = sssDensity;
		result.sssR = sssColor.r;
		result.sssG = sssColor.g;
		result.sssB = sssColor.b;
		result.containsTerrains = containsTerrains;
		result.probes = probes;
		result.fakeShadowBias = fakeShadowBias;
		result.transparentSelfShadow = transparentSelfShadow;
		result.flipNormal = flipNormal;
		return result;
	}
}
