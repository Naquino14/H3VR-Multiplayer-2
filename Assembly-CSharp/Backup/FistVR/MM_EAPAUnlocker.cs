// Decompiled with JetBrains decompiler
// Type: FistVR.MM_EAPAUnlocker
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MM_EAPAUnlocker : MonoBehaviour
  {
    public Image[] ButtonImages;
    public Sprite[] ButtonSprites;
    public AudioSource Aud;
    public AudioClip AudClip_Unlock;
    public AudioClip AudClip_Spawn;
    public AudioClip AudClip_Fail;
    public int[] Costs_Bronze;
    public int[] Costs_Silver;
    public int[] Costs_Gold;
    public GameObject Prefab_EAPABox;
    public Transform EAPABox_SpawnPoint;
    public MM_EAPAUnlocker.EAPA[] EAPAs;
    public AudioSource Music;

    public void Start()
    {
      for (int i = 0; i < this.ButtonImages.Length; ++i)
      {
        if (GM.MMFlags.IsEAPAUnlocked(i))
          this.ButtonImages[i].sprite = this.ButtonSprites[i];
      }
    }

    public void UnlockEAPA(int i)
    {
      if (GM.MMFlags.IsEAPAUnlocked(i))
      {
        MM_EAPACrate component = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_EAPABox, this.EAPABox_SpawnPoint.position, this.EAPABox_SpawnPoint.rotation).GetComponent<MM_EAPACrate>();
        GameObject g1 = (GameObject) null;
        GameObject g2 = (GameObject) null;
        GameObject g3 = (GameObject) null;
        GameObject g4 = (GameObject) null;
        GameObject g5 = (GameObject) null;
        if ((UnityEngine.Object) this.EAPAs[i].O1 != (UnityEngine.Object) null)
          g1 = this.EAPAs[i].O1.GetGameObject();
        if ((UnityEngine.Object) this.EAPAs[i].O2 != (UnityEngine.Object) null)
          g2 = this.EAPAs[i].O2.GetGameObject();
        if ((UnityEngine.Object) this.EAPAs[i].O3 != (UnityEngine.Object) null)
          g3 = this.EAPAs[i].O3.GetGameObject();
        if ((UnityEngine.Object) this.EAPAs[i].O4 != (UnityEngine.Object) null)
          g4 = this.EAPAs[i].O4.GetGameObject();
        if ((UnityEngine.Object) this.EAPAs[i].O5 != (UnityEngine.Object) null)
          g5 = this.EAPAs[i].O5.GetGameObject();
        component.SetGOs(g1, g2, g3, g4, g5);
        this.PlaySound(this.AudClip_Spawn);
      }
      else
      {
        bool flag = true;
        if (this.Costs_Bronze[i] > 0 && !GM.MMFlags.HasCurrency(MMCurrency.MarinatedMedallions, this.Costs_Bronze[i]))
          flag = false;
        if (this.Costs_Silver[i] > 0 && !GM.MMFlags.HasCurrency(MMCurrency.TenderloinTokens, this.Costs_Silver[i]))
          flag = false;
        if (this.Costs_Gold[i] > 0 && !GM.MMFlags.HasCurrency(MMCurrency.NutriciousNuggets, this.Costs_Gold[i]))
          flag = false;
        if (flag)
        {
          GM.MMFlags.RemoveCurrency(MMCurrency.MarinatedMedallions, this.Costs_Bronze[i]);
          GM.MMFlags.RemoveCurrency(MMCurrency.TenderloinTokens, this.Costs_Silver[i]);
          GM.MMFlags.RemoveCurrency(MMCurrency.NutriciousNuggets, this.Costs_Gold[i]);
          GM.MMFlags.UnlockEAPA(i);
          GM.MMFlags.SaveToFile();
          this.PlaySound(this.AudClip_Unlock);
          this.ButtonImages[i].sprite = this.ButtonSprites[i];
          if (this.Music.isPlaying)
            return;
          this.Music.Play();
        }
        else
          this.PlaySound(this.AudClip_Fail);
      }
    }

    private void PlaySound(AudioClip c)
    {
      if (this.Aud.isPlaying)
        return;
      this.Aud.clip = c;
      this.Aud.Play();
    }

    [Serializable]
    public class EAPA
    {
      public FVRObject O1;
      public FVRObject O2;
      public FVRObject O3;
      public FVRObject O4;
      public FVRObject O5;
    }
  }
}
