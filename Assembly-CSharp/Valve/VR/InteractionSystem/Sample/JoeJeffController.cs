// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.JoeJeffController
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class JoeJeffController : MonoBehaviour
  {
    public Transform Joystick;
    public float joyMove = 0.1f;
    public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move");
    public SteamVR_Action_Boolean jumpAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("platformer", "Jump");
    public JoeJeff character;
    public Renderer jumpHighlight;
    private Vector3 movement;
    private bool jump;
    private float glow;
    private SteamVR_Input_Sources hand;
    private Interactable interactable;

    private void Start() => this.interactable = this.GetComponent<Interactable>();

    private void Update()
    {
      if ((bool) (Object) this.interactable.attachedToHand)
      {
        this.hand = this.interactable.attachedToHand.handType;
        Vector2 axis = this.moveAction[this.hand].axis;
        this.movement = new Vector3(axis.x, 0.0f, axis.y);
        this.jump = this.jumpAction[this.hand].stateDown;
        this.glow = Mathf.Lerp(this.glow, !this.jumpAction[this.hand].state ? 1f : 1.5f, Time.deltaTime * 20f);
      }
      else
      {
        this.movement = (Vector3) Vector2.zero;
        this.jump = false;
        this.glow = 0.0f;
      }
      this.Joystick.localPosition = this.movement * this.joyMove;
      this.movement = Quaternion.AngleAxis(this.transform.eulerAngles.y, Vector3.up) * this.movement;
      this.jumpHighlight.sharedMaterial.SetColor("_EmissionColor", Color.white * this.glow);
      this.character.Move(this.movement * 2f, this.jump);
    }
  }
}
