using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] int GoldDropPercent = 30;
    [SerializeField] int PotionDropPercent = 30;
    [SerializeField] Gold GoldPrefab = null;
    [SerializeField] Potion LifePotionPrefab = null;
    [SerializeField] Potion ManaPotionPrefab = null;
    [SerializeField] ProjectileBase ProjPrefab;

    [SerializeField]
    [Dropdown(nameof(IDList))]
    string _ResourceID = "";
    List<string> IDList { get => EnemyResourceTable.Instance.GetAllInfo().Select(info => info.EnemyID).ToList(); }

    public SpecEnemy Spec { get; private set; } = null;

    BaseObject mBase = null;

    private void Awake()
    {
        mBase = GetComponentInParent<BaseObject>();

        Spec = mBase.GetComponentInChildren<SpecEnemy>();
        Spec.InitData(_ResourceID);
        Spec.LinkOption(mBase.Buffs.TotalBuffOption);
    }

    void Start()
    {
        mBase.Health.OnDied.AddListener(OnDeath);
    }

    public void OnDeath()
    {
        mBase.Body.LockBody = true;

        if (MyUtils.IsPercentHit((int)Spec.ItemDrop.PercentValue))
            DropItem();

        if (MyUtils.IsPercentHit(GoldDropPercent))
            DropGold();

        if (MyUtils.IsPercentHit(PotionDropPercent))
            DropPotion();
    }

    void DropItem()
    {
        ItemObject.Create(mBase.Body.Center, Quaternion.identity);
    }
    void DropGold()
    {
        Gold itemObj = Instantiate(GoldPrefab, mBase.Body.Center, Quaternion.identity);
        itemObj.GoldAmount = Spec.GoldOnDeath;
    }
    void DropPotion()
    {
        if (MyUtils.IsPercentHit(50))
            Instantiate(LifePotionPrefab, mBase.Body.Center, Quaternion.identity);
        else
            Instantiate(ManaPotionPrefab, mBase.Body.Center, Quaternion.identity);
    }


    public void DoAttackMelee(BaseObject target)
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBase.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                float damage = mBase.Spec.BaseAttack;
                health.GetDamaged(damage);
            }
        });
    }

    public void DoAttackShotToPlayer(BaseObject target)
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBase.Body.Center + new Vector2(transform.right.x, 0);
        Vector2 dirToTarget = (target.Body.Center - startPos).normalized;
        ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
        obj.transform.right = dirToTarget;
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                float damage = mBase.Spec.BaseAttack;
                health.GetDamaged(damage);
            }
        });
    }
    public void DoAttackShotForward(BaseObject target)
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBase.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                float damage = mBase.Spec.BaseAttack;
                health.GetDamaged(damage);
            }
        });
    }
    public void DoAttackThrowToPlayer(BaseObject target)
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBase.Body.Head;
        ProjectileBase obj = ProjectileBase.Create(ProjPrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);

        // 수류탄 투척시 대상 거리에 따른 초기 속도 조절(실험에 근거한 데이터 및 수식..) 
        Vector3 dist = target.Body.Center - mBase.Body.Center;
        float distYRate = Mathf.Max(dist.y, 0);
        Vector2 startVel = Vector2.zero;
        startVel.y = Mathf.Clamp((10 + (dist.y * 1.1f)), 5, 20);
        startVel.x = Mathf.Abs(dist.x * 1.1f) + (distYRate * 0.7f);
        if (mBase.transform.right.x < 0)
            startVel.x *= -1;

        obj.Stats.MoveSpeed = startVel.magnitude;
        obj.transform.right = startVel.normalized;

        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                float damage = mBase.Spec.BaseAttack;
                health.GetDamaged(damage);
            }
        });
    }
    public void SetTarget(BaseObject target)
    {
        mBase.GetComponentInChildren<EnemyAI>().SetTarget(target);
    }
}
