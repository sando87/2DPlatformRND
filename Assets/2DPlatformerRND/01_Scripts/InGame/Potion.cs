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
                if (inven.CurrentLifePotionCount < 10)
                {
                    inven.CurrentLifePotionCount++;
                    Destroy(gameObject);
                }
            }
            else
            {
                if (inven.CurrentManaPotionCount < 10)
                {
                    inven.CurrentManaPotionCount++;
                    Destroy(gameObject);
                }
            }
        }
    }
}
