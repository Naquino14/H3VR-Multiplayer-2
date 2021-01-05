// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Equippable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Throwable))]
  public class Equippable : MonoBehaviour
  {
    [Tooltip("Array of children you do not want to be mirrored. Text, logos, etc.")]
    public Transform[] antiFlip;
    public WhichHand defaultHand = WhichHand.Right;
    private Vector3 initialScale;
    private Interactable interactable;

    [HideInInspector]
    public SteamVR_Input_Sources attachedHandType => (bool) (Object) this.interactable.attachedToHand ? this.interactable.attachedToHand.handType : SteamVR_Input_Sources.Any;

    private void Start()
    {
      this.initialScale = this.transform.localScale;
      this.interactable = this.GetComponent<Interactable>();
    }

    private void Update()
    {
      if (!(bool) (Object) this.interactable.attachedToHand)
        return;
      Vector3 initialScale = this.initialScale;
      if (this.attachedHandType == SteamVR_Input_Sources.RightHand && this.defaultHand == WhichHand.Right || this.attachedHandType == SteamVR_Input_Sources.LeftHand && this.defaultHand == WhichHand.Left)
      {
        initialScale.x *= 1f;
        for (int index = 0; index < this.antiFlip.Length; ++index)
          this.antiFlip[index].localScale = new Vector3(1f, 1f, 1f);
      }
      else
      {
        initialScale.x *= -1f;
        for (int index = 0; index < this.antiFlip.Length; ++index)
          this.antiFlip[index].localScale = new Vector3(-1f, 1f, 1f);
      }
      this.transform.localScale = initialScale;
    }
  }
}
