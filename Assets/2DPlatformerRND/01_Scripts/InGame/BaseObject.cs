using UnityEngine;


namespace PahlBit
{
    public class BaseObject : MonoBehaviour
    {
        public PlayerUnitInput PlayerInput { get => GetComponentInChildren<PlayerUnitInput>(); }
        public PlayerStateMachine StateMachine { get => GetComponentInChildren<PlayerStateMachine>(); }
        public PlayerController PlayerCTRL { get => GetComponentInChildren<PlayerController>(); }
        public AnimatorHelper AnimHelper { get => GetComponentInChildren<AnimatorHelper>(); }
        
        void Awake()
        {
        }

        void Start()
        {
            
        }
    }
}
