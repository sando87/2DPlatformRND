using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{

    public class Equipment : MonoBehaviour
    {
        public PlayerRoot PlayerRoot => GetComponentInParent<PlayerRoot>();

        public PropertyEffect TotalItemEffect { get; private set; } = new PropertyEffect();

        public void Init()
        {
        }

        public void UpdateState()
        {
        }

        public void EquipItem(string itemID)
        {
            GameDataItem itemData = TableItem.Instance.GetInfo(GameDataItem.ToID(itemID));
            if (itemData != null)
            {
                // TotalItemEffect
            }

        }

        public void UnEquipItem(string itemID)
        {

        }


    }
}