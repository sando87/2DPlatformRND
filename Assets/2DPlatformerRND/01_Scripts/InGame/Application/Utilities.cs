using UnityEngine;

namespace PahlBit
{
    public static class Utilities
    {
        public static BaseObject ExGetBase(this MonoBehaviour mono)
        {
            return mono.GetComponentInParent<BaseObject>();
        }

    }
}