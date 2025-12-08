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

        ItemObject itemPrefab = Resources.Load<ItemObject>("Prefabs/Items/" + itemInfo.ResourceData.PrefabName);
        ItemObject itemObj = Instantiate(itemPrefab, position, rotation);
        itemObj.ItemInfo = itemInfo;
        return itemObj;
    }

    public void PickItem(Collider2D col)
    {
        BaseObject pickerPlayer = col.GetComponentInParent<BaseObject>();
        ItemInventory inventory = pickerPlayer.GetComponentInChildren<ItemInventory>();
        inventory.AddItem(ItemInfo);

        Destroy(gameObject);
    }
}
