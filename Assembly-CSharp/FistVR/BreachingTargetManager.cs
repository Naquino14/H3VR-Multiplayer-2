// Decompiled with JetBrains decompiler
// Type: FistVR.BreachingTargetManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class BreachingTargetManager : MonoBehaviour
  {
    public GameObject HotDogTargetRef;
    public GameObject IPSCTargetRef;
    public GameObject DecorationRef;
    private GameObject SpawnedDogs;
    private GameObject SpawnedIPSC;
    private GameObject SpawnedDecoration;
    private int IPSCScore;
    private float m_time;
    public Text IPSCScoreText;
    private bool m_isCounting;
    private int numTargetsHit;
    private AudioSource Aud;
    public List<Transform> PosList;
    public List<Transform> PosList_IPSC;
    public List<Transform> PosList_Sosig;
    public ZosigEnemyTemplate Template;
    private List<Sosig> m_spawnedSosigs = new List<Sosig>();

    private void Start()
    {
      this.Aud = this.GetComponent<AudioSource>();
      this.HotDogTargetRef.SetActive(false);
      this.IPSCTargetRef.SetActive(false);
      this.DecorationRef.SetActive(false);
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
    }

    private void OnDestroy() => GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);

    private void PlayerDied()
    {
      this.IPSCScore = 0;
      this.numTargetsHit = 0;
      this.m_time = 0.0f;
      this.m_isCounting = false;
      this.UpdateScore();
      this.ResetDecoration();
      this.ClearHotDogs();
      this.ClearIPSC();
      this.ClearSosigs();
    }

    public void ResetDecoration()
    {
      if ((Object) this.SpawnedDecoration != (Object) null)
        Object.Destroy((Object) this.SpawnedDecoration);
      this.SpawnedDecoration = Object.Instantiate<GameObject>(this.DecorationRef, Vector3.zero, Quaternion.identity);
      this.SpawnedDecoration.SetActive(true);
    }

    public void InitiateHotDogSequence(bool isRandomPos)
    {
      this.IPSCScore = 0;
      this.numTargetsHit = 0;
      this.m_time = 0.0f;
      this.UpdateScore();
      this.ResetDecoration();
      this.m_isCounting = false;
      this.ClearHotDogs();
      this.ClearIPSC();
      this.ClearSosigs();
      this.SpawnHotDogs(isRandomPos);
    }

    public void InitiateIPSCSequence(bool isRandomPos)
    {
      this.IPSCScore = 0;
      this.numTargetsHit = 0;
      this.m_time = 0.0f;
      this.UpdateScore();
      this.ResetDecoration();
      this.ClearHotDogs();
      this.ClearIPSC();
      this.ClearSosigs();
      this.SpawnIPSC(isRandomPos);
      this.m_isCounting = true;
    }

    public void InitiateSosigSequence(int num)
    {
      this.IPSCScore = 0;
      this.numTargetsHit = 0;
      this.m_time = 0.0f;
      this.UpdateScore();
      this.ResetDecoration();
      this.ClearHotDogs();
      this.ClearIPSC();
      this.ClearSosigs();
      this.SpawnSosigs(num);
    }

    private void Update()
    {
      if (!this.m_isCounting)
        return;
      this.m_time += Time.deltaTime;
      this.UpdateScore();
    }

    private void SpawnHotDogs(bool isRandomPos)
    {
      this.SpawnedDogs = Object.Instantiate<GameObject>(this.HotDogTargetRef, Vector3.zero, Quaternion.identity);
      this.SpawnedDogs.SetActive(true);
      if (!isRandomPos)
        return;
      BreachingTransformGroup component = this.SpawnedDogs.GetComponent<BreachingTransformGroup>();
      this.PosList.Shuffle<Transform>();
      this.PosList.Shuffle<Transform>();
      this.PosList.Shuffle<Transform>();
      for (int index = 0; index < component.Set.Count; ++index)
      {
        component.Set[index].position = this.PosList[index].position;
        component.Set[index].rotation = this.PosList[index].rotation;
      }
    }

    private void SpawnIPSC(bool isRandomPos)
    {
      this.SpawnedIPSC = Object.Instantiate<GameObject>(this.IPSCTargetRef, Vector3.zero, Quaternion.identity);
      this.SpawnedIPSC.SetActive(true);
      if (!isRandomPos)
        return;
      BreachingTransformGroup component = this.SpawnedIPSC.GetComponent<BreachingTransformGroup>();
      this.PosList_IPSC.Shuffle<Transform>();
      this.PosList_IPSC.Shuffle<Transform>();
      this.PosList_IPSC.Shuffle<Transform>();
      for (int index = 0; index < component.Set.Count; ++index)
      {
        component.Set[index].position = this.PosList_IPSC[index].position;
        component.Set[index].rotation = this.PosList_IPSC[index].rotation;
      }
    }

    private void ClearHotDogs()
    {
      if (!((Object) this.SpawnedDogs != (Object) null))
        return;
      Object.Destroy((Object) this.SpawnedDogs);
    }

    private void ClearIPSC()
    {
      if (!((Object) this.SpawnedIPSC != (Object) null))
        return;
      Object.Destroy((Object) this.SpawnedIPSC);
    }

    public void RegisterScore(int i)
    {
      this.IPSCScore += i;
      ++this.numTargetsHit;
      if (this.numTargetsHit >= 20)
      {
        this.m_isCounting = false;
        this.Aud.Play();
      }
      this.UpdateScore();
    }

    private void UpdateScore()
    {
      this.IPSCScoreText.text = "IPSC Target Score: " + this.IPSCScore.ToString() + "/100";
      Text ipscScoreText1 = this.IPSCScoreText;
      ipscScoreText1.text = ipscScoreText1.text + "\nTime: " + this.FloatToTime(this.m_time, "#0:00.00");
      float toConvert = Mathf.Round(this.m_time * 100f) * 0.01f + (float) (100 - this.IPSCScore);
      Text ipscScoreText2 = this.IPSCScoreText;
      ipscScoreText2.text = ipscScoreText2.text + "\nFinal Score: " + this.FloatToTime(toConvert, "#0:00.00");
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (BreachingTargetManager.\u003C\u003Ef__switch\u0024map2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BreachingTargetManager.\u003C\u003Ef__switch\u0024map2 = new Dictionary<string, int>(13)
          {
            {
              "00.0",
              0
            },
            {
              "#0.0",
              1
            },
            {
              "00.00",
              2
            },
            {
              "00.000",
              3
            },
            {
              "#00.000",
              4
            },
            {
              "#0:00",
              5
            },
            {
              "#00:00",
              6
            },
            {
              "0:00.0",
              7
            },
            {
              "#0:00.0",
              8
            },
            {
              "0:00.00",
              9
            },
            {
              "#0:00.00",
              10
            },
            {
              "0:00.000",
              11
            },
            {
              "#0:00.000",
              12
            }
          };
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        if (BreachingTargetManager.\u003C\u003Ef__switch\u0024map2.TryGetValue(format, out num))
        {
          switch (num)
          {
            case 0:
              return string.Format("{0:00}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 1:
              return string.Format("{0:#0}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 2:
              return string.Format("{0:00}:{1:00}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 3:
              return string.Format("{0:00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 4:
              return string.Format("{0:#00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 5:
              return string.Format("{0:#0}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 6:
              return string.Format("{0:#00}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 7:
              return string.Format("{0:0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 8:
              return string.Format("{0:#0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 9:
              return string.Format("{0:0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 10:
              return string.Format("{0:#0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 11:
              return string.Format("{0:0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 12:
              return string.Format("{0:#0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
          }
        }
      }
      return "error";
    }

    private void SpawnSosigs(int num)
    {
      this.PosList_Sosig.Shuffle<Transform>();
      this.PosList_Sosig.Shuffle<Transform>();
      this.PosList_Sosig.Shuffle<Transform>();
      for (int index = 0; index < num; ++index)
        this.SpawnEnemy(this.Template, this.PosList_Sosig[index], 1);
    }

    private void ClearSosigs()
    {
      for (int index = 0; index < this.m_spawnedSosigs.Count; ++index)
      {
        if ((Object) this.m_spawnedSosigs[index] != (Object) null)
          this.m_spawnedSosigs[index].TickDownToClear(0.1f);
      }
      this.m_spawnedSosigs.Clear();
    }

    private void SpawnEnemy(ZosigEnemyTemplate t, Transform point, int IFF)
    {
      Sosig sosig = this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)], t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject(), point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.WearableTemplates[Random.Range(0, t.WearableTemplates.Count)], IFF);
      sosig.CommandGuardPoint(point.position, false);
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      onUnitSphere.Normalize();
      sosig.SetDominantGuardDirection(onUnitSphere);
      this.m_spawnedSosigs.Add(sosig);
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigWearableConfig w,
      int IFF)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.Inventory.FillAllAmmo();
      componentInChildren.E.IFFCode = IFF;
      SosigWeapon component = Object.Instantiate<GameObject>(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
      component.SetAutoDestroy(true);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Headwear)
        this.SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Facewear)
        this.SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Torsowear)
        this.SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear)
        this.SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Backpacks)
        this.SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
      if ((Object) component != (Object) null)
      {
        componentInChildren.InitHands();
        componentInChildren.ForceEquip(component);
      }
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<GameObject> gs, SosigLink l)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)], l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (this.m_spawnedSosigs.Count < 1 || !this.m_spawnedSosigs.Contains(s))
        return;
      s.TickDownToClear(15f);
      this.m_spawnedSosigs.Remove(s);
    }
  }
}
