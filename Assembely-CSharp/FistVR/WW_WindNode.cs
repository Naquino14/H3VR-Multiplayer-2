using UnityEngine;

namespace FistVR
{
	public class WW_WindNode : MonoBehaviour
	{
		public float WhiteOut;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawCube(base.transform.position, new Vector3(WhiteOut * 3f, WhiteOut * 3f, WhiteOut * 3f));
		}
	}
}
