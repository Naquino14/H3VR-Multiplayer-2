using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FlintlockFlint : FVRPhysicalObject
	{
		public Vector3 m_flintUses = Vector3.one;

		public MeshFilter FlintMesh;

		public List<Mesh> FlintMeshes;

		protected override void Awake()
		{
			base.Awake();
			m_flintUses = new Vector3(Random.Range(8, 15), Random.Range(5, 9), Random.Range(4, 8));
		}

		public void UpdateState()
		{
			if (m_flintUses.x > 0f)
			{
				SetFlintState(FlintlockWeapon.FlintState.New);
			}
			else if (m_flintUses.y > 0f)
			{
				SetFlintState(FlintlockWeapon.FlintState.Used);
			}
			else if (m_flintUses.z > 0f)
			{
				SetFlintState(FlintlockWeapon.FlintState.Worn);
			}
			else
			{
				SetFlintState(FlintlockWeapon.FlintState.Broken);
			}
		}

		public void SetFlintState(FlintlockWeapon.FlintState f)
		{
			FlintMesh.mesh = FlintMeshes[(int)f];
		}
	}
}
