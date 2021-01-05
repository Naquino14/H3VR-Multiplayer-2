// Decompiled with JetBrains decompiler
// Type: FistVR.MM_Dispenser
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class MM_Dispenser : MonoBehaviour
  {
    public Image Display;
    public Text TextReadout;
    public string CurrencyName;
    public MMCurrency Currency;
    public Transform SpawnPoint;
    public FVRObject CurrencyPrefab;
    public Sprite Sprite_Known;
    public Sprite Sprite_Unknown;
    public AudioSource Aud;
    public AudioClip AudClip_Dispense;
    public AudioClip AudClip_Fail;
    private float tick = 1f;

    private void Start()
    {
      this.UpdateReadOut();
      this.tick = Random.Range(0.5f, 1f);
    }

    private void Update()
    {
      this.tick -= Time.deltaTime;
      if ((double) this.tick >= 0.0)
        return;
      this.tick = Random.Range(0.5f, 1f);
      this.UpdateReadOut();
    }

    private void UpdateReadOut()
    {
      if (GM.MMFlags.IsCurrencyKnown(this.Currency))
      {
        this.TextReadout.text = this.CurrencyName + "\n" + "[" + GM.MMFlags.MMMTCs[(int) this.Currency].ToString() + "]" + "\n" + "Click To Dispense";
        this.Display.sprite = this.Sprite_Known;
      }
      else
      {
        this.TextReadout.text = "Unknown Currency\n[0]";
        this.Display.sprite = this.Sprite_Unknown;
      }
    }

    public void DispenseButton()
    {
      if (GM.MMFlags.HasCurrency(this.Currency))
      {
        Object.Instantiate<GameObject>(this.CurrencyPrefab.GetGameObject(), this.SpawnPoint.position, this.SpawnPoint.rotation).GetComponent<FVRPhysicalObject>().RootRigidbody.velocity = -this.transform.forward;
        this.Aud.PlayOneShot(this.AudClip_Dispense, 0.5f);
        GM.MMFlags.RemoveCurrency(this.Currency, 1);
        GM.MMFlags.SaveToFile();
      }
      else
        this.Aud.PlayOneShot(this.AudClip_Fail, 0.5f);
      this.UpdateReadOut();
    }
  }
}
