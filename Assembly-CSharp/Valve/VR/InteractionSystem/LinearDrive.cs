// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.LinearDrive
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class LinearDrive : MonoBehaviour
  {
    public Transform startPosition;
    public Transform endPosition;
    public LinearMapping linearMapping;
    public bool repositionGameObject = true;
    public bool maintainMomemntum = true;
    public float momemtumDampenRate = 5f;
    protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;
    protected float initialMappingOffset;
    protected int numMappingChangeSamples = 5;
    protected float[] mappingChangeSamples;
    protected float prevMapping;
    protected float mappingChangeRate;
    protected int sampleCount;
    protected Interactable interactable;

    protected virtual void Awake()
    {
      this.mappingChangeSamples = new float[this.numMappingChangeSamples];
      this.interactable = this.GetComponent<Interactable>();
    }

    protected virtual void Start()
    {
      if ((Object) this.linearMapping == (Object) null)
        this.linearMapping = this.GetComponent<LinearMapping>();
      if ((Object) this.linearMapping == (Object) null)
        this.linearMapping = this.gameObject.AddComponent<LinearMapping>();
      this.initialMappingOffset = this.linearMapping.value;
      if (!this.repositionGameObject)
        return;
      this.UpdateLinearMapping(this.transform);
    }

    protected virtual void HandHoverUpdate(Hand hand)
    {
      GrabTypes grabStarting = hand.GetGrabStarting();
      if (!((Object) this.interactable.attachedToHand == (Object) null) || grabStarting == GrabTypes.None)
        return;
      this.initialMappingOffset = this.linearMapping.value - this.CalculateLinearMapping(hand.transform);
      this.sampleCount = 0;
      this.mappingChangeRate = 0.0f;
      hand.AttachObject(this.gameObject, grabStarting, this.attachmentFlags);
    }

    protected virtual void HandAttachedUpdate(Hand hand)
    {
      this.UpdateLinearMapping(hand.transform);
      if (!hand.IsGrabEnding(this.gameObject))
        return;
      hand.DetachObject(this.gameObject);
    }

    protected virtual void OnDetachedFromHand(Hand hand) => this.CalculateMappingChangeRate();

    protected void CalculateMappingChangeRate()
    {
      this.mappingChangeRate = 0.0f;
      int num = Mathf.Min(this.sampleCount, this.mappingChangeSamples.Length);
      if (num == 0)
        return;
      for (int index = 0; index < num; ++index)
        this.mappingChangeRate += this.mappingChangeSamples[index];
      this.mappingChangeRate /= (float) num;
    }

    protected void UpdateLinearMapping(Transform updateTransform)
    {
      this.prevMapping = this.linearMapping.value;
      this.linearMapping.value = Mathf.Clamp01(this.initialMappingOffset + this.CalculateLinearMapping(updateTransform));
      this.mappingChangeSamples[this.sampleCount % this.mappingChangeSamples.Length] = (float) (1.0 / (double) Time.deltaTime * ((double) this.linearMapping.value - (double) this.prevMapping));
      ++this.sampleCount;
      if (!this.repositionGameObject)
        return;
      this.transform.position = Vector3.Lerp(this.startPosition.position, this.endPosition.position, this.linearMapping.value);
    }

    protected float CalculateLinearMapping(Transform updateTransform)
    {
      Vector3 rhs = this.endPosition.position - this.startPosition.position;
      float magnitude = rhs.magnitude;
      rhs.Normalize();
      return Vector3.Dot(updateTransform.position - this.startPosition.position, rhs) / magnitude;
    }

    protected virtual void Update()
    {
      if (!this.maintainMomemntum || (double) this.mappingChangeRate == 0.0)
        return;
      this.mappingChangeRate = Mathf.Lerp(this.mappingChangeRate, 0.0f, this.momemtumDampenRate * Time.deltaTime);
      this.linearMapping.value = Mathf.Clamp01(this.linearMapping.value + this.mappingChangeRate * Time.deltaTime);
      if (!this.repositionGameObject)
        return;
      this.transform.position = Vector3.Lerp(this.startPosition.position, this.endPosition.position, this.linearMapping.value);
    }
  }
}
