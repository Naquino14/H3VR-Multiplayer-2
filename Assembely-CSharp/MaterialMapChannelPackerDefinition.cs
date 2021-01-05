using System;
using System.Collections.Generic;
using Alloy;
using UnityEngine;

public class MaterialMapChannelPackerDefinition : ScriptableObject
{
	public List<PackedMapDefinition> PackedMaps;

	[Header("Global settings")]
	public NormalMapChannelTextureChannelMapping NRMChannel = new NormalMapChannelTextureChannelMapping();

	[Space(15f)]
	public string VarianceText;

	public string AutoRegenerateText;

	public PackedMapDefinition PackedPack => PackedMaps[0];

	public PackedMapDefinition DetailPack => PackedMaps[1];

	public PackedMapDefinition TerrainPack => PackedMaps[2];

	public bool IsPackedMap(string path)
	{
		for (int i = 0; i < PackedMaps.Count; i++)
		{
			PackedMapDefinition packedMapDefinition = PackedMaps[i];
			if (path.EndsWith(packedMapDefinition.Suffix, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
