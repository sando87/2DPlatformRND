using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class ProjLaser : ProjectileBase
    {
        [SerializeField] ProjLaser _ReflectLaser = null;
        [SerializeField] SpriteRenderer _LaserLoop = null;
        [SerializeField] BoxCollider2D _LaserBody = null;
        [SerializeField] float _StartLength = 12;

        public int UpdateIndex { get; private set; } = 0;
        public IReactableLaser ReflectedFrom { get; private set; } = null;

        Dictionary<IReactableLaser, ProjLaser> mReflectionPair = new Dictionary<IReactableLaser, ProjLaser>();

        List<IReactableLaser> mTempReactors = new List<IReactableLaser>();
        List<IReactableLaser> mRemoveKeys = new List<IReactableLaser>();
        private int mCurrentUpdateIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            SetLaserLength(_StartLength);
        }

        protected override void Update()
        {
            base.Update();

            mCurrentUpdateIndex++;
            AffectLaserReflect();
            RemoveOldReflections();
        }

        public override void DoEndProjectile()
        {
            base.DoEndProjectile();

            foreach (var pair in mReflectionPair)
            {
                pair.Value.DoEndProjectile();
            }
            mReflectionPair.Clear();
        }

        void UpdateReflection(Vector2 pos, Vector2 dir, float length, int updateIndex, IReactableLaser reflectedFrom)
        {
            SetLaserLength(length);
            mBaseObj.transform.ExSetWorldPositionXY(pos);
            mBaseObj.transform.right = dir;
            UpdateIndex = updateIndex;
            ReflectedFrom = reflectedFrom;
        }

        void AffectLaserReflect()
        {
            mTempReactors.Clear();
            UtilitiesPhy2D.OverlapBox(mBaseObj.Body.Rect, 1 << gameObject.layer, mTempReactors);
            foreach (IReactableLaser laserRector in mTempReactors)
            {
                if (laserRector == ReflectedFrom)
                    continue;

                laserRector.OnReactLaserReflection(this);
                Vector2 pos = laserRector.ReflectPos;
                Vector2 dir = laserRector.ReflectDir;

                float newLength = (mBaseObj.transform.position.ExToVector2() - pos).magnitude;
                SetLaserLength(newLength);

                if (mReflectionPair.ContainsKey(laserRector))
                {
                    mReflectionPair[laserRector].UpdateReflection(pos, dir, _StartLength - newLength, mCurrentUpdateIndex, laserRector);
                }
                else
                {
                    ProjLaser newLaser = CreateReflectLaser(pos, dir);
                    mReflectionPair[laserRector] = newLaser;
                    newLaser.UpdateReflection(pos, dir, _StartLength - newLength, mCurrentUpdateIndex, laserRector);
                }
            }
        }

        void RemoveOldReflections()
        {
            mRemoveKeys.Clear();
            foreach (var pair in mReflectionPair)
            {
                if (pair.Value.UpdateIndex != mCurrentUpdateIndex)
                {
                    pair.Value.DoEndProjectile();
                    mRemoveKeys.Add(pair.Key);
                }
            }

            for (int i = 0; i < mRemoveKeys.Count; i++)
            {
                mReflectionPair.Remove(mRemoveKeys[i]);
            }
        }

        public ProjLaser CreateReflectLaser(Vector2 pos, Vector2 dir)
        {
            // 스킬 오브젝트 생성
            ProjLaser proj = Create(_ReflectLaser, pos, dir, mBaseObj.gameObject.layer) as ProjLaser;
            proj.OnHit.AddListener((col) =>
            {
                OnHit?.Invoke(col);
            });
            return proj;
        }

        public void SetLaserLength(float length)
        {
            Vector2 imgSize = _LaserLoop.size;
            imgSize.x = length - 0.3f; // 0.3f를 살짝 빼줘야 비쥬얼적으로 더 자연스럽게 보임.
            _LaserLoop.size = imgSize;

            Vector2 colSize = _LaserBody.size;
            colSize.x = length;
            _LaserBody.size = colSize;

            Vector3 pos = _LaserBody.transform.localPosition;
            pos.x = length * 0.5f;
            _LaserBody.transform.localPosition = pos;
        }
    }
}