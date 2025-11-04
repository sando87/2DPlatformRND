using DG.Tweening;
using UnityEngine;

namespace PahlBit
{
    public class ObjectPhysics : MonoBehaviour
    {
        public float VelocityX { get { return mRB2D.linearVelocity.x; } set { mRB2D.linearVelocity = new Vector2(value, mRB2D.linearVelocity.y); } }
        public float VelocityY { get { return mRB2D.linearVelocity.y; } set { mRB2D.linearVelocity = new Vector2(mRB2D.linearVelocity.x, value); } }
        public Vector2 Velocity { get { return mRB2D.linearVelocity; } set { mRB2D.linearVelocity = value; } }

        private Rigidbody2D mRB2D = null;

        private void Awake()
        {
            mRB2D = GetComponent<Rigidbody2D>();
        }

        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Impulse)
        {
            mRB2D.AddForce(force, mode);
        }
    }
}