using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class Banger : FVRPhysicalObject, IFVRDamageable
	{
		public enum BangerType
		{
			Impact,
			Remote,
			Timed,
			Proximity
		}

		public enum BangerPayloadSize
		{
			Small,
			Medium,
			Large
		}

		public class PowerUpSplode
		{
			public FVRObject Obj;

			public PowerUpIntensity Int;

			public PowerUpDuration Dur;

			public bool isPuke;

			public bool isInverted;
		}

		[Header("Banger Params")]
		public BangerType BType;

		public BangerPayloadSize BSize;

		public GameObject Display;

		public Renderer Rend;

		public List<Material> Mats;

		public Rigidbody RB;

		private bool m_isArmed;

		private bool m_isExploding;

		private bool m_isDestroyed;

		private float m_timeToPayload = 0.1f;

		public List<GameObject> Payloads = new List<GameObject>();

		private int m_curPayLoad;

		private float m_timeToPowerupSplode = 0.1f;

		private List<PowerUpSplode> PowerupSplodes = new List<PowerUpSplode>();

		private int m_curPowerupSplode;

		public List<GameObject> Shrapnel = new List<GameObject>();

		public List<int> ShrapnelLeftToFire = new List<int>();

		public int numShrapnelPerFrame = 5;

		private int m_curShrapnelSet;

		[Header("Dial Params")]
		public BangerDial BDial;

		[Header("Prox Params")]
		public LayerMask LM_Prox;

		public float ProxRange;

		private float m_proxTick = 1f;

		[Header("Arming Params")]
		public BangerSwitch BSwitch;

		public GameObject Light_Unarmed;

		public GameObject Light_Armed;

		public AudioEvent AudEvent_ArmingSound;

		public AudioEvent AudEvent_DeArmingSound;

		public AudioEvent AudEvent_Detonante;

		private float m_timeSinceArmed;

		public Text TextList;

		private string descrip = string.Empty;

		[Header("BespokePayloads")]
		public List<GameObject> ShrapnelOnlyBaseExplosion;

		public FVRObject BaseShrapnelObj;

		public List<GameObject> Splodes_Flash;

		private Vector2 m_shrapnelVel = new Vector2(0.1f, 0.3f);

		private bool m_isSticky;

		private bool m_isSilent;

		private bool m_isHoming;

		private bool m_canbeshot;

		public PhysicMaterial PhysMat_Bouncy;

		private bool m_isStuck;

		private FixedJoint m_j;

		private bool m_hasJoint;

		public bool IsArmed => m_isArmed;

		public override bool IsInteractable()
		{
			if (m_isStuck)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override bool IsDistantGrabbable()
		{
			if (m_isStuck)
			{
				return false;
			}
			return base.IsDistantGrabbable();
		}

		public void Damage(Damage d)
		{
			if (m_canbeshot && d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				StartExploding();
			}
		}

		public void SetMat(int i)
		{
			Rend.material = Mats[i];
		}

		public void Arm()
		{
			m_isArmed = true;
			Light_Unarmed.SetActive(value: false);
			Light_Armed.SetActive(value: true);
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_ArmingSound, base.transform.position);
			m_timeSinceArmed = 0f;
			if (BType != BangerType.Remote)
			{
			}
		}

		public void DeArm()
		{
			if (m_isExploding)
			{
				return;
			}
			if (m_isStuck && m_isSticky)
			{
				base.RootRigidbody.isKinematic = false;
				if (m_j != null)
				{
					Object.Destroy(m_j);
				}
				m_isStuck = false;
			}
			m_isArmed = false;
			Light_Unarmed.SetActive(value: true);
			Light_Armed.SetActive(value: false);
			SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_DeArmingSound, base.transform.position);
			m_timeSinceArmed = 0f;
			if (BType != BangerType.Remote)
			{
			}
		}

		public void Complete()
		{
			if (Payloads.Count < 1)
			{
				for (int i = 0; i < ShrapnelOnlyBaseExplosion.Count; i++)
				{
					Payloads.Add(ShrapnelOnlyBaseExplosion[i]);
				}
			}
			TextList.text = descrip;
		}

		public void LoadPayload(FVRPhysicalObject o)
		{
			if (o is RotrwHerb)
			{
				RotrwHerb rotrwHerb = o as RotrwHerb;
				if (rotrwHerb.Type == RotrwHerb.HerbType.GiantBlueRaspberry)
				{
					m_isSilent = true;
				}
				for (int i = 0; i < rotrwHerb.PayloadOnDetonate.Count; i++)
				{
					Payloads.Add(rotrwHerb.PayloadOnDetonate[i]);
				}
				switch (rotrwHerb.Type)
				{
				case RotrwHerb.HerbType.KatchupLeaf:
					descrip += "BOOM! \n";
					break;
				case RotrwHerb.HerbType.MustardWillow:
					descrip += "Burny \n";
					break;
				case RotrwHerb.HerbType.PricklyPickle:
					descrip += "Bright \n";
					break;
				case RotrwHerb.HerbType.GiantBlueRaspberry:
					descrip += "Shhhhh \n";
					break;
				case RotrwHerb.HerbType.DeadlyEggplant:
					descrip += "Smokey \n";
					break;
				}
			}
			else if (o is FVRGrenade)
			{
				FVRGrenade fVRGrenade = o as FVRGrenade;
				if (fVRGrenade.ExplosionFX != null)
				{
					Payloads.Add(fVRGrenade.ExplosionFX);
				}
				if (fVRGrenade.ExplosionSoundFX != null)
				{
					Payloads.Add(fVRGrenade.ExplosionSoundFX);
				}
				descrip += "BOOM! \n";
			}
			else if (o is FVRCappedGrenade)
			{
				FVRCappedGrenade fVRCappedGrenade = o as FVRCappedGrenade;
				for (int j = 0; j < fVRCappedGrenade.SpawnOnDestruction.Length; j++)
				{
					Payloads.Add(fVRCappedGrenade.SpawnOnDestruction[j]);
				}
				descrip += "BOOM! \n";
			}
			else if (o is Molotov)
			{
				Molotov molotov = o as Molotov;
				Payloads.Add(molotov.Prefab_FireSplosion);
				descrip += "Burny \n";
			}
			else if (o is FVRFusedThrowable)
			{
				FVRFusedThrowable fVRFusedThrowable = o as FVRFusedThrowable;
				if (fVRFusedThrowable.Fuse.ExplosionVFX != null)
				{
					Payloads.Add(fVRFusedThrowable.Fuse.ExplosionVFX);
				}
				if (fVRFusedThrowable.Fuse.ExplosionSFX != null)
				{
					Payloads.Add(fVRFusedThrowable.Fuse.ExplosionSFX);
				}
				descrip += "BOOM! \n";
			}
			else if (o is RotrwMeatCore)
			{
				RotrwMeatCore rotrwMeatCore = o as RotrwMeatCore;
				for (int k = 0; k < rotrwMeatCore.BangerSplosions.Count; k++)
				{
					Payloads.Add(rotrwMeatCore.BangerSplosions[k]);
				}
				switch (rotrwMeatCore.Type)
				{
				case RotrwMeatCore.CoreType.Tasty:
					descrip += "Nooothin \n";
					break;
				case RotrwMeatCore.CoreType.Moldy:
					descrip += "Nooothin \n";
					break;
				case RotrwMeatCore.CoreType.Spikey:
					m_isSticky = true;
					descrip += "Sticky \n";
					break;
				case RotrwMeatCore.CoreType.Zippy:
					m_isHoming = true;
					descrip += "GonnaGetcha! \n";
					break;
				case RotrwMeatCore.CoreType.Weighty:
					descrip += "WHAMMO \n";
					break;
				case RotrwMeatCore.CoreType.Juicy:
					descrip += "WHooosh \n";
					break;
				case RotrwMeatCore.CoreType.Shiny:
					descrip += "Sparkles! \n";
					break;
				case RotrwMeatCore.CoreType.Burny:
					descrip += "Burny \n";
					break;
				}
			}
			else if (o is BaitPie)
			{
				BaitPie baitPie = o as BaitPie;
				Payloads.Add(baitPie.CloudPrefab);
			}
			else if (o is FVRFireArmMagazine)
			{
				FVRFireArmMagazine fVRFireArmMagazine = o as FVRFireArmMagazine;
				int numRounds = fVRFireArmMagazine.m_numRounds;
				for (int l = 0; l < numRounds; l++)
				{
					GameObject gameObject = fVRFireArmMagazine.RemoveRound(b: false);
					AddShrapnel(gameObject.GetComponent<FVRFireArmRound>(), 1);
				}
				descrip += "Sharp bits \n";
			}
			else if (o is FVRFireArmClip)
			{
				FVRFireArmClip fVRFireArmClip = o as FVRFireArmClip;
				int numRounds2 = fVRFireArmClip.m_numRounds;
				for (int m = 0; m < numRounds2; m++)
				{
					GameObject gameObject2 = fVRFireArmClip.RemoveRound(b: false);
					AddShrapnel(gameObject2.GetComponent<FVRFireArmRound>(), 1);
				}
				descrip += "Sharp bits \n";
			}
			else if (o is Speedloader)
			{
				Speedloader speedloader = o as Speedloader;
				int count = speedloader.Chambers.Count;
				for (int n = 0; n < count; n++)
				{
					if (speedloader.Chambers[n].IsLoaded)
					{
						GameObject gameObject3 = AM.GetRoundSelfPrefab(speedloader.Chambers[n].Type, speedloader.Chambers[n].LoadedClass).GetGameObject();
						AddShrapnel(gameObject3.GetComponent<FVRFireArmRound>(), 1);
					}
				}
				descrip += "Sharp bits \n";
			}
			else if (o is FVRFireArmRound)
			{
				AddShrapnel(o as FVRFireArmRound, 1);
				descrip += "Sharp bit \n";
			}
			else if (o is RW_Powerup)
			{
				RW_Powerup rW_Powerup = o as RW_Powerup;
				PowerUpSplode powerUpSplode = new PowerUpSplode();
				powerUpSplode.Obj = rW_Powerup.ObjectWrapper;
				powerUpSplode.Dur = rW_Powerup.PowerupDuration;
				powerUpSplode.Int = rW_Powerup.PowerupIntensity;
				powerUpSplode.isPuke = rW_Powerup.isPuke;
				powerUpSplode.isInverted = rW_Powerup.isInverted;
				PowerupSplodes.Add(powerUpSplode);
				if (rW_Powerup.isInverted)
				{
					switch (rW_Powerup.PowerupType)
					{
					case PowerupType.Health:
						descrip += "Yummy! \n";
						break;
					case PowerupType.Invincibility:
						descrip += "Yes \n";
						break;
					case PowerupType.InfiniteAmmo:
						descrip += "LotsOfGuns \n";
						break;
					case PowerupType.Ghosted:
						descrip += "Boo! \n";
						break;
					case PowerupType.Explosive:
						descrip += "BOOM! \n";
						break;
					case PowerupType.FarOutMeat:
						descrip += "Narly duuuude \n";
						break;
					case PowerupType.HomeTown:
						descrip += "IWannaGoHoooome \n";
						break;
					case PowerupType.MuscleMeat:
						descrip += "*Flexes* \n";
						break;
					case PowerupType.QuadDamage:
						descrip += "Unreal \n";
						break;
					case PowerupType.Regen:
						descrip += "Feelin good \n";
						break;
					case PowerupType.SnakeEye:
						descrip += "Snaaaaake \n";
						break;
					case PowerupType.WheredIGo:
						descrip += "Huh? \n";
						break;
					case PowerupType.Cyclops:
						descrip += "Cyclopses \n";
						break;
					case PowerupType.Blort:
						descrip += "BLORT! \n";
						break;
					case PowerupType.Uncooked:
						descrip += "Nooothin \n";
						break;
					}
				}
				else
				{
					switch (rW_Powerup.PowerupType)
					{
					case PowerupType.Health:
						descrip += "Yummy! \n";
						break;
					case PowerupType.Invincibility:
						descrip += "Yes \n";
						break;
					case PowerupType.InfiniteAmmo:
						descrip += "LotsOfGuns \n";
						break;
					case PowerupType.Ghosted:
						descrip += "Boo! \n";
						break;
					case PowerupType.Explosive:
						descrip += "BOOM! \n";
						break;
					case PowerupType.FarOutMeat:
						descrip += "Narly duuuude \n";
						break;
					case PowerupType.HomeTown:
						descrip += "IWannaGoHoooome \n";
						break;
					case PowerupType.MuscleMeat:
						descrip += "*Flexes* \n";
						break;
					case PowerupType.QuadDamage:
						descrip += "Unreal \n";
						break;
					case PowerupType.Regen:
						descrip += "Feelin good \n";
						break;
					case PowerupType.SnakeEye:
						descrip += "Snaaaaake \n";
						break;
					case PowerupType.WheredIGo:
						descrip += "Huh? \n";
						break;
					case PowerupType.Cyclops:
						descrip += "Cyclopses \n";
						break;
					case PowerupType.Blort:
						descrip += "BLORT! \n";
						break;
					case PowerupType.Uncooked:
						descrip += "Nooothin \n";
						break;
					}
				}
			}
			else if (o is FVRFireArm)
			{
				m_shrapnelVel.x += 0.1f;
				m_shrapnelVel.y += 0.25f;
				descrip += "MOAR GUN \n";
			}
			else if (o is ShatterablePhysicalObject)
			{
				m_canbeshot = true;
				descrip += "Fragile \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Magnification)
			{
				ThrowVelMultiplier *= 1.4f;
				descrip += "GootaGoFaaaast \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Adapter)
			{
				m_isSticky = true;
				descrip += "Sticky \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Grip)
			{
				m_isSticky = true;
				descrip += "Sticky \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Stock)
			{
				ThrowAngMultiplier = 0f;
				descrip += "Stable \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.RecoilMitigation)
			{
				base.RootRigidbody.drag = 4f;
				base.RootRigidbody.angularDrag = 4f;
				descrip += "What A Drag \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Reflex)
			{
				SetToBouncy();
				descrip += "Bouncies! \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Illumination)
			{
				for (int num = 0; num < Splodes_Flash.Count; num++)
				{
					Payloads.Add(Splodes_Flash[num]);
				}
				descrip += "Bright \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Laser)
			{
				m_isHoming = true;
				descrip += "GonnaGetcha! \n";
			}
			else if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Suppression)
			{
				m_isSilent = true;
				descrip += "Shhhhh! \n";
			}
			else
			{
				GameObject gameObject4 = BaseShrapnelObj.GetGameObject();
				FVRFireArmRound component = gameObject4.GetComponent<FVRFireArmRound>();
				int amount = Random.Range(3, 15);
				AddShrapnel(component, amount);
				descrip += "Sharp bits \n";
			}
		}

		private void SetToBouncy()
		{
			for (int i = 0; i < m_colliders.Length; i++)
			{
				m_colliders[i].material = PhysMat_Bouncy;
			}
			base.RootRigidbody.drag = 0f;
		}

		private void AddShrapnel(FVRFireArmRound r, int amount)
		{
			int num = Shrapnel.IndexOf(r.BallisticProjectilePrefab);
			if (num > -1)
			{
				ShrapnelLeftToFire[num] += r.NumProjectiles * amount;
				return;
			}
			Shrapnel.Add(r.BallisticProjectilePrefab);
			ShrapnelLeftToFire.Add(r.NumProjectiles * amount);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isArmed && m_isSticky && m_isStuck)
			{
				if (m_j == null)
				{
					m_hasJoint = false;
				}
				if (m_hasJoint && m_j.connectedBody == null)
				{
					Object.Destroy(m_j);
					m_isStuck = false;
				}
			}
			if (BType == BangerType.Timed)
			{
				if (m_isArmed)
				{
					BDial.TickDown();
				}
			}
			else if (BType == BangerType.Proximity)
			{
				if (!base.IsHeld && m_isArmed)
				{
					m_timeSinceArmed += Time.deltaTime;
				}
				else
				{
					m_timeSinceArmed = 0f;
				}
				if (m_timeSinceArmed > 5f && Physics.CheckSphere(base.transform.position, ProxRange, LM_Prox, QueryTriggerInteraction.Ignore))
				{
					StartExploding();
				}
			}
			if (!m_isExploding)
			{
				return;
			}
			m_timeToPayload -= Time.deltaTime;
			if (m_timeToPayload <= 0f)
			{
				SpawnPayload(m_curPayLoad);
			}
			m_timeToPowerupSplode -= Time.deltaTime;
			if (m_timeToPowerupSplode <= 0f)
			{
				SpawnPowerupSplode(m_curPowerupSplode);
			}
			if (m_curShrapnelSet < ShrapnelLeftToFire.Count)
			{
				int num = Mathf.Min(numShrapnelPerFrame, ShrapnelLeftToFire[m_curShrapnelSet]);
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = Object.Instantiate(Shrapnel[m_curShrapnelSet], base.transform.position, Random.rotation);
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.Fire(component.MuzzleVelocityBase * Random.Range(0.2f, 0.5f), Random.onUnitSphere, null);
				}
				ShrapnelLeftToFire[m_curShrapnelSet] -= num;
				if (ShrapnelLeftToFire[m_curShrapnelSet] <= 0)
				{
					m_curShrapnelSet++;
				}
			}
			bool flag = true;
			if (m_curPowerupSplode < PowerupSplodes.Count)
			{
				flag = false;
			}
			if (m_curPayLoad < Payloads.Count)
			{
				flag = false;
			}
			if (m_curShrapnelSet < ShrapnelLeftToFire.Count)
			{
				flag = false;
			}
			if (flag)
			{
				m_isDestroyed = true;
				Object.Destroy(base.gameObject);
			}
		}

		private void SpawnPayload(int i)
		{
			if (m_isDestroyed || m_curPayLoad >= Payloads.Count)
			{
				return;
			}
			if (i == 0)
			{
				Display.SetActive(value: false);
			}
			GameObject gameObject = Object.Instantiate(Payloads[m_curPayLoad], base.transform.position + Random.onUnitSphere * 0.02f, Random.rotation);
			if (m_isSilent)
			{
				ExplosionSound component = gameObject.GetComponent<ExplosionSound>();
				if (component != null)
				{
					component.CancelSound();
				}
			}
			RW_Powerup component2 = gameObject.GetComponent<RW_Powerup>();
			if (component2 != null)
			{
				component2.Detonate();
			}
			m_curPayLoad++;
			m_timeToPayload = Random.Range(0.05f, 0.1f);
		}

		private void SpawnPowerupSplode(int i)
		{
			if (!m_isDestroyed && m_curPowerupSplode < PowerupSplodes.Count)
			{
				if (i == 0)
				{
					Display.SetActive(value: false);
				}
				GameObject gameObject = Object.Instantiate(PowerupSplodes[m_curPowerupSplode].Obj.GetGameObject(), base.transform.position + Random.onUnitSphere * 0.02f, Random.rotation);
				RW_Powerup component = gameObject.GetComponent<RW_Powerup>();
				component.SetParams(component.PowerupType, PowerupSplodes[m_curPowerupSplode].Int, PowerupSplodes[m_curPowerupSplode].Dur, PowerupSplodes[m_curPowerupSplode].isPuke, PowerupSplodes[m_curPowerupSplode].isInverted);
				component.Detonate();
				m_curPowerupSplode++;
				m_timeToPowerupSplode = Random.Range(0.05f, 0.1f);
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (BType == BangerType.Impact)
			{
				if (col.relativeVelocity.magnitude > 2.5f)
				{
					StartExploding();
				}
			}
			else if (m_isArmed && m_isSticky && !m_isStuck)
			{
				m_isStuck = true;
				ForceBreakInteraction();
				if (col.collider.attachedRigidbody != null)
				{
					m_j = base.gameObject.AddComponent<FixedJoint>();
					m_j.connectedBody = col.collider.attachedRigidbody;
					m_j.enableCollision = false;
					m_hasJoint = true;
				}
				else
				{
					base.RootRigidbody.isKinematic = true;
				}
			}
		}

		public void StartExploding()
		{
			base.RootRigidbody.isKinematic = true;
			if (m_j != null)
			{
				Object.Destroy(m_j);
			}
			SetAllCollidersToLayer(triggersToo: true, "NoCol");
			if (m_isArmed && !m_isExploding)
			{
				if (BType == BangerType.Remote)
				{
				}
				m_isExploding = true;
				float delay = Vector3.Distance(GM.CurrentPlayerBody.transform.position, base.transform.position) / 343f;
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Detonante, base.transform.position, delay);
				switch (BType)
				{
				case BangerType.Impact:
					m_timeToPayload = 0.02f;
					break;
				case BangerType.Proximity:
					m_timeToPayload = 0.15f;
					break;
				case BangerType.Remote:
					m_timeToPayload = 0.35f;
					break;
				case BangerType.Timed:
					m_timeToPayload = 0.15f;
					break;
				}
			}
		}
	}
}
