using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace PahlBit
{
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] AnimatorStateEventSet[] _stateEvents = null;


        private Dictionary<AnimStateNameHash, AnimatorStateEventSet> mAnimatorEvents = new Dictionary<AnimStateNameHash, AnimatorStateEventSet>();

        Animator mAnimator = null;

        void Awake()
        {
            mAnimator = GetComponent<Animator>();

            InitEvents();
        }

        void InitEvents()
        {
            foreach (var eventSet in _stateEvents)
            {
                mAnimatorEvents[eventSet.StateNameHash] = eventSet;
            }
        }

        public void SetParamFloat(string paramName, float value)
        {
            mAnimator.SetFloat(paramName, value);
        }
        public void SetParamInt(string paramName, int value)
        {
            mAnimator.SetInteger(paramName, value);
        }
        public void SetParamBool(string paramName, bool value)
        {
            mAnimator.SetBool(paramName, value);
        }
        public void SetParamTrigger(string paramName)
        {
            mAnimator.SetTrigger(paramName);
        }
        public void CrossFadeToState(string stateName, int layer = 0)
        {
            mAnimator.CrossFade(stateName, 0, layer, 0);
        }
        public void CrossFadeToState(int stateHashName, int layer = 0)
        {
            mAnimator.CrossFade(stateHashName, 0, layer, 0);
        }

        public int GetCurrentStateNameHash(int layer)
        {
            return mAnimator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
        }


        Action<int> _onFire;
        async public UniTask PlayAnim(int stateHashName, CancellationToken ct, Action<int> onFire = null, int layer = 0)
        {
            try
            {
                _onFire = onFire;
                mAnimator.CrossFade(stateHashName, 0, layer, 0);
                await UniTask.WaitUntil(() => mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != stateHashName, cancellationToken: ct);
            }
            finally
            {
                _onFire = null;
            }
        }



        public void Hit()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }

        public void Hit0()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }
        public void Hit1()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 1);
        }
        public void Hit2()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 2);
        }

        public void FootR()
        {
        }

        public void FootL()
        {
        }

        public void Land()
        {
        }

        public void Shoot()
        {
            int curStateHashName = mAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            InvokeEventMiddle(curStateHashName, 0);
        }


        public void InvokeEventEnter(AnimStateNameHash stateNameHash)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventEnter.Invoke();
            }
        }
        public void InvokeEventMiddle(AnimStateNameHash stateNameHash, int index)
        {
            _onFire?.Invoke(index);

            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventMiddle.Invoke(index);
            }
        }
        public void InvokeEventLeave(AnimStateNameHash stateNameHash)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventLeave.Invoke();
            }
        }



        public void AddEventEnter(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (!mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash] = new AnimatorStateEventSet(stateNameHash);
            }
            mAnimatorEvents[stateNameHash].EventEnter.AddListener(action);
        }
        public void AddEventMiddle(AnimStateNameHash stateNameHash, UnityAction<int> action)
        {
            if (!mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash] = new AnimatorStateEventSet(stateNameHash);
            }
            mAnimatorEvents[stateNameHash].EventMiddle.AddListener(action);
        }
        public void AddEventLeave(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (!mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash] = new AnimatorStateEventSet(stateNameHash);
            }
            mAnimatorEvents[stateNameHash].EventLeave.AddListener(action);
        }
        public void RemoveEventEnter(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventEnter.RemoveListener(action);
            }
        }
        public void RemoveEventMiddle(AnimStateNameHash stateNameHash, UnityAction<int> action)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventMiddle.RemoveListener(action);
            }
        }
        public void RemoveEventLeave(AnimStateNameHash stateNameHash, UnityAction action)
        {
            if (mAnimatorEvents.ContainsKey(stateNameHash))
            {
                mAnimatorEvents[stateNameHash].EventLeave.RemoveListener(action);
            }
        }

    }

    [System.Serializable]
    public class AnimatorStateEventSet
    {
        [AnimatorStateHash]
        public int StateNameHash = 0;
        public UnityEvent EventEnter = new UnityEvent();
        public UnityEvent<int> EventMiddle = new UnityEvent<int>();
        public UnityEvent EventLeave = new UnityEvent();

        public AnimatorStateEventSet(int stateNameHash)
        {
            StateNameHash = stateNameHash;
        }
    }
}