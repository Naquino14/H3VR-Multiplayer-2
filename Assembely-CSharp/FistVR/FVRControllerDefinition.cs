using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "Controls/ControllerDefinition", order = 0)]
	public class FVRControllerDefinition : ScriptableObject
	{
		public string Name;

		public Vector3 PoseTransformOffset;

		public Vector3 PoseTransformRotOffset;

		public Vector3 InteractionSphereOffset;
	}
}
