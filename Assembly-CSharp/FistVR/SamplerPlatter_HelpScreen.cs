// Decompiled with JetBrains decompiler
// Type: FistVR.SamplerPlatter_HelpScreen
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SamplerPlatter_HelpScreen : MonoBehaviour
  {
    public Texture2D PickUpObjectImage;
    public Texture2D PickUpObjectImage_Touch;
    public Texture2D PickUpObjectImage_Index;
    private Texture2D m_currentImage;
    public Renderer Rend;
    public List<SamplerPlatter_HelpScreen.ReactiveTutorialImageSequence> Sequences = new List<SamplerPlatter_HelpScreen.ReactiveTutorialImageSequence>();
    private Dictionary<string, SamplerPlatter_HelpScreen.ReactiveTutorialImageSequence> ObjTutDic = new Dictionary<string, SamplerPlatter_HelpScreen.ReactiveTutorialImageSequence>();
    private FVRPhysicalObject m_currentlyHeldObject;
    private ControlMode CMode;
    private bool m_isStreamlined;

    private void Start()
    {
      GM.CurrentSceneSettings.ObjectPickedUpEvent += new FVRSceneSettings.FVRObjectPickedUp(this.ObjectPickedUp);
      for (int index1 = 0; index1 < this.Sequences.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.Sequences[index1].Objects.Count; ++index2)
          this.ObjTutDic.Add(this.Sequences[index1].Objects[index2].ItemID, this.Sequences[index1]);
      }
    }

    private void OnDestroy() => GM.CurrentSceneSettings.ObjectPickedUpEvent -= new FVRSceneSettings.FVRObjectPickedUp(this.ObjectPickedUp);

    private void ObjectPickedUp(FVRPhysicalObject obj)
    {
      if (!((UnityEngine.Object) this.m_currentlyHeldObject != (UnityEngine.Object) obj) || !((UnityEngine.Object) obj.ObjectWrapper != (UnityEngine.Object) null) || !this.ObjTutDic.ContainsKey(obj.ObjectWrapper.ItemID))
        return;
      this.m_currentlyHeldObject = obj;
      this.UpdateImage();
    }

    private void UpdateImage()
    {
      if ((UnityEngine.Object) this.m_currentlyHeldObject != (UnityEngine.Object) null && !this.m_currentlyHeldObject.IsHeld)
        this.m_currentlyHeldObject = (FVRPhysicalObject) null;
      if ((UnityEngine.Object) this.m_currentlyHeldObject == (UnityEngine.Object) null)
      {
        switch (this.CMode)
        {
          case ControlMode.Vive:
            this.SetMaterialTexture(this.PickUpObjectImage);
            break;
          case ControlMode.Oculus:
            this.SetMaterialTexture(this.PickUpObjectImage_Touch);
            break;
          case ControlMode.WMR:
            this.SetMaterialTexture(this.PickUpObjectImage);
            break;
          case ControlMode.Index:
            this.SetMaterialTexture(this.PickUpObjectImage_Index);
            break;
        }
      }
      else
      {
        int tutorialState = this.m_currentlyHeldObject.GetTutorialState();
        SamplerPlatter_HelpScreen.ReactiveTutorialImageSequence tutorialImageSequence = this.ObjTutDic[this.m_currentlyHeldObject.ObjectWrapper.ItemID];
        switch (this.CMode)
        {
          case ControlMode.Vive:
            this.SetMaterialTexture(tutorialImageSequence.ImageSequence[tutorialState]);
            break;
          case ControlMode.Oculus:
            if (this.m_isStreamlined && tutorialImageSequence.ImageSequence_TouchStreamlined.Count > tutorialState && (UnityEngine.Object) tutorialImageSequence.ImageSequence_TouchStreamlined[tutorialState] != (UnityEngine.Object) null)
            {
              this.SetMaterialTexture(tutorialImageSequence.ImageSequence_TouchStreamlined[tutorialState]);
              break;
            }
            this.SetMaterialTexture(tutorialImageSequence.ImageSequence_Touch[tutorialState]);
            break;
          case ControlMode.WMR:
            this.SetMaterialTexture(tutorialImageSequence.ImageSequence[tutorialState]);
            break;
          case ControlMode.Index:
            if (this.m_isStreamlined && tutorialImageSequence.ImageSequence_IndexStreamlined.Count > tutorialState && (UnityEngine.Object) tutorialImageSequence.ImageSequence_IndexStreamlined[tutorialState] != (UnityEngine.Object) null)
            {
              this.SetMaterialTexture(tutorialImageSequence.ImageSequence_IndexStreamlined[tutorialState]);
              break;
            }
            this.SetMaterialTexture(tutorialImageSequence.ImageSequence_Index[tutorialState]);
            break;
        }
      }
    }

    private void Update()
    {
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if (GM.CurrentMovementManager.Hands[index].HasInit)
        {
          this.CMode = GM.CurrentMovementManager.Hands[index].CMode;
          this.m_isStreamlined = GM.CurrentMovementManager.Hands[index].IsInStreamlinedMode;
        }
      }
      this.UpdateImage();
    }

    private void SetMaterialTexture(Texture2D t)
    {
      if (!((UnityEngine.Object) this.m_currentImage != (UnityEngine.Object) t))
        return;
      this.m_currentImage = t;
      this.Rend.material.SetTexture("_MainTex", (Texture) t);
      this.Rend.material.SetTexture("_IncandescenceMap", (Texture) t);
    }

    [Serializable]
    public class ReactiveTutorialImageSequence
    {
      public List<string> ObjectIDs;
      public List<FVRObject> Objects;
      public List<Texture2D> ImageSequence = new List<Texture2D>();
      public List<Texture2D> ImageSequence_Touch = new List<Texture2D>();
      public List<Texture2D> ImageSequence_TouchStreamlined = new List<Texture2D>();
      public List<Texture2D> ImageSequence_Index = new List<Texture2D>();
      public List<Texture2D> ImageSequence_IndexStreamlined = new List<Texture2D>();
    }
  }
}
