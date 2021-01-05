// Decompiled with JetBrains decompiler
// Type: FistVR.PaperTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class PaperTarget : MonoBehaviour, IFVRDamageable
  {
    public Texture2D MaskTexture;
    public GameObject[] BulletHolePrefabs;
    public Transform XYGridOrigin;
    public List<GameObject> CurrentShots = new List<GameObject>();
    public List<GameObject> CurrentDisplayShots = new List<GameObject>();
    private Vector3 tarPos = Vector3.zero;
    private bool isMoving;
    private float StartPos;
    private float EndPost;
    private float LerpTick;
    private float MoveSpeed = 0.25f;
    private float StoredDistance = 30f;
    public Transform DisplayTarget;
    public Transform DisplayUpperLeft;
    public Transform DisplayUpperRight;
    public Transform DisplayLowerLeft;
    public GameObject DisplayHitObject;
    public GameObject DisplayLastHitObject;
    private GameObject m_lastHitDot;
    public TargetRangePanel MyPanel;
    public List<float> times = new List<float>();

    public void Awake()
    {
      this.tarPos = this.transform.position;
      this.StoredDistance = this.MyPanel.MaxMeters;
    }

    public Vector3 GetCurrentScoring()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.CurrentShots.Count; ++index)
      {
        if ((Object) this.CurrentShots[index] != (Object) null)
        {
          ++zero.x;
          zero.y += (float) this.CurrentShots[index].GetComponent<PaperTargetBulletHole>().Score;
        }
      }
      zero.z = this.StoredDistance;
      return zero;
    }

    public void ClearHoles()
    {
      for (int index = this.CurrentShots.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.CurrentShots[index]);
      this.CurrentShots.Clear();
      for (int index = this.CurrentDisplayShots.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.CurrentDisplayShots[index]);
      this.CurrentDisplayShots.Clear();
      Object.Destroy((Object) this.m_lastHitDot);
      this.m_lastHitDot = (GameObject) null;
      this.StoredDistance = this.MyPanel.MaxMeters;
    }

    public Vector3 ClearHolesAndReportScore()
    {
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.CurrentShots.Count; ++index)
      {
        if ((Object) this.CurrentShots[index] != (Object) null)
        {
          ++zero.x;
          zero.y += (float) this.CurrentShots[index].GetComponent<PaperTargetBulletHole>().Score;
        }
      }
      zero.z = this.StoredDistance;
      for (int index = this.CurrentShots.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.CurrentShots[index]);
      this.CurrentShots.Clear();
      for (int index = this.CurrentDisplayShots.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.CurrentDisplayShots[index]);
      this.CurrentDisplayShots.Clear();
      Object.Destroy((Object) this.m_lastHitDot);
      this.m_lastHitDot = (GameObject) null;
      this.StoredDistance = 30f;
      return zero;
    }

    public void Update()
    {
      if (!this.isMoving)
        return;
      if ((double) this.LerpTick < 1.0)
      {
        this.LerpTick += Time.deltaTime * this.MoveSpeed;
      }
      else
      {
        this.LerpTick = 1f;
        this.isMoving = false;
      }
      this.transform.position = new Vector3(0.0f, this.tarPos.y, Mathf.Lerp(this.StartPos, this.EndPost, this.LerpTick));
    }

    public void GoToDest(int i)
    {
      float desiredDistance = this.MyPanel.GetDesiredDistance();
      this.isMoving = true;
      this.StartPos = this.transform.position.z;
      this.EndPost = desiredDistance;
      this.LerpTick = 0.0f;
    }

    public void ResetDist()
    {
      float num = 0.0f;
      this.isMoving = true;
      this.StartPos = this.transform.position.z;
      this.EndPost = num;
      this.LerpTick = 0.0f;
    }

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      this.times.Add(Time.time);
      Vector3 vector3 = this.XYGridOrigin.InverseTransformPoint(dam.point);
      vector3.z = 0.0f;
      vector3.x = Mathf.Clamp(vector3.x, 0.0f, 1f);
      vector3.y = Mathf.Clamp(vector3.y, 0.0f, 1f);
      int score = Mathf.RoundToInt(this.MaskTexture.GetPixel(Mathf.RoundToInt((float) this.MaskTexture.width * vector3.x), Mathf.RoundToInt((float) this.MaskTexture.width * vector3.y)).a * 10f);
      this.SpawnBulletHole(dam.point, score);
      this.SpawnDisplayBulletHole(new Vector2(vector3.x, vector3.y));
      this.StoredDistance = Mathf.Min(this.StoredDistance, this.transform.position.z);
      this.MyPanel.UpdatePaperSheet();
    }

    public void SpawnBulletHole(Vector3 point, int score)
    {
      Vector3 position = point + -this.transform.forward * Random.Range(1f / 1000f, 0.008f);
      GameObject gameObject = Object.Instantiate<GameObject>(this.BulletHolePrefabs[Random.Range(0, this.BulletHolePrefabs.Length)], position, Quaternion.LookRotation(-this.transform.forward, Random.onUnitSphere));
      gameObject.transform.SetParent(this.transform);
      gameObject.GetComponent<PaperTargetBulletHole>().Score = score;
      this.CurrentShots.Add(gameObject);
    }

    public void SpawnDisplayBulletHole(Vector2 coord)
    {
      Vector3 vector3 = Vector3.Lerp(this.DisplayUpperLeft.position, this.DisplayUpperRight.position, coord.x);
      vector3.y = Mathf.Lerp(this.DisplayUpperLeft.position.y, this.DisplayLowerLeft.position.y, coord.y);
      Vector3 position = vector3 + -this.DisplayTarget.forward * Random.Range(0.0001f, 0.0003f);
      if ((Object) this.m_lastHitDot == (Object) null)
      {
        this.m_lastHitDot = Object.Instantiate<GameObject>(this.DisplayLastHitObject, position, Quaternion.LookRotation(-this.DisplayTarget.forward, Random.onUnitSphere));
        this.m_lastHitDot.transform.SetParent(this.DisplayTarget);
      }
      else
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.DisplayHitObject, this.m_lastHitDot.transform.position, this.m_lastHitDot.transform.rotation);
        gameObject.transform.SetParent(this.DisplayTarget);
        this.CurrentDisplayShots.Add(gameObject);
        this.m_lastHitDot.transform.position = position;
      }
    }
  }
}
