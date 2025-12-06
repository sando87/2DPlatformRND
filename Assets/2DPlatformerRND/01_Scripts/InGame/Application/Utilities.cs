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
        public static float ExNextFloatNormalized(this System.Random random)
        {
            // Next(int) 는 0 ~ int.MaxValue-1
            int value = random.Next(int.MaxValue);      // 0 ~ 2,147,483,646
            return (float)value / (int.MaxValue - 1); // 0 ~ 1 포함
        }

    }
}