// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockBarrel
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FlintlockBarrel : MonoBehaviour
  {
    private FlintlockWeapon m_weapon;
    private FlintlockFlashPan m_pan;
    [Header("Barrel")]
    public Transform Muzzle;
    public Transform LodgePoint_Paper;
    public Transform LodgePoint_Shot;
    public float BarrelLength = 0.21f;
    public List<FlintlockBarrel.LoadedElement> LoadedElements = new List<FlintlockBarrel.LoadedElement>();
    public List<Renderer> ProxyRends0;
    public List<Renderer> ProxyRends1;
    public MeshFilter ProxyPowder0;
    public MeshFilter ProxyPowder1;
    public List<Mesh> ProxyPowderPiles;
    public List<GameObject> EjectedObjectPrefabs;
    public Vector4 LoadedElementSizes = new Vector4(1f / 1000f, 7f / 400f, 0.0195f, 0.01f);
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

    public FlintlockWeapon GetWeapon() => this.m_weapon;

    public void SetWeapon(FlintlockWeapon w) => this.m_weapon = w;

    public void SetPan(FlintlockFlashPan p) => this.m_pan = p;

    public float GetLengthOfElement(FlintlockBarrel.LoadedElementType Type, int PowderAmount)
    {
      switch (Type)
      {
        case FlintlockBarrel.LoadedElementType.Powder:
          return this.LoadedElementSizes.x * (float) PowderAmount;
        case FlintlockBarrel.LoadedElementType.Shot:
          return this.LoadedElementSizes.y;
        case FlintlockBarrel.LoadedElementType.ShotInPaper:
          return this.LoadedElementSizes.z;
        case FlintlockBarrel.LoadedElementType.Wadding:
          return this.LoadedElementSizes.w;
        default:
          return 0.0f;
      }
    }

    private bool CanElementFit(FlintlockBarrel.LoadedElementType Type) => this.LoadedElements.Count == 0 || (double) this.LoadedElements[this.LoadedElements.Count - 1].Position > (double) this.GetLengthOfElement(Type, 5);

    private void Awake() => this.RegenerateMuzzleEffects();

    private void RegenerateMuzzleEffects()
    {
      for (int index = 0; index < this.m_muzzleSystems.Count; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_muzzleSystems[index].PSystem);
      this.m_muzzleSystems.Clear();
      MuzzleEffect[] muzzleEffects = this.MuzzleEffects;
      for (int index1 = 0; index1 < muzzleEffects.Length; ++index1)
      {
        if (muzzleEffects[index1].Entry != MuzzleEffectEntry.None)
        {
          MuzzleEffectConfig muzzleConfig = FXM.GetMuzzleConfig(muzzleEffects[index1].Entry);
          MuzzleEffectSize size = muzzleEffects[index1].Size;
          GameObject gameObject = !GM.CurrentSceneSettings.IsSceneLowLight ? UnityEngine.Object.Instantiate<GameObject>(muzzleConfig.Prefabs_Highlight[(int) size], this.Muzzle.position, this.Muzzle.rotation) : UnityEngine.Object.Instantiate<GameObject>(muzzleConfig.Prefabs_Lowlight[(int) size], this.Muzzle.position, this.Muzzle.rotation);
          if ((UnityEngine.Object) muzzleEffects[index1].OverridePoint == (UnityEngine.Object) null)
          {
            gameObject.transform.SetParent(this.Muzzle.transform);
          }
          else
          {
            gameObject.transform.SetParent(muzzleEffects[index1].OverridePoint);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
          }
          MuzzlePSystem muzzlePsystem = new MuzzlePSystem();
          muzzlePsystem.PSystem = gameObject.GetComponent<ParticleSystem>();
          muzzlePsystem.OverridePoint = muzzleEffects[index1].OverridePoint;
          int index2 = (int) size;
          muzzlePsystem.NumParticlesPerShot = !GM.CurrentSceneSettings.IsSceneLowLight ? muzzleConfig.NumParticles_Highlight[index2] : muzzleConfig.NumParticles_Lowlight[index2];
          this.m_muzzleSystems.Add(muzzlePsystem);
        }
      }
    }

    public void FireMuzzleSmoke()
    {
      if (GM.CurrentSceneSettings.IsSceneLowLight)
        FXM.InitiateMuzzleFlash(this.Muzzle.position, this.Muzzle.forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
      for (int index = 0; index < this.m_muzzleSystems.Count; ++index)
      {
        if ((UnityEngine.Object) this.m_muzzleSystems[index].OverridePoint == (UnityEngine.Object) null)
          this.m_muzzleSystems[index].PSystem.transform.position = this.Muzzle.position;
        this.m_muzzleSystems[index].PSystem.Emit(this.m_muzzleSystems[index].NumParticlesPerShot);
      }
    }

    private bool IsBarrelPlugged() => this.LoadedElements.Count != 0 && this.LoadedElements[this.LoadedElements.Count - 1].Type != FlintlockBarrel.LoadedElementType.Powder && (double) this.LoadedElements[this.LoadedElements.Count - 1].Position < 0.00999999977648258 && (this.LoadedElements[this.LoadedElements.Count - 1].Type == FlintlockBarrel.LoadedElementType.Shot || this.LoadedElements[this.LoadedElements.Count - 1].Type == FlintlockBarrel.LoadedElementType.ShotInPaper);

    public void OnTriggerEnter(Collider other)
    {
      if ((UnityEngine.Object) other.attachedRigidbody == (UnityEngine.Object) null || (double) Vector3.Angle(this.Muzzle.forward, Vector3.up) > 90.0)
        return;
      GameObject gameObject = other.attachedRigidbody.gameObject;
      if (gameObject.CompareTag("flintlock_shot"))
      {
        if (!this.CanElementFit(FlintlockBarrel.LoadedElementType.Shot) || this.m_weapon.RamRod.gameObject.activeSelf && this.m_weapon.RamRod.RState == FlintlockPseudoRamRod.RamRodState.Barrel && (UnityEngine.Object) this.m_weapon.RamRod.GetCurBarrel() == (UnityEngine.Object) this || this.IsBarrelPlugged())
          return;
        this.m_weapon.PlayAudioAsHandling(this.AudEvent_InsertByType[1], this.Muzzle.position);
        this.InsertElement(FlintlockBarrel.LoadedElementType.Shot);
        UnityEngine.Object.Destroy((UnityEngine.Object) other.gameObject);
      }
      else if (gameObject.CompareTag("flintlock_paper"))
      {
        if (!this.CanElementFit(FlintlockBarrel.LoadedElementType.ShotInPaper) || this.m_weapon.RamRod.gameObject.activeSelf && this.m_weapon.RamRod.RState == FlintlockPseudoRamRod.RamRodState.Barrel && (UnityEngine.Object) this.m_weapon.RamRod.GetCurBarrel() == (UnityEngine.Object) this || (this.IsBarrelPlugged() || (double) Vector3.Angle(this.Muzzle.forward, gameObject.transform.forward) > 80.0))
          return;
        FlintlockPaperCartridge component = gameObject.GetComponent<FlintlockPaperCartridge>();
        if (component.CState == FlintlockPaperCartridge.CartridgeState.Whole)
          return;
        this.m_weapon.PlayAudioAsHandling(this.AudEvent_InsertByType[2], this.Muzzle.position);
        for (int index = 0; index < component.numPowderChunksLeft; ++index)
          this.InsertElement(FlintlockBarrel.LoadedElementType.Powder);
        this.InsertElement(FlintlockBarrel.LoadedElementType.ShotInPaper);
        UnityEngine.Object.Destroy((UnityEngine.Object) other.gameObject);
      }
      else if (gameObject.CompareTag("flintlock_wadding"))
      {
        if (!this.CanElementFit(FlintlockBarrel.LoadedElementType.Wadding))
          return;
        this.m_weapon.PlayAudioAsHandling(this.AudEvent_InsertByType[3], this.Muzzle.position);
        this.InsertElement(FlintlockBarrel.LoadedElementType.Wadding);
        UnityEngine.Object.Destroy((UnityEngine.Object) other.gameObject);
      }
      else if (gameObject.CompareTag("flintlock_powdergrain"))
      {
        if (!this.CanElementFit(FlintlockBarrel.LoadedElementType.Powder))
          return;
        if ((double) this.m_insertSoundRefire > 0.150000005960464)
          this.m_weapon.PlayAudioAsHandling(this.AudEvent_InsertByType[0], this.Muzzle.position);
        this.InsertElement(FlintlockBarrel.LoadedElementType.Powder);
        UnityEngine.Object.Destroy((UnityEngine.Object) other.gameObject);
      }
      else
      {
        if (!gameObject.CompareTag("flintlock_ramrod"))
          return;
        FlintlockRamRod component = gameObject.GetComponent<FlintlockRamRod>();
        if (!component.IsHeld)
          return;
        FVRViveHand hand = component.m_hand;
        component.ForceBreakInteraction();
        this.m_weapon.RamRod.gameObject.SetActive(true);
        this.m_weapon.RamRod.RState = FlintlockPseudoRamRod.RamRodState.Barrel;
        this.m_weapon.RamRod.MountToBarrel(this, hand);
        this.Tamp(0.05f, 1f / 1000f);
        hand.ForceSetInteractable((FVRInteractiveObject) this.m_weapon.RamRod);
        this.m_weapon.RamRod.BeginInteraction(hand);
        UnityEngine.Object.Destroy((UnityEngine.Object) other.gameObject);
      }
    }

    public void Tamp(float delta, float depth)
    {
      if ((double) this.TampRefire < 0.150000005960464 || (double) Mathf.Abs(delta) < 0.00999999977648258)
        return;
      if (this.LoadedElements.Count == 1)
      {
        if ((double) Mathf.Abs(depth) <= (double) this.LoadedElements[0].Position)
          return;
        float position = this.LoadedElements[0].Position;
        float min = this.LoadedElements[0].Position + Mathf.Abs(delta);
        float max = this.BarrelLength - this.GetLengthOfElement(this.LoadedElements[0].Type, this.LoadedElements[0].PowderAmount);
        float num = Mathf.Clamp(min, min, max);
        this.LoadedElements[0].Position = num;
        if ((double) Mathf.Abs(position - num) > 1.0 / 1000.0)
          this.m_weapon.PlayAudioAsHandling(this.AudEvent_Tamp, this.Muzzle.position);
        else
          this.m_weapon.PlayAudioAsHandling(this.AudEvent_TampEnd, this.Muzzle.position);
        this.TampRefire = 0.0f;
      }
      else
      {
        if (this.LoadedElements.Count <= 1 || (double) Mathf.Abs(depth) <= (double) this.LoadedElements[this.LoadedElements.Count - 1].Position)
          return;
        float position = this.LoadedElements[this.LoadedElements.Count - 1].Position;
        float min = this.LoadedElements[this.LoadedElements.Count - 1].Position + Mathf.Abs(delta);
        float max = this.LoadedElements[this.LoadedElements.Count - 2].Position - this.GetLengthOfElement(this.LoadedElements[this.LoadedElements.Count - 1].Type, this.LoadedElements[this.LoadedElements.Count - 1].PowderAmount);
        float num = Mathf.Clamp(min, min, max);
        this.LoadedElements[this.LoadedElements.Count - 1].Position = num;
        if ((double) Mathf.Abs(position - num) > 1.0 / 1000.0)
          this.m_weapon.PlayAudioAsHandling(this.AudEvent_Tamp, this.Muzzle.position);
        else
          this.m_weapon.PlayAudioAsHandling(this.AudEvent_TampEnd, this.Muzzle.position);
        this.TampRefire = 0.0f;
      }
    }

    public float GetMaxDepth() => this.LoadedElements.Count < 1 ? this.BarrelLength : this.LoadedElements[this.LoadedElements.Count - 1].Position;

    private void InsertElement(FlintlockBarrel.LoadedElementType type)
    {
      if (this.LoadedElements.Count > 0)
      {
        if (type == FlintlockBarrel.LoadedElementType.Powder && this.LoadedElements[this.LoadedElements.Count - 1].Type == FlintlockBarrel.LoadedElementType.Powder)
          ++this.LoadedElements[this.LoadedElements.Count - 1].PowderAmount;
        else
          this.LoadedElements.Add(new FlintlockBarrel.LoadedElement()
          {
            Type = type,
            Position = 0.0f,
            PowderAmount = 1
          });
      }
      else
        this.LoadedElements.Add(new FlintlockBarrel.LoadedElement()
        {
          Type = type,
          Position = 0.0f,
          PowderAmount = 1
        });
    }

    private int ExpellElement(FlintlockBarrel.LoadedElementType type, int PowderAmount)
    {
      if (type != FlintlockBarrel.LoadedElementType.Powder)
      {
        Vector3 position = this.Muzzle.position + this.Muzzle.forward * this.GetLengthOfElement(type, PowderAmount) * 0.75f;
        UnityEngine.Object.Instantiate<GameObject>(this.EjectedObjectPrefabs[(int) type], position, this.Muzzle.rotation);
        return 0;
      }
      int num = PowderAmount - 1;
      Vector3 position1 = this.Muzzle.position + UnityEngine.Random.onUnitSphere * 0.005f + this.Muzzle.forward * this.GetLengthOfElement(type, 1) * 0.75f;
      UnityEngine.Object.Instantiate<GameObject>(this.EjectedObjectPrefabs[(int) type], position1, this.Muzzle.rotation);
      return num;
    }

    public void Ignite()
    {
      if (this.LoadedElements.Count <= 0 || this.LoadedElements[0].Type != FlintlockBarrel.LoadedElementType.Powder)
        return;
      this.m_isIgnited = true;
      this.m_igniteTick = UnityEngine.Random.Range(0.01f, 0.03f);
    }

    private void Update()
    {
      if ((double) this.TampRefire < 0.200000002980232)
        this.TampRefire += Time.deltaTime;
      if ((double) this.m_insertSoundRefire < 0.200000002980232)
        this.m_insertSoundRefire += Time.deltaTime;
      if (this.m_isIgnited)
      {
        this.m_igniteTick -= Time.deltaTime;
        if ((double) this.m_igniteTick > 0.0)
          return;
        this.Fire();
      }
      else
      {
        this.BarrelContentsSim();
        this.BarrelContentsDraw();
      }
    }

    private int GetNumProjectilesInBarrel()
    {
      int num = 0;
      for (int index = 0; index < this.LoadedElements.Count; ++index)
      {
        if (this.LoadedElements[index].Type == FlintlockBarrel.LoadedElementType.Shot || this.LoadedElements[index].Type == FlintlockBarrel.LoadedElementType.ShotInPaper)
          num += this.LoadedElements[index].PowderAmount;
      }
      return num;
    }

    private int GetNumPowder()
    {
      int num = 0;
      for (int index = 0; index < this.LoadedElements.Count; ++index)
      {
        if (this.LoadedElements[index].Type == FlintlockBarrel.LoadedElementType.Powder)
          num += this.LoadedElements[index].PowderAmount;
      }
      return num;
    }

    private float GetMinProjPos()
    {
      float a = this.BarrelLength;
      for (int index = 0; index < this.LoadedElements.Count; ++index)
      {
        if (this.LoadedElements[index].Type != FlintlockBarrel.LoadedElementType.Powder)
          a = Mathf.Min(a, this.LoadedElements[index].Position);
      }
      return a;
    }

    private float GetMaxProjPos()
    {
      float a = 0.0f;
      for (int index = 0; index < this.LoadedElements.Count; ++index)
      {
        if (this.LoadedElements[index].Type != FlintlockBarrel.LoadedElementType.Powder)
          a = Mathf.Max(a, this.LoadedElements[index].Position);
      }
      return a;
    }

    public void BurnOffOuter()
    {
      if (this.LoadedElements.Count == 0 || this.LoadedElements[this.LoadedElements.Count - 1].Type != FlintlockBarrel.LoadedElementType.Powder)
        return;
      int powderAmount = this.LoadedElements[this.LoadedElements.Count - 1].PowderAmount;
      this.LoadedElements.RemoveAt(this.LoadedElements.Count - 1);
      float t = Mathf.Lerp(0.0f, 1f, (float) powderAmount / 60f);
      float spread = this.Spread;
      float num = Mathf.Lerp(this.MuzzleOverFireSystemScaleRange.x, this.MuzzleOverFireSystemScaleRange.y, t);
      float f = Mathf.Lerp(this.MuzzleOverFireSystemEmitRange.x, this.MuzzleOverFireSystemEmitRange.y, t);
      this.MuzzleOverFireSystem.transform.localEulerAngles = new Vector3(num, num, num);
      this.MuzzleOverFireSystem.Emit(Mathf.RoundToInt(f));
      if (this.LoadedElements.Count == 0 && (double) this.m_pan.GetPanContents() > 0.0)
      {
        float panContents = this.m_pan.GetPanContents();
        this.m_pan.FlashBlast(Mathf.RoundToInt(panContents), Mathf.RoundToInt(panContents));
        this.m_pan.Ignite();
      }
      this.FireMuzzleSmoke();
      this.m_weapon.Fire(0.0f);
      if ((UnityEngine.Object) this.m_weapon.RamRod.GetCurBarrel() == (UnityEngine.Object) this)
      {
        this.m_weapon.RamRod.gameObject.SetActive(false);
        UnityEngine.Object.Instantiate<GameObject>(this.m_weapon.RamRodProj, this.Muzzle.position, this.Muzzle.rotation).GetComponent<BallisticProjectile>().Fire(this.Muzzle.forward, (FVRFireArm) this.m_weapon);
        this.m_weapon.RamRod.GameObject.SetActive(false);
        this.m_weapon.RamRod.MountToBarrel((FlintlockBarrel) null, (FVRViveHand) null);
      }
      for (int index = 0; (double) index < (double) f; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.IgniteProjectile_Visible, this.Muzzle.position - this.Muzzle.forward * 0.005f, this.Muzzle.rotation);
        gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range((float) (-(double) spread * 2.0), spread * 2f), UnityEngine.Random.Range((float) (-(double) spread * 2.0), spread * 2f), 0.0f));
        BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
        component.Fire(component.MuzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) this.m_weapon);
      }
      this.m_weapon.PlayAudioGunShot(true, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
    }

    private void Fire()
    {
      if (this.LoadedElements.Count > 0 && this.LoadedElements[0].Type != FlintlockBarrel.LoadedElementType.Powder)
      {
        this.m_isIgnited = false;
      }
      else
      {
        bool flag1 = false;
        bool flag2 = false;
        int projectilesInBarrel = this.GetNumProjectilesInBarrel();
        int numPowder = this.GetNumPowder();
        float recoilMult = Mathf.Lerp(0.2f, 3.1f, (float) numPowder / 60f);
        float t = Mathf.Lerp(0.0f, 1f, (float) numPowder / 60f);
        float spread = this.Spread;
        float num1 = this.PowderToVelMultCurve.Evaluate((float) numPowder);
        if (projectilesInBarrel > 0)
          num1 *= 1f / (float) projectilesInBarrel;
        else
          recoilMult = 0.05f;
        if ((double) recoilMult > 3.0)
          flag2 = true;
        if (projectilesInBarrel > 3 && numPowder > 15)
          flag1 = true;
        if (projectilesInBarrel > 0 && numPowder > 100)
          flag1 = true;
        float num2 = spread + this.Spread * 0.2f * (float) (projectilesInBarrel - 1);
        if (flag1)
          num2 *= 5f;
        float num3 = Mathf.Lerp(this.MuzzleOverFireSystemScaleRange.x, this.MuzzleOverFireSystemScaleRange.y, t);
        float f = Mathf.Lerp(this.MuzzleOverFireSystemEmitRange.x, this.MuzzleOverFireSystemEmitRange.y, t);
        this.MuzzleOverFireSystem.transform.localEulerAngles = new Vector3(num3, num3, num3);
        this.MuzzleOverFireSystem.Emit(Mathf.RoundToInt(f));
        this.m_pan.FlashBlast(Mathf.RoundToInt(f) * 2, Mathf.RoundToInt(f) * 2);
        int num4 = 3 * projectilesInBarrel;
        if (projectilesInBarrel > 0 && numPowder < num4)
        {
          this.LoadedElements.RemoveAt(0);
          this.m_weapon.PlayAudioAsHandling(this.AudEvent_Squib, this.m_pan.transform.position);
          this.m_isIgnited = false;
        }
        else
        {
          this.FireMuzzleSmoke();
          this.m_weapon.Fire(recoilMult);
          if ((UnityEngine.Object) this.m_weapon.RamRod.GetCurBarrel() == (UnityEngine.Object) this)
          {
            this.m_weapon.RamRod.gameObject.SetActive(false);
            UnityEngine.Object.Instantiate<GameObject>(this.m_weapon.RamRodProj, this.Muzzle.position, this.Muzzle.rotation).GetComponent<BallisticProjectile>().Fire(this.Muzzle.forward, (FVRFireArm) this.m_weapon);
            this.m_weapon.RamRod.GameObject.SetActive(false);
            this.m_weapon.RamRod.MountToBarrel((FlintlockBarrel) null, (FVRViveHand) null);
          }
          for (int index = 0; (double) index < (double) f; ++index)
          {
            Vector3 vector3 = this.Muzzle.forward * 0.005f;
            GameObject original = this.IgniteProjectile_Visible;
            if (projectilesInBarrel > 0)
              original = this.IgniteProjectile_NotVisible;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, this.Muzzle.position - vector3, this.Muzzle.rotation);
            gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range((float) (-(double) num2 * 2.0), num2 * 2f), UnityEngine.Random.Range((float) (-(double) num2 * 2.0), num2 * 2f), 0.0f));
            BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
            component.Fire(component.MuzzleVelocityBase, gameObject.transform.forward, (FVRFireArm) this.m_weapon);
          }
          if (flag1)
            this.m_weapon.PlayAudioGunShot(true, FVRTailSoundClass.Explosion, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
          else if (projectilesInBarrel > 0)
            this.m_weapon.PlayAudioGunShot(true, FVRTailSoundClass.Shotgun, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
          else
            this.m_weapon.PlayAudioGunShot(true, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedLarge, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
          float num5 = 1f * UnityEngine.Random.Range((float) (1.0 - (double) this.GetMinProjPos() / (double) this.BarrelLength), (float) (1.0 - (double) this.GetMaxProjPos() / (double) this.BarrelLength));
          float max = num2 + num5;
          for (int index = 0; index < projectilesInBarrel; ++index)
          {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ProjectilePrefab, this.Muzzle.position - this.Muzzle.forward * 0.005f, this.Muzzle.rotation);
            gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(-max, max), UnityEngine.Random.Range(-max, max), 0.0f));
            BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
            component.Fire(component.MuzzleVelocityBase * num1 * this.VelocityMult, gameObject.transform.forward, (FVRFireArm) this.m_weapon);
          }
          this.ClearBarrel();
          if (flag2)
          {
            this.m_weapon.ForceBreakInteraction();
            this.m_weapon.RootRigidbody.velocity = this.m_weapon.transform.forward * -8f + this.m_weapon.transform.up * 1f;
            this.m_weapon.RootRigidbody.angularVelocity = this.m_weapon.transform.right * -5f;
          }
          if (flag1)
            this.m_weapon.Blowup();
          this.m_isIgnited = false;
        }
      }
    }

    private void ClearBarrel() => this.LoadedElements.Clear();

    private void BarrelContentsSim()
    {
      float num1 = Vector3.Angle(this.Muzzle.forward, Vector3.up);
      if ((double) num1 > 90.0)
      {
        if (this.LoadedElements.Count <= 0)
          return;
        for (int index = this.LoadedElements.Count - 1; index >= 0; --index)
        {
          if (this.LoadedElements[index].Type == FlintlockBarrel.LoadedElementType.Powder)
          {
            float min = 0.0f;
            float max = this.BarrelLength;
            bool flag = true;
            if (index + 1 < this.LoadedElements.Count)
            {
              min = this.LoadedElements[index + 1].Position + this.GetLengthOfElement(this.LoadedElements[index + 1].Type, this.LoadedElements[index + 1].PowderAmount);
              flag = false;
            }
            if (index - 1 >= 0)
              max = this.LoadedElements[index - 1].Position;
            float num2 = this.LoadedElements[index].Position - (float) (((double) num1 - 90.0) / 90.0 * 2.0) * Time.deltaTime;
            if (flag && (double) num2 <= 0.0)
            {
              int num3 = this.ExpellElement(this.LoadedElements[index].Type, this.LoadedElements[index].PowderAmount);
              if (num3 <= 0)
                this.LoadedElements.RemoveAt(index);
              else
                this.LoadedElements[index].PowderAmount = num3;
            }
            else
            {
              float num3 = Mathf.Clamp(num2, min, max);
              this.LoadedElements[index].Position = num3;
            }
          }
        }
      }
      else
      {
        if (this.LoadedElements.Count <= 0)
          return;
        for (int index = this.LoadedElements.Count - 1; index >= 0; --index)
        {
          if (this.LoadedElements[index].Type == FlintlockBarrel.LoadedElementType.Powder)
          {
            float min = 0.0f;
            float max = this.BarrelLength - this.GetLengthOfElement(this.LoadedElements[index].Type, this.LoadedElements[index].PowderAmount);
            if (index + 1 < this.LoadedElements.Count)
              min = this.LoadedElements[index + 1].Position + this.GetLengthOfElement(this.LoadedElements[index + 1].Type, this.LoadedElements[index + 1].PowderAmount);
            if (index - 1 >= 0)
              max = this.LoadedElements[index - 1].Position;
            float num2 = Mathf.Clamp(this.LoadedElements[index].Position + (float) ((1.0 - (double) num1 / 90.0) * 2.0) * Time.deltaTime, min, max);
            this.LoadedElements[index].Position = num2;
          }
        }
      }
    }

    private void BarrelContentsDraw()
    {
      if (this.LoadedElements.Count > 1)
      {
        int index1 = this.LoadedElements.Count - 1;
        int index2 = this.LoadedElements.Count - 2;
        for (int index3 = 0; index3 < this.ProxyRends0.Count; ++index3)
        {
          if (this.LoadedElements[index1].Type == (FlintlockBarrel.LoadedElementType) index3)
          {
            this.ProxyRends0[index3].enabled = true;
            this.ProxyRends0[index3].transform.position = this.Muzzle.position - this.Muzzle.forward * this.LoadedElements[index1].Position;
            if (this.LoadedElements[index1].Type != FlintlockBarrel.LoadedElementType.Powder && (double) this.LoadedElements[index1].Position < 0.00999999977648258)
            {
              if (this.LoadedElements[index1].Type == FlintlockBarrel.LoadedElementType.Shot)
                this.ProxyRends0[index3].transform.position = this.LodgePoint_Shot.position;
              else if (this.LoadedElements[index1].Type == FlintlockBarrel.LoadedElementType.ShotInPaper)
                this.ProxyRends0[index3].transform.position = this.LodgePoint_Paper.position;
            }
            if (this.LoadedElements[index1].Type == FlintlockBarrel.LoadedElementType.Powder)
            {
              int powderAmount = this.LoadedElements[index1].PowderAmount;
              this.ProxyPowder0.mesh = powderAmount <= 15 ? (powderAmount <= 9 ? (powderAmount <= 4 ? this.ProxyPowderPiles[0] : this.ProxyPowderPiles[1]) : this.ProxyPowderPiles[2]) : this.ProxyPowderPiles[3];
            }
          }
          else
            this.ProxyRends0[index3].enabled = false;
        }
        for (int index3 = 0; index3 < this.ProxyRends1.Count; ++index3)
        {
          if (this.LoadedElements[index2].Type == (FlintlockBarrel.LoadedElementType) index3)
          {
            this.ProxyRends1[index3].enabled = true;
            this.ProxyRends1[index3].transform.position = this.Muzzle.position - this.Muzzle.forward * this.LoadedElements[index2].Position;
            if (this.LoadedElements[index2].Type == FlintlockBarrel.LoadedElementType.Powder)
            {
              int powderAmount = this.LoadedElements[index2].PowderAmount;
              this.ProxyPowder1.mesh = powderAmount <= 15 ? (powderAmount <= 9 ? (powderAmount <= 4 ? this.ProxyPowderPiles[0] : this.ProxyPowderPiles[1]) : this.ProxyPowderPiles[2]) : this.ProxyPowderPiles[3];
            }
          }
          else
            this.ProxyRends1[index3].enabled = false;
        }
      }
      else if (this.LoadedElements.Count > 0)
      {
        for (int index = 0; index < this.ProxyRends0.Count; ++index)
        {
          if (this.LoadedElements[0].Type == (FlintlockBarrel.LoadedElementType) index)
          {
            this.ProxyRends0[index].enabled = true;
            this.ProxyRends0[index].transform.position = this.Muzzle.position - this.Muzzle.forward * this.LoadedElements[0].Position;
            if (this.LoadedElements[0].Type != FlintlockBarrel.LoadedElementType.Powder && (double) this.LoadedElements[0].Position < 0.00999999977648258)
            {
              if (this.LoadedElements[0].Type == FlintlockBarrel.LoadedElementType.Shot)
                this.ProxyRends0[index].transform.position = this.LodgePoint_Shot.position;
              else if (this.LoadedElements[0].Type == FlintlockBarrel.LoadedElementType.ShotInPaper)
                this.ProxyRends0[index].transform.position = this.LodgePoint_Paper.position;
            }
            if (this.LoadedElements[0].Type == FlintlockBarrel.LoadedElementType.Powder)
            {
              int powderAmount = this.LoadedElements[0].PowderAmount;
              this.ProxyPowder0.mesh = powderAmount <= 15 ? (powderAmount <= 9 ? (powderAmount <= 4 ? this.ProxyPowderPiles[0] : this.ProxyPowderPiles[1]) : this.ProxyPowderPiles[2]) : this.ProxyPowderPiles[3];
            }
          }
          else
            this.ProxyRends0[index].enabled = false;
        }
        for (int index = 0; index < this.ProxyRends1.Count; ++index)
          this.ProxyRends1[index].enabled = false;
      }
      else
      {
        for (int index = 0; index < this.ProxyRends0.Count; ++index)
          this.ProxyRends0[index].enabled = false;
        for (int index = 0; index < this.ProxyRends1.Count; ++index)
          this.ProxyRends1[index].enabled = false;
      }
    }

    public enum LoadedElementType
    {
      Powder,
      Shot,
      ShotInPaper,
      Wadding,
    }

    [Serializable]
    public class LoadedElement
    {
      public FlintlockBarrel.LoadedElementType Type;
      public float Position;
      public int PowderAmount;
    }
  }
}
