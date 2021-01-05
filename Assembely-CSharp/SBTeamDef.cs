using System;
using System.Collections.Generic;

[Serializable]
public class SBTeamDef
{
	public string TeamName;

	public int VersionNumber;

	public List<SBTeamMember> Placeables;
}
