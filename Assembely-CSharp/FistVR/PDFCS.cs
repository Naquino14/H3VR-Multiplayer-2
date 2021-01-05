using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PDFCS : MonoBehaviour
	{
		[Serializable]
		public class Thruster
		{
			public enum Facing
			{
				PX,
				NX,
				PY,
				NY,
				PZ,
				NZ
			}

			public PDFCS FCS;

			public bool isDebug;

			private Vector3 thrustDir = Vector3.zero;

			public Facing ThrustDirLocal;

			public bool isAngular;

			public Vector3 PositionLocal;

			public PDComponentID ParentSys;

			public bool IsAfterburner;

			public PDThrusterConfig TConfig;

			private float m_targetLoad;

			private float m_currentLoad;

			public void SetTargetLoad(float l)
			{
				m_targetLoad = l;
			}

			public void Init()
			{
				switch (ThrustDirLocal)
				{
				case Facing.PX:
					thrustDir = Vector3.right;
					break;
				case Facing.PY:
					thrustDir = Vector3.up;
					break;
				case Facing.PZ:
					thrustDir = Vector3.forward;
					break;
				case Facing.NX:
					thrustDir = Vector3.left;
					break;
				case Facing.NY:
					thrustDir = Vector3.down;
					break;
				case Facing.NZ:
					thrustDir = Vector3.back;
					break;
				}
			}

			public float GetMaxLinearOutput()
			{
				float maxThrustForce = TConfig.MaxThrustForce;
				return maxThrustForce * TConfig.IntegrityToMaxLoadCurve.Evaluate(FCS.PD.GetSys((int)ParentSys).GetIntegrity());
			}

			public void Tick(float t)
			{
				float num = 0f;
				if (m_targetLoad < m_currentLoad)
				{
					num = TConfig.ThrustEngagementResponseSpeedDown;
				}
				else
				{
					num = TConfig.ThrustEngagementResponseSpeedUp;
					num *= TConfig.ThrustResponseCurve.Evaluate(m_currentLoad);
				}
				m_currentLoad = Mathf.MoveTowards(m_currentLoad, m_targetLoad, t * num);
				float value = m_currentLoad * TConfig.MaxThrustForce;
				value = Mathf.Clamp(value, 0f, TConfig.MaxThrustForce * TConfig.IntegrityToMaxLoadCurve.Evaluate(FCS.PD.GetSys((int)ParentSys).GetIntegrity()) * TConfig.HeatToMaxLoadCurve.Evaluate(FCS.PD.GetSys((int)ParentSys).GetHeat()));
				float h = TConfig.LoadToHeatCurve.Evaluate(m_currentLoad) * TConfig.HeatGenerationBase * t;
				FCS.PD.GetSys((int)ParentSys).AddHeat(h);
				if (!isAngular)
				{
					Vector3 force = FCS.transform.TransformVector(thrustDir * value);
					if (!IsAfterburner)
					{
						float t2 = 0f;
						if (FCS.DistFromGround > 3f)
						{
							t2 = (FCS.DistFromGround - 3f) * 0.2f;
						}
						if (force.y > 0f)
						{
							force.y = Mathf.Lerp(force.y, 0f, t2);
						}
					}
					FCS.RB.AddForce(force, ForceMode.Acceleration);
				}
				else
				{
					FCS.RB.AddRelativeTorque(thrustDir * value, ForceMode.Acceleration);
				}
			}
		}

		public PD PD;

		public PDFCSConfig FCSConfig;

		public Transform Root;

		public Rigidbody RB;

		public Transform Cam;

		public List<Thruster> LinearThrusters;

		public List<Thruster> AngularThrusters;

		public List<Thruster> Afterburners;

		private bool m_isStabilizationActive = true;

		private bool m_fireAfterBurners;

		private Vector3 localLinearControlMag = Vector3.zero;

		private Vector3 m_desiredLinearVelocityWorld = Vector3.zero;

		private Vector3 localAngularControlMag = Vector3.zero;

		private Vector3 m_desiredAngularVelocity = Vector3.zero;

		public LayerMask LM_GroundCheck;

		private RaycastHit m_hit;

		public float DistFromGround = 2f;

		private float cx;

		private float cy;

		private float cz;

		private void Start()
		{
			RB.maxAngularVelocity = 12f;
			for (int i = 0; i < LinearThrusters.Count; i++)
			{
				LinearThrusters[i].Init();
			}
			for (int j = 0; j < AngularThrusters.Count; j++)
			{
				AngularThrusters[j].Init();
			}
			for (int k = 0; k < Afterburners.Count; k++)
			{
				Afterburners[k].Init();
			}
		}

		private void Update()
		{
			FlyByWire();
		}

		private void FlyByWire()
		{
			if (Input.GetButtonDown("Left Stick Click"))
			{
			}
			if (Input.GetButton("Left Stick Click"))
			{
				m_fireAfterBurners = true;
			}
			else
			{
				m_fireAfterBurners = false;
			}
			localLinearControlMag = Vector3.zero;
			localLinearControlMag.x = GetSignedSquare(Input.GetAxis("LeftStickXAxis")) * 20f;
			localLinearControlMag.y = GetSignedSquare(Input.GetAxis("D-Pad Y Axis")) * 20f;
			localLinearControlMag.z = GetSignedSquare(0f - Input.GetAxis("LeftStickYAxis")) * 40f;
			if (m_fireAfterBurners)
			{
				localLinearControlMag.z = 100f;
			}
			m_desiredLinearVelocityWorld = base.transform.TransformVector(localLinearControlMag);
			localAngularControlMag = Vector3.zero;
			localAngularControlMag.x = GetSignedSquare(Input.GetAxis("RightStickYAxis"));
			localAngularControlMag.y = GetSignedSquare(Input.GetAxis("RightStickXAxis"));
			if (Input.GetButton("Left Bumper"))
			{
				localAngularControlMag.z += 1f;
			}
			if (Input.GetButton("Right Bumper"))
			{
				localAngularControlMag.z -= 1f;
			}
			localAngularControlMag *= 5f;
			cx = Mathf.Lerp(cx, localAngularControlMag.x * 3f, Time.deltaTime * 1f);
			cy = Mathf.Lerp(cy, localAngularControlMag.y * 3f, Time.deltaTime * 1f);
			cz = Mathf.Lerp(cz, localAngularControlMag.z * 6f, Time.deltaTime * 1f);
			Cam.localEulerAngles = new Vector3(cx, cy, cz);
			m_desiredAngularVelocity = localAngularControlMag;
		}

		private void FixedUpdate()
		{
			ThrustControl();
		}

		private void ThrustControl()
		{
			float deltaTime = Time.deltaTime;
			if (Physics.Raycast(base.transform.position, Vector3.down, out m_hit, 100f, LM_GroundCheck, QueryTriggerInteraction.Ignore))
			{
				DistFromGround = m_hit.distance;
			}
			else
			{
				DistFromGround = 100f;
			}
			float num = 1f;
			num += (1f - Mathf.Clamp(DistFromGround, 0f, 1f)) * 50f;
			Vector3 vector = base.transform.InverseTransformVector(m_desiredLinearVelocityWorld);
			Vector3 velocity = RB.velocity;
			if (m_isStabilizationActive)
			{
				vector -= base.transform.InverseTransformVector(Physics.gravity * num);
				vector -= base.transform.InverseTransformDirection(velocity);
			}
			else
			{
				vector -= base.transform.InverseTransformDirection(velocity);
			}
			if (vector.x > 0f)
			{
				SetLinearThrustLoadTargets(Thruster.Facing.PX, vector.x);
				KillLoadTargetLinear(Thruster.Facing.NX);
			}
			else if (vector.x < 0f)
			{
				SetLinearThrustLoadTargets(Thruster.Facing.NX, 0f - vector.x);
				KillLoadTargetLinear(Thruster.Facing.PX);
			}
			else
			{
				KillLoadTargetLinear(Thruster.Facing.NX);
				KillLoadTargetLinear(Thruster.Facing.PX);
			}
			if (vector.y > 0f)
			{
				SetLinearThrustLoadTargets(Thruster.Facing.PY, vector.y);
				KillLoadTargetLinear(Thruster.Facing.NY);
			}
			else if (vector.y < 0f)
			{
				SetLinearThrustLoadTargets(Thruster.Facing.NY, 0f - vector.y);
				KillLoadTargetLinear(Thruster.Facing.PY);
			}
			else
			{
				KillLoadTargetLinear(Thruster.Facing.NY);
				KillLoadTargetLinear(Thruster.Facing.PY);
			}
			if (vector.z > 0f)
			{
				SetLinearThrustLoadTargets(Thruster.Facing.PZ, vector.z);
				KillLoadTargetLinear(Thruster.Facing.NZ);
			}
			else if (vector.z < 0f)
			{
				SetLinearThrustLoadTargets(Thruster.Facing.NZ, 0f - vector.z);
				KillLoadTargetLinear(Thruster.Facing.PZ);
			}
			else
			{
				KillLoadTargetLinear(Thruster.Facing.NZ);
				KillLoadTargetLinear(Thruster.Facing.PZ);
			}
			Vector3 angularVelocity = RB.angularVelocity;
			Vector3 desiredAngularVelocity = m_desiredAngularVelocity;
			desiredAngularVelocity -= base.transform.InverseTransformDirection(RB.angularVelocity);
			if (desiredAngularVelocity.x > 0f)
			{
				SetAngularThrustLoadTargets(Thruster.Facing.PX, desiredAngularVelocity.x);
				KillLoadTargetAngular(Thruster.Facing.NX);
			}
			else if (desiredAngularVelocity.x < 0f)
			{
				SetAngularThrustLoadTargets(Thruster.Facing.NX, 0f - desiredAngularVelocity.x);
				KillLoadTargetAngular(Thruster.Facing.PX);
			}
			else
			{
				KillLoadTargetAngular(Thruster.Facing.NX);
				KillLoadTargetAngular(Thruster.Facing.PX);
			}
			if (desiredAngularVelocity.y > 0f)
			{
				SetAngularThrustLoadTargets(Thruster.Facing.PY, desiredAngularVelocity.y);
				KillLoadTargetAngular(Thruster.Facing.NY);
			}
			else if (desiredAngularVelocity.y < 0f)
			{
				SetAngularThrustLoadTargets(Thruster.Facing.NY, 0f - desiredAngularVelocity.y);
				KillLoadTargetAngular(Thruster.Facing.PY);
			}
			else
			{
				KillLoadTargetAngular(Thruster.Facing.NY);
				KillLoadTargetAngular(Thruster.Facing.PY);
			}
			if (desiredAngularVelocity.z > 0f)
			{
				SetAngularThrustLoadTargets(Thruster.Facing.PZ, desiredAngularVelocity.z);
				KillLoadTargetAngular(Thruster.Facing.NZ);
			}
			else if (desiredAngularVelocity.z < 0f)
			{
				SetAngularThrustLoadTargets(Thruster.Facing.NZ, 0f - desiredAngularVelocity.z);
				KillLoadTargetAngular(Thruster.Facing.PZ);
			}
			else
			{
				KillLoadTargetAngular(Thruster.Facing.NZ);
				KillLoadTargetAngular(Thruster.Facing.PZ);
			}
			if (m_fireAfterBurners)
			{
				SetAfterburnerThrustLoadTargets(Thruster.Facing.PZ, vector.z);
				KillLoadTargetLinear(Thruster.Facing.NZ);
				KillLoadTargetLinear(Thruster.Facing.PZ);
			}
			else
			{
				KillLoadTargetAfterburned(Thruster.Facing.PZ);
			}
			for (int i = 0; i < LinearThrusters.Count; i++)
			{
				LinearThrusters[i].Tick(deltaTime);
			}
			for (int j = 0; j < AngularThrusters.Count; j++)
			{
				AngularThrusters[j].Tick(deltaTime);
			}
			for (int k = 0; k < Afterburners.Count; k++)
			{
				Afterburners[k].Tick(deltaTime);
			}
		}

		private void KillLoadTargetLinear(Thruster.Facing faceing)
		{
			for (int i = 0; i < LinearThrusters.Count; i++)
			{
				if (LinearThrusters[i].ThrustDirLocal == faceing)
				{
					LinearThrusters[i].SetTargetLoad(0f);
				}
			}
		}

		private void KillLoadTargetAngular(Thruster.Facing faceing)
		{
			for (int i = 0; i < AngularThrusters.Count; i++)
			{
				if (AngularThrusters[i].ThrustDirLocal == faceing)
				{
					AngularThrusters[i].SetTargetLoad(0f);
				}
			}
		}

		private void KillLoadTargetAfterburned(Thruster.Facing faceing)
		{
			for (int i = 0; i < Afterburners.Count; i++)
			{
				if (Afterburners[i].ThrustDirLocal == faceing)
				{
					Afterburners[i].SetTargetLoad(0f);
				}
			}
		}

		private void SetLinearThrustLoadTargets(Thruster.Facing faceing, float amountNeededInDir)
		{
			float num = 0f;
			for (int i = 0; i < LinearThrusters.Count; i++)
			{
				if (LinearThrusters[i].ThrustDirLocal == faceing)
				{
					num += LinearThrusters[i].GetMaxLinearOutput();
					if (LinearThrusters[i].isDebug)
					{
						Debug.Log("output " + LinearThrusters[i].GetMaxLinearOutput());
					}
				}
			}
			float targetLoad = 0f;
			if (num > 0f)
			{
				targetLoad = Mathf.Clamp(amountNeededInDir / num, 0f, 1f);
			}
			for (int j = 0; j < LinearThrusters.Count; j++)
			{
				if (LinearThrusters[j].ThrustDirLocal == faceing)
				{
					LinearThrusters[j].SetTargetLoad(targetLoad);
				}
			}
		}

		private void SetAngularThrustLoadTargets(Thruster.Facing faceing, float amountNeededInDir)
		{
			float num = 0f;
			for (int i = 0; i < AngularThrusters.Count; i++)
			{
				if (AngularThrusters[i].ThrustDirLocal == faceing)
				{
					num += AngularThrusters[i].GetMaxLinearOutput();
					if (AngularThrusters[i].isDebug)
					{
						Debug.Log("output " + AngularThrusters[i].GetMaxLinearOutput());
					}
				}
			}
			float targetLoad = 0f;
			if (num > 0f)
			{
				targetLoad = Mathf.Clamp(amountNeededInDir / num, 0f, 1f);
			}
			for (int j = 0; j < AngularThrusters.Count; j++)
			{
				if (AngularThrusters[j].ThrustDirLocal == faceing)
				{
					AngularThrusters[j].SetTargetLoad(targetLoad);
				}
			}
		}

		private void SetAfterburnerThrustLoadTargets(Thruster.Facing faceing, float amountNeededInDir)
		{
			float num = 0f;
			for (int i = 0; i < Afterburners.Count; i++)
			{
				if (Afterburners[i].ThrustDirLocal == faceing)
				{
					num += Afterburners[i].GetMaxLinearOutput();
					if (Afterburners[i].isDebug)
					{
						Debug.Log("output " + Afterburners[i].GetMaxLinearOutput());
					}
				}
			}
			float targetLoad = 0f;
			if (num > 0f)
			{
				targetLoad = Mathf.Clamp(amountNeededInDir / num, 0f, 1f);
			}
			for (int j = 0; j < Afterburners.Count; j++)
			{
				if (Afterburners[j].ThrustDirLocal == faceing)
				{
					Afterburners[j].SetTargetLoad(targetLoad);
				}
			}
		}

		[ContextMenu("ConfigThrusters")]
		public void ConfigThrusters()
		{
			LinearThrusters.Clear();
			AngularThrusters.Clear();
			for (int i = 0; i < FCSConfig.Configs.Count; i++)
			{
				PDFCSConfig.ThrusterConfig thrusterConfig = FCSConfig.Configs[i];
				Thruster thruster = new Thruster();
				thruster.FCS = this;
				thruster.ThrustDirLocal = thrusterConfig.TDir;
				thruster.isAngular = thrusterConfig.IsAng;
				thruster.ParentSys = thrusterConfig.Sys;
				thruster.TConfig = thrusterConfig.TConfig;
				thruster.IsAfterburner = thrusterConfig.IsGAB;
				if (thruster.IsAfterburner)
				{
					Afterburners.Add(thruster);
				}
				else if (thruster.isAngular)
				{
					AngularThrusters.Add(thruster);
				}
				else
				{
					LinearThrusters.Add(thruster);
				}
			}
		}

		private float GetSignedSquare(float i)
		{
			return Mathf.Pow(i, 2f) * Mathf.Sign(i);
		}
	}
}
