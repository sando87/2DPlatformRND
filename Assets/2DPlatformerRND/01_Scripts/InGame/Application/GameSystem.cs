using UnityEngine;

namespace PahlBit
{
    public static class GameSystem
    {
        public static ItemInfo AssignNewItem()
        {
            ItemInfo newItem = new ItemInfo();
            newItem.InstanceID = System.Guid.NewGuid().ToString();
            newItem.ResourceID = TableItem.Instance.GetRandomItem().ID;
            newItem.IsEquipped = false;
            newItem.Level = 1;
            newItem.Count = 1;
            newItem.PositionIndex = -1;
            return newItem;
        }
    }
}