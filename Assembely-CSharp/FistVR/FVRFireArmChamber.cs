using UnityEngine;

namespace FistVR
{
	public class FVRFireArmChamber : FVRInteractiveObject
	{
		[Header("FireArm Chamber Config")]
		public FVRFireArm Firearm;

		public FireArmRoundType RoundType;

		public FVRFirearmAudioSet OverrideAudioSet;

		[Header("Chamber Params")]
		public bool IsManuallyExtractable;

		public bool IsManuallyChamberable;

		public float ChamberVelocityMultiplier = 1f;

		private FVRFireArmRound m_round;

		[Header("Chamber State")]
		public bool IsAccessible;

		public bool IsFull;

		public bool IsSpent;

		[Header("Proxy Stuff")]
		public GameObject LoadedPhys;

		public Transform ProxyRound;

		public MeshFilter ProxyMesh;

		public MeshRenderer ProxyRenderer;

		public bool DoesAutoChamber;

		public FireArmRoundClass AutoChamberClass = FireArmRoundClass.FMJ;

		public FVRFireArmRound GetRound()
		{
			return m_round;
		}

		[ContextMenu("checkmult")]
		public void checkmult()
		{
			if (ChamberVelocityMultiplier != 1f)
			{
				Debug.Log(base.transform.root.gameObject.name + " has mult of " + ChamberVelocityMultiplier);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObject gameObject = new GameObject("Proxy");
			ProxyRound = gameObject.transform;
			ProxyRound.SetParent(base.transform);
			ProxyRound.localPosition = Vector3.zero;
			ProxyRound.localEulerAngles = Vector3.zero;
			ProxyMesh = gameObject.AddComponent<MeshFilter>();
			ProxyRenderer = gameObject.AddComponent<MeshRenderer>();
		}

		protected override void Start()
		{
			base.Start();
			if (DoesAutoChamber)
			{
				Autochamber(AutoChamberClass);
			}
		}

		public void UpdateProxyDisplay()
		{
			if (m_round == null)
			{
				ProxyMesh.mesh = null;
				ProxyRenderer.material = null;
				ProxyRenderer.enabled = false;
				return;
			}
			if (IsSpent)
			{
				if (m_round.FiredRenderer != null)
				{
					ProxyMesh.mesh = m_round.FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
					ProxyRenderer.material = m_round.FiredRenderer.sharedMaterial;
				}
				else
				{
					ProxyMesh.mesh = null;
				}
			}
			else
			{
				ProxyMesh.mesh = AM.GetRoundMesh(m_round.RoundType, m_round.RoundClass);
				ProxyRenderer.material = AM.GetRoundMaterial(m_round.RoundType, m_round.RoundClass);
			}
			ProxyRenderer.enabled = true;
		}

		public void PlayChamberingAudio()
		{
			if (Firearm != null)
			{
				Firearm.PlayAudioEvent(FirearmAudioEventType.ChamberManual);
			}
			else if (OverrideAudioSet != null)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, OverrideAudioSet.ChamberManual, base.transform.position);
			}
		}

		public override bool IsInteractable()
		{
			if (IsManuallyExtractable)
			{
				if (IsAccessible && IsFull)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public void Autochamber(FireArmRoundClass frClass)
		{
			if (AM.DoesClassExistForType(frClass, RoundType))
			{
				GameObject gameObject = AM.GetRoundSelfPrefab(RoundType, frClass).GetGameObject();
				SetRound(gameObject.GetComponent<FVRFireArmRound>());
			}
		}

		public void Unload()
		{
			SetRound(null);
		}

		public void SetRound(FVRFireArmRound round)
		{
			if (round != null)
			{
				IsFull = true;
				IsSpent = round.IsSpent;
				m_round = AM.GetRoundSelfPrefab(round.RoundType, round.RoundClass).GetGameObject().GetComponent<FVRFireArmRound>();
				if (LoadedPhys != null)
				{
					LoadedPhys.SetActive(value: true);
				}
			}
			else
			{
				IsFull = false;
				m_round = null;
				if (LoadedPhys != null)
				{
					LoadedPhys.SetActive(value: false);
				}
			}
			UpdateProxyDisplay();
		}

		public FVRFireArmRound EjectRound(Vector3 EjectionPosition, Vector3 EjectionVelocity, Vector3 EjectionAngularVelocity, bool ForceCaseLessEject = false)
		{
			if (m_round != null)
			{
				bool flag = false;
				if (Firearm != null)
				{
					flag = true;
					if (Firearm.HasImpactController)
					{
						Firearm.AudioImpactController.SetCollisionsTickDownMax(0.2f);
					}
				}
				FVRFireArmRound fVRFireArmRound = null;
				if (!m_round.IsCaseless || ForceCaseLessEject)
				{
					GameObject gameObject = Object.Instantiate(m_round.gameObject, EjectionPosition, base.transform.rotation);
					fVRFireArmRound = gameObject.GetComponent<FVRFireArmRound>();
					if (flag)
					{
						fVRFireArmRound.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
					}
					fVRFireArmRound.RootRigidbody.velocity = Vector3.Lerp(EjectionVelocity * 0.7f, EjectionVelocity, Random.value) + GM.CurrentMovementManager.GetFilteredVel();
					fVRFireArmRound.RootRigidbody.maxAngularVelocity = 200f;
					fVRFireArmRound.RootRigidbody.angularVelocity = Vector3.Lerp(EjectionAngularVelocity * 0.3f, EjectionAngularVelocity, Random.value);
					if (IsSpent)
					{
						fVRFireArmRound.SetKillCounting(b: true);
						fVRFireArmRound.Fire();
					}
				}
				SetRound(null);
				return fVRFireArmRound;
			}
			return null;
		}

		public bool Fire()
		{
			if (IsFull && m_round != null && !IsSpent)
			{
				IsSpent = true;
				UpdateProxyDisplay();
				return true;
			}
			return false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (IsManuallyExtractable && IsAccessible && (IsFull & (m_round != null)))
			{
				FVRFireArmRound fVRFireArmRound = EjectRound(hand.Input.Pos, Vector3.zero, Vector3.zero);
				if (fVRFireArmRound != null)
				{
					fVRFireArmRound.BeginInteraction(hand);
					hand.ForceSetInteractable(fVRFireArmRound);
					SetRound(null);
				}
			}
		}
	}
}
