using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Mode Profile", menuName = "HG/ModeProfile", order = 0)]
	public class HG_ModeProfile : ScriptableObject
	{
		public string Title;

		[Multiline(20)]
		public string DescriptionText;
	}
}
