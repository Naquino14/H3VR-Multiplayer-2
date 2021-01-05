// Decompiled with JetBrains decompiler
// Type: FistVR.AnimalNoiseMakerJavier
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AnimalNoiseMakerJavier : FVRPhysicalObject
  {
    private bool m_isPrimed;
    private bool m_hasActivated;
    private bool m_hasSploded;
    public Transform[] CandleSpot;
    public GameObject[] CandlePrefabs;
    private float timeSinceSpawn = 10.5f;
    private float timeSinceFlash = 15f;
    private float timeTilSplode = 20.9f;
    public GameObject Flame;
    public GameObject[] SPlodes;
    private float curPos = 0.35f;
    private Vector3 m_curPos = Vector3.zero;
    private float m_startScale = 0.35f;
    private Vector3 m_startPos = Vector3.zero;
    private float m_endScale = 7f;
    private Vector3 m_endPos = Vector3.zero;
    private float m_transitionTick;
    private Vector3 Wiggledir = Vector3.zero;

    public override bool IsInteractable() => !this.m_hasActivated && base.IsInteractable();

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) Vector3.Dot(this.transform.up, Vector3.up) < -0.100000001490116 && !this.m_isPrimed)
        this.m_isPrimed = true;
      if ((double) Vector3.Dot(this.transform.up, Vector3.up) > 0.300000011920929 && this.m_isPrimed && (!this.m_hasActivated && this.IsHeld))
      {
        this.m_hasActivated = true;
        this.BeginSequence();
      }
      if (!this.m_hasActivated)
        return;
      this.m_transitionTick += 0.1f * Time.deltaTime;
      this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_transitionTick);
      float num = Mathf.Lerp(this.m_startScale, this.m_endScale, this.m_transitionTick);
      this.transform.localScale = new Vector3(num, num, num);
      Vector3 a = Vector3.Cross(this.Wiggledir, Vector3.up);
      this.transform.rotation = Quaternion.LookRotation(-this.Wiggledir, Vector3.up + Vector3.Lerp(a, -a, 1f - Mathf.Abs(Mathf.Sin(this.m_transitionTick * 20f))));
      this.timeSinceSpawn -= Time.deltaTime;
      if ((double) this.timeSinceSpawn <= 0.0)
      {
        this.timeSinceSpawn = Random.Range(0.75f, 1f);
        Object.Instantiate<GameObject>(this.CandlePrefabs[Random.Range(0, this.CandlePrefabs.Length)], this.transform.position + Vector3.up * Random.Range(5f, 10f) + Random.onUnitSphere * 4f, Quaternion.identity);
      }
      this.timeSinceFlash -= Time.deltaTime;
      if ((double) this.timeSinceFlash <= 0.0)
      {
        this.timeSinceFlash = Random.Range(0.025f, 0.075f);
        FXM.InitiateMuzzleFlash(this.transform.position + Random.onUnitSphere, Vector3.up, Random.Range(0.8f, 5f), Color.white, Random.Range(2f, 6f));
      }
      if ((double) this.m_transitionTick > 0.200000002980232 && !this.Flame.activeSelf)
        this.Flame.SetActive(true);
      this.timeTilSplode -= Time.deltaTime;
      if ((double) this.timeTilSplode > 0.0 || this.m_hasSploded)
        return;
      this.m_hasSploded = true;
      for (int index = 0; index < this.SPlodes.Length; ++index)
        Object.Instantiate<GameObject>(this.SPlodes[index], this.transform.position, Quaternion.identity);
      Object.Destroy((Object) this.gameObject);
    }

    private void BeginSequence()
    {
      FVRViveHand hand = this.m_hand;
      this.EndInteraction(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) null);
      this.m_startPos = this.transform.position;
      this.RootRigidbody.isKinematic = true;
      Vector3 vector3 = this.transform.position - Camera.main.transform.position;
      vector3.y = 0.0f;
      this.Wiggledir = vector3;
      vector3 = vector3.normalized * 2f;
      this.m_endPos = this.m_startPos + vector3;
      this.GetComponent<AudioSource>().Play();
    }
  }
}
