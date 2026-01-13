using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gold : MonoBehaviour
{
    public int GoldAmount { get; set; } = 5;

    public void OnPickedUp(Collider2D col)
    {
        ItemInventory inven = col.ExGetBase().GetComponentInChildren<ItemInventory>();
        if (inven != null)
        {
            inven.CurrentGold += GoldAmount;
        }

        Destroy(gameObject);
    }
}
