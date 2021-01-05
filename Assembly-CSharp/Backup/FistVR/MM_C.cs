// Decompiled with JetBrains decompiler
// Type: FistVR.MM_C
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MM_C : MonoBehaviour
  {
    private float checktick = 10f;
    public GameObject prefab;
    private bool c;
    public AudioSource A;
    private float tickdown = 25f;
    private float mTick;

    private void Update()
    {
      if ((double) this.checktick > 0.0)
        this.checktick -= Time.deltaTime;
      if ((double) this.checktick <= 0.0)
      {
        this.checktick = Random.Range(5f, 10f);
        this.Check();
      }
      if (!this.c)
        return;
      if ((double) this.tickdown > 0.0)
        this.tickdown -= Time.deltaTime;
      else if ((double) this.mTick > 0.0)
      {
        this.mTick -= Time.deltaTime;
      }
      else
      {
        this.mTick = Random.Range(1f, 3f);
        this.STime();
      }
    }

    private void STime() => Object.Instantiate<GameObject>(this.prefab, GM.CurrentPlayerBody.transform.position + Vector3.up * 80f + Random.onUnitSphere * 2f, Random.rotation).GetComponent<BallisticProjectile>().Fire(60f, -Vector3.up, (FVRFireArm) null);

    private void Check()
    {
      if (this.c)
        return;
      for (int index = 15; index < GM.MMFlags.MMMTCs.Length; ++index)
      {
        int num = 1000;
        if (GM.MMFlags.MMMTCs[index] > num)
        {
          this.c = true;
          if (!this.A.isPlaying)
            this.A.Play();
        }
      }
    }
  }
}
