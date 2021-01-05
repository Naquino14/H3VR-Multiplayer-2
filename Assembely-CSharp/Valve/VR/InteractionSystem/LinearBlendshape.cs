using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class LinearBlendshape : MonoBehaviour
	{
		public LinearMapping linearMapping;

		public SkinnedMeshRenderer skinnedMesh;

		private float lastValue;

		private void Awake()
		{
			if (skinnedMesh == null)
			{
				skinnedMesh = GetComponent<SkinnedMeshRenderer>();
			}
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}
		}

		private void Update()
		{
			float value = linearMapping.value;
			if (value != lastValue)
			{
				float value2 = Util.RemapNumberClamped(value, 0f, 1f, 1f, 100f);
				skinnedMesh.SetBlendShapeWeight(0, value2);
			}
			lastValue = value;
		}
	}
}
