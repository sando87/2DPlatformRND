using System.Collections.Generic;
using UnityEngine;

namespace PahlBit
{
    public static class UtilitiesPhy2D
    {
        private static Collider2D[] _results = new Collider2D[32];
        private static RaycastHit2D[] _hits = new RaycastHit2D[32];

        public static BaseObject[] OverlapCircleAll(Vector2 center, float radius, int layerMask, InteractMask interactMask)
        {
            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.useLayerMask = true;
            filter2D.layerMask = layerMask;
            filter2D.useTriggers = true;

            int count = Physics2D.OverlapCircle(center, radius, filter2D, _results);
            if (count <= 0)
                return null;

            List<BaseObject> rets = new List<BaseObject>();
            for (int i = 0; i < count; i++)
            {
                Collider2D col = _results[i];
                InteractableCollider interCol = col.GetComponent<InteractableCollider>();
                if (interCol == null)
                    continue;

                InteractMask mask = interactMask & interCol.MyProperty;
                if (mask != InteractMask.Nothing)
                {
                    rets.Add(col.ExGetBase());
                }
            }

            return rets.ToArray();
        }

        public static BaseObject CircleCast(Vector2 center, float radius, Vector2 direction, float distance, int layerMask, InteractMask interactMask)
        {
            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.useLayerMask = true;
            filter2D.layerMask = layerMask;
            filter2D.useTriggers = true;

            int count = Physics2D.CircleCast(center, radius, direction, filter2D, _hits, distance);
            if (count <= 0)
                return null;

            BaseObject target = null;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < count; i++)
            {
                Collider2D col = _hits[i].collider;
                InteractableCollider interCol = col.GetComponent<InteractableCollider>();
                if (interCol == null)
                    continue;

                InteractMask mask = interactMask & interCol.MyProperty;
                if (mask != InteractMask.Nothing)
                {
                    if (minDist > _hits[i].distance)
                    {
                        minDist = _hits[i].distance;
                        target = col.ExGetBase();
                    }
                }
            }

            return target;
        }

    }
}