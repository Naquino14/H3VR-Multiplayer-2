using UnityEngine;

namespace FistVR
{
	public class AIWeinerBotBodyController : AIBodyPiece
	{
		public Transform AimingTransform;

		public AIMeleeWeapon[] Weapons;

		public AISensorSystem SensorSystem;

		public void SetFireAtWill(bool b)
		{
			for (int i = 0; i < Weapons.Length; i++)
			{
				if (Weapons[i] != null)
				{
					Weapons[i].SetFireAtWill(b);
				}
			}
		}

		public void SetTargetPoint(Vector3 v)
		{
		}
	}
}
