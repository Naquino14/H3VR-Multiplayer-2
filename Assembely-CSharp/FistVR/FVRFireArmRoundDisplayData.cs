using System;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu]
	public class FVRFireArmRoundDisplayData : ScriptableObject
	{
		[Serializable]
		public class DisplayDataClass
		{
			public string Name;

			public FireArmRoundClass Class;

			private Mesh m_mesh;

			private Material m_material;

			public FVRObject ObjectID;

			public Mesh Mesh
			{
				get
				{
					if (m_mesh == null)
					{
						m_mesh = ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().UnfiredRenderer.GetComponent<MeshFilter>().sharedMesh;
					}
					return m_mesh;
				}
			}

			public Material Material
			{
				get
				{
					if (m_material == null)
					{
						m_material = ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().UnfiredRenderer.sharedMaterial;
					}
					return m_material;
				}
			}
		}

		public string DisplayName = string.Empty;

		public FireArmRoundType Type;

		public FVRObject.OTagFirearmRoundPower RoundPower;

		public DisplayDataClass[] Classes;

		public bool IsMeatFortress;

		public AnimationCurve BDCC;

		public int ZeroWhichAmmo;

		public float ZeroingVel;

		public float ZeroingMass;

		public float ZeroingXDim;

		public AnimationCurve BulletDropCurve;

		public AnimationCurve BulletWindCurve;

		public AnimationCurve VelMultByBarrelLengthCurve;

		public DisplayDataClass GetDisplayClass(FireArmRoundClass c)
		{
			for (int i = 0; i < Classes.Length; i++)
			{
				if (Classes[i].Class == c)
				{
					return Classes[i];
				}
			}
			return null;
		}

		[ContextMenu("PopulateBasics")]
		public void PopulateBasics()
		{
			BallisticProjectile component = Classes[ZeroWhichAmmo].ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().BallisticProjectilePrefab.GetComponent<BallisticProjectile>();
			ZeroingVel = component.MuzzleVelocityBase;
			ZeroingMass = component.Mass;
			ZeroingXDim = component.Dimensions.x;
		}

		[ContextMenu("TestVel")]
		public void GetTestVel()
		{
			Keyframe[] array = new Keyframe[11];
			for (int i = 0; i < array.Length; i++)
			{
				ref Keyframe reference = ref array[i];
				reference = new Keyframe((float)i * 0.1f, 0f);
			}
			Keyframe[] array2 = new Keyframe[11];
			for (int j = 0; j < array2.Length; j++)
			{
				ref Keyframe reference2 = ref array2[j];
				reference2 = new Keyframe((float)j * 0.1f, 0f);
			}
			Vector3 vector = Vector3.zero;
			float num = 0f;
			float num2 = 0.012f;
			Vector3 vector2 = new Vector3(0f, 0f, ZeroingVel);
			float zeroingMass = ZeroingMass;
			float zeroingXDim = ZeroingXDim;
			bool flag = false;
			int num3 = 0;
			for (int k = 0; k < 1000; k++)
			{
				if (vector.z >= (float)num3 * 100f && num3 < array.Length)
				{
					array[num3].value = vector.y;
					array2[num3].value = vector.x;
					num3++;
				}
				if (vector.z >= 1000f)
				{
					Debug.Log("Height at 1000m is " + vector.y + " at " + k + "step, with " + vector2.magnitude + " vel");
					break;
				}
				if (!flag && vector2.magnitude < 430f)
				{
					flag = true;
					Debug.Log("Dropped subsonic at distance of" + vector.z + " and height of" + vector.y + " at " + k + "step, with " + vector2.magnitude + " vel");
				}
				float materialDensity = 1.225f;
				vector2 += Vector3.down * 9.81f * num2;
				vector2 = ApplyDrag(vector2, materialDensity, num2, zeroingXDim, zeroingMass);
				Vector3 vector3 = vector + vector2 * num2;
				num += Vector3.Distance(vector3, vector);
				vector = vector3;
			}
			BulletDropCurve = new AnimationCurve(array);
			BulletWindCurve = new AnimationCurve(array2);
			for (int l = 0; l < array.Length; l++)
			{
				BulletDropCurve.SmoothTangents(l, 1f);
			}
			for (int m = 0; m < array2.Length; m++)
			{
				BulletDropCurve.SmoothTangents(m, 1f);
			}
		}

		private Vector3 ApplyDrag(Vector3 velocity, float materialDensity, float time, float XDim, float Mass)
		{
			float num = (float)Math.PI * Mathf.Pow(XDim * 0.5f, 2f);
			float magnitude = velocity.magnitude;
			Vector3 normalized = velocity.normalized;
			float currentDragCoefficient = GetCurrentDragCoefficient(velocity.magnitude);
			return normalized * Mathf.Clamp(magnitude - (-velocity * (materialDensity * 0.5f * currentDragCoefficient * num / Mass) * magnitude).magnitude * time, 0f, magnitude);
		}

		private float GetCurrentDragCoefficient(float velocityMS)
		{
			return BDCC.Evaluate(velocityMS * 0.00291545f);
		}
	}
}
