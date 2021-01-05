using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "Entities/EntityDefinition", order = 0)]
	public class FVREntityDefinition : ScriptableObjectHasWeakLink
	{
		public string EntityID = string.Empty;

		public GameObject MainPrefab;

		public GameObject ProxyPrefab;

		public Sprite Sprite;

		public string DisplayName;

		[TextArea(3, 13)]
		public string Details;

		public List<FVREntityTag> Tags;

		public Vector3 ProxyExtents = new Vector3(1f, 1f, 1f);

		public override void OnBuild()
		{
		}
	}
}
