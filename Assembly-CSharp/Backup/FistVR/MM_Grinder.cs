// Decompiled with JetBrains decompiler
// Type: FistVR.MM_Grinder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class MM_Grinder : MonoBehaviour
  {
    public Transform EjectionPoint;
    public Transform InFunnelFXPoint;
    public GameObject Prefab_Expel;
    public GameObject Prefab_Funnel;
    private float m_curRot;
    private float m_rotTilShot = 36f;
    public int amountOfOtherObjects;
    public int[] contents = new int[18];
    public AudioSource Aud;
    public AudioClip Audclip_DispenseRecipe;
    public AudioClip Audclip_DispenseSolo;
    public AudioClip Audclip_DispenseJunk;
    public AudioClip Audclip_Insert;
    public FVRObject[] CurrencyObjs;
    public MM_Grinder.Recipe[] Recipes;

    public void Crank(float f)
    {
      float num = Mathf.Clamp(f * 0.4f, 0.0f, 2f);
      this.m_curRot += num;
      if ((double) this.m_curRot > 180.0)
        this.m_curRot -= 360f;
      this.m_rotTilShot -= num;
      if ((double) this.m_rotTilShot > 0.0)
        return;
      this.Smash();
      this.m_rotTilShot = 36f;
    }

    private void Smash()
    {
      for (int index = 0; index < this.Recipes.Length; ++index)
      {
        if (this.contents[(int) this.Recipes[index].Type1] > 0 && this.contents[(int) this.Recipes[index].Type2] > 0)
        {
          --this.contents[(int) this.Recipes[index].Type1];
          --this.contents[(int) this.Recipes[index].Type2];
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.CurrencyObjs[(int) this.Recipes[index].Result].GetGameObject(), this.EjectionPoint.position, UnityEngine.Random.rotation);
          UnityEngine.Object.Instantiate<GameObject>(this.Prefab_Funnel, this.EjectionPoint.position, UnityEngine.Random.rotation);
          gameObject.GetComponent<Rigidbody>().velocity = this.EjectionPoint.forward * UnityEngine.Random.Range(0.5f, 2.5f);
          this.Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
          this.Aud.PlayOneShot(this.Audclip_DispenseRecipe, UnityEngine.Random.Range(0.7f, 1f));
          return;
        }
      }
      for (int index = this.contents.Length - 1; index >= 0; --index)
      {
        if (this.contents[index] > 0)
        {
          --this.contents[index];
          UnityEngine.Object.Instantiate<GameObject>(this.CurrencyObjs[index].GetGameObject(), this.EjectionPoint.position, UnityEngine.Random.rotation).GetComponent<Rigidbody>().velocity = this.EjectionPoint.forward * UnityEngine.Random.Range(0.5f, 2.5f);
          this.Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
          this.Aud.PlayOneShot(this.Audclip_DispenseSolo, UnityEngine.Random.Range(0.7f, 1f));
          return;
        }
      }
      if (this.amountOfOtherObjects <= 0)
        return;
      --this.amountOfOtherObjects;
      UnityEngine.Object.Instantiate<GameObject>(this.CurrencyObjs[UnityEngine.Random.Range(0, 3)].GetGameObject(), this.EjectionPoint.position, UnityEngine.Random.rotation).GetComponent<Rigidbody>().velocity = this.EjectionPoint.forward * UnityEngine.Random.Range(0.5f, 2.5f);
      this.Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
      this.Aud.PlayOneShot(this.Audclip_DispenseJunk, UnityEngine.Random.Range(0.7f, 1f));
    }

    private void OnTriggerEnter(Collider col) => this.Checkcol(col);

    private void Checkcol(Collider col)
    {
      if (!((UnityEngine.Object) col.attachedRigidbody != (UnityEngine.Object) null))
        return;
      FVRPhysicalObject component = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || (UnityEngine.Object) component.QuickbeltSlot != (UnityEngine.Object) null)
        return;
      if (component is MM_Currency)
      {
        MM_Currency mmCurrency = component as MM_Currency;
        int num = this.contents[(int) mmCurrency.Type] + 1;
        this.contents[(int) mmCurrency.Type] = num;
      }
      else
        ++this.amountOfOtherObjects;
      UnityEngine.Object.Instantiate<GameObject>(this.Prefab_Funnel, this.InFunnelFXPoint.position, UnityEngine.Random.rotation);
      this.Aud.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
      this.Aud.PlayOneShot(this.Audclip_Insert, UnityEngine.Random.Range(0.7f, 1f));
      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
    }

    [Serializable]
    public class Recipe
    {
      public MMCurrency Type1;
      public MMCurrency Type2;
      public MMCurrency Result;
    }
  }
}
