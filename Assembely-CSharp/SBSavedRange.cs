using System;
using System.Collections.Generic;

[Serializable]
public class SBSavedRange
{
	public string SceneName;

	public int VersionNumber;

	public List<SBPlaceableDef> Placeables;

	public SerializableStringDictionary SceneFlags;

	public SerializableStringDictionary TeamFlags;
}
