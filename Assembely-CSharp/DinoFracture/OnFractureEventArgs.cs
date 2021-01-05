using UnityEngine;

namespace DinoFracture
{
	public sealed class OnFractureEventArgs
	{
		public FractureGeometry OriginalObject;

		public GameObject FracturePiecesRootObject;

		public OnFractureEventArgs(FractureGeometry orig, GameObject root)
		{
			OriginalObject = orig;
			FracturePiecesRootObject = root;
		}
	}
}
