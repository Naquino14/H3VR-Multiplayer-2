// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.SeeThru
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class SeeThru : MonoBehaviour
  {
    public Material seeThruMaterial;
    private GameObject seeThru;
    private Interactable interactable;
    private Renderer sourceRenderer;
    private Renderer destRenderer;

    private void Awake()
    {
      this.interactable = this.GetComponentInParent<Interactable>();
      this.seeThru = new GameObject("_see_thru");
      this.seeThru.transform.parent = this.transform;
      this.seeThru.transform.localPosition = Vector3.zero;
      this.seeThru.transform.localRotation = Quaternion.identity;
      this.seeThru.transform.localScale = Vector3.one;
      MeshFilter component1 = this.GetComponent<MeshFilter>();
      if ((Object) component1 != (Object) null)
        this.seeThru.AddComponent<MeshFilter>().sharedMesh = component1.sharedMesh;
      MeshRenderer component2 = this.GetComponent<MeshRenderer>();
      if ((Object) component2 != (Object) null)
      {
        this.sourceRenderer = (Renderer) component2;
        this.destRenderer = (Renderer) this.seeThru.AddComponent<MeshRenderer>();
      }
      SkinnedMeshRenderer component3 = this.GetComponent<SkinnedMeshRenderer>();
      if ((Object) component3 != (Object) null)
      {
        SkinnedMeshRenderer skinnedMeshRenderer = this.seeThru.AddComponent<SkinnedMeshRenderer>();
        this.sourceRenderer = (Renderer) component3;
        this.destRenderer = (Renderer) skinnedMeshRenderer;
        skinnedMeshRenderer.sharedMesh = component3.sharedMesh;
        skinnedMeshRenderer.rootBone = component3.rootBone;
        skinnedMeshRenderer.bones = component3.bones;
        skinnedMeshRenderer.quality = component3.quality;
        skinnedMeshRenderer.updateWhenOffscreen = component3.updateWhenOffscreen;
      }
      if ((Object) this.sourceRenderer != (Object) null && (Object) this.destRenderer != (Object) null)
      {
        int length = this.sourceRenderer.sharedMaterials.Length;
        Material[] materialArray = new Material[length];
        for (int index = 0; index < length; ++index)
          materialArray[index] = this.seeThruMaterial;
        this.destRenderer.sharedMaterials = materialArray;
        for (int index = 0; index < this.destRenderer.materials.Length; ++index)
          this.destRenderer.materials[index].renderQueue = 2001;
        for (int index = 0; index < this.sourceRenderer.materials.Length; ++index)
        {
          if (this.sourceRenderer.materials[index].renderQueue == 2000)
            this.sourceRenderer.materials[index].renderQueue = 2002;
        }
      }
      this.seeThru.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
      this.interactable.onAttachedToHand += new Interactable.OnAttachedToHandDelegate(this.AttachedToHand);
      this.interactable.onDetachedFromHand += new Interactable.OnDetachedFromHandDelegate(this.DetachedFromHand);
    }

    private void OnDisable()
    {
      this.interactable.onAttachedToHand -= new Interactable.OnAttachedToHandDelegate(this.AttachedToHand);
      this.interactable.onDetachedFromHand -= new Interactable.OnDetachedFromHandDelegate(this.DetachedFromHand);
    }

    private void AttachedToHand(Hand hand) => this.seeThru.SetActive(true);

    private void DetachedFromHand(Hand hand) => this.seeThru.SetActive(false);

    private void Update()
    {
      if (!this.seeThru.activeInHierarchy)
        return;
      int num = Mathf.Min(this.sourceRenderer.materials.Length, this.destRenderer.materials.Length);
      for (int index = 0; index < num; ++index)
      {
        this.destRenderer.materials[index].mainTexture = this.sourceRenderer.materials[index].mainTexture;
        this.destRenderer.materials[index].color *= this.sourceRenderer.materials[index].color;
      }
    }
  }
}
