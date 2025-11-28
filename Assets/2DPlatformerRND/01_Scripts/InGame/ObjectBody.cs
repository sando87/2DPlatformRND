using DG.Tweening;
using UnityEngine;

namespace PahlBit
{
    public class ObjectBody : MonoBehaviour
    {
        BoxCollider2D mCollider = null;

        public Vector2 Center { get => transform.position.ExToVector2() + mCollider.offset; }
        public Vector2 Size { get => mCollider.size; }
        public Vector2 Foot { get => Center - (transform.up * Size * 0.5f); }
        public Vector2 Head { get => Center + (transform.up * Size * 0.5f); }
        public Vector2 Front { get => Center + (transform.right * Size * 0.5f); }
        public Vector2 Back { get => Center - (transform.right * Size * 0.5f); }
        public Rect Rect { get => mCollider.ExToRect(); }
        public Vector2 FrontDir { get => transform.right; }

        public bool LockBody { get => mCollider.enabled; set => mCollider.enabled = value; }

        void Awake()
        {
            mCollider = GetComponent<BoxCollider2D>();
        }
    }
}