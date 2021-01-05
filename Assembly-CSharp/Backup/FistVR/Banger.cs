// Decompiled with JetBrains decompiler
// Type: FistVR.Banger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class Banger : FVRPhysicalObject, IFVRDamageable
  {
    [Header("Banger Params")]
    public Banger.BangerType BType;
    public Banger.BangerPayloadSize BSize;
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
    private List<Banger.PowerUpSplode> PowerupSplodes = new List<Banger.PowerUpSplode>();
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

    public bool IsArmed => this.m_isArmed;

    public override bool IsInteractable() => !this.m_isStuck && base.IsInteractable();

    public override bool IsDistantGrabbable() => !this.m_isStuck && base.IsDistantGrabbable();

    public void Damage(FistVR.Damage d)
    {
      if (!this.m_canbeshot || d.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      this.StartExploding();
    }

    public void SetMat(int i) => this.Rend.material = this.Mats[i];

    public void Arm()
    {
      this.m_isArmed = true;
      this.Light_Unarmed.SetActive(false);
      this.Light_Armed.SetActive(true);
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_ArmingSound, this.transform.position);
      this.m_timeSinceArmed = 0.0f;
      if (this.BType != Banger.BangerType.Remote)
        ;
    }

    public void DeArm()
    {
      if (this.m_isExploding)
        return;
      if (this.m_isStuck && this.m_isSticky)
      {
        this.RootRigidbody.isKinematic = false;
        if ((Object) this.m_j != (Object) null)
          Object.Destroy((Object) this.m_j);
        this.m_isStuck = false;
      }
      this.m_isArmed = false;
      this.Light_Unarmed.SetActive(true);
      this.Light_Armed.SetActive(false);
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_DeArmingSound, this.transform.position);
      this.m_timeSinceArmed = 0.0f;
      if (this.BType != Banger.BangerType.Remote)
        ;
    }

    public void Complete()
    {
      if (this.Payloads.Count < 1)
      {
        for (int index = 0; index < this.ShrapnelOnlyBaseExplosion.Count; ++index)
          this.Payloads.Add(this.ShrapnelOnlyBaseExplosion[index]);
      }
      this.TextList.text = this.descrip;
    }

    public void LoadPayload(FVRPhysicalObject o)
    {
      switch (o)
      {
        case RotrwHerb _:
          RotrwHerb rotrwHerb = o as RotrwHerb;
          if (rotrwHerb.Type == RotrwHerb.HerbType.GiantBlueRaspberry)
            this.m_isSilent = true;
          for (int index = 0; index < rotrwHerb.PayloadOnDetonate.Count; ++index)
            this.Payloads.Add(rotrwHerb.PayloadOnDetonate[index]);
          switch (rotrwHerb.Type)
          {
            case RotrwHerb.HerbType.KatchupLeaf:
              this.descrip += "BOOM! \n";
              return;
            case RotrwHerb.HerbType.MustardWillow:
              this.descrip += "Burny \n";
              return;
            case RotrwHerb.HerbType.PricklyPickle:
              this.descrip += "Bright \n";
              return;
            case RotrwHerb.HerbType.GiantBlueRaspberry:
              this.descrip += "Shhhhh \n";
              return;
            case RotrwHerb.HerbType.DeadlyEggplant:
              this.descrip += "Smokey \n";
              return;
            default:
              return;
          }
        case FVRGrenade _:
          FVRGrenade fvrGrenade = o as FVRGrenade;
          if ((Object) fvrGrenade.ExplosionFX != (Object) null)
            this.Payloads.Add(fvrGrenade.ExplosionFX);
          if ((Object) fvrGrenade.ExplosionSoundFX != (Object) null)
            this.Payloads.Add(fvrGrenade.ExplosionSoundFX);
          this.descrip += "BOOM! \n";
          break;
        case FVRCappedGrenade _:
          FVRCappedGrenade fvrCappedGrenade = o as FVRCappedGrenade;
          for (int index = 0; index < fvrCappedGrenade.SpawnOnDestruction.Length; ++index)
            this.Payloads.Add(fvrCappedGrenade.SpawnOnDestruction[index]);
          this.descrip += "BOOM! \n";
          break;
        case Molotov _:
          this.Payloads.Add((o as Molotov).Prefab_FireSplosion);
          this.descrip += "Burny \n";
          break;
        case FVRFusedThrowable _:
          FVRFusedThrowable fvrFusedThrowable = o as FVRFusedThrowable;
          if ((Object) fvrFusedThrowable.Fuse.ExplosionVFX != (Object) null)
            this.Payloads.Add(fvrFusedThrowable.Fuse.ExplosionVFX);
          if ((Object) fvrFusedThrowable.Fuse.ExplosionSFX != (Object) null)
            this.Payloads.Add(fvrFusedThrowable.Fuse.ExplosionSFX);
          this.descrip += "BOOM! \n";
          break;
        case RotrwMeatCore _:
          RotrwMeatCore rotrwMeatCore = o as RotrwMeatCore;
          for (int index = 0; index < rotrwMeatCore.BangerSplosions.Count; ++index)
            this.Payloads.Add(rotrwMeatCore.BangerSplosions[index]);
          switch (rotrwMeatCore.Type)
          {
            case RotrwMeatCore.CoreType.Tasty:
              this.descrip += "Nooothin \n";
              return;
            case RotrwMeatCore.CoreType.Moldy:
              this.descrip += "Nooothin \n";
              return;
            case RotrwMeatCore.CoreType.Spikey:
              this.m_isSticky = true;
              this.descrip += "Sticky \n";
              return;
            case RotrwMeatCore.CoreType.Zippy:
              this.m_isHoming = true;
              this.descrip += "GonnaGetcha! \n";
              return;
            case RotrwMeatCore.CoreType.Weighty:
              this.descrip += "WHAMMO \n";
              return;
            case RotrwMeatCore.CoreType.Juicy:
              this.descrip += "WHooosh \n";
              return;
            case RotrwMeatCore.CoreType.Shiny:
              this.descrip += "Sparkles! \n";
              return;
            case RotrwMeatCore.CoreType.Burny:
              this.descrip += "Burny \n";
              return;
            default:
              return;
          }
        case BaitPie _:
          this.Payloads.Add((o as BaitPie).CloudPrefab);
          break;
        case FVRFireArmMagazine _:
          FVRFireArmMagazine fvrFireArmMagazine = o as FVRFireArmMagazine;
          int numRounds1 = fvrFireArmMagazine.m_numRounds;
          for (int index = 0; index < numRounds1; ++index)
            this.AddShrapnel(fvrFireArmMagazine.RemoveRound(false).GetComponent<FVRFireArmRound>(), 1);
          this.descrip += "Sharp bits \n";
          break;
        case FVRFireArmClip _:
          FVRFireArmClip fvrFireArmClip = o as FVRFireArmClip;
          int numRounds2 = fvrFireArmClip.m_numRounds;
          for (int index = 0; index < numRounds2; ++index)
            this.AddShrapnel(fvrFireArmClip.RemoveRound(false).GetComponent<FVRFireArmRound>(), 1);
          this.descrip += "Sharp bits \n";
          break;
        case Speedloader _:
          Speedloader speedloader = o as Speedloader;
          int count = speedloader.Chambers.Count;
          for (int index = 0; index < count; ++index)
          {
            if (speedloader.Chambers[index].IsLoaded)
              this.AddShrapnel(AM.GetRoundSelfPrefab(speedloader.Chambers[index].Type, speedloader.Chambers[index].LoadedClass).GetGameObject().GetComponent<FVRFireArmRound>(), 1);
          }
          this.descrip += "Sharp bits \n";
          break;
        case FVRFireArmRound _:
          this.AddShrapnel(o as FVRFireArmRound, 1);
          this.descrip += "Sharp bit \n";
          break;
        case RW_Powerup _:
          RW_Powerup rwPowerup = o as RW_Powerup;
          this.PowerupSplodes.Add(new Banger.PowerUpSplode()
          {
            Obj = rwPowerup.ObjectWrapper,
            Dur = rwPowerup.PowerupDuration,
            Int = rwPowerup.PowerupIntensity,
            isPuke = rwPowerup.isPuke,
            isInverted = rwPowerup.isInverted
          });
          if (rwPowerup.isInverted)
          {
            switch (rwPowerup.PowerupType + 2)
            {
              case PowerupType.Health:
                this.descrip += "BOOM! \n";
                return;
              case PowerupType.QuadDamage:
                this.descrip += "Nooothin \n";
                return;
              case PowerupType.InfiniteAmmo:
                this.descrip += "Yummy! \n";
                return;
              case PowerupType.Invincibility:
                this.descrip += "Unreal \n";
                return;
              case PowerupType.Ghosted:
                this.descrip += "LotsOfGuns \n";
                return;
              case PowerupType.FarOutMeat:
                this.descrip += "Yes \n";
                return;
              case PowerupType.MuscleMeat:
                this.descrip += "Boo! \n";
                return;
              case PowerupType.HomeTown:
                this.descrip += "Narly duuuude \n";
                return;
              case PowerupType.SnakeEye:
                this.descrip += "*Flexes* \n";
                return;
              case PowerupType.Blort:
                this.descrip += "IWannaGoHoooome \n";
                return;
              case PowerupType.Regen:
                this.descrip += "Snaaaaake \n";
                return;
              case PowerupType.Cyclops:
                this.descrip += "BLORT! \n";
                return;
              case PowerupType.WheredIGo:
                this.descrip += "Feelin good \n";
                return;
              case PowerupType.ChillOut:
                this.descrip += "Cyclopses \n";
                return;
              case PowerupType.Debuff:
                this.descrip += "Huh? \n";
                return;
              default:
                return;
            }
          }
          else
          {
            switch (rwPowerup.PowerupType + 2)
            {
              case PowerupType.Health:
                this.descrip += "BOOM! \n";
                return;
              case PowerupType.QuadDamage:
                this.descrip += "Nooothin \n";
                return;
              case PowerupType.InfiniteAmmo:
                this.descrip += "Yummy! \n";
                return;
              case PowerupType.Invincibility:
                this.descrip += "Unreal \n";
                return;
              case PowerupType.Ghosted:
                this.descrip += "LotsOfGuns \n";
                return;
              case PowerupType.FarOutMeat:
                this.descrip += "Yes \n";
                return;
              case PowerupType.MuscleMeat:
                this.descrip += "Boo! \n";
                return;
              case PowerupType.HomeTown:
                this.descrip += "Narly duuuude \n";
                return;
              case PowerupType.SnakeEye:
                this.descrip += "*Flexes* \n";
                return;
              case PowerupType.Blort:
                this.descrip += "IWannaGoHoooome \n";
                return;
              case PowerupType.Regen:
                this.descrip += "Snaaaaake \n";
                return;
              case PowerupType.Cyclops:
                this.descrip += "BLORT! \n";
                return;
              case PowerupType.WheredIGo:
                this.descrip += "Feelin good \n";
                return;
              case PowerupType.ChillOut:
                this.descrip += "Cyclopses \n";
                return;
              case PowerupType.Debuff:
                this.descrip += "Huh? \n";
                return;
              default:
                return;
            }
          }
        case FVRFireArm _:
          this.m_shrapnelVel.x += 0.1f;
          this.m_shrapnelVel.y += 0.25f;
          this.descrip += "MOAR GUN \n";
          break;
        case ShatterablePhysicalObject _:
          this.m_canbeshot = true;
          this.descrip += "Fragile \n";
          break;
        default:
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Magnification)
          {
            this.ThrowVelMultiplier *= 1.4f;
            this.descrip += "GootaGoFaaaast \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Adapter)
          {
            this.m_isSticky = true;
            this.descrip += "Sticky \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Grip)
          {
            this.m_isSticky = true;
            this.descrip += "Sticky \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Stock)
          {
            this.ThrowAngMultiplier = 0.0f;
            this.descrip += "Stable \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.RecoilMitigation)
          {
            this.RootRigidbody.drag = 4f;
            this.RootRigidbody.angularDrag = 4f;
            this.descrip += "What A Drag \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Reflex)
          {
            this.SetToBouncy();
            this.descrip += "Bouncies! \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Illumination)
          {
            for (int index = 0; index < this.Splodes_Flash.Count; ++index)
              this.Payloads.Add(this.Splodes_Flash[index]);
            this.descrip += "Bright \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Laser)
          {
            this.m_isHoming = true;
            this.descrip += "GonnaGetcha! \n";
            break;
          }
          if (o.ObjectWrapper.TagAttachmentFeature == FVRObject.OTagAttachmentFeature.Suppression)
          {
            this.m_isSilent = true;
            this.descrip += "Shhhhh! \n";
            break;
          }
          this.AddShrapnel(this.BaseShrapnelObj.GetGameObject().GetComponent<FVRFireArmRound>(), Random.Range(3, 15));
          this.descrip += "Sharp bits \n";
          break;
      }
    }

    private void SetToBouncy()
    {
      for (int index = 0; index < this.m_colliders.Length; ++index)
        this.m_colliders[index].material = this.PhysMat_Bouncy;
      this.RootRigidbody.drag = 0.0f;
    }

    private void AddShrapnel(FVRFireArmRound r, int amount)
    {
      int num = this.Shrapnel.IndexOf(r.BallisticProjectilePrefab);
      if (num > -1)
      {
        List<int> shrapnelLeftToFire;
        int index;
        (shrapnelLeftToFire = this.ShrapnelLeftToFire)[index = num] = shrapnelLeftToFire[index] + r.NumProjectiles * amount;
      }
      else
      {
        this.Shrapnel.Add(r.BallisticProjectilePrefab);
        this.ShrapnelLeftToFire.Add(r.NumProjectiles * amount);
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_isArmed && this.m_isSticky && this.m_isStuck)
      {
        if ((Object) this.m_j == (Object) null)
          this.m_hasJoint = false;
        if (this.m_hasJoint && (Object) this.m_j.connectedBody == (Object) null)
        {
          Object.Destroy((Object) this.m_j);
          this.m_isStuck = false;
        }
      }
      if (this.BType == Banger.BangerType.Timed)
      {
        if (this.m_isArmed)
          this.BDial.TickDown();
      }
      else if (this.BType == Banger.BangerType.Proximity)
      {
        if (!this.IsHeld && this.m_isArmed)
          this.m_timeSinceArmed += Time.deltaTime;
        else
          this.m_timeSinceArmed = 0.0f;
        if ((double) this.m_timeSinceArmed > 5.0 && Physics.CheckSphere(this.transform.position, this.ProxRange, (int) this.LM_Prox, QueryTriggerInteraction.Ignore))
          this.StartExploding();
      }
      if (!this.m_isExploding)
        return;
      this.m_timeToPayload -= Time.deltaTime;
      if ((double) this.m_timeToPayload <= 0.0)
        this.SpawnPayload(this.m_curPayLoad);
      this.m_timeToPowerupSplode -= Time.deltaTime;
      if ((double) this.m_timeToPowerupSplode <= 0.0)
        this.SpawnPowerupSplode(this.m_curPowerupSplode);
      if (this.m_curShrapnelSet < this.ShrapnelLeftToFire.Count)
      {
        int num = Mathf.Min(this.numShrapnelPerFrame, this.ShrapnelLeftToFire[this.m_curShrapnelSet]);
        for (int index = 0; index < num; ++index)
        {
          BallisticProjectile component = Object.Instantiate<GameObject>(this.Shrapnel[this.m_curShrapnelSet], this.transform.position, Random.rotation).GetComponent<BallisticProjectile>();
          component.Fire(component.MuzzleVelocityBase * Random.Range(0.2f, 0.5f), Random.onUnitSphere, (FVRFireArm) null);
        }
        this.ShrapnelLeftToFire[this.m_curShrapnelSet] = this.ShrapnelLeftToFire[this.m_curShrapnelSet] - num;
        if (this.ShrapnelLeftToFire[this.m_curShrapnelSet] <= 0)
          ++this.m_curShrapnelSet;
      }
      bool flag = true;
      if (this.m_curPowerupSplode < this.PowerupSplodes.Count)
        flag = false;
      if (this.m_curPayLoad < this.Payloads.Count)
        flag = false;
      if (this.m_curShrapnelSet < this.ShrapnelLeftToFire.Count)
        flag = false;
      if (!flag)
        return;
      this.m_isDestroyed = true;
      Object.Destroy((Object) this.gameObject);
    }

    private void SpawnPayload(int i)
    {
      if (this.m_isDestroyed || this.m_curPayLoad >= this.Payloads.Count)
        return;
      if (i == 0)
        this.Display.SetActive(false);
      GameObject gameObject = Object.Instantiate<GameObject>(this.Payloads[this.m_curPayLoad], this.transform.position + Random.onUnitSphere * 0.02f, Random.rotation);
      if (this.m_isSilent)
      {
        ExplosionSound component = gameObject.GetComponent<ExplosionSound>();
        if ((Object) component != (Object) null)
          component.CancelSound();
      }
      RW_Powerup component1 = gameObject.GetComponent<RW_Powerup>();
      if ((Object) component1 != (Object) null)
        component1.Detonate();
      ++this.m_curPayLoad;
      this.m_timeToPayload = Random.Range(0.05f, 0.1f);
    }

    private void SpawnPowerupSplode(int i)
    {
      if (this.m_isDestroyed || this.m_curPowerupSplode >= this.PowerupSplodes.Count)
        return;
      if (i == 0)
        this.Display.SetActive(false);
      RW_Powerup component = Object.Instantiate<GameObject>(this.PowerupSplodes[this.m_curPowerupSplode].Obj.GetGameObject(), this.transform.position + Random.onUnitSphere * 0.02f, Random.rotation).GetComponent<RW_Powerup>();
      component.SetParams(component.PowerupType, this.PowerupSplodes[this.m_curPowerupSplode].Int, this.PowerupSplodes[this.m_curPowerupSplode].Dur, this.PowerupSplodes[this.m_curPowerupSplode].isPuke, this.PowerupSplodes[this.m_curPowerupSplode].isInverted);
      component.Detonate();
      ++this.m_curPowerupSplode;
      this.m_timeToPowerupSplode = Random.Range(0.05f, 0.1f);
    }

    public override void OnCollisionEnter(Collision col)
    {
      base.OnCollisionEnter(col);
      if (this.BType == Banger.BangerType.Impact)
      {
        if ((double) col.relativeVelocity.magnitude <= 2.5)
          return;
        this.StartExploding();
      }
      else
      {
        if (!this.m_isArmed || !this.m_isSticky || this.m_isStuck)
          return;
        this.m_isStuck = true;
        this.ForceBreakInteraction();
        if ((Object) col.collider.attachedRigidbody != (Object) null)
        {
          this.m_j = this.gameObject.AddComponent<FixedJoint>();
          this.m_j.connectedBody = col.collider.attachedRigidbody;
          this.m_j.enableCollision = false;
          this.m_hasJoint = true;
        }
        else
          this.RootRigidbody.isKinematic = true;
      }
    }

    public void StartExploding()
    {
      this.RootRigidbody.isKinematic = true;
      if ((Object) this.m_j != (Object) null)
        Object.Destroy((Object) this.m_j);
      this.SetAllCollidersToLayer(true, "NoCol");
      if (!this.m_isArmed || this.m_isExploding)
        return;
      if (this.BType != Banger.BangerType.Remote)
        ;
      this.m_isExploding = true;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, this.AudEvent_Detonante, this.transform.position, Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.transform.position) / 343f);
      switch (this.BType)
      {
        case Banger.BangerType.Impact:
          this.m_timeToPayload = 0.02f;
          break;
        case Banger.BangerType.Remote:
          this.m_timeToPayload = 0.35f;
          break;
        case Banger.BangerType.Timed:
          this.m_timeToPayload = 0.15f;
          break;
        case Banger.BangerType.Proximity:
          this.m_timeToPayload = 0.15f;
          break;
      }
    }

    public enum BangerType
    {
      Impact,
      Remote,
      Timed,
      Proximity,
    }

    public enum BangerPayloadSize
    {
      Small,
      Medium,
      Large,
    }

    public class PowerUpSplode
    {
      public FVRObject Obj;
      public PowerUpIntensity Int;
      public PowerUpDuration Dur;
      public bool isPuke;
      public bool isInverted;
    }
  }
}
