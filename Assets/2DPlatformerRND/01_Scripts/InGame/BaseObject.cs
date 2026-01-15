using UnityEngine;


namespace PahlBit
{
    public class BaseObject : MonoBehaviour
    {
        public FiniteStateMachine StateMachine { get => GetComponentInChildren<FiniteStateMachine>(); }
        public AnimatorHelper AnimHelper { get => GetComponentInChildren<AnimatorHelper>(); }
        public ObjectBody Body { get => GetComponentInChildren<ObjectBody>(); }
        public ObjectPhysics Phy { get => GetComponentInChildren<ObjectPhysics>(); }
        public PlayerUnitInput Input { get => GetComponentInChildren<PlayerUnitInput>(); }
        public Health Health { get => GetComponentInChildren<Health>(); }
        public PlayerController Ctrl { get => GetComponentInChildren<PlayerController>(); }

        void Awake()
        {
        }

        void Start()
        {

        }

        public void DestroyObj()
        {
            Destroy(gameObject);
        }
    }
}
