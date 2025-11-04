using UnityEngine;


namespace PahlBit
{
    public class BaseObject : MonoBehaviour
    {
        public FiniteStateMachine StateMachine { get => GetComponentInChildren<FiniteStateMachine>(); }
        public AnimatorHelper AnimHelper { get => GetComponentInChildren<AnimatorHelper>(); }
        public ObjectBody Body { get => GetComponentInChildren<ObjectBody>(); }
        public ObjectPhysics Phy { get => GetComponentInChildren<ObjectPhysics>(); }
        
        void Awake()
        {
        }

        void Start()
        {
            
        }
    }
}
