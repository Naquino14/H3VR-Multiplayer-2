// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockPseudoRamRod
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockPseudoRamRod : FVRInteractiveObject
  {
    public Transform Root;
    public FlintlockPseudoRamRod.RamRodState RState;
    public Transform Point_Lower_Rear;
    public Transform Point_Lower_Forward;
    private float lastHandZ;
    private float m_ramZ;
    private float m_minZ_lower;
    private float m_maxZ_lower;
    public GameObject RamRodPrefab;
    private float m_minZ_barrel;
    private float m_maxZ_barrel;
    private FlintlockBarrel m_curBarrel;
    [Header("Audio")]
    public AudioEvent AudEvent_Grab;
    public AudioEvent AudEvent_ExtractHolder;
    public AudioEvent AudEvent_ExtractBarrel;
    public AudioEvent AudEvent_InsertHolder;
    public AudioEvent AudEvent_InsertBarrel;
    private float m_curHandRodOffsetZ;

    public FlintlockBarrel GetCurBarrel() => this.m_curBarrel;

    protected override void Awake()
    {
      base.Awake();
      this.m_ramZ = this.transform.localPosition.z;
      this.m_minZ_lower = this.Point_Lower_Rear.localPosition.z;
      this.m_maxZ_lower = this.Point_Lower_Forward.localPosition.z;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      Vector3 zero = Vector3.zero;
      Vector3 vector3 = this.RState != FlintlockPseudoRamRod.RamRodState.Lower ? this.m_curBarrel.Muzzle.InverseTransformPoint(hand.Input.Pos) : this.Root.InverseTransformPoint(hand.Input.Pos);
      SM.PlayGenericSound(this.AudEvent_Grab, this.transform.position);
      this.lastHandZ = vector3.z;
      this.m_curHandRodOffsetZ = this.transform.InverseTransformPoint(hand.Input.Pos).z;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 zero = Vector3.zero;
      float z = (this.RState != FlintlockPseudoRamRod.RamRodState.Lower ? this.m_curBarrel.Muzzle.InverseTransformPoint(hand.Input.Pos) : this.Root.InverseTransformPoint(hand.Input.Pos)).z;
      float num = z - this.lastHandZ;
      this.MoveRamRod(this.transform.InverseTransformPoint(hand.Input.Pos).z - this.m_curHandRodOffsetZ, hand);
      this.lastHandZ = z;
    }

    public void MountToUnder(FVRViveHand h)
    {
      this.transform.SetParent(this.Root);
      this.RState = FlintlockPseudoRamRod.RamRodState.Lower;
      this.m_ramZ = this.m_maxZ_lower - 1f / 500f;
      SM.PlayGenericSound(this.AudEvent_InsertHolder, this.transform.position);
      this.transform.localPosition = new Vector3(this.Point_Lower_Rear.localPosition.x, this.Point_Lower_Rear.localPosition.y, this.m_ramZ);
      this.m_curHandRodOffsetZ = this.transform.InverseTransformPoint(h.Input.Pos).z;
    }

    public void MountToBarrel(FlintlockBarrel b, FVRViveHand h)
    {
      if ((Object) b == (Object) null)
      {
        this.m_curBarrel = (FlintlockBarrel) null;
        this.gameObject.SetActive(false);
      }
      else
      {
        this.transform.SetParent(b.Muzzle);
        this.m_maxZ_barrel = 0.01f;
        this.m_minZ_barrel = -b.BarrelLength;
        this.m_curBarrel = b;
        this.m_ramZ = -0.02f;
        SM.PlayGenericSound(this.AudEvent_InsertBarrel, this.transform.position);
        this.transform.localPosition = new Vector3(this.Point_Lower_Rear.localPosition.x, this.Point_Lower_Rear.localPosition.y, this.m_ramZ);
        if (!((Object) h != (Object) null))
          return;
        this.m_curHandRodOffsetZ = this.transform.InverseTransformPoint(h.Input.Pos).z;
      }
    }

    private void MoveRamRod(float delta, FVRViveHand hand)
    {
      if (this.RState == FlintlockPseudoRamRod.RamRodState.Lower)
      {
        float ramZ = this.m_ramZ;
        this.m_ramZ += delta;
        this.m_ramZ = Mathf.Clamp(this.m_ramZ, this.m_minZ_lower, this.m_ramZ);
        float num = this.m_ramZ - ramZ;
        this.transform.localPosition = new Vector3(this.Point_Lower_Rear.localPosition.x, this.Point_Lower_Rear.localPosition.y, this.m_ramZ);
        if ((double) this.m_ramZ < (double) this.m_maxZ_lower)
          return;
        GameObject gameObject = Object.Instantiate<GameObject>(this.RamRodPrefab, this.transform.position, this.transform.rotation);
        this.ForceBreakInteraction();
        FlintlockRamRod component = gameObject.GetComponent<FlintlockRamRod>();
        hand.ForceSetInteractable((FVRInteractiveObject) component);
        component.BeginInteraction(hand);
        SM.PlayGenericSound(this.AudEvent_ExtractHolder, this.transform.position);
        this.Hide();
      }
      else
      {
        this.m_ramZ += delta;
        float z = Mathf.Clamp(this.m_ramZ, -this.m_curBarrel.GetMaxDepth(), this.m_ramZ);
        this.transform.localPosition = new Vector3(0.0f, 0.0f, z);
        if ((double) this.m_ramZ < (double) z)
          this.m_curBarrel.Tamp((float) (((double) this.m_ramZ - (double) z) * 1.0), this.m_ramZ);
        if ((double) this.m_ramZ >= 0.0)
        {
          GameObject gameObject = Object.Instantiate<GameObject>(this.RamRodPrefab, this.transform.position, this.transform.rotation);
          this.ForceBreakInteraction();
          FlintlockRamRod component = gameObject.GetComponent<FlintlockRamRod>();
          hand.ForceSetInteractable((FVRInteractiveObject) component);
          component.BeginInteraction(hand);
          SM.PlayGenericSound(this.AudEvent_ExtractBarrel, this.transform.position);
          this.m_curBarrel = (FlintlockBarrel) null;
          this.Hide();
        }
        this.m_ramZ = z;
      }
    }

    private void Hide() => this.gameObject.SetActive(false);

    public enum RamRodState
    {
      Lower,
      Barrel,
    }
  }
}
