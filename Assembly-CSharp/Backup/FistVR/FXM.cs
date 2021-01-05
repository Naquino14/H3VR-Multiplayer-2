// Decompiled with JetBrains decompiler
// Type: FistVR.FXM
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class FXM : ManagerSingleton<FXM>
  {
    public GameObject FastPoolManagerPrefab;
    private FastPoolManager m_poolManager;
    public GameObject[] VFX_Impact_Generic;
    public GameObject[] VFX_Impact_Sparks;
    public GameObject[] VFX_Impact_BotMeat;
    public GameObject[] VFX_Impact_RotMeat;
    public GameObject[] VFX_Impact_Water;
    public GameObject[] VFX_Impact_SparksRed;
    public GameObject[] VFX_Impact_SparksGreen;
    public GameObject[] VFX_Impact_SparksBlue;
    public GameObject MuzzleFireLightPrefab;
    private AlloyAreaLight MuzzleFireLight;
    private Light MuzzleFireLightOG;
    private float m_muzzleFireTick;
    public List<GameObject> FIREFX;
    public List<GameObject> ClownModeFX;
    private List<SLAM> m_registeredSLAAMS;
    [Header("BulletHoles")]
    public GameObject[] BulletHolePrefabs_Wood;
    public GameObject[] BulletHolePrefabs_Metal;
    public GameObject[] BulletHolePrefabs_Plaster;
    public GameObject[] BulletHolePrefabs_PlasticRubber;
    public GameObject[] BulletHolePrefabs_Tile;
    public GameObject[] BulletHolePrefabs_Glass;
    public GameObject[] BulletHolePrefabs_Brick;
    public GameObject[] BulletHolePrefabs_Rock;
    public GameObject[] BulletHolePrefabs_SandDirt;
    public GameObject[] BulletHolePrefabs;
    public Material[] BulletHoleMaterials;
    private List<Renderer> m_decals = new List<Renderer>();
    private int m_maxDecalCount = 200;
    private int decalIndex;
    private Dictionary<MuzzleEffectEntry, MuzzleEffectConfig> muzzleDic = new Dictionary<MuzzleEffectEntry, MuzzleEffectConfig>();

    public static List<SLAM> RegisteredSLAAMS => ManagerSingleton<FXM>.Instance.m_registeredSLAAMS;

    public static void ResetDecals()
    {
      if (ManagerSingleton<FXM>.Instance.m_decals.Count > 0)
      {
        for (int index = ManagerSingleton<FXM>.Instance.m_decals.Count - 1; index >= 0; --index)
          Object.Destroy((Object) ManagerSingleton<FXM>.Instance.m_decals[index]);
      }
      ManagerSingleton<FXM>.Instance.m_decals.Clear();
      ManagerSingleton<FXM>.Instance.decalIndex = 0;
      ManagerSingleton<FXM>.Instance.m_maxDecalCount = GM.Options.SimulationOptions.MaxHitDecals[GM.Options.SimulationOptions.MaxHitDecalIndex];
    }

    public static void SpawnBulletDecal(
      BulletHoleDecalType t,
      Vector3 point,
      Vector3 normal,
      float damageSize)
    {
      switch (t)
      {
        case BulletHoleDecalType.Wood:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[0], point, normal, damageSize);
          break;
        case BulletHoleDecalType.Metal:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[1], point, normal, damageSize);
          break;
        case BulletHoleDecalType.Plaster:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[2], point, normal, damageSize);
          break;
        case BulletHoleDecalType.PlasticRubber:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[3], point, normal, damageSize);
          break;
        case BulletHoleDecalType.Tile:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[4], point, normal, damageSize);
          break;
        case BulletHoleDecalType.Glass:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[5], point, normal, damageSize);
          break;
        case BulletHoleDecalType.Brick:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[6], point, normal, damageSize);
          break;
        case BulletHoleDecalType.Rock:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[7], point, normal, damageSize);
          break;
        case BulletHoleDecalType.SandDirt:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[8], point, normal, damageSize);
          break;
        case BulletHoleDecalType.GlowBlue:
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[9], point, normal, damageSize);
          ManagerSingleton<FXM>.Instance.SpawnBulletDecal(ManagerSingleton<FXM>.Instance.BulletHoleMaterials[10], point, normal, damageSize * 0.6f);
          break;
      }
    }

    private void SpawnBulletDecal(Material m, Vector3 point, Vector3 normal, float damageSize)
    {
      if (this.m_decals.Count < this.m_maxDecalCount)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(ManagerSingleton<FXM>.Instance.BulletHolePrefabs[Random.Range(0, ManagerSingleton<FXM>.Instance.BulletHolePrefabs.Length)], point, Quaternion.LookRotation(normal));
        float num = Random.Range(15f, 18f);
        gameObject.transform.localScale = new Vector3(damageSize * num, damageSize * num, damageSize * num);
        Renderer component = gameObject.GetComponent<Renderer>();
        component.material = m;
        ManagerSingleton<FXM>.Instance.m_decals.Add(component);
      }
      else
      {
        this.m_decals[this.decalIndex].material = m;
        this.m_decals[this.decalIndex].transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
        float num = Random.Range(15f, 18f);
        this.m_decals[this.decalIndex].transform.localScale = new Vector3(damageSize * num, damageSize * num, damageSize * num);
        ++this.decalIndex;
        if (this.decalIndex < this.m_maxDecalCount)
          return;
        this.decalIndex = 0;
      }
    }

    public static void ClearDecalPools()
    {
      ManagerSingleton<FXM>.Instance.m_decals.Clear();
      ManagerSingleton<FXM>.Instance.decalIndex = 0;
    }

    public static GameObject GetClownFX(int i) => ManagerSingleton<FXM>.Instance.ClownModeFX[i];

    protected override void Awake()
    {
      base.Awake();
      this.CheckForPoolManager();
      this.GenerateMuzzleFlashLight();
      this.GenerateMuzzleEffectDictionaries();
      this.m_registeredSLAAMS = new List<SLAM>();
      this.m_maxDecalCount = GM.Options.SimulationOptions.MaxHitDecals[GM.Options.SimulationOptions.MaxHitDecalIndex];
    }

    public static Dictionary<MuzzleEffectEntry, MuzzleEffectConfig> MuzzleDic => ManagerSingleton<FXM>.Instance.muzzleDic;

    public static MuzzleEffectConfig GetMuzzleConfig(MuzzleEffectEntry entry) => FXM.MuzzleDic[entry];

    private void GenerateMuzzleEffectDictionaries()
    {
      MuzzleEffectConfig[] muzzleEffectConfigArray = Resources.LoadAll<MuzzleEffectConfig>("MuzzleEffects");
      for (int index = 0; index < muzzleEffectConfigArray.Length; ++index)
        this.muzzleDic.Add(muzzleEffectConfigArray[index].Entry, muzzleEffectConfigArray[index]);
    }

    public static void DetonateSPAAMS()
    {
      for (int index = 0; index < FXM.RegisteredSLAAMS.Count; ++index)
      {
        if ((Object) FXM.RegisteredSLAAMS[index] != (Object) null && FXM.RegisteredSLAAMS[index].Mode == SLAM.SLAMMode.ThrownArmed)
          FXM.RegisteredSLAAMS[index].Invoke("Detonate", 0.1f);
      }
    }

    public static void RegisterSLAM(SLAM s) => FXM.RegisteredSLAAMS.Add(s);

    public static void DeRegisterSLAM(SLAM s) => FXM.RegisteredSLAAMS.Remove(s);

    private void GenerateMuzzleFlashLight()
    {
      if (!((Object) this.MuzzleFireLight == (Object) null))
        return;
      GameObject gameObject = Object.Instantiate<GameObject>(this.MuzzleFireLightPrefab, Vector3.zero, Quaternion.identity);
      this.MuzzleFireLight = gameObject.GetComponent<AlloyAreaLight>();
      this.MuzzleFireLightOG = gameObject.GetComponent<Light>();
      this.MuzzleFireLight.gameObject.SetActive(false);
    }

    public static void Ignite(FVRIgnitable i, float ignitionPower)
    {
      if (!i.IsIgniteable() || (double) ignitionPower < (double) i.IgnitionThreshold)
        return;
      GameObject original = ManagerSingleton<FXM>.Instance.FIREFX[(int) i.FireType];
      Transform spawnPos = i.GetSpawnPos();
      GameObject gameObject = Object.Instantiate<GameObject>(original, spawnPos.position, spawnPos.rotation);
      gameObject.transform.SetParent(spawnPos);
      ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
      component.Emit(1);
      i.Ignite(component);
    }

    private void Update()
    {
      if ((double) this.m_muzzleFireTick <= 0.0 && this.MuzzleFireLight.gameObject.activeSelf)
      {
        this.MuzzleFireLight.gameObject.SetActive(false);
        this.m_muzzleFireTick = 0.0f;
      }
      this.MuzzleFireLight.Intensity = this.m_muzzleFireTick * 2f;
      if ((double) this.m_muzzleFireTick <= 0.0)
        return;
      this.m_muzzleFireTick -= Time.deltaTime * 20f;
    }

    public static void InitiateMuzzleFlashLowPriority(
      Vector3 pos,
      Vector3 dir,
      float intensity,
      Color col,
      float rangeMult)
    {
      ManagerSingleton<FXM>.Instance.initiateMuzzleFlashLowPriority(pos, dir, intensity, col, rangeMult);
    }

    private void initiateMuzzleFlashLowPriority(
      Vector3 pos,
      Vector3 dir,
      float intensity,
      Color col,
      float rangeMult)
    {
      if (this.MuzzleFireLight.gameObject.activeInHierarchy && (Object) GM.CurrentPlayerBody != (Object) null)
      {
        float num = Vector3.Distance(this.MuzzleFireLight.transform.position, GM.CurrentPlayerBody.Head.transform.position);
        if ((double) Vector3.Distance(pos, GM.CurrentPlayerBody.Head.transform.position) >= (double) num)
          return;
        ManagerSingleton<FXM>.Instance.initiateMuzzleFlash(pos, dir, intensity, col, rangeMult);
      }
      else
        ManagerSingleton<FXM>.Instance.initiateMuzzleFlash(pos, dir, intensity, col, rangeMult);
    }

    public static void InitiateMuzzleFlash(
      Vector3 pos,
      Vector3 dir,
      float intensity,
      Color col,
      float rangeMult)
    {
      ManagerSingleton<FXM>.Instance.initiateMuzzleFlash(pos, dir, intensity, col, rangeMult);
    }

    private void initiateMuzzleFlash(
      Vector3 pos,
      Vector3 dir,
      float intensity,
      Color col,
      float rangeMult)
    {
      this.MuzzleFireLight.gameObject.SetActive(true);
      this.MuzzleFireLight.transform.position = pos;
      this.MuzzleFireLight.transform.up = dir;
      this.m_muzzleFireTick = intensity;
      this.MuzzleFireLight.Intensity = this.m_muzzleFireTick * 2f;
      this.MuzzleFireLight.Color = col;
      this.MuzzleFireLightOG.range = rangeMult * 5f;
    }

    private void CheckForPoolManager()
    {
      if (!((Object) this.m_poolManager == (Object) null))
        return;
      if ((Object) Object.FindObjectOfType<FastPoolManager>() != (Object) null)
        this.m_poolManager = Object.FindObjectOfType<FastPoolManager>();
      else
        this.m_poolManager = Object.Instantiate<GameObject>(this.FastPoolManagerPrefab).GetComponent<FastPoolManager>();
    }

    private void OnLevelWasLoaded(int i)
    {
      this.CheckForPoolManager();
      this.GenerateMuzzleFlashLight();
    }

    public static void SpawnImpactEffect(
      Vector3 pos,
      Vector3 lookDir,
      int mat,
      ImpactEffectMagnitude mag,
      bool forwardBack)
    {
      ManagerSingleton<FXM>.Instance.spawnImpactEffect(pos, lookDir, mat, mag, forwardBack);
    }

    private void spawnImpactEffect(
      Vector3 pos,
      Vector3 lookDir,
      int mat,
      ImpactEffectMagnitude mag,
      bool forwardBack)
    {
      switch (mat)
      {
        case 0:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_Generic[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_Generic[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_Generic[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_Generic[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 1:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_Sparks[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_Sparks[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), Color.white, Random.Range(0.5f, 1f));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_Sparks[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_Sparks[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), Color.white, Random.Range(0.5f, 1.5f));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_Sparks[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_Sparks[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), Color.white, Random.Range(1.7f, 2.2f));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_Sparks[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 2:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_BotMeat[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_BotMeat[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_BotMeat[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_BotMeat[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 3:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_Water[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_Water[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_Water[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_Water[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 4:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_SparksRed[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksRed[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), new Color(1f, 0.4f, 0.4f, 1f), Random.Range(0.5f, 1f));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_SparksRed[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksRed[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), new Color(1f, 0.4f, 0.4f, 1f), Random.Range(0.5f, 1.5f));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_SparksRed[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksRed[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), new Color(1f, 0.4f, 0.4f, 1f), Random.Range(1.7f, 2.2f));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_SparksRed[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 5:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_SparksGreen[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksGreen[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), new Color(0.4f, 1f, 0.4f, 1f), Random.Range(0.5f, 1f));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_SparksGreen[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksGreen[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), new Color(0.4f, 1f, 0.4f, 1f), Random.Range(0.5f, 1.5f));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_SparksGreen[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksGreen[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), new Color(0.4f, 1f, 0.4f, 1f), Random.Range(1.7f, 2.2f));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_SparksGreen[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 6:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_SparksBlue[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksBlue[1].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 1.5f), new Color(0.4f, 0.4f, 1f, 1f), Random.Range(0.5f, 1f));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_SparksBlue[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksBlue[2].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(0.5f, 2.5f), new Color(0.4f, 0.4f, 1f, 1f), Random.Range(0.5f, 1.5f));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_SparksBlue[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              if (forwardBack)
                this.VFX_Impact_SparksBlue[3].FastInstantiate(pos, Quaternion.LookRotation(-lookDir, Vector3.up));
              this.initiateMuzzleFlashLowPriority(pos, lookDir, Random.Range(1.5f, 3.5f), new Color(0.4f, 0.4f, 1f, 1f), Random.Range(1.7f, 2.2f));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_SparksBlue[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
        case 7:
          switch (mag)
          {
            case ImpactEffectMagnitude.Small:
              this.VFX_Impact_RotMeat[1].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Medium:
              this.VFX_Impact_RotMeat[2].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Large:
              this.VFX_Impact_RotMeat[3].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            case ImpactEffectMagnitude.Tiny:
              this.VFX_Impact_RotMeat[0].FastInstantiate(pos, Quaternion.LookRotation(lookDir, Vector3.up));
              return;
            default:
              return;
          }
      }
    }

    public enum FireFXType
    {
      Sosig,
      MolotovWick,
      RW_Nodule,
    }
  }
}
