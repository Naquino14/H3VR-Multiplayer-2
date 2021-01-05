// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.InputModule
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

namespace Valve.VR.InteractionSystem
{
  public class InputModule : BaseInputModule
  {
    private GameObject submitObject;
    private static InputModule _instance;

    public static InputModule instance
    {
      get
      {
        if ((Object) InputModule._instance == (Object) null)
          InputModule._instance = Object.FindObjectOfType<InputModule>();
        return InputModule._instance;
      }
    }

    public override bool ShouldActivateModule() => base.ShouldActivateModule() && (Object) this.submitObject != (Object) null;

    public void HoverBegin(GameObject gameObject)
    {
      PointerEventData pointerEventData = new PointerEventData(this.eventSystem);
      ExecuteEvents.Execute<IPointerEnterHandler>(gameObject, (BaseEventData) pointerEventData, ExecuteEvents.pointerEnterHandler);
    }

    public void HoverEnd(GameObject gameObject)
    {
      PointerEventData pointerEventData = new PointerEventData(this.eventSystem);
      pointerEventData.selectedObject = (GameObject) null;
      ExecuteEvents.Execute<IPointerExitHandler>(gameObject, (BaseEventData) pointerEventData, ExecuteEvents.pointerExitHandler);
    }

    public void Submit(GameObject gameObject) => this.submitObject = gameObject;

    public override void Process()
    {
      if (!(bool) (Object) this.submitObject)
        return;
      BaseEventData baseEventData = this.GetBaseEventData();
      baseEventData.selectedObject = this.submitObject;
      ExecuteEvents.Execute<ISubmitHandler>(this.submitObject, baseEventData, ExecuteEvents.submitHandler);
      this.submitObject = (GameObject) null;
    }
  }
}
