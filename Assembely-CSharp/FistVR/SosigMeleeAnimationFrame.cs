using UnityEngine;

namespace FistVR
{
	public class SosigMeleeAnimationFrame : MonoBehaviour
	{
		public SosigMeleeAnimationWriter w;

		public int Index;

		public void OnDrawGizmos()
		{
			if (!w.ShowGizmos || w.Frames.Count <= 0)
			{
				return;
			}
			Color interpolatedColor = w.GetInterpolatedColor(Index);
			Gizmos.color = new Color(interpolatedColor.r, interpolatedColor.g, interpolatedColor.b, 0.5f);
			if (w.IsShieldGizmo)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
				Matrix4x4 matrix = Gizmos.matrix;
				Gizmos.matrix *= matrix4x;
				Gizmos.DrawWireCube(Vector3.zero, w.ShieldDimensions);
				Gizmos.matrix = matrix;
				return;
			}
			Gizmos.DrawSphere(base.transform.position, 0.025f);
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * w.WeaponLength_Forward);
			Gizmos.DrawLine(base.transform.position, base.transform.position - base.transform.forward * w.WeaponLength_Behind);
			if (w.usesForwardBit)
			{
				Gizmos.DrawLine(base.transform.position + base.transform.forward * w.WeaponLength_Forward, base.transform.position + base.transform.forward * w.WeaponLength_Forward - base.transform.up * w.WeaponLength_ForwardBit);
			}
		}

		public void OnDrawGizmosSelected()
		{
			if (!w.ShowGizmos || w.Frames.Count <= 0)
			{
				return;
			}
			Color interpolatedColor = w.GetInterpolatedColor(Index);
			Gizmos.color = new Color(interpolatedColor.r, interpolatedColor.g, interpolatedColor.b, 0.1f);
			if (w.IsShieldGizmo)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
				Matrix4x4 matrix = Gizmos.matrix;
				Gizmos.matrix *= matrix4x;
				Gizmos.DrawCube(Vector3.zero, w.ShieldDimensions * 0.95f);
				Gizmos.matrix = matrix;
				return;
			}
			Gizmos.DrawSphere(base.transform.position, 0.015f);
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * w.WeaponLength_Forward);
			Gizmos.DrawLine(base.transform.position, base.transform.position - base.transform.forward * w.WeaponLength_Behind);
			if (w.usesForwardBit)
			{
				Gizmos.DrawLine(base.transform.position + base.transform.forward * w.WeaponLength_Forward, base.transform.position + base.transform.forward * w.WeaponLength_Forward - base.transform.up * w.WeaponLength_ForwardBit);
			}
		}
	}
}
