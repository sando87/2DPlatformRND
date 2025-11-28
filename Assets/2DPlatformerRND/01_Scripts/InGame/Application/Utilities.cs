using UnityEngine;

namespace PahlBit
{
    public static class Utilities
    {
        public static BaseObject ExGetBase(this MonoBehaviour mono)
        {
            return mono.GetComponentInParent<BaseObject>();
        }
        public static BaseObject ExGetBase(this Collider2D col)
        {
            return col.GetComponentInParent<BaseObject>();
        }
        public static void ExSetLayerAll(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.ExSetLayerAll(layer);
            }
        }

    }
}