// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSequenceLibrary
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Sequence Library Def", menuName = "OmniSequencer/Libraries/Sequence Library", order = 0)]
  public class OmniSequenceLibrary : ScriptableObject
  {
    public OmniSequenceLibrary.SequenceLibraryTheme[] Themes;

    [Serializable]
    public class SequenceLibraryTheme
    {
      public OmniSequencerSequenceDefinition.OmniSequenceTheme Theme;
      public Sprite Sprite;
      public OmniSequencerSequenceDefinition[] SequenceList;
      [Multiline(16)]
      public string ThemeDetails;
    }
  }
}
