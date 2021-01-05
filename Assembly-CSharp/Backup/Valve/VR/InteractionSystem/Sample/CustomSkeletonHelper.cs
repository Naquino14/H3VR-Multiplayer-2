// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.CustomSkeletonHelper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class CustomSkeletonHelper : MonoBehaviour
  {
    public CustomSkeletonHelper.Retargetable wrist;
    public CustomSkeletonHelper.Finger[] fingers;
    public CustomSkeletonHelper.Thumb[] thumbs;

    private void Update()
    {
      for (int index = 0; index < this.fingers.Length; ++index)
      {
        CustomSkeletonHelper.Finger finger = this.fingers[index];
        finger.metacarpal.destination.rotation = finger.metacarpal.source.rotation;
        finger.proximal.destination.rotation = finger.proximal.source.rotation;
        finger.middle.destination.rotation = finger.middle.source.rotation;
        finger.distal.destination.rotation = finger.distal.source.rotation;
      }
      for (int index = 0; index < this.thumbs.Length; ++index)
      {
        CustomSkeletonHelper.Thumb thumb = this.thumbs[index];
        thumb.metacarpal.destination.rotation = thumb.metacarpal.source.rotation;
        thumb.middle.destination.rotation = thumb.middle.source.rotation;
        thumb.distal.destination.rotation = thumb.distal.source.rotation;
      }
      this.wrist.destination.position = this.wrist.source.position;
      this.wrist.destination.rotation = this.wrist.source.rotation;
    }

    public enum MirrorType
    {
      None,
      LeftToRight,
      RightToLeft,
    }

    [Serializable]
    public class Retargetable
    {
      public Transform source;
      public Transform destination;

      public Retargetable(Transform source, Transform destination)
      {
        this.source = source;
        this.destination = destination;
      }
    }

    [Serializable]
    public class Thumb
    {
      public CustomSkeletonHelper.Retargetable metacarpal;
      public CustomSkeletonHelper.Retargetable middle;
      public CustomSkeletonHelper.Retargetable distal;
      public Transform aux;

      public Thumb(
        CustomSkeletonHelper.Retargetable metacarpal,
        CustomSkeletonHelper.Retargetable middle,
        CustomSkeletonHelper.Retargetable distal,
        Transform aux)
      {
        this.metacarpal = metacarpal;
        this.middle = middle;
        this.distal = distal;
        this.aux = aux;
      }
    }

    [Serializable]
    public class Finger
    {
      public CustomSkeletonHelper.Retargetable metacarpal;
      public CustomSkeletonHelper.Retargetable proximal;
      public CustomSkeletonHelper.Retargetable middle;
      public CustomSkeletonHelper.Retargetable distal;
      public Transform aux;

      public Finger(
        CustomSkeletonHelper.Retargetable metacarpal,
        CustomSkeletonHelper.Retargetable proximal,
        CustomSkeletonHelper.Retargetable middle,
        CustomSkeletonHelper.Retargetable distal,
        Transform aux)
      {
        this.metacarpal = metacarpal;
        this.proximal = proximal;
        this.middle = middle;
        this.distal = distal;
        this.aux = aux;
      }
    }
  }
}
