using UnityEngine;


namespace PahlBit
{
    public class BaseObject : MonoBehaviour
    {
        public PlayerUnitInput PlayerInput { get; private set; }
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerController PlayerCTRL { get; private set; } 
        public AnimatorHelper AnimHelper { get; private set; }
        
        void Awake()
        {
            PlayerInput = GetComponentInChildren<PlayerUnitInput>();
            StateMachine = GetComponentInChildren<PlayerStateMachine>();
            PlayerCTRL = GetComponentInChildren<PlayerController>();
            AnimHelper = GetComponentInChildren<AnimatorHelper>();
        }

        void Start()
        {
            
        }
    }
}
