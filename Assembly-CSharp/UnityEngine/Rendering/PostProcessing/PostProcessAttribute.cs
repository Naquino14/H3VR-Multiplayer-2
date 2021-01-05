// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessAttribute
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class PostProcessAttribute : Attribute
  {
    public readonly System.Type renderer;
    public readonly PostProcessEvent eventType;
    public readonly string menuItem;
    public readonly bool allowInSceneView;
    internal readonly bool builtinEffect;

    public PostProcessAttribute(
      System.Type renderer,
      PostProcessEvent eventType,
      string menuItem,
      bool allowInSceneView = true)
    {
      this.renderer = renderer;
      this.eventType = eventType;
      this.menuItem = menuItem;
      this.allowInSceneView = allowInSceneView;
      this.builtinEffect = false;
    }

    internal PostProcessAttribute(System.Type renderer, string menuItem, bool allowInSceneView = true)
    {
      this.renderer = renderer;
      this.menuItem = menuItem;
      this.allowInSceneView = allowInSceneView;
      this.builtinEffect = true;
    }
  }
}
