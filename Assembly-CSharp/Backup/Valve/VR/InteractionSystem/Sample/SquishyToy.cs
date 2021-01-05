// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.SquishyToy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class SquishyToy : MonoBehaviour
  {
    public Interactable interactable;
    public SkinnedMeshRenderer renderer;
    public bool affectMaterial = true;
    public SteamVR_Action_Single gripSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
    public SteamVR_Action_Single pinchSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
    private Rigidbody rigidbody;

    private void Start()
    {
      if ((Object) this.rigidbody == (Object) null)
        this.rigidbody = this.GetComponent<Rigidbody>();
      if ((Object) this.interactable == (Object) null)
        this.interactable = this.GetComponent<Interactable>();
      if (!((Object) this.renderer == (Object) null))
        return;
      this.renderer = this.GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      if ((bool) (Object) this.interactable.attachedToHand)
      {
        num1 = this.gripSqueeze.GetAxis(this.interactable.attachedToHand.handType);
        num2 = this.pinchSqueeze.GetAxis(this.interactable.attachedToHand.handType);
      }
      this.renderer.SetBlendShapeWeight(0, Mathf.Lerp(this.renderer.GetBlendShapeWeight(0), num1 * 100f, Time.deltaTime * 10f));
      if (this.renderer.sharedMesh.blendShapeCount > 1)
        this.renderer.SetBlendShapeWeight(1, Mathf.Lerp(this.renderer.GetBlendShapeWeight(1), num2 * 100f, Time.deltaTime * 10f));
      if (!this.affectMaterial)
        return;
      this.renderer.material.SetFloat("_Deform", Mathf.Pow(num1 * 1f, 0.5f));
      if (!this.renderer.material.HasProperty("_PinchDeform"))
        return;
      this.renderer.material.SetFloat("_PinchDeform", Mathf.Pow(num2 * 1f, 0.5f));
    }
  }
}
