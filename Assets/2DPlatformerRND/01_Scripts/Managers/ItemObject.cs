using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemObject : MonoBehaviour
{
    public ItemInfo ItemInfo { get; private set; }

    public static ItemObject Create(Vector3 position, Quaternion rotation)
    {
        ItemInfo itemInfo = new ItemInfo();
        itemInfo.InitRandomItem();

        ItemObject itemObj = Instantiate(itemInfo.ResourceData.AssetData.Prefab, position, rotation);
        itemObj.ItemInfo = itemInfo;
        return itemObj;
    }

    public void OnPickedUpBy(Collider2D col)
    {
        BaseObject pickerPlayer = col.GetComponentInParent<BaseObject>();
        ItemInventory inventory = pickerPlayer.GetComponentInChildren<ItemInventory>();
        inventory.AddItem(ItemInfo);

        Destroy(gameObject);
    }
}
