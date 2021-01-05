// Decompiled with JetBrains decompiler
// Type: InspectorButtonAttribute
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
  public static float kDefaultButtonWidth = 80f;
  public readonly string MethodName;
  private float _buttonWidth = InspectorButtonAttribute.kDefaultButtonWidth;

  public InspectorButtonAttribute(string MethodName) => this.MethodName = MethodName;

  public float ButtonWidth
  {
    get => this._buttonWidth;
    set => this._buttonWidth = value;
  }
}
