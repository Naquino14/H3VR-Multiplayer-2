using UnityEngine;

[CreateAssetMenu]
public class PTargetProfile : ScriptableObject
{
	public string displayName;

	public Sprite displayIcon;

	public string displayDetails;

	public float healthMultiplier = 1f;

	public int gridSizeX = 32;

	public int gridSizeY = 32;

	public float targetWidth = 0.5f;

	public float targetHeight = 0.5f;

	public int renderTextureResolutionX = 1024;

	public int renderTextureResolutionY = 1024;

	public bool renderTextureUseMipmap = true;

	public FilterMode renderTextureFilterMode = FilterMode.Trilinear;

	public int renderTextureAnisoLevel = 4;

	public Texture2D scoreMap;

	public int[] scores;

	public PTargetDecal background;

	public PTargetDecal[] tearDecals;

	public PTargetDecal[] bulletDecals;

	public float cellWidth => targetWidth / (float)gridSizeX;

	public float cellHeight => targetHeight / (float)gridSizeY;

	public float cellArea => cellWidth * cellHeight;

	public float cellHealth => cellArea * healthMultiplier;
}
