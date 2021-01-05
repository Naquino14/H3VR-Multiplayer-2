using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_CharacterDatabase", menuName = "MeatFortress/SquadDefinition", order = 0)]
	public class MF_SquadDefinition : ScriptableObject
	{
		public List<MF_Class> MemberClasses = new List<MF_Class>();
	}
}
