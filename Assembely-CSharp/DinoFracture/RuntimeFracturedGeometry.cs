using UnityEngine;

namespace DinoFracture
{
	public class RuntimeFracturedGeometry : FractureGeometry
	{
		public bool Asynchronous = true;

		protected override AsyncFractureResult FractureInternal(Vector3 localPos)
		{
			FractureDetails details = default(FractureDetails);
			details.NumPieces = NumFracturePieces;
			details.NumIterations = NumIterations;
			details.UVScale = FractureUVScale.Piece;
			details.FractureCenter = localPos;
			details.FractureRadius = FractureRadius;
			details.Asynchronous = Asynchronous;
			return Fracture(details, hideAfterFracture: true);
		}
	}
}
