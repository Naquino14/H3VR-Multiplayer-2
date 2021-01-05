// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ControllerHoverHighlight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class ControllerHoverHighlight : MonoBehaviour
  {
    public Material highLightMaterial;
    public bool fireHapticsOnHightlight = true;
    protected Hand hand;
    protected RenderModel renderModel;
    protected SteamVR_Events.Action renderModelLoadedAction;

    protected void Awake() => this.hand = this.GetComponentInParent<Hand>();

    protected void OnHandInitialized(int deviceIndex)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.hand.renderModelPrefab);
      gameObject.transform.parent = this.transform;
      gameObject.transform.localPosition = Vector3.zero;
      gameObject.transform.localRotation = Quaternion.identity;
      gameObject.transform.localScale = this.hand.renderModelPrefab.transform.localScale;
      this.renderModel = gameObject.GetComponent<RenderModel>();
      this.renderModel.SetInputSource(this.hand.handType);
      this.renderModel.OnHandInitialized(deviceIndex);
      this.renderModel.SetMaterial(this.highLightMaterial);
      this.hand.SetHoverRenderModel(this.renderModel);
      this.renderModel.onControllerLoaded += new System.Action(this.RenderModel_onControllerLoaded);
      this.renderModel.Hide();
    }

    private void RenderModel_onControllerLoaded() => this.renderModel.Hide();

    protected void OnParentHandHoverBegin(Interactable other)
    {
      if (!this.isActiveAndEnabled || !((UnityEngine.Object) other.transform.parent != (UnityEngine.Object) this.transform.parent))
        return;
      this.ShowHighlight();
    }

    private void OnParentHandHoverEnd(Interactable other) => this.HideHighlight();

    private void OnParentHandInputFocusAcquired()
    {
      if (!this.isActiveAndEnabled || !(bool) (UnityEngine.Object) this.hand.hoveringInteractable || !((UnityEngine.Object) this.hand.hoveringInteractable.transform.parent != (UnityEngine.Object) this.transform.parent))
        return;
      this.ShowHighlight();
    }

    private void OnParentHandInputFocusLost() => this.HideHighlight();

    public void ShowHighlight()
    {
      if ((UnityEngine.Object) this.renderModel == (UnityEngine.Object) null)
        return;
      if (this.fireHapticsOnHightlight)
        this.hand.TriggerHapticPulse((ushort) 500);
      this.renderModel.Show();
    }

    public void HideHighlight()
    {
      if ((UnityEngine.Object) this.renderModel == (UnityEngine.Object) null)
        return;
      if (this.fireHapticsOnHightlight)
        this.hand.TriggerHapticPulse((ushort) 300);
      this.renderModel.Hide();
    }
  }
}
