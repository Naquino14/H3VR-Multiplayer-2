using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class FloppyHand : MonoBehaviour
	{
		[Serializable]
		public class Finger
		{
			public enum eulerAxis
			{
				X,
				Y,
				Z
			}

			public float mass;

			[Range(0f, 1f)]
			public float pos;

			public Vector3 forwardAxis;

			public SkinnedMeshRenderer renderer;

			[HideInInspector]
			public SteamVR_Action_Single squeezyAction;

			public SteamVR_Input_Sources inputSource;

			public Transform[] bones;

			public Transform referenceBone;

			public Vector2 referenceAngles;

			public eulerAxis referenceAxis;

			[HideInInspector]
			public float flexAngle;

			private Vector3[] rotation;

			private Vector3[] velocity;

			private Transform[] boneTips;

			private Vector3[] oldTipPosition;

			private Vector3[] oldTipDelta;

			private Vector3[,] inertiaSmoothing;

			private float squeezySmooth;

			private int inertiaSteps = 10;

			private float k = 400f;

			private float damping = 8f;

			private Quaternion[] startRot;

			public void ApplyForce(Vector3 worldForce)
			{
				for (int i = 0; i < startRot.Length; i++)
				{
					velocity[i] += worldForce / 50f;
				}
			}

			public void Init()
			{
				startRot = new Quaternion[bones.Length];
				rotation = new Vector3[bones.Length];
				velocity = new Vector3[bones.Length];
				oldTipPosition = new Vector3[bones.Length];
				oldTipDelta = new Vector3[bones.Length];
				boneTips = new Transform[bones.Length];
				inertiaSmoothing = new Vector3[bones.Length, inertiaSteps];
				for (int i = 0; i < bones.Length; i++)
				{
					ref Quaternion reference = ref startRot[i];
					reference = bones[i].localRotation;
					if (i < bones.Length - 1)
					{
						boneTips[i] = bones[i + 1];
					}
				}
			}

			public void UpdateFinger(float deltaTime)
			{
				if (deltaTime == 0f)
				{
					return;
				}
				float f = 0f;
				if (squeezyAction != null && squeezyAction.GetActive(inputSource))
				{
					f = squeezyAction.GetAxis(inputSource);
				}
				squeezySmooth = Mathf.Lerp(squeezySmooth, Mathf.Sqrt(f), deltaTime * 10f);
				if (renderer.sharedMesh.blendShapeCount > 0)
				{
					renderer.SetBlendShapeWeight(0, squeezySmooth * 100f);
				}
				float ang = 0f;
				if (referenceAxis == eulerAxis.X)
				{
					ang = referenceBone.localEulerAngles.x;
				}
				if (referenceAxis == eulerAxis.Y)
				{
					ang = referenceBone.localEulerAngles.y;
				}
				if (referenceAxis == eulerAxis.Z)
				{
					ang = referenceBone.localEulerAngles.z;
				}
				ang = FixAngle(ang);
				pos = Mathf.InverseLerp(referenceAngles.x, referenceAngles.y, ang);
				if (mass > 0f)
				{
					for (int i = 0; i < bones.Length; i++)
					{
						bool flag = boneTips[i] != null;
						if (flag)
						{
							Vector3 vector = (boneTips[i].localPosition - bones[i].InverseTransformPoint(oldTipPosition[i])) / deltaTime;
							Vector3 vector2 = (vector - oldTipDelta[i]) / deltaTime;
							oldTipDelta[i] = vector;
							Vector3 vector3 = vector * -2f;
							vector2 *= -2f;
							for (int num = inertiaSteps - 1; num > 0; num--)
							{
								ref Vector3 reference = ref inertiaSmoothing[i, num];
								reference = inertiaSmoothing[i, num - 1];
							}
							inertiaSmoothing[i, 0] = vector2;
							Vector3 zero = Vector3.zero;
							for (int j = 0; j < inertiaSteps; j++)
							{
								zero += inertiaSmoothing[i, j];
							}
							zero /= (float)inertiaSteps;
							zero = PowVector(zero / 20f, 3f) * 20f;
							Vector3 fromDirection = forwardAxis;
							Vector3 toDirection = forwardAxis + vector3;
							Vector3 toDirection2 = forwardAxis + zero;
							Quaternion quaternion = Quaternion.FromToRotation(fromDirection, toDirection);
							Quaternion quaternion2 = Quaternion.FromToRotation(fromDirection, toDirection2);
							velocity[i] += FixVector(quaternion.eulerAngles) * 2f * deltaTime;
							velocity[i] += FixVector(quaternion2.eulerAngles) * 50f * deltaTime;
							ref Vector3 reference2 = ref velocity[i];
							reference2 = Vector3.ClampMagnitude(velocity[i], 1000f);
						}
						Vector3 vector4 = pos * Vector3.right * (flexAngle / (float)bones.Length);
						Vector3 vector5 = (0f - k) * (rotation[i] - vector4);
						Vector3 vector6 = damping * velocity[i];
						Vector3 vector7 = vector5 - vector6;
						Vector3 vector8 = vector7 / mass;
						velocity[i] += vector8 * deltaTime;
						rotation[i] += velocity[i] * Time.deltaTime;
						ref Vector3 reference3 = ref rotation[i];
						reference3 = Vector3.ClampMagnitude(rotation[i], 180f);
						if (flag)
						{
							ref Vector3 reference4 = ref oldTipPosition[i];
							reference4 = boneTips[i].position;
						}
					}
				}
				else
				{
					Debug.LogError("<b>[SteamVR Interaction]</b> finger mass is zero");
				}
			}

			public void ApplyTransforms()
			{
				for (int i = 0; i < bones.Length; i++)
				{
					bones[i].localRotation = startRot[i];
					bones[i].Rotate(rotation[i], Space.Self);
				}
			}

			private Vector3 FixVector(Vector3 ang)
			{
				return new Vector3(FixAngle(ang.x), FixAngle(ang.y), FixAngle(ang.z));
			}

			private float FixAngle(float ang)
			{
				if (ang > 180f)
				{
					ang = -360f + ang;
				}
				return ang;
			}

			private Vector3 PowVector(Vector3 vector, float power)
			{
				Vector3 vector2 = new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
				vector.x = Mathf.Pow(Mathf.Abs(vector.x), power) * vector2.x;
				vector.y = Mathf.Pow(Mathf.Abs(vector.y), power) * vector2.y;
				vector.z = Mathf.Pow(Mathf.Abs(vector.z), power) * vector2.z;
				return vector;
			}
		}

		protected float fingerFlexAngle = 140f;

		public SteamVR_Action_Single squeezyAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");

		public SteamVR_Input_Sources inputSource;

		public Finger[] fingers;

		public Vector3 constforce;

		private void Start()
		{
			for (int i = 0; i < fingers.Length; i++)
			{
				fingers[i].Init();
				fingers[i].flexAngle = fingerFlexAngle;
				fingers[i].squeezyAction = squeezyAction;
				fingers[i].inputSource = inputSource;
			}
		}

		private void Update()
		{
			for (int i = 0; i < fingers.Length; i++)
			{
				fingers[i].ApplyForce(constforce);
				fingers[i].UpdateFinger(Time.deltaTime);
				fingers[i].ApplyTransforms();
			}
		}
	}
}
