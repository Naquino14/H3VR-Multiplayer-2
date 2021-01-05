// Decompiled with JetBrains decompiler
// Type: FistVR.FVRViveHandMenu
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class FVRViveHandMenu : MonoBehaviour
  {
    private FVRViveHandMenu.MenuState curState;
    public Transform Target;
    public Text Text_Main;
    public Text Text_Teleport;
    public Text Text_SceneReset;
    public Text Text_Grab;
    public Text Text_Moo;
    public FVRViveHand Hand;

    public void LateUpdate()
    {
      this.transform.position = this.Target.position;
      this.transform.rotation = this.Target.rotation;
    }

    public void SetMenuState(FVRViveHandMenu.MenuState state)
    {
      if (this.curState != state)
        this.Hand.Buzz(this.Hand.Buzzer.Buzz_OnMenuOption);
      this.curState = state;
      switch (state)
      {
        case FVRViveHandMenu.MenuState.Main:
          this.Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          break;
        case FVRViveHandMenu.MenuState.TouchNone:
          this.Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          break;
        case FVRViveHandMenu.MenuState.TouchTeleport:
          this.Text_Teleport.color = new Color(0.1f, 1f, 0.1f, 1f);
          this.Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          break;
        case FVRViveHandMenu.MenuState.TouchSceneReset:
          this.Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_SceneReset.color = new Color(0.1f, 1f, 0.1f, 1f);
          this.Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          break;
        case FVRViveHandMenu.MenuState.TouchGrab:
          this.Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Grab.color = new Color(0.1f, 1f, 0.1f, 1f);
          this.Text_Moo.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          break;
        case FVRViveHandMenu.MenuState.TouchMoo:
          this.Text_Teleport.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_SceneReset.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Grab.color = new Color(0.8f, 0.8f, 0.8f, 1f);
          this.Text_Moo.color = new Color(0.1f, 1f, 0.1f, 1f);
          break;
      }
    }

    public enum MenuState
    {
      Main,
      TouchNone,
      TouchTeleport,
      TouchSceneReset,
      TouchGrab,
      TouchMoo,
    }
  }
}
