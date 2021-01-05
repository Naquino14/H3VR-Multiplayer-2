using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AKMSightCalibrator : MonoBehaviour
	{
		public List<float> Drops;

		public List<float> DropDistances;

		public Transform Muzzle;

		public Transform FrontSightPoint;

		public int selected;

		public Transform Chamber;

		public FVRFireArmRoundDisplayData Data;

		public int ZeroWhich;

		public float ZeroingVel;

		public float ZeroingMass;

		public float ZeroingXDim;

		public AnimationCurve BDCC;

		public AnimationCurve DropCurve;

		[ContextMenu("PopulateBasics")]
		public void PopulateBasics()
		{
			BallisticProjectile component = Data.Classes[ZeroWhich].ObjectID.GetGameObject().GetComponent<FVRFireArmRound>().BallisticProjectilePrefab.GetComponent<BallisticProjectile>();
			ZeroingVel = component.MuzzleVelocityBase;
			ZeroingMass = component.Mass;
			ZeroingXDim = component.Dimensions.x;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void OnDrawGizmos()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			for (int i = 0; i < Drops.Count; i++)
			{
				Vector3 vector = Muzzle.position + Muzzle.forward * DropDistances[i] + Vector3.up * Drops[i];
				Vector3 vector2 = FrontSightPoint.position - vector;
				Vector3 vector3 = vector2 + vector2.normalized;
				if (i == selected)
				{
					Gizmos.color = Color.green;
				}
				else
				{
					Gizmos.color = Color.Lerp(Color.red, Color.yellow, (float)i / 11f);
				}
				Gizmos.DrawLine(vector, vector + vector3);
			}
		}

		[ContextMenu("TestVel")]
		public void GetTestVel()
		{
			Keyframe[] array = new Keyframe[21];
			for (int i = 0; i < array.Length; i++)
			{
				ref Keyframe reference = ref array[i];
				reference = new Keyframe((float)i * 0.1f, 0f);
			}
			Vector3 vector = Vector3.zero;
			float num = 0f;
			float num2 = 0.012f;
			float zeroingVel = ZeroingVel;
			float time = Vector3.Distance(Chamber.position, Muzzle.position) * 39.3701f;
			float num3 = Data.VelMultByBarrelLengthCurve.Evaluate(time);
			zeroingVel *= num3;
			Vector3 vector2 = new Vector3(0f, 0f, zeroingVel);
			float zeroingMass = ZeroingMass;
			float zeroingXDim = ZeroingXDim;
			bool flag = false;
			int num4 = 0;
			for (int j = 0; j < 2000; j++)
			{
				if (vector.z >= (float)num4 * 100f && num4 < array.Length)
				{
					array[num4].value = vector.y;
					num4++;
				}
				if (vector.z >= 2000f)
				{
					Debug.Log("Height at 1000m is " + vector.y + " at " + j + "step, with " + vector2.magnitude + " vel");
					break;
				}
				if (!flag && vector2.magnitude < 430f)
				{
					flag = true;
					Debug.Log("Dropped subsonic at distance of" + vector.z + " and height of" + vector.y + " at " + j + "step, with " + vector2.magnitude + " vel");
				}
				float materialDensity = 1.225f;
				vector2 += Vector3.down * 9.81f * num2;
				vector2 = ApplyDrag(vector2, materialDensity, num2, zeroingXDim, zeroingMass);
				Vector3 vector3 = vector + vector2 * num2;
				num += Vector3.Distance(vector3, vector);
				vector = vector3;
			}
			DropCurve = new AnimationCurve(array);
			for (int k = 0; k < array.Length; k++)
			{
				DropCurve.SmoothTangents(k, 1f);
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

		[ContextMenu("PopDrops")]
		public void PopDrops()
		{
			for (int i = 0; i < Drops.Count; i++)
			{
				Drops[i] = DropCurve.Evaluate(DropDistances[i] * 0.001f);
			}
		}
	}
}
