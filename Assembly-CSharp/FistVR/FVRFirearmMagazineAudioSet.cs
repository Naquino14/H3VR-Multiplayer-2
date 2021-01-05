// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFirearmMagazineAudioSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Audio Set", menuName = "AudioPooling/FireArmMagazineAudioSet", order = 0)]
  public class FVRFirearmMagazineAudioSet : ScriptableObject
  {
    public AudioEvent MagazineIn;
    public AudioEvent MagazineOut;
    public AudioEvent MagazineInsertRound;
    public AudioEvent MagazineEjectRound;
  }
}
