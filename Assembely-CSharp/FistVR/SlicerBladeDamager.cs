using UnityEngine;

namespace FistVR
{
	public class SlicerBladeDamager : MonoBehaviour, IFVRDamageable
	{
		public SlicerBladeMaster Master;

		public int BladeIndex;

		public void Damage(Damage dam)
		{
			Master.DamageBlade(dam.Dam_TotalKinetic * 0.5f, BladeIndex, dam.point);
		}
	}
}
