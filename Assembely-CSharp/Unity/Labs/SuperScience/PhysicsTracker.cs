using System;
using UnityEngine;

namespace Unity.Labs.SuperScience
{
	[Serializable]
	public class PhysicsTracker
	{
		private struct Sample
		{
			public float distance;

			public float angle;

			public Vector3 offset;

			public Vector3 axisOffset;

			public float speed;

			public float angularSpeed;

			public float time;

			public void Accumulate(ref Sample other, float scalar, Vector3 directionAnchor, Vector3 axisAnchor)
			{
				distance += other.distance * scalar;
				angle += other.angle * scalar;
				offset += other.offset * Vector3.Dot(directionAnchor, other.offset) * scalar;
				axisOffset += other.axisOffset * Vector3.Dot(axisAnchor, other.axisOffset) * scalar;
				time += other.time * scalar;
				speed = Mathf.Lerp(speed, other.speed, scalar);
				angularSpeed = Mathf.Lerp(angularSpeed, other.angularSpeed, scalar);
			}
		}

		private const float k_Period = 0.125f;

		private const float k_HalfPeriod = 0.0625f;

		private const int k_Steps = 4;

		private const float k_SamplePeriod = 0.03125f;

		private const float k_NewSampleWeight = 2f;

		private const float k_AdditiveWeight = 1f;

		private const float k_PredictedPeriod = 5f / 32f;

		private const int k_SampleLength = 5;

		private const float k_MinOffset = 0.001f;

		private const float k_MinAngle = 0.5f;

		private const float k_MinLength = 1E-05f;

		private int m_CurrentSampleIndex = -1;

		private Sample[] m_Samples = new Sample[5];

		private Vector3 m_LastOffsetPosition = Vector3.zero;

		private Vector3 m_LastDirectionPosition = Vector3.zero;

		private Quaternion m_LastRotation = Quaternion.identity;

		public float Speed
		{
			get;
			private set;
		}

		public float AccelerationStrength
		{
			get;
			private set;
		}

		public Vector3 Direction
		{
			get;
			private set;
		}

		public Vector3 Velocity
		{
			get;
			private set;
		}

		public Vector3 Acceleration
		{
			get;
			private set;
		}

		public float AngularSpeed
		{
			get;
			private set;
		}

		public Vector3 AngularAxis
		{
			get;
			private set;
		}

		public Vector3 AngularVelocity
		{
			get;
			private set;
		}

		public float AngularAccelerationStrength
		{
			get;
			private set;
		}

		public Vector3 AngularAcceleration
		{
			get;
			private set;
		}

		public void Reset(Vector3 currentPosition, Quaternion currentRotation, Vector3 currentVelocity, Vector3 currentAngularVelocity)
		{
			m_LastOffsetPosition = currentPosition;
			m_LastDirectionPosition = currentPosition;
			m_LastRotation = currentRotation;
			Speed = currentVelocity.magnitude;
			Direction = currentVelocity.normalized;
			Velocity = currentVelocity;
			AccelerationStrength = 0f;
			Acceleration = Vector3.zero;
			AngularSpeed = currentAngularVelocity.magnitude * 57.29578f;
			AngularAxis = currentAngularVelocity.normalized;
			AngularVelocity = currentAngularVelocity;
			m_CurrentSampleIndex = 0;
			ref Sample reference = ref m_Samples[0];
			reference = new Sample
			{
				distance = Speed * 0.125f,
				offset = Velocity * 0.125f,
				angle = AngularSpeed * 0.125f,
				axisOffset = AngularAxis * 0.125f,
				speed = Speed,
				angularSpeed = AngularSpeed,
				time = 0.125f
			};
		}

		public void Update(Vector3 newPosition, Quaternion newRotation, float timeSlice)
		{
			if (m_CurrentSampleIndex == -1)
			{
				Reset(newPosition, newRotation, Vector3.zero, Vector3.zero);
			}
			else
			{
				if (timeSlice <= 0f)
				{
					return;
				}
				Vector3 vector = newPosition - m_LastOffsetPosition;
				float magnitude = vector.magnitude;
				m_LastOffsetPosition = newPosition;
				Vector3 vector2 = newPosition - m_LastDirectionPosition;
				if (vector2.magnitude < 0.001f)
				{
					vector2 = Direction;
				}
				else
				{
					vector2.Normalize();
					m_LastDirectionPosition = newPosition;
				}
				Quaternion quaternion = newRotation * Quaternion.Inverse(m_LastRotation);
				Vector3 axis = Vector3.zero;
				quaternion.ToAngleAxis(out var angle, out axis);
				if (angle < 0.5f)
				{
					angle = 0f;
					axis = AngularAxis;
				}
				else
				{
					m_LastRotation = newRotation;
				}
				float num = 1f + angle / 90f;
				m_Samples[m_CurrentSampleIndex].distance += magnitude;
				m_Samples[m_CurrentSampleIndex].offset += vector;
				m_Samples[m_CurrentSampleIndex].angle += angle;
				m_Samples[m_CurrentSampleIndex].time += timeSlice;
				if (Vector3.Dot(axis, m_Samples[m_CurrentSampleIndex].axisOffset) < 0f)
				{
					m_Samples[m_CurrentSampleIndex].axisOffset += -axis * num;
				}
				else
				{
					m_Samples[m_CurrentSampleIndex].axisOffset += axis * num;
				}
				Sample sample = default(Sample);
				int num2 = m_CurrentSampleIndex;
				while (sample.time < 0.125f)
				{
					float scalar = Mathf.Clamp01((0.125f - sample.time) / m_Samples[num2].time);
					sample.Accumulate(ref m_Samples[num2], scalar, vector2, axis);
					num2 = (num2 + 1) % 5;
				}
				float speed = sample.speed;
				float angularSpeed = sample.angularSpeed;
				num2 = m_CurrentSampleIndex;
				while (sample.time < 5f / 32f)
				{
					float scalar2 = Mathf.Clamp01((5f / 32f - sample.time) / m_Samples[num2].time);
					sample.Accumulate(ref m_Samples[num2], scalar2, vector2, axis);
					num2 = (num2 + 1) % 5;
				}
				Speed = sample.distance / (5f / 32f);
				if (sample.offset.magnitude > 1E-05f)
				{
					Direction = sample.offset.normalized;
				}
				else
				{
					float num3 = Vector3.Dot(Direction, vector2);
					if (num3 < 0f)
					{
						num3 = 0f - num3;
						Direction = -Direction;
					}
					Direction = Vector3.Lerp(vector2, Direction, num3).normalized;
				}
				Velocity = Direction * Speed;
				AngularSpeed = sample.angle / (5f / 32f);
				if (sample.axisOffset.magnitude > 1E-05f)
				{
					axis = sample.axisOffset.normalized;
				}
				float num4 = Vector3.Dot(AngularAxis, axis);
				if (num4 < 0f)
				{
					num4 = 0f - num4;
					AngularAxis = -AngularAxis;
				}
				AngularAxis = Vector3.Lerp(axis, AngularAxis, num4).normalized;
				AngularVelocity = AngularAxis * AngularSpeed * ((float)Math.PI / 180f);
				float num5 = Speed - speed;
				float num6 = AngularSpeed - angularSpeed;
				AccelerationStrength = num5 / 0.125f;
				Acceleration = AccelerationStrength * Direction;
				AngularAccelerationStrength = num6 / 0.125f;
				AngularAcceleration = AngularAxis * AngularAccelerationStrength * ((float)Math.PI / 180f);
				if (!(m_Samples[m_CurrentSampleIndex].time < 0.03125f))
				{
					m_Samples[m_CurrentSampleIndex].speed = Speed;
					m_Samples[m_CurrentSampleIndex].angularSpeed = AngularSpeed;
					m_CurrentSampleIndex = (m_CurrentSampleIndex - 1 + 5) % 5;
					m_Samples[m_CurrentSampleIndex] = default(Sample);
				}
			}
		}
	}
}
