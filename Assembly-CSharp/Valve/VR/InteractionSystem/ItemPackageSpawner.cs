// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ItemPackageSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class ItemPackageSpawner : MonoBehaviour
  {
    public ItemPackage _itemPackage;
    private bool useItemPackagePreview = true;
    private bool useFadedPreview;
    private GameObject previewObject;
    public bool requireGrabActionToTake;
    public bool requireReleaseActionToReturn;
    public bool showTriggerHint;
    [EnumFlags]
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;
    public bool takeBackItem;
    public bool acceptDifferentItems;
    private GameObject spawnedItem;
    private bool itemIsSpawned;
    public UnityEvent pickupEvent;
    public UnityEvent dropEvent;
    public bool justPickedUpItem;

    public ItemPackage itemPackage
    {
      get => this._itemPackage;
      set => this.CreatePreviewObject();
    }

    private void CreatePreviewObject()
    {
      if (!this.useItemPackagePreview)
        return;
      this.ClearPreview();
      if (!this.useItemPackagePreview || (Object) this.itemPackage == (Object) null)
        return;
      if (!this.useFadedPreview)
      {
        if (!((Object) this.itemPackage.previewPrefab != (Object) null))
          return;
        this.previewObject = Object.Instantiate<GameObject>(this.itemPackage.previewPrefab, this.transform.position, Quaternion.identity);
        this.previewObject.transform.parent = this.transform;
        this.previewObject.transform.localRotation = Quaternion.identity;
      }
      else
      {
        if (!((Object) this.itemPackage.fadedPreviewPrefab != (Object) null))
          return;
        this.previewObject = Object.Instantiate<GameObject>(this.itemPackage.fadedPreviewPrefab, this.transform.position, Quaternion.identity);
        this.previewObject.transform.parent = this.transform;
        this.previewObject.transform.localRotation = Quaternion.identity;
      }
    }

    private void Start() => this.VerifyItemPackage();

    private void VerifyItemPackage()
    {
      if ((Object) this.itemPackage == (Object) null)
        this.ItemPackageNotValid();
      if (!((Object) this.itemPackage.itemPrefab == (Object) null))
        return;
      this.ItemPackageNotValid();
    }

    private void ItemPackageNotValid()
    {
      Debug.LogError((object) ("<b>[SteamVR Interaction]</b> ItemPackage assigned to " + this.gameObject.name + " is not valid. Destroying this game object."));
      Object.Destroy((Object) this.gameObject);
    }

    private void ClearPreview()
    {
      foreach (Transform transform in this.transform)
      {
        if ((double) Time.time > 0.0)
          Object.Destroy((Object) transform.gameObject);
        else
          Object.DestroyImmediate((Object) transform.gameObject);
      }
    }

    private void Update()
    {
      if (!this.itemIsSpawned || !((Object) this.spawnedItem == (Object) null))
        return;
      this.itemIsSpawned = false;
      this.useFadedPreview = false;
      this.dropEvent.Invoke();
      this.CreatePreviewObject();
    }

    private void OnHandHoverBegin(Hand hand)
    {
      if ((Object) this.GetAttachedItemPackage(hand) == (Object) this.itemPackage && this.takeBackItem && !this.requireReleaseActionToReturn)
        this.TakeBackItem(hand);
      if (!this.requireGrabActionToTake)
        this.SpawnAndAttachObject(hand, GrabTypes.Scripted);
      if (!this.requireGrabActionToTake || !this.showTriggerHint)
        return;
      hand.ShowGrabHint("PickUp");
    }

    private void TakeBackItem(Hand hand)
    {
      this.RemoveMatchingItemsFromHandStack(this.itemPackage, hand);
      if (this.itemPackage.packageType != ItemPackage.ItemPackageType.TwoHanded)
        return;
      this.RemoveMatchingItemsFromHandStack(this.itemPackage, hand.otherHand);
    }

    private ItemPackage GetAttachedItemPackage(Hand hand)
    {
      if ((Object) hand.currentAttachedObject == (Object) null)
        return (ItemPackage) null;
      ItemPackageReference component = hand.currentAttachedObject.GetComponent<ItemPackageReference>();
      return (Object) component == (Object) null ? (ItemPackage) null : component.itemPackage;
    }

    private void HandHoverUpdate(Hand hand)
    {
      if (this.takeBackItem && this.requireReleaseActionToReturn && hand.isActive)
      {
        ItemPackage attachedItemPackage = this.GetAttachedItemPackage(hand);
        if ((Object) attachedItemPackage == (Object) this.itemPackage && hand.IsGrabEnding(attachedItemPackage.gameObject))
        {
          this.TakeBackItem(hand);
          return;
        }
      }
      if (!this.requireGrabActionToTake || hand.GetGrabStarting() == GrabTypes.None)
        return;
      this.SpawnAndAttachObject(hand, GrabTypes.Scripted);
    }

    private void OnHandHoverEnd(Hand hand)
    {
      if (!this.justPickedUpItem && this.requireGrabActionToTake && this.showTriggerHint)
        hand.HideGrabHint();
      this.justPickedUpItem = false;
    }

    private void RemoveMatchingItemsFromHandStack(ItemPackage package, Hand hand)
    {
      if ((Object) hand == (Object) null)
        return;
      for (int index = 0; index < hand.AttachedObjects.Count; ++index)
      {
        ItemPackageReference component = hand.AttachedObjects[index].attachedObject.GetComponent<ItemPackageReference>();
        if ((Object) component != (Object) null)
        {
          ItemPackage itemPackage = component.itemPackage;
          if ((Object) itemPackage != (Object) null && (Object) itemPackage == (Object) package)
          {
            GameObject attachedObject = hand.AttachedObjects[index].attachedObject;
            hand.DetachObject(attachedObject);
          }
        }
      }
    }

    private void RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType packageType, Hand hand)
    {
      for (int index = 0; index < hand.AttachedObjects.Count; ++index)
      {
        ItemPackageReference component = hand.AttachedObjects[index].attachedObject.GetComponent<ItemPackageReference>();
        if ((Object) component != (Object) null && component.itemPackage.packageType == packageType)
        {
          GameObject attachedObject = hand.AttachedObjects[index].attachedObject;
          hand.DetachObject(attachedObject);
        }
      }
    }

    private void SpawnAndAttachObject(Hand hand, GrabTypes grabType)
    {
      if ((Object) hand.otherHand != (Object) null && (Object) this.GetAttachedItemPackage(hand.otherHand) == (Object) this.itemPackage)
        this.TakeBackItem(hand.otherHand);
      if (this.showTriggerHint)
        hand.HideGrabHint();
      if ((Object) this.itemPackage.otherHandItemPrefab != (Object) null && hand.otherHand.hoverLocked)
      {
        Debug.Log((object) "<b>[SteamVR Interaction]</b> Not attaching objects because other hand is hoverlocked and we can't deliver both items.");
      }
      else
      {
        if (this.itemPackage.packageType == ItemPackage.ItemPackageType.OneHanded)
        {
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand);
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand);
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand.otherHand);
        }
        if (this.itemPackage.packageType == ItemPackage.ItemPackageType.TwoHanded)
        {
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand);
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand.otherHand);
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand);
          this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand.otherHand);
        }
        this.spawnedItem = Object.Instantiate<GameObject>(this.itemPackage.itemPrefab);
        this.spawnedItem.SetActive(true);
        hand.AttachObject(this.spawnedItem, grabType, this.attachmentFlags);
        if ((Object) this.itemPackage.otherHandItemPrefab != (Object) null && hand.otherHand.isActive)
        {
          GameObject objectToAttach = Object.Instantiate<GameObject>(this.itemPackage.otherHandItemPrefab);
          objectToAttach.SetActive(true);
          hand.otherHand.AttachObject(objectToAttach, grabType, this.attachmentFlags);
        }
        this.itemIsSpawned = true;
        this.justPickedUpItem = true;
        if (!this.takeBackItem)
          return;
        this.useFadedPreview = true;
        this.pickupEvent.Invoke();
        this.CreatePreviewObject();
      }
    }
  }
}
