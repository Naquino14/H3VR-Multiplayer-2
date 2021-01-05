// Decompiled with JetBrains decompiler
// Type: FistVR.ReactiveSteelTarget
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ReactiveSteelTarget : MonoBehaviour, IFVRDamageable
  {
    private Rigidbody rb;
    private bool m_hasRB;
    public AudioEvent HitEvent;
    public float HitSoundRefire = 0.1f;
    private float m_refireTick;
    public GameObject[] BulletHolePrefabs;
    private List<GameObject> m_currentHoles = new List<GameObject>();
    public FVRPooledAudioType PoolType = FVRPooledAudioType.GenericLongRange;
    private int holeindex;

    public void Awake()
    {
      this.rb = this.GetComponent<Rigidbody>();
      if (!((Object) this.rb != (Object) null))
        return;
      this.m_hasRB = true;
    }

    public void Update()
    {
      if ((double) this.m_refireTick <= 0.0)
        return;
      this.m_refireTick -= Time.deltaTime;
    }

    private void PlayHitSound(float soundMultiplier)
    {
      if ((double) this.m_refireTick > 0.0)
        return;
      this.m_refireTick = this.HitSoundRefire;
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f;
      SM.PlayCoreSoundDelayedOverrides(this.PoolType, this.HitEvent, this.transform.position, this.HitEvent.VolumeRange * soundMultiplier, this.HitEvent.PitchRange, num + 0.04f);
    }

    public void Damage(FistVR.Damage dam)
    {
      if (dam.Class != FistVR.Damage.DamageClass.Projectile)
        return;
      Vector3 position = dam.point + dam.hitNormal * Random.Range(1f / 500f, 0.008f);
      if (this.BulletHolePrefabs.Length > 0)
      {
        if (this.m_currentHoles.Count > 20)
        {
          ++this.holeindex;
          if (this.holeindex > 19)
            this.holeindex = 0;
          this.m_currentHoles[this.holeindex].transform.position = position;
          this.m_currentHoles[this.holeindex].transform.rotation = Quaternion.LookRotation(dam.hitNormal, Random.onUnitSphere);
        }
        else
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.BulletHolePrefabs[Random.Range(0, this.BulletHolePrefabs.Length)], position, Quaternion.LookRotation(dam.hitNormal, Random.onUnitSphere));
          gameObject.transform.SetParent(this.transform);
          this.m_currentHoles.Add(gameObject);
        }
      }
      if (this.m_hasRB)
        this.rb.AddForceAtPosition(dam.strikeDir * dam.Dam_Blunt * 0.01f, dam.point, ForceMode.Impulse);
      this.PlayHitSound(1f);
    }

    public void ClearHoles()
    {
      for (int index = this.m_currentHoles.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.m_currentHoles[index]);
      this.m_currentHoles.Clear();
    }
  }
}
