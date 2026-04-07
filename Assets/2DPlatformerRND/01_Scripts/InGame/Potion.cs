using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class Potion : MonoBehaviour
{
    [SerializeField] bool _IsHPPotion = true;

    public void OnPickedUp(Collider2D col)
    {
        ItemInventory inven = col.ExGetBase().GetComponentInChildren<ItemInventory>();
        if (inven != null)
        {
            if (_IsHPPotion)
            {
                inven.CurrentLifePotionCount++;
            }
            else
            {
                inven.CurrentManaPotionCount++;
            }
        }

        Destroy(gameObject);
    }
}
