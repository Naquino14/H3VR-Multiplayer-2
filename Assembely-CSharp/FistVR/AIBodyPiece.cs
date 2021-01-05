using UnityEngine;

namespace FistVR
{
	public class AIBodyPiece : FVRDestroyableObject
	{
		[Header("AI Body Piece Config")]
		public AIDamagePlate[] DamagePlates;

		public bool UsesPlateSystem = true;

		public bool IsPlateDamaged;

		public bool IsPlateDisabled;

		public override void Awake()
		{
			base.Awake();
		}

		public override void Update()
		{
			base.Update();
			if (UsesPlateSystem)
			{
				CheckPlateDamage();
			}
		}

		public virtual void FixedUpdate()
		{
		}

		public virtual void CheckPlateDamage()
		{
			int num = 0;
			for (int i = 0; i < DamagePlates.Length; i++)
			{
				if (DamagePlates[i].IsDown)
				{
					num++;
				}
			}
			if (num > 0)
			{
				SetPlateDamaged(b: true);
			}
			else
			{
				SetPlateDamaged(b: false);
			}
			if (num > 1)
			{
				SetPlateDisabled(b: true);
			}
			else
			{
				SetPlateDisabled(b: false);
			}
		}

		public void ResetAllPlates()
		{
			for (int i = 0; i < DamagePlates.Length; i++)
			{
				DamagePlates[i].Reset();
			}
			IsPlateDamaged = false;
			IsPlateDisabled = false;
		}

		public virtual bool SetPlateDamaged(bool b)
		{
			if (IsPlateDamaged != b)
			{
				IsPlateDamaged = b;
				return true;
			}
			return false;
		}

		public virtual bool SetPlateDisabled(bool b)
		{
			if (IsPlateDisabled != b)
			{
				IsPlateDisabled = b;
				return true;
			}
			return false;
		}
	}
}
