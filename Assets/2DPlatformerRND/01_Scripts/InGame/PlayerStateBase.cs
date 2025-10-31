using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateBase : MonoBehaviour
    {
        [SerializeField][Range(0, 10)] int _Layer = 0;
        [SerializeField][Range(0, 10)] int _Priority = 0;

        public int Layer { get { return _Layer; } }
        public int Priority { get { return _Priority; } }
        
        public BaseObject Base { get; private set; }

        [Foldout("Events")]
        public UnityEvent EventEnter = new UnityEvent();
        [Foldout("Events")]
        public UnityEvent EventLeave = new UnityEvent();

        public virtual void InitState()
        {
            Base = GetComponentInParent<BaseObject>();
        }

        public virtual void HandleInput()
        {
            // 입력 처리 코드
        }

        public virtual void EnterState(object param)
        {
            // 상태에 진입할 때 실행되는 코드
            EventEnter?.Invoke();
        }
        public virtual void UpdateState()
        {
            // 매 프레임마다 실행되는 코드
        }
        public virtual void FixedUpdateState()
        {
            // 매 프레임마다 실행되는 코드
        }
        public virtual void LeaveState()
        {
            // 상태에서 벗어날 때 실행되는 코드
            EventLeave?.Invoke();
        }

        protected void ChangeStateToIdle()
        {
            Base.StateMachine.ChangeStateToIdle(Layer);
        }
        protected void ChangeStateToThis()
        {
            Base.StateMachine.ChangeState(this);
        }
        protected void CrossFadeState(string stateName)
        {
            Base.AnimHelper.CrossFadeToState(stateName, Layer);
        }

    }
}
