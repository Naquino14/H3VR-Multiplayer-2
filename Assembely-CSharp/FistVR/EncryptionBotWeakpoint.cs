using UnityEngine;

namespace FistVR
{
	public class EncryptionBotWeakpoint : MonoBehaviour, IFVRDamageable
	{
		public EncryptionBotMine Mine;

		public EncryptionBotHardened Hard;

		public EncryptionBotMissileBoat Boat;

		public EncryptionBotAgile Agile;

		public bool PinPrick = true;

		public bool DodgePoint;

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Explosive && d.Dam_TotalKinetic < 2000f)
			{
				return;
			}
			if (Mine != null)
			{
				Mine.Explode();
			}
			if (Hard != null)
			{
				Hard.Explode();
			}
			if (Boat != null)
			{
				Boat.Explode();
			}
			if (Agile != null)
			{
				if (DodgePoint && d.Dam_TotalKinetic < 10000f)
				{
					Agile.Evade(d.strikeDir);
				}
				else
				{
					Agile.Explode();
				}
			}
		}
	}
}
