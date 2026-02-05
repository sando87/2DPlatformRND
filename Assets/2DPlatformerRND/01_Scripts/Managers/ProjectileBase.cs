using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ProjectileBase : MonoBehaviour
    {
        [field: SerializeField]
        public ProjectileInfo Stats { get; set; }

        private BaseObject mBaseObj = null;
        private InteractableCollider mInteractCollider = null;
        private Dictionary<Collider2D, float> mHitColliders = new Dictionary<Collider2D, float>();

        private Vector2 mStartPos = Vector2.zero;

        public UnityEvent<Collider2D> OnHit;
        public UnityEvent OnEnd;

        public static ProjectileBase Create(ProjectileBase prefab, Vector3 position, Quaternion rotation, int layer)
        {
            ProjectileBase obj = Instantiate(prefab, position, rotation);
            obj.mStartPos = position;
            obj.gameObject.ExSetLayerAll(layer);
            return obj;
        }

        void Awake()
        {
            mBaseObj = this.ExGetBase();
            mInteractCollider = GetComponentInChildren<InteractableCollider>();
            InitColliderEvents();
        }

        void InitColliderEvents()
        {
            mInteractCollider.OnInteractEnter.AddListener((col) =>
            {
                mHitColliders[col] = Time.time;
                OnHit?.Invoke(col);
            });
            mInteractCollider.OnInteractLeave.AddListener((col) =>
            {
                mHitColliders.Remove(col);
            });
        }

        void Start()
        {
            if (Stats.Duration > 0)
                EndAfterDuration();

            if (Stats.MoveSpeed > 0)
                LaunchProjectile();

            if (Stats.FireAngle != 0)
            {
                Quaternion newRotation = transform.rotation * Quaternion.Euler(0f, 0f, Stats.FireAngle);
                transform.rotation = newRotation;
            }
        }

        void Update()
        {
            if (Stats.Interval > 0)
                HitEventEveryInterval();

            if (Stats.AttackRange > 0)
                EndAfterDistance();
        }

        void LaunchProjectile()
        {
            Vector2 vel = transform.right * Stats.MoveSpeed;
            mBaseObj.Phy.VelocityX = vel.x;
            mBaseObj.Phy.VelocityY = vel.y;
        }
        void EndAfterDistance()
        {
            if ((mStartPos - transform.position.ExToVector2()).magnitude > Stats.AttackRange)
            {
                DoEndProjectile();
            }
        }

        void EndAfterDuration()
        {
            this.ExDelayedCoroutine(Stats.Duration, DoEndProjectile);
        }

        public void DoEndProjectile()
        {
            mBaseObj.Phy.Velocity = Vector2.zero;
            mBaseObj.Phy.LockGravity = true;
            mBaseObj.Body.LockBody = true;
            OnEnd?.Invoke();
        }

        void HitEventEveryInterval()
        {
            // 현재 Hit된 콜라이더들을 interval마다 OnHit콜백 호출해줌
            double interval = Stats.Interval;
            foreach (var kvp in mHitColliders)
            {
                Collider2D col = kvp.Key;
                float lastHitTime = kvp.Value;
                if (Time.time - lastHitTime >= interval)
                {
                    mHitColliders[col] = Time.time;
                    OnHit?.Invoke(col);
                }
            }
        }

        // public readonly struct HitColInfo
        // {
        //     public HitColInfo(float hitTime, Collider2D collider)
        //     {
        //         HitTime = hitTime;
        //         Collider = collider;
        //     }
        //     public float HitTime { get; }
        //     public Collider2D Collider { get; }
        // }

    }
}