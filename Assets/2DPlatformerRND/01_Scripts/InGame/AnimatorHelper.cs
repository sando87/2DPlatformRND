using UnityEngine;

namespace PahlBit
{
    public class AnimatorHelper : MonoBehaviour
    {
        Animator mAnimator = null;

        void Awake()
        {
            mAnimator = GetComponent<Animator>();
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
        public void CrossFadeToState(string stateName, float transactionDuration = 0)
        {
            mAnimator.CrossFade(stateName, transactionDuration);
        }



        public void Hit()
        {
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
        }

    }
}