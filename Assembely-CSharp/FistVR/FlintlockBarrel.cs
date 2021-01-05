using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FlintlockBarrel : MonoBehaviour
	{
		public enum LoadedElementType
		{
			Powder,
			Shot,
			ShotInPaper,
			Wadding
		}

		[Serializable]
		public class LoadedElement
		{
			public LoadedElementType Type;

			public float Position;

			public int PowderAmount;
		}

		private FlintlockWeapon m_weapon;

		private FlintlockFlashPan m_pan;

		[Header("Barrel")]
		public Transform Muzzle;

		public Transform LodgePoint_Paper;

		public Transform LodgePoint_Shot;

		public float BarrelLength = 0.21f;

		public List<LoadedElement> LoadedElements = new List<LoadedElement>();

		public List<Renderer> ProxyRends0;

		public List<Renderer> ProxyRends1;

		public MeshFilter ProxyPowder0;

		public MeshFilter ProxyPowder1;

		public List<Mesh> ProxyPowderPiles;

		public List<GameObject> EjectedObjectPrefabs;

		public Vector4 LoadedElementSizes = new Vector4(0.001f, 0.0175f, 0.0195f, 0.01f);

		[Header("Projectile Stuff")]
		public GameObject ProjectilePrefab;

		public float Spread = 0.6f;

		public float VelocityMult = 1f;

		public GameObject IgniteProjectile_Visible;

		public GameObject IgniteProjectile_NotVisible;

		public AnimationCurve PowderToVelMultCurve;

		[Header("Audio")]
		public AudioEvent AudEvent_Tamp;

		public AudioEvent AudEvent_TampEnd;

		public AudioEvent AudEvent_Squib;

		public List<AudioEvent> AudEvent_InsertByType;

		private float m_insertSoundRefire = 0.2f;

		[Header("MuzzleEffects")]
		public MuzzleEffectSize DefaultMuzzleEffectSize = MuzzleEffectSize.Standard;

		public MuzzleEffect[] MuzzleEffects;

		private List<MuzzlePSystem> m_muzzleSystems = new List<MuzzlePSystem>();

		public ParticleSystem MuzzleOverFireSystem;

		public Vector2 MuzzleOverFireSystemScaleRange = new Vector2(0.3f, 2f);

		public Vector2 MuzzleOverFireSystemEmitRange = new Vector2(3f, 20f);

		public Vector2 FlashBlastSmokeRange = new Vector2(4f, 10f);

		public Vector2 FlashBlastFireRange = new Vector2(2f, 10f);

		private bool m_isIgnited;

		private float m_igniteTick;

		private float TampRefire = 0.2f;

		public FlintlockWeapon GetWeapon()
		{
			return m_weapon;
		}

		public void SetWeapon(FlintlockWeapon w)
		{
			m_weapon = w;
		}

		public void SetPan(FlintlockFlashPan p)
		{
			m_pan = p;
		}

		public float GetLengthOfElement(LoadedElementType Type, int PowderAmount)
		{
			return Type switch
			{
				LoadedElementType.Powder => LoadedElementSizes.x * (float)PowderAmount, 
				LoadedElementType.Shot => LoadedElementSizes.y, 
				LoadedElementType.ShotInPaper => LoadedElementSizes.z, 
				LoadedElementType.Wadding => LoadedElementSizes.w, 
				_ => 0f, 
			};
		}

		private bool CanElementFit(LoadedElementType Type)
		{
			if (LoadedElements.Count == 0)
			{
				return true;
			}
			if (LoadedElements[LoadedElements.Count - 1].Position > GetLengthOfElement(Type, 5))
			{
				return true;
			}
			return false;
		}

		private void Awake()
		{
			RegenerateMuzzleEffects();
		}

		private void RegenerateMuzzleEffects()
		{
			for (int i = 0; i < m_muzzleSystems.Count; i++)
			{
				UnityEngine.Object.Destroy(m_muzzleSystems[i].PSystem);
			}
			m_muzzleSystems.Clear();
			MuzzleEffect[] muzzleEffects = MuzzleEffects;
			for (int j = 0; j < muzzleEffects.Length; j++)
			{
				if (muzzleEffects[j].Entry != 0)
				{
					MuzzleEffectConfig muzzleConfig = FXM.GetMuzzleConfig(muzzleEffects[j].Entry);
					MuzzleEffectSize size = muzzleEffects[j].Size;
					GameObject gameObject = ((!GM.CurrentSceneSettings.IsSceneLowLight) ? UnityEngine.Object.Instantiate(muzzleConfig.Prefabs_Highlight[(int)size], Muzzle.position, Muzzle.rotation) : UnityEngine.Object.Instantiate(muzzleConfig.Prefabs_Lowlight[(int)size], Muzzle.position, Muzzle.rotation));
					if (muzzleEffects[j].OverridePoint == null)
					{
						gameObject.transform.SetParent(Muzzle.transform);
					}
					else
					{
						gameObject.transform.SetParent(muzzleEffects[j].OverridePoint);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localEulerAngles = Vector3.zero;
					}
					MuzzlePSystem muzzlePSystem = new MuzzlePSystem();
					muzzlePSystem.PSystem = gameObject.GetComponent<ParticleSystem>();
					muzzlePSystem.OverridePoint = muzzleEffects[j].OverridePoint;
					int index = (int)size;
					if (GM.CurrentSceneSettings.IsSceneLowLight)
					{
						muzzlePSystem.NumParticlesPerShot = muzzleConfig.NumParticles_Lowlight[index];
					}
					else
					{
						muzzlePSystem.NumParticlesPerShot = muzzleConfig.NumParticles_Highlight[index];
					}
					m_muzzleSystems.Add(muzzlePSystem);
				}
			}
		}

		public void FireMuzzleSmoke()
		{
			if (GM.CurrentSceneSettings.IsSceneLowLight)
			{
				FXM.InitiateMuzzleFlash(Muzzle.position, Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
			}
			for (int i = 0; i < m_muzzleSystems.Count; i++)
			{
				if (m_muzzleSystems[i].OverridePoint == null)
				{
					m_muzzleSystems[i].PSystem.transform.position = Muzzle.position;
				}
				m_muzzleSystems[i].PSystem.Emit(m_muzzleSystems[i].NumParticlesPerShot);
			}
		}

		private bool IsBarrelPlugged()
		{
			if (LoadedElements.Count == 0)
			{
				return false;
			}
			if (LoadedElements[LoadedElements.Count - 1].Type != 0 && LoadedElements[LoadedElements.Count - 1].Position < 0.01f && (LoadedElements[LoadedElements.Count - 1].Type == LoadedElementType.Shot || LoadedElements[LoadedElements.Count - 1].Type == LoadedElementType.ShotInPaper))
			{
				return true;
			}
			return false;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			float num = Vector3.Angle(Muzzle.forward, Vector3.up);
			if (num > 90f)
			{
				return;
			}
			GameObject gameObject = other.attachedRigidbody.gameObject;
			if (gameObject.CompareTag("flintlock_shot"))
			{
				if (CanElementFit(LoadedElementType.Shot) && (!m_weapon.RamRod.gameObject.activeSelf || m_weapon.RamRod.RState != FlintlockPseudoRamRod.RamRodState.Barrel || !(m_weapon.RamRod.GetCurBarrel() == this)) && !IsBarrelPlugged())
				{
					m_weapon.PlayAudioAsHandling(AudEvent_InsertByType[1], Muzzle.position);
					InsertElement(LoadedElementType.Shot);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else if (gameObject.CompareTag("flintlock_paper"))
			{
				if (!CanElementFit(LoadedElementType.ShotInPaper) || (m_weapon.RamRod.gameObject.activeSelf && m_weapon.RamRod.RState == FlintlockPseudoRamRod.RamRodState.Barrel && m_weapon.RamRod.GetCurBarrel() == this) || IsBarrelPlugged() || Vector3.Angle(Muzzle.forward, gameObject.transform.forward) > 80f)
				{
					return;
				}
				FlintlockPaperCartridge component = gameObject.GetComponent<FlintlockPaperCartridge>();
				if (component.CState != 0)
				{
					m_weapon.PlayAudioAsHandling(AudEvent_InsertByType[2], Muzzle.position);
					for (int i = 0; i < component.numPowderChunksLeft; i++)
					{
						InsertElement(LoadedElementType.Powder);
					}
					InsertElement(LoadedElementType.ShotInPaper);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else if (gameObject.CompareTag("flintlock_wadding"))
			{
				if (CanElementFit(LoadedElementType.Wadding))
				{
					m_weapon.PlayAudioAsHandling(AudEvent_InsertByType[3], Muzzle.position);
					InsertElement(LoadedElementType.Wadding);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else if (gameObject.CompareTag("flintlock_powdergrain"))
			{
				if (CanElementFit(LoadedElementType.Powder))
				{
					if (m_insertSoundRefire > 0.15f)
					{
						m_weapon.PlayAudioAsHandling(AudEvent_InsertByType[0], Muzzle.position);
					}
					InsertElement(LoadedElementType.Powder);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
			else if (gameObject.CompareTag("flintlock_ramrod"))
			{
				FlintlockRamRod component2 = gameObject.GetComponent<FlintlockRamRod>();
				if (component2.IsHeld)
				{
					FVRViveHand hand = component2.m_hand;
					component2.ForceBreakInteraction();
					m_weapon.RamRod.gameObject.SetActive(value: true);
					m_weapon.RamRod.RState = FlintlockPseudoRamRod.RamRodState.Barrel;
					m_weapon.RamRod.MountToBarrel(this, hand);
					Tamp(0.05f, 0.001f);
					hand.ForceSetInteractable(m_weapon.RamRod);
					m_weapon.RamRod.BeginInteraction(hand);
					UnityEngine.Object.Destroy(other.gameObject);
				}
			}
		}

		public void Tamp(float delta, float depth)
		{
			if (TampRefire < 0.15f || Mathf.Abs(delta) < 0.01f)
			{
				return;
			}
			if (LoadedElements.Count == 1)
			{
				if (Mathf.Abs(depth) > LoadedElements[0].Position)
				{
					float position = LoadedElements[0].Position;
					float num = LoadedElements[0].Position + Mathf.Abs(delta);
					float max = BarrelLength - GetLengthOfElement(LoadedElements[0].Type, LoadedElements[0].PowderAmount);
					num = Mathf.Clamp(num, num, max);
					LoadedElements[0].Position = num;
					if (Mathf.Abs(position - num) > 0.001f)
					{
						m_weapon.PlayAudioAsHandling(AudEvent_Tamp, Muzzle.position);
					}
					else
					{
						m_weapon.PlayAudioAsHandling(AudEvent_TampEnd, Muzzle.position);
					}
					TampRefire = 0f;
				}
			}
			else if (LoadedElements.Count > 1 && Mathf.Abs(depth) > LoadedElements[LoadedElements.Count - 1].Position)
			{
				float position2 = LoadedElements[LoadedElements.Count - 1].Position;
				float num2 = LoadedElements[LoadedElements.Count - 1].Position + Mathf.Abs(delta);
				float max2 = LoadedElements[LoadedElements.Count - 2].Position - GetLengthOfElement(LoadedElements[LoadedElements.Count - 1].Type, LoadedElements[LoadedElements.Count - 1].PowderAmount);
				num2 = Mathf.Clamp(num2, num2, max2);
				LoadedElements[LoadedElements.Count - 1].Position = num2;
				if (Mathf.Abs(position2 - num2) > 0.001f)
				{
					m_weapon.PlayAudioAsHandling(AudEvent_Tamp, Muzzle.position);
				}
				else
				{
					m_weapon.PlayAudioAsHandling(AudEvent_TampEnd, Muzzle.position);
				}
				TampRefire = 0f;
			}
		}

		public float GetMaxDepth()
		{
			if (LoadedElements.Count < 1)
			{
				return BarrelLength;
			}
			return LoadedElements[LoadedElements.Count - 1].Position;
		}

		private void InsertElement(LoadedElementType type)
		{
			if (LoadedElements.Count > 0)
			{
				if (type == LoadedElementType.Powder && LoadedElements[LoadedElements.Count - 1].Type == LoadedElementType.Powder)
				{
					LoadedElements[LoadedElements.Count - 1].PowderAmount++;
					return;
				}
				LoadedElement loadedElement = new LoadedElement();
				loadedElement.Type = type;
				loadedElement.Position = 0f;
				loadedElement.PowderAmount = 1;
				LoadedElements.Add(loadedElement);
			}
			else
			{
				LoadedElement loadedElement2 = new LoadedElement();
				loadedElement2.Type = type;
				loadedElement2.Position = 0f;
				loadedElement2.PowderAmount = 1;
				LoadedElements.Add(loadedElement2);
			}
		}

		private int ExpellElement(LoadedElementType type, int PowderAmount)
		{
			if (type != 0)
			{
				Vector3 position = Muzzle.position + Muzzle.forward * GetLengthOfElement(type, PowderAmount) * 0.75f;
				UnityEngine.Object.Instantiate(EjectedObjectPrefabs[(int)type], position, Muzzle.rotation);
				return 0;
			}
			int result = PowderAmount - 1;
			Vector3 position2 = Muzzle.position + UnityEngine.Random.onUnitSphere * 0.005f + Muzzle.forward * GetLengthOfElement(type, 1) * 0.75f;
			UnityEngine.Object.Instantiate(EjectedObjectPrefabs[(int)type], position2, Muzzle.rotation);
			return result;
		}

		public void Ignite()
		{
			if (LoadedElements.Count > 0 && LoadedElements[0].Type == LoadedElementType.Powder)
			{
				m_isIgnited = true;
				m_igniteTick = UnityEngine.Random.Range(0.01f, 0.03f);
			}
		}

		private void Update()
		{
			if (TampRefire < 0.2f)
			{
				TampRefire += Time.deltaTime;
			}
			if (m_insertSoundRefire < 0.2f)
			{
				m_insertSoundRefire += Time.deltaTime;
			}
			if (m_isIgnited)
			{
				m_igniteTick -= Time.deltaTime;
				if (m_igniteTick <= 0f)
				{
					Fire();
				}
			}
			else
			{
				BarrelContentsSim();
				BarrelContentsDraw();
			}
		}

		private int GetNumProjectilesInBarrel()
		{
			int num = 0;
			for (int i = 0; i < LoadedElements.Count; i++)
			{
				if (LoadedElements[i].Type == LoadedElementType.Shot || LoadedElements[i].Type == LoadedElementType.ShotInPaper)
				{
					num += LoadedElements[i].PowderAmount;
				}
			}
			return num;
		}

		private int GetNumPowder()
		{
			int num = 0;
			for (int i = 0; i < LoadedElements.Count; i++)
			{
				if (LoadedElements[i].Type == LoadedElementType.Powder)
				{
					num += LoadedElements[i].PowderAmount;
				}
			}
			return num;
		}

		private float GetMinProjPos()
		{
			float num = BarrelLength;
			for (int i = 0; i < LoadedElements.Count; i++)
			{
				if (LoadedElements[i].Type != 0)
				{
					num = Mathf.Min(num, LoadedElements[i].Position);
				}
			}
			return num;
		}

		private float GetMaxProjPos()
		{
			float num = 0f;
			for (int i = 0; i < LoadedElements.Count; i++)
			{
				if (LoadedElements[i].Type != 0)
				{
					num = Mathf.Max(num, LoadedElements[i].Position);
				}
			}
			return num;
		}

		public void BurnOffOuter()
		{
			if (LoadedElements.Count != 0 && LoadedElements[LoadedElements.Count - 1].Type == LoadedElementType.Powder)
			{
				int powderAmount = LoadedElements[LoadedElements.Count - 1].PowderAmount;
				LoadedElements.RemoveAt(LoadedElements.Count - 1);
				float t = Mathf.Lerp(0f, 1f, (float)powderAmount / 60f);
				float spread = Spread;
				float num = Mathf.Lerp(MuzzleOverFireSystemScaleRange.x, MuzzleOverFireSystemScaleRange.y, t);
				float num2 = Mathf.Lerp(MuzzleOverFireSystemEmitRange.x, MuzzleOverFireSystemEmitRange.y, t);
				MuzzleOverFireSystem.transform.localEulerAngles = new Vector3(num, num, num);
				MuzzleOverFireSystem.Emit(Mathf.RoundToInt(num2));
				if (LoadedElements.Count == 0 && m_pan.GetPanContents() > 0f)
				{
					float panContents = m_pan.GetPanContents();
					m_pan.FlashBlast(Mathf.RoundToInt(panContents), Mathf.RoundToInt(panContents));
					m_pan.Ignite();
				}
				FireMuzzleSmoke();
				m_weapon.Fire(0f);
				if (m_weapon.RamRod.GetCurBarrel() == this)
				{
					m_weapon.RamRod.gameObject.SetActive(value: false);
					GameObject gameObject = UnityEngine.Object.Instantiate(m_weapon.RamRodProj, Muzzle.position, Muzzle.rotation);
					gameObject.GetComponent<BallisticProjectile>().Fire(Muzzle.forward, m_weapon);
					m_weapon.RamRod.GameObject.SetActive(value: false);
					m_weapon.RamRod.MountToBarrel(null, null);
				}
				for (int i = 0; (float)i < num2; i++)
				{
					Vector3 vector = Muzzle.forward * 0.005f;
					GameObject igniteProjectile_Visible = IgniteProjectile_Visible;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(igniteProjectile_Visible, Muzzle.position - vector, Muzzle.rotation);
					gameObject2.transform.Rotate(new Vector3(UnityEngine.Random.Range((0f - spread) * 2f, spread * 2f), UnityEngine.Random.Range((0f - spread) * 2f, spread * 2f), 0f));
					BallisticProjectile component = gameObject2.GetComponent<BallisticProjectile>();
					component.Fire(component.MuzzleVelocityBase, gameObject2.transform.forward, m_weapon);
				}
				m_weapon.PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
		}

		private void Fire()
		{
			if (LoadedElements.Count > 0 && LoadedElements[0].Type != 0)
			{
				m_isIgnited = false;
				return;
			}
			bool flag = false;
			bool flag2 = false;
			int numProjectilesInBarrel = GetNumProjectilesInBarrel();
			int numPowder = GetNumPowder();
			float num = Mathf.Lerp(0.2f, 3.1f, (float)numPowder / 60f);
			float t = Mathf.Lerp(0f, 1f, (float)numPowder / 60f);
			float spread = Spread;
			float num2 = PowderToVelMultCurve.Evaluate(numPowder);
			if (numProjectilesInBarrel > 0)
			{
				num2 *= 1f / (float)numProjectilesInBarrel;
			}
			else
			{
				num = 0.05f;
			}
			if (num > 3f)
			{
				flag2 = true;
			}
			if (numProjectilesInBarrel > 3 && numPowder > 15)
			{
				flag = true;
			}
			if (numProjectilesInBarrel > 0 && numPowder > 100)
			{
				flag = true;
			}
			spread += Spread * 0.2f * (float)(numProjectilesInBarrel - 1);
			if (flag)
			{
				spread *= 5f;
			}
			float num3 = Mathf.Lerp(MuzzleOverFireSystemScaleRange.x, MuzzleOverFireSystemScaleRange.y, t);
			float num4 = Mathf.Lerp(MuzzleOverFireSystemEmitRange.x, MuzzleOverFireSystemEmitRange.y, t);
			MuzzleOverFireSystem.transform.localEulerAngles = new Vector3(num3, num3, num3);
			MuzzleOverFireSystem.Emit(Mathf.RoundToInt(num4));
			m_pan.FlashBlast(Mathf.RoundToInt(num4) * 2, Mathf.RoundToInt(num4) * 2);
			int num5 = 3 * numProjectilesInBarrel;
			if (numProjectilesInBarrel > 0 && numPowder < num5)
			{
				LoadedElements.RemoveAt(0);
				m_weapon.PlayAudioAsHandling(AudEvent_Squib, m_pan.transform.position);
				m_isIgnited = false;
				return;
			}
			FireMuzzleSmoke();
			m_weapon.Fire(num);
			if (m_weapon.RamRod.GetCurBarrel() == this)
			{
				m_weapon.RamRod.gameObject.SetActive(value: false);
				GameObject gameObject = UnityEngine.Object.Instantiate(m_weapon.RamRodProj, Muzzle.position, Muzzle.rotation);
				gameObject.GetComponent<BallisticProjectile>().Fire(Muzzle.forward, m_weapon);
				m_weapon.RamRod.GameObject.SetActive(value: false);
				m_weapon.RamRod.MountToBarrel(null, null);
			}
			for (int i = 0; (float)i < num4; i++)
			{
				Vector3 vector = Muzzle.forward * 0.005f;
				GameObject original = IgniteProjectile_Visible;
				if (numProjectilesInBarrel > 0)
				{
					original = IgniteProjectile_NotVisible;
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original, Muzzle.position - vector, Muzzle.rotation);
				gameObject2.transform.Rotate(new Vector3(UnityEngine.Random.Range((0f - spread) * 2f, spread * 2f), UnityEngine.Random.Range((0f - spread) * 2f, spread * 2f), 0f));
				BallisticProjectile component = gameObject2.GetComponent<BallisticProjectile>();
				component.Fire(component.MuzzleVelocityBase, gameObject2.transform.forward, m_weapon);
			}
			if (flag)
			{
				m_weapon.PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.Explosion, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			else if (numProjectilesInBarrel > 0)
			{
				m_weapon.PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.Shotgun, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			else
			{
				m_weapon.PlayAudioGunShot(IsHighPressure: true, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			float min = 1f - GetMinProjPos() / BarrelLength;
			float max = 1f - GetMaxProjPos() / BarrelLength;
			float num6 = 1f * UnityEngine.Random.Range(min, max);
			spread += num6;
			for (int j = 0; j < numProjectilesInBarrel; j++)
			{
				Vector3 vector2 = Muzzle.forward * 0.005f;
				GameObject gameObject3 = UnityEngine.Object.Instantiate(ProjectilePrefab, Muzzle.position - vector2, Muzzle.rotation);
				gameObject3.transform.Rotate(new Vector3(UnityEngine.Random.Range(0f - spread, spread), UnityEngine.Random.Range(0f - spread, spread), 0f));
				BallisticProjectile component2 = gameObject3.GetComponent<BallisticProjectile>();
				component2.Fire(component2.MuzzleVelocityBase * num2 * VelocityMult, gameObject3.transform.forward, m_weapon);
			}
			ClearBarrel();
			if (flag2)
			{
				m_weapon.ForceBreakInteraction();
				m_weapon.RootRigidbody.velocity = m_weapon.transform.forward * -8f + m_weapon.transform.up * 1f;
				m_weapon.RootRigidbody.angularVelocity = m_weapon.transform.right * -5f;
			}
			if (flag)
			{
				m_weapon.Blowup();
			}
			m_isIgnited = false;
		}

		private void ClearBarrel()
		{
			LoadedElements.Clear();
		}

		private void BarrelContentsSim()
		{
			float num = Vector3.Angle(Muzzle.forward, Vector3.up);
			if (num > 90f)
			{
				if (LoadedElements.Count <= 0)
				{
					return;
				}
				for (int num2 = LoadedElements.Count - 1; num2 >= 0; num2--)
				{
					if (LoadedElements[num2].Type == LoadedElementType.Powder)
					{
						float min = 0f;
						float max = BarrelLength;
						bool flag = true;
						if (num2 + 1 < LoadedElements.Count)
						{
							min = LoadedElements[num2 + 1].Position + GetLengthOfElement(LoadedElements[num2 + 1].Type, LoadedElements[num2 + 1].PowderAmount);
							flag = false;
						}
						if (num2 - 1 >= 0)
						{
							max = LoadedElements[num2 - 1].Position;
						}
						float position = LoadedElements[num2].Position;
						float num3 = (num - 90f) / 90f * 2f;
						float num4 = position - num3 * Time.deltaTime;
						if (flag && num4 <= 0f)
						{
							int num5 = ExpellElement(LoadedElements[num2].Type, LoadedElements[num2].PowderAmount);
							if (num5 <= 0)
							{
								LoadedElements.RemoveAt(num2);
							}
							else
							{
								LoadedElements[num2].PowderAmount = num5;
							}
						}
						else
						{
							num4 = Mathf.Clamp(num4, min, max);
							LoadedElements[num2].Position = num4;
						}
					}
				}
			}
			else
			{
				if (LoadedElements.Count <= 0)
				{
					return;
				}
				for (int num6 = LoadedElements.Count - 1; num6 >= 0; num6--)
				{
					if (LoadedElements[num6].Type == LoadedElementType.Powder)
					{
						float min2 = 0f;
						float max2 = BarrelLength - GetLengthOfElement(LoadedElements[num6].Type, LoadedElements[num6].PowderAmount);
						if (num6 + 1 < LoadedElements.Count)
						{
							min2 = LoadedElements[num6 + 1].Position + GetLengthOfElement(LoadedElements[num6 + 1].Type, LoadedElements[num6 + 1].PowderAmount);
						}
						if (num6 - 1 >= 0)
						{
							max2 = LoadedElements[num6 - 1].Position;
						}
						float position2 = LoadedElements[num6].Position;
						float num7 = (1f - num / 90f) * 2f;
						float value = position2 + num7 * Time.deltaTime;
						value = Mathf.Clamp(value, min2, max2);
						LoadedElements[num6].Position = value;
					}
				}
			}
		}

		private void BarrelContentsDraw()
		{
			if (LoadedElements.Count > 1)
			{
				int index = LoadedElements.Count - 1;
				int index2 = LoadedElements.Count - 2;
				for (int i = 0; i < ProxyRends0.Count; i++)
				{
					if (LoadedElements[index].Type == (LoadedElementType)i)
					{
						ProxyRends0[i].enabled = true;
						ProxyRends0[i].transform.position = Muzzle.position - Muzzle.forward * LoadedElements[index].Position;
						if (LoadedElements[index].Type != 0 && LoadedElements[index].Position < 0.01f)
						{
							if (LoadedElements[index].Type == LoadedElementType.Shot)
							{
								ProxyRends0[i].transform.position = LodgePoint_Shot.position;
							}
							else if (LoadedElements[index].Type == LoadedElementType.ShotInPaper)
							{
								ProxyRends0[i].transform.position = LodgePoint_Paper.position;
							}
						}
						if (LoadedElements[index].Type == LoadedElementType.Powder)
						{
							int powderAmount = LoadedElements[index].PowderAmount;
							if (powderAmount > 15)
							{
								ProxyPowder0.mesh = ProxyPowderPiles[3];
							}
							else if (powderAmount > 9)
							{
								ProxyPowder0.mesh = ProxyPowderPiles[2];
							}
							else if (powderAmount > 4)
							{
								ProxyPowder0.mesh = ProxyPowderPiles[1];
							}
							else
							{
								ProxyPowder0.mesh = ProxyPowderPiles[0];
							}
						}
					}
					else
					{
						ProxyRends0[i].enabled = false;
					}
				}
				for (int j = 0; j < ProxyRends1.Count; j++)
				{
					if (LoadedElements[index2].Type == (LoadedElementType)j)
					{
						ProxyRends1[j].enabled = true;
						ProxyRends1[j].transform.position = Muzzle.position - Muzzle.forward * LoadedElements[index2].Position;
						if (LoadedElements[index2].Type == LoadedElementType.Powder)
						{
							int powderAmount2 = LoadedElements[index2].PowderAmount;
							if (powderAmount2 > 15)
							{
								ProxyPowder1.mesh = ProxyPowderPiles[3];
							}
							else if (powderAmount2 > 9)
							{
								ProxyPowder1.mesh = ProxyPowderPiles[2];
							}
							else if (powderAmount2 > 4)
							{
								ProxyPowder1.mesh = ProxyPowderPiles[1];
							}
							else
							{
								ProxyPowder1.mesh = ProxyPowderPiles[0];
							}
						}
					}
					else
					{
						ProxyRends1[j].enabled = false;
					}
				}
			}
			else if (LoadedElements.Count > 0)
			{
				for (int k = 0; k < ProxyRends0.Count; k++)
				{
					if (LoadedElements[0].Type == (LoadedElementType)k)
					{
						ProxyRends0[k].enabled = true;
						ProxyRends0[k].transform.position = Muzzle.position - Muzzle.forward * LoadedElements[0].Position;
						if (LoadedElements[0].Type != 0 && LoadedElements[0].Position < 0.01f)
						{
							if (LoadedElements[0].Type == LoadedElementType.Shot)
							{
								ProxyRends0[k].transform.position = LodgePoint_Shot.position;
							}
							else if (LoadedElements[0].Type == LoadedElementType.ShotInPaper)
							{
								ProxyRends0[k].transform.position = LodgePoint_Paper.position;
							}
						}
						if (LoadedElements[0].Type == LoadedElementType.Powder)
						{
							int powderAmount3 = LoadedElements[0].PowderAmount;
							if (powderAmount3 > 15)
							{
								ProxyPowder0.mesh = ProxyPowderPiles[3];
							}
							else if (powderAmount3 > 9)
							{
								ProxyPowder0.mesh = ProxyPowderPiles[2];
							}
							else if (powderAmount3 > 4)
							{
								ProxyPowder0.mesh = ProxyPowderPiles[1];
							}
							else
							{
								ProxyPowder0.mesh = ProxyPowderPiles[0];
							}
						}
					}
					else
					{
						ProxyRends0[k].enabled = false;
					}
				}
				for (int l = 0; l < ProxyRends1.Count; l++)
				{
					ProxyRends1[l].enabled = false;
				}
			}
			else
			{
				for (int m = 0; m < ProxyRends0.Count; m++)
				{
					ProxyRends0[m].enabled = false;
				}
				for (int n = 0; n < ProxyRends1.Count; n++)
				{
					ProxyRends1[n].enabled = false;
				}
			}
		}
	}
}
