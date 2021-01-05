// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PropertySheetFactory
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PropertySheetFactory
  {
    private readonly Dictionary<Shader, PropertySheet> m_Sheets;

    public PropertySheetFactory() => this.m_Sheets = new Dictionary<Shader, PropertySheet>();

    public PropertySheet Get(string shaderName) => this.Get(Shader.Find(shaderName));

    public PropertySheet Get(Shader shader)
    {
      PropertySheet propertySheet1;
      if (this.m_Sheets.TryGetValue(shader, out propertySheet1))
        return propertySheet1;
      string str = !((UnityEngine.Object) shader == (UnityEngine.Object) null) ? shader.name : throw new ArgumentException(string.Format("Invalid shader ({0})", (object) shader));
      Material material = new Material(shader);
      material.name = string.Format("PostProcess - {0}", (object) str.Substring(str.LastIndexOf('/') + 1));
      material.hideFlags = HideFlags.DontSave;
      PropertySheet propertySheet2 = new PropertySheet(material);
      this.m_Sheets.Add(shader, propertySheet2);
      return propertySheet2;
    }

    public void Release()
    {
      foreach (PropertySheet propertySheet in this.m_Sheets.Values)
        propertySheet.Release();
      this.m_Sheets.Clear();
    }
  }
}
