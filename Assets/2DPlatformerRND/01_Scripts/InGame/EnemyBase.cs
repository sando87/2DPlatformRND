using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Awake()
    {
    }


    private void Update()
    {
    }

    void DoDeath()
    {
        animator.SetTrigger("death");
        GetComponentInChildren<InteractableCollider>().enabled = false;
    }

    public void OnColliderEnter(Collider2D col)
    {
        DoDeath();
    }
}
