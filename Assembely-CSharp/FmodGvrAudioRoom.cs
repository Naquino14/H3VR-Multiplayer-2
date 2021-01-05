using UnityEngine;

[AddComponentMenu("GoogleVR/Audio/FmodGvrAudioRoom")]
public class FmodGvrAudioRoom : MonoBehaviour
{
	public enum SurfaceMaterial
	{
		Transparent,
		AcousticCeilingTiles,
		BrickBare,
		BrickPainted,
		ConcreteBlockCoarse,
		ConcreteBlockPainted,
		CurtainHeavy,
		FiberglassInsulation,
		GlassThin,
		GlassThick,
		Grass,
		LinoleumOnConcrete,
		Marble,
		Metal,
		ParquetOnConcrete,
		PlasterRough,
		PlasterSmooth,
		PlywoodPanel,
		PolishedConcreteOrTile,
		Sheetrock,
		WaterOrIceSurface,
		WoodCeiling,
		WoodPanel
	}

	public SurfaceMaterial leftWall = SurfaceMaterial.ConcreteBlockCoarse;

	public SurfaceMaterial rightWall = SurfaceMaterial.ConcreteBlockCoarse;

	public SurfaceMaterial floor = SurfaceMaterial.ParquetOnConcrete;

	public SurfaceMaterial ceiling = SurfaceMaterial.PlasterRough;

	public SurfaceMaterial backWall = SurfaceMaterial.ConcreteBlockCoarse;

	public SurfaceMaterial frontWall = SurfaceMaterial.ConcreteBlockCoarse;

	public float reflectivity = 1f;

	public float reverbGainDb;

	public float reverbBrightness;

	public float reverbTime = 1f;

	public Vector3 size = Vector3.one;

	private void OnEnable()
	{
		FmodGvrAudio.UpdateAudioRoom(this, FmodGvrAudio.IsListenerInsideRoom(this));
	}

	private void OnDisable()
	{
		FmodGvrAudio.UpdateAudioRoom(this, roomEnabled: false);
	}

	private void Update()
	{
		FmodGvrAudio.UpdateAudioRoom(this, FmodGvrAudio.IsListenerInsideRoom(this));
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, size);
	}
}
