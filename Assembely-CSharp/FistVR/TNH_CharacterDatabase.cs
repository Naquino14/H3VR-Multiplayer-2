using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_CharacterDatabase", menuName = "TNH/CharacterDatabase", order = 0)]
	public class TNH_CharacterDatabase : ScriptableObject
	{
		public List<TNH_CharacterDef> Characters;

		public TNH_CharacterDef GetDef(TNH_Char c)
		{
			for (int i = 0; i < Characters.Count; i++)
			{
				if (Characters[i].CharacterID == c)
				{
					return Characters[i];
				}
			}
			return null;
		}
	}
}
